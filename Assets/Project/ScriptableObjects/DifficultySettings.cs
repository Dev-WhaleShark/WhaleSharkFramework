using UnityEngine;

namespace WhaleShark.Config
{
    [CreateAssetMenu(menuName = "WhaleShark/Config/DifficultySettings")]
    public class DifficultySettings : ScriptableObject
    {
        [Header("Player Modifiers")]
        [Range(0.1f, 2f)]
        public float playerHealthMultiplier = 1f;
        [Range(0.1f, 2f)]
        public float playerDamageMultiplier = 1f;
        [Range(0.1f, 2f)]
        public float playerSpeedMultiplier = 1f;

        [Header("Enemy Modifiers")]
        [Range(0.1f, 3f)]
        public float enemyHealthMultiplier = 1f;
        [Range(0.1f, 3f)]
        public float enemyDamageMultiplier = 1f;
        [Range(0.1f, 2f)]
        public float enemySpeedMultiplier = 1f;
        [Range(0.1f, 2f)]
        public float enemySpawnRateMultiplier = 1f;

        [Header("Progression")]
        public AnimationCurve difficultyProgression = AnimationCurve.Linear(0, 1, 300, 2); // 5분에 걸쳐 2배 증가
        public float maxDifficultyTime = 300f; // 5분

        [Header("Rewards")]
        [Range(0.1f, 3f)]
        public float scoreMultiplier = 1f;
        [Range(0.1f, 3f)]
        public float experienceMultiplier = 1f;

        public float GetDifficultyMultiplier(float gameTime)
        {
            float normalizedTime = Mathf.Clamp01(gameTime / maxDifficultyTime);
            return difficultyProgression.Evaluate(normalizedTime);
        }
    }
}