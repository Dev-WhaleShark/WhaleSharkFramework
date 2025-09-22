using UnityEngine;
using UnityEngine.InputSystem;

namespace WhaleShark.Core
{
    public class InputManager : MonoBehaviour
    {
        public static InputManager I;

        [Header("Input Actions")]
        public InputActionAsset inputActions;

        InputActionMap playerActionMap;
        InputActionMap uiActionMap;

        // Player Actions
        InputAction moveAction;
        InputAction jumpAction;
        InputAction interactAction;

        // UI Actions
        InputAction pauseAction;
        InputAction cancelAction;

        public Vector2 MoveInput { get; private set; }
        public bool JumpPressed { get; private set; }
        public bool JumpHeld { get; private set; }
        public bool InteractPressed { get; private set; }

        void Awake()
        {
            if (I == null)
            {
                I = this;
                DontDestroyOnLoad(gameObject);
                SetupInputActions();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            EnablePlayerInput();
        }

        void SetupInputActions()
        {
            if (inputActions == null) return;

            playerActionMap = inputActions.FindActionMap("Player");
            uiActionMap = inputActions.FindActionMap("UI");

            // Player Actions
            moveAction = playerActionMap?.FindAction("Move");
            jumpAction = playerActionMap?.FindAction("Jump");
            interactAction = playerActionMap?.FindAction("Interact");

            // UI Actions
            pauseAction = uiActionMap?.FindAction("Pause");
            cancelAction = uiActionMap?.FindAction("Cancel");

            // 이벤트 연결
            if (jumpAction != null)
            {
                jumpAction.performed += OnJumpPerformed;
                jumpAction.canceled += OnJumpCanceled;
            }

            if (interactAction != null)
                interactAction.performed += OnInteractPerformed;

            if (pauseAction != null)
                pauseAction.performed += OnPausePerformed;
        }

        void Update()
        {
            UpdateInputValues();
        }

        void UpdateInputValues()
        {
            MoveInput = moveAction?.ReadValue<Vector2>() ?? Vector2.zero;
            JumpHeld = jumpAction?.IsPressed() ?? false;
        }

        void OnJumpPerformed(InputAction.CallbackContext context)
        {
            JumpPressed = true;
        }

        void OnJumpCanceled(InputAction.CallbackContext context)
        {
            JumpPressed = false;
        }

        void OnInteractPerformed(InputAction.CallbackContext context)
        {
            InteractPressed = true;
        }

        void OnPausePerformed(InputAction.CallbackContext context)
        {
            // GameManager를 동적으로 찾아서 호출 (순환 의존성 방지)
            var gameManager = FindFirstObjectByType<WhaleShark.Gameplay.GameManager>();
            gameManager?.TogglePause();
        }

        public void EnablePlayerInput()
        {
            playerActionMap?.Enable();
            uiActionMap?.Enable();
        }

        public void DisablePlayerInput()
        {
            playerActionMap?.Disable();
        }

        public void EnableUIInput()
        {
            uiActionMap?.Enable();
        }

        public void DisableUIInput()
        {
            uiActionMap?.Disable();
        }

        void LateUpdate()
        {
            // 프레임 끝에서 일회성 입력 초기화
            JumpPressed = false;
            InteractPressed = false;
        }

        void OnEnable()
        {
            inputActions?.Enable();
        }

        void OnDisable()
        {
            inputActions?.Disable();
        }
    }
}