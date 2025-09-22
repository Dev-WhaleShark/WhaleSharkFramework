using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WhaleShark.Core;

namespace WhaleShark.UI
{
    public class Toast : MonoBehaviour
    {
        [Header("Settings")]
        public float displayDuration = 2f;
        public float fadeInDuration = 0.3f;
        public float fadeOutDuration = 0.5f;
        public Vector3 moveOffset = Vector3.up * 50f;

        [Header("UI References")]
        public TextMeshProUGUI messageText;
        public CanvasGroup canvasGroup;

        Queue<string> messageQueue = new Queue<string>();
        bool isDisplaying = false;

        void Awake()
        {
            if (!canvasGroup) canvasGroup = GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
        }

        void Start()
        {
            EventBus.Toast += ShowToast;
        }

        void OnDestroy()
        {
            EventBus.Toast -= ShowToast;
        }

        public void ShowToast(string message)
        {
            messageQueue.Enqueue(message);

            if (!isDisplaying)
            {
                StartCoroutine(DisplayNextToast());
            }
        }

        IEnumerator DisplayNextToast()
        {
            while (messageQueue.Count > 0)
            {
                isDisplaying = true;
                var message = messageQueue.Dequeue();

                yield return StartCoroutine(DisplayToast(message));

                yield return new WaitForSeconds(0.1f); // 토스트 간 간격
            }

            isDisplaying = false;
        }

        IEnumerator DisplayToast(string message)
        {
            messageText.text = message;
            Vector3 startPos = transform.localPosition;
            Vector3 endPos = startPos + moveOffset;

            // 페이드 인 + 위로 이동
            yield return StartCoroutine(AnimateToast(startPos, endPos, 0f, 1f, fadeInDuration));

            // 표시 지속
            yield return new WaitForSeconds(displayDuration);

            // 페이드 아웃 + 계속 위로 이동
            Vector3 finalPos = endPos + moveOffset;
            yield return StartCoroutine(AnimateToast(endPos, finalPos, 1f, 0f, fadeOutDuration));

            // 위치 리셋
            transform.localPosition = startPos;
        }

        IEnumerator AnimateToast(Vector3 startPos, Vector3 endPos, float startAlpha, float endAlpha, float duration)
        {
            float t = 0f;

            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                float progress = t / duration;

                transform.localPosition = Vector3.LerpUnclamped(startPos, endPos, TweenUtil.EaseOutCubic(progress));
                canvasGroup.alpha = Mathf.LerpUnclamped(startAlpha, endAlpha, progress);

                yield return null;
            }

            transform.localPosition = endPos;
            canvasGroup.alpha = endAlpha;
        }
    }
}