using UnityEngine;
using WhaleShark.Core;

namespace WhaleShark.Gameplay
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager I;

        [Header("Game State")]
        public bool isPaused = false;
        public int currentScore = 0;
        public int currentLevel = 1;
        public float gameTime = 0f;

        [Header("Player")]
        public Transform playerTransform;
        public int playerHealth = 100;
        public int maxHealth = 100;

        void Awake()
        {
            if (I == null)
            {
                I = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            EventBus.PauseToggled += OnPauseToggled;
            EventBus.PlayerDied += OnPlayerDied;
        }

        void OnDestroy()
        {
            EventBus.PauseToggled -= OnPauseToggled;
            EventBus.PlayerDied -= OnPlayerDied;
        }

        void Update()
        {
            if (!isPaused)
            {
                gameTime += Time.deltaTime;
            }

            HandleInput();
        }

        void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }

#if UNITY_EDITOR || DEVELOPMENT_BUILD
            // 디버그 단축키
            if (Input.GetKeyDown(KeyCode.F1))
            {
                AddScore(100);
            }

            if (Input.GetKeyDown(KeyCode.F2))
            {
                TakeDamage(10);
            }

            if (Input.GetKeyDown(KeyCode.F3))
            {
                Heal(20);
            }
#endif
        }

        public void TogglePause()
        {
            isPaused = !isPaused;
            EventBus.RaisePause(isPaused);
        }

        public void AddScore(int points)
        {
            currentScore += points;
            EventBus.RaiseScoreChanged(currentScore);

            if (currentScore > SaveService.Data.highScore)
            {
                SaveService.UpdateHighScore(currentScore);
                EventBus.RaiseToast($"새로운 최고 기록! {currentScore}점");
            }
        }

        public void TakeDamage(int damage)
        {
            playerHealth = Mathf.Max(0, playerHealth - damage);
            EventBus.RaiseHealthChanged(playerHealth);

            if (playerHealth <= 0)
            {
                EventBus.RaisePlayerDied();
            }
        }

        public void Heal(int amount)
        {
            playerHealth = Mathf.Min(maxHealth, playerHealth + amount);
            EventBus.RaiseHealthChanged(playerHealth);
        }

        void OnPauseToggled(bool paused)
        {
            isPaused = paused;
        }

        void OnPlayerDied()
        {
            Debug.Log("Player died!");
            EventBus.RaiseToast("Game Over!");

            // 게임 오버 처리
            SaveService.Save();
        }

        public void RestartGame()
        {
            SceneLoader.Load("Gameplay");
        }

        public void GoToMainMenu()
        {
            SceneLoader.Load("MainMenu");
        }
    }
}