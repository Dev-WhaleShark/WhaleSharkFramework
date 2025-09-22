using UnityEngine;

namespace WhaleShark.Config
{
    [CreateAssetMenu(menuName = "WhaleShark/Config/EnemyConfig")]
    public class EnemyConfig : ScriptableObject
    {
        [Header("Basic Stats")]
        public string enemyName = "Basic Enemy";
        public float maxHealth = 50f;
        public float moveSpeed = 3f;
        public float damage = 10f;

        [Header("AI Behavior")]
        public float detectionRange = 8f;
        public float attackRange = 2f;
        public float attackCooldown = 1.5f;
        public float patrolRadius = 5f;

        [Header("Movement")]
        public float acceleration = 10f;
        public float deceleration = 15f;
        public float rotationSpeed = 180f;

        [Header("Rewards")]
        public int scoreValue = 100;
        public float experienceValue = 25f;
        public GameObject[] dropItems;
        [Range(0f, 1f)]
        public float dropChance = 0.3f;

        [Header("Effects")]
        public GameObject deathEffect;
        public AudioClip deathSound;
        public GameObject hitEffect;
        public AudioClip hitSound;
    }
}