using UnityEngine;
using WhaleShark.Core;

namespace WhaleShark.Gameplay
{
    [RequireComponent(typeof(Rigidbody2D))]
    public class Projectile : MonoBehaviour, IPoolable
    {
        [Header("Projectile Settings")]
        public float damage = 10f;
        public float speed = 20f;
        public float lifetime = 5f;
        public LayerMask hitLayers = ~0;

        [Header("Effects")]
        public GameObject hitEffect;
        public AudioClip hitSound;
        public bool destroyOnHit = true;

        Rigidbody2D rb;
        float timer;
        bool isActive;

        public void SetDamage(float newDamage) => damage = newDamage;
        public void SetSpeed(float newSpeed) => speed = newSpeed;
        public void SetLifetime(float newLifetime) => lifetime = newLifetime;

        void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        void Update()
        {
            if (!isActive) return;

            timer += Time.deltaTime;
            if (timer >= lifetime)
            {
                ReturnToPool();
            }
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (!isActive) return;

            if (((1 << other.gameObject.layer) & hitLayers) != 0)
            {
                HandleHit(other);
            }
        }

        void HandleHit(Collider2D target)
        {
            // 데미지 적용
            var health = target.GetComponent<Health>();
            if (health != null)
            {
                health.TakeDamage(damage, transform.position);
            }

            // 히트 이펙트
            if (hitEffect != null)
            {
                Instantiate(hitEffect, transform.position, Quaternion.identity);
            }

            // 히트 사운드
            if (hitSound != null)
            {
                AudioManager.PlaySE(hitSound);
            }

            if (destroyOnHit)
            {
                ReturnToPool();
            }
        }

        public void Fire(Vector2 direction)
        {
            if (rb != null)
            {
                rb.linearVelocity = direction.normalized * speed;
            }

            // 발사 방향으로 회전
            if (direction != Vector2.zero)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            }
        }

        void ReturnToPool()
        {
            var pool = GetComponentInParent<SimplePool>();
            if (pool != null)
            {
                pool.Despawn(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void OnSpawned()
        {
            isActive = true;
            timer = 0f;
        }

        public void OnDespawned()
        {
            isActive = false;
            if (rb != null)
                rb.linearVelocity = Vector2.zero;
        }
    }
}