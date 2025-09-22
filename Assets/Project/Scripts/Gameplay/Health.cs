using UnityEngine;
using WhaleShark.Core;

namespace WhaleShark.Gameplay
{
    public class Health : MonoBehaviour
    {
        [Header("Health Settings")]
        public float maxHealth = 100f;
        public bool destroyOnDeath = true;
        public bool isPlayer = false;

        [Header("Effects")]
        public GameObject hitEffect;
        public GameObject deathEffect;
        public AudioClip hitSound;
        public AudioClip deathSound;

        [Header("Invincibility")]
        public float invincibilityDuration = 0.5f;
        public bool flashOnHit = true;
        public SpriteRenderer spriteRenderer;

        float currentHealth;
        bool isInvincible = false;
        bool isDead = false;

        public float CurrentHealth => currentHealth;
        public float HealthRatio => currentHealth / maxHealth;
        public bool IsDead => isDead;

        public System.Action<float> OnHealthChanged;
        public System.Action OnDied;
        public System.Action<float> OnDamageTaken;

        void Awake()
        {
            currentHealth = maxHealth;
            if (!spriteRenderer) spriteRenderer = GetComponent<SpriteRenderer>();
        }

        void Start()
        {
            if (isPlayer)
            {
                EventBus.HealthChanged += OnPlayerHealthChanged;
            }
        }

        void OnDestroy()
        {
            if (isPlayer)
            {
                EventBus.HealthChanged -= OnPlayerHealthChanged;
            }
        }

        public void TakeDamage(float damage, Vector3 hitPosition = default)
        {
            if (isDead || isInvincible) return;

            currentHealth = Mathf.Max(0, currentHealth - damage);
            OnHealthChanged?.Invoke(currentHealth);
            OnDamageTaken?.Invoke(damage);

            // 이펙트 및 사운드
            if (hitEffect != null)
                Instantiate(hitEffect, hitPosition != default ? hitPosition : transform.position, Quaternion.identity);

            if (hitSound != null)
                AudioManager.PlaySE(hitSound);

            // 히트 이벤트 발생
            EventBus.RaiseHit(hitPosition != default ? hitPosition : transform.position);

            // 플레이어 체력 변경 알림
            if (isPlayer)
                EventBus.RaiseHealthChanged(Mathf.RoundToInt(currentHealth));

            // 무적 시간
            if (invincibilityDuration > 0)
                StartCoroutine(InvincibilityRoutine());

            // 사망 체크
            if (currentHealth <= 0)
            {
                Die();
            }
        }

        public void Heal(float amount)
        {
            if (isDead) return;

            currentHealth = Mathf.Min(maxHealth, currentHealth + amount);
            OnHealthChanged?.Invoke(currentHealth);

            if (isPlayer)
                EventBus.RaiseHealthChanged(Mathf.RoundToInt(currentHealth));
        }

        void Die()
        {
            if (isDead) return;

            isDead = true;
            OnDied?.Invoke();

            // 데스 이펙트 및 사운드
            if (deathEffect != null)
                Instantiate(deathEffect, transform.position, Quaternion.identity);

            if (deathSound != null)
                AudioManager.PlaySE(deathSound);

            // 플레이어 사망 알림
            if (isPlayer)
                EventBus.RaisePlayerDied();

            // 오브젝트 파괴
            if (destroyOnDeath)
                Destroy(gameObject, 0.1f); // 약간의 딜레이로 이벤트 처리 시간 확보
        }

        System.Collections.IEnumerator InvincibilityRoutine()
        {
            isInvincible = true;
            float elapsed = 0f;
            bool visible = true;

            while (elapsed < invincibilityDuration)
            {
                elapsed += Time.deltaTime;

                if (flashOnHit && spriteRenderer != null)
                {
                    visible = !visible;
                    spriteRenderer.color = visible ? Color.white : new Color(1, 1, 1, 0.5f);
                    yield return new WaitForSeconds(0.1f);
                }
                else
                {
                    yield return null;
                }
            }

            isInvincible = false;
            if (spriteRenderer != null)
                spriteRenderer.color = Color.white;
        }

        void OnPlayerHealthChanged(int health)
        {
            if (!isPlayer) return;
            currentHealth = health;
        }

        public void SetMaxHealth(float newMaxHealth)
        {
            float ratio = HealthRatio;
            maxHealth = newMaxHealth;
            currentHealth = maxHealth * ratio;
            OnHealthChanged?.Invoke(currentHealth);
        }
    }
}