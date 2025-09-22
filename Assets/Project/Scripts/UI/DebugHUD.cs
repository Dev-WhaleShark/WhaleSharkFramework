using TMPro;
using UnityEngine;
using UnityEngine.UI;
using WhaleShark.Core;

namespace WhaleShark.UI
{
#if UNITY_EDITOR || DEVELOPMENT_BUILD
    public class DebugHUD : MonoBehaviour
    {
        [Header("UI References")]
        public TextMeshProUGUI fpsText;
        public TextMeshProUGUI gameInfoText;
        public Slider timeScaleSlider;
        public Button addScoreButton;
        public Button damageButton;
        public Button healButton;
        public Button toggleHUDButton;

        [Header("Settings")]
        public KeyCode toggleKey = KeyCode.F12;
        public bool showOnStart = true;

        bool isVisible;
        float deltaTime;
        CanvasGroup canvasGroup;

        void Awake()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (!canvasGroup) canvasGroup = gameObject.AddComponent<CanvasGroup>();

            SetVisible(showOnStart);
        }

        void Start()
        {
            SetupUI();
        }

        void Update()
        {
            if (Input.GetKeyDown(toggleKey))
            {
                ToggleVisibility();
            }

            if (isVisible)
            {
                UpdateFPS();
                UpdateGameInfo();
            }
        }

        void SetupUI()
        {
            if (timeScaleSlider)
            {
                timeScaleSlider.value = Time.timeScale;
                timeScaleSlider.onValueChanged.AddListener(OnTimeScaleChanged);
            }

            if (addScoreButton)
                addScoreButton.onClick.AddListener(() => {
                    var gm = FindFirstObjectByType<WhaleShark.Gameplay.GameManager>();
                    gm?.AddScore(100);
                });

            if (damageButton)
                damageButton.onClick.AddListener(() => {
                    var gm = FindFirstObjectByType<WhaleShark.Gameplay.GameManager>();
                    gm?.TakeDamage(10);
                });

            if (healButton)
                healButton.onClick.AddListener(() => {
                    var gm = FindFirstObjectByType<WhaleShark.Gameplay.GameManager>();
                    gm?.Heal(20);
                });

            if (toggleHUDButton)
                toggleHUDButton.onClick.AddListener(ToggleVisibility);
        }

        void UpdateFPS()
        {
            if (!fpsText) return;

            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
            float fps = 1.0f / deltaTime;

            Color color = fps >= 50 ? Color.green : fps >= 30 ? Color.yellow : Color.red;
            fpsText.text = $"FPS: <color=#{ColorUtility.ToHtmlStringRGB(color)}>{fps:0}</color>";
        }

        void UpdateGameInfo()
        {
            if (!gameInfoText) return;

            var gm = FindFirstObjectByType<WhaleShark.Gameplay.GameManager>();
            if (gm == null) return;

            gameInfoText.text = $"Score: {gm.currentScore}\n" +
                               $"Health: {gm.playerHealth}/{gm.maxHealth}\n" +
                               $"Time: {gm.gameTime:F1}s\n" +
                               $"TimeScale: {Time.timeScale:F2}";
        }

        void OnTimeScaleChanged(float value)
        {
            Time.timeScale = value;
        }

        void ToggleVisibility()
        {
            SetVisible(!isVisible);
        }

        void SetVisible(bool visible)
        {
            isVisible = visible;
            canvasGroup.alpha = visible ? 1f : 0f;
            canvasGroup.interactable = visible;
            canvasGroup.blocksRaycasts = visible;
        }
    }
#endif
}