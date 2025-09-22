using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace WhaleShark.Core
{
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance;

        void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public static void Load(string sceneName, Action<float> onProgress = null)
            => Instance.StartCoroutine(Instance.LoadRoutine(sceneName, onProgress));

        IEnumerator LoadRoutine(string sceneName, Action<float> onProgress)
        {
            var op = SceneManager.LoadSceneAsync(sceneName);
            op.allowSceneActivation = false;

            while (op.progress < 0.9f)
            {
                onProgress?.Invoke(op.progress);
                yield return null;
            }
            op.allowSceneActivation = true;

            // 다음 프레임까지 대기 후 페이드 인 (씬 활성화 안정화)
            yield return null;
        }
    }
}