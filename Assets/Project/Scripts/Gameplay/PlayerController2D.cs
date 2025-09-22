using UnityEngine;
using WhaleShark.Config;
using WhaleShark.Core;

namespace WhaleShark.Gameplay
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerController2D : MonoBehaviour
    {
        public MoveConfig config;

        [Header("Debug")]
        public bool showGroundCheck = true;

        Rigidbody2D rb;
        bool jumpQueued;
        float moveX;
        float coyoteTimer;
        float jumpBufferTimer;
        int jumpCount;
        bool isGrounded;
        bool wasGrounded;
        InputManager input;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        void Start()
        {
            input = InputManager.I;

            // 카메라 타겟 설정
            if (CameraController.I != null)
                CameraController.I.SetFollowTarget(transform);

            // 게임매니저에 플레이어 등록
            if (GameManager.I != null)
                GameManager.I.playerTransform = transform;
        }

        void Update()
        {
            HandleInput();
            CheckGrounded();
            HandleCoyoteTime();
            HandleJumpBuffer();
        }

        void FixedUpdate()
        {
            HandleMovement();
            HandleJump();
            HandleVariableJumpHeight();
        }

        void HandleInput()
        {
            if (input != null)
            {
                // New Input System 사용
                moveX = input.MoveInput.x;
                if (input.JumpPressed)
                {
                    jumpQueued = true;
                    jumpBufferTimer = config.jumpBufferTime;
                }
            }
            else
            {
                // Fallback to old input system
                moveX = Input.GetAxisRaw("Horizontal");
                if (Input.GetButtonDown("Jump"))
                {
                    jumpQueued = true;
                    jumpBufferTimer = config.jumpBufferTime;
                }
            }
        }

        void CheckGrounded()
        {
            wasGrounded = isGrounded;
            var hit = Physics2D.Raycast(transform.position, Vector2.down, config.groundCheckDist, config.groundMask);
            isGrounded = hit.collider != null;

            // 착지 시 점프 카운트 리셋
            if (isGrounded && !wasGrounded)
            {
                jumpCount = 0;
            }
        }

        void HandleCoyoteTime()
        {
            if (isGrounded)
                coyoteTimer = config.coyoteTime;
            else
                coyoteTimer -= Time.deltaTime;
        }

        void HandleJumpBuffer()
        {
            if (jumpBufferTimer > 0)
                jumpBufferTimer -= Time.deltaTime;
        }

        void HandleMovement()
        {
            float targetSpeed = moveX * config.moveSpeed;

            // 공중에서는 제어력 감소
            if (!isGrounded)
                targetSpeed *= config.airControl;

            rb.linearVelocity = new Vector2(targetSpeed, rb.linearVelocity.y);

            // 스프라이트 플립
            if (Mathf.Abs(moveX) > 0.01f)
                transform.localScale = new Vector3(Mathf.Sign(moveX), 1, 1);
        }

        void HandleJump()
        {
            bool canJump = (coyoteTimer > 0 || jumpCount < config.maxJumps) && jumpBufferTimer > 0;

            if (canJump)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0);
                rb.AddForce(Vector2.up * config.jumpPower, ForceMode2D.Impulse);

                jumpCount++;
                jumpBufferTimer = 0;
                coyoteTimer = 0;
            }

            jumpQueued = false;
        }

        void HandleVariableJumpHeight()
        {
            if (rb.linearVelocity.y < 0)
            {
                // 떨어질 때 중력 증가
                rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (config.fallMultiplier - 1) * Time.fixedDeltaTime;
            }
            else if (rb.linearVelocity.y > 0 && !(input?.JumpHeld ?? Input.GetButton("Jump")))
            {
                // 짧은 점프
                rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (config.lowJumpMultiplier - 1) * Time.fixedDeltaTime;
            }
        }

        void OnDrawGizmosSelected()
        {
            if (!showGroundCheck) return;

            Gizmos.color = isGrounded ? Color.green : Color.red;
            Gizmos.DrawLine(transform.position, transform.position + Vector3.down * config.groundCheckDist);
        }
    }
}