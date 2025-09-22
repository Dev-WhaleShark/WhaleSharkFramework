using System.Collections;
using UnityEngine;

namespace WhaleShark.Core
{
    public class Boot : MonoBehaviour
    {
        [Header("Settings")]
        public string firstSceneName = "Gameplay";
        public float bootDelay = 1f;

        IEnumerator Start()
        {
            // 저장 데이터 로드
            SaveService.Load();

            // 부트 딜레이
            yield return new WaitForSeconds(bootDelay);

            // 부드럽게 등장
            yield return FadeCanvas.In();

            // 첫 번째 씬으로 이동
            SceneLoader.Load(firstSceneName);
        }
    }
}