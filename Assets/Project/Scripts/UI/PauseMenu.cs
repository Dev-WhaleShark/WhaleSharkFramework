using UnityEngine;
using UnityEngine.UI;
using WhaleShark.Core;

namespace WhaleShark.UI
{
    public class PauseMenu : UIWindow
    {
        [Header("UI References")]
        public Button resumeButton;
        public Button settingsButton;
        public Button mainMenuButton;
        public Button quitButton;

        void Start()
        {
            if (resumeButton) resumeButton.onClick.AddListener(Resume);
            if (settingsButton) settingsButton.onClick.AddListener(OpenSettings);
            if (mainMenuButton) mainMenuButton.onClick.AddListener(GoToMainMenu);
            if (quitButton) quitButton.onClick.AddListener(QuitGame);

            EventBus.PauseToggled += OnPauseToggled;
        }

        void OnDestroy()
        {
            EventBus.PauseToggled -= OnPauseToggled;
        }

        void OnPauseToggled(bool isPaused)
        {
            if (isPaused)
            {
                UIStack.Push(this);
                Time.timeScale = 0f;
            }
            else
            {
                UIStack.Pop();
                Time.timeScale = 1f;
            }
        }

        void Resume()
        {
            EventBus.RaisePause(false);
        }

        void OpenSettings()
        {
            // 설정 창 열기 (추후 구현)
            Debug.Log("Settings menu not implemented yet");
        }

        void GoToMainMenu()
        {
            Time.timeScale = 1f;
            SceneLoader.Load("MainMenu");
        }

        void QuitGame()
        {
            Time.timeScale = 1f;
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}