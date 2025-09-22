using System.Collections;
using UnityEngine;

namespace WhaleShark.Core
{
    [RequireComponent(typeof(CanvasGroup))]
    public class FadeCanvas : MonoBehaviour
    {
        public static FadeCanvas I;
        public float duration = 0.25f;
        CanvasGroup cg;

        void Awake()
        {
            I = this;
            cg = GetComponent<CanvasGroup>();
            DontDestroyOnLoad(gameObject);
        }

        public static IEnumerator Out() => I.FadeTo(1f);
        public static IEnumerator In() => I.FadeTo(0f);

        IEnumerator FadeTo(float target)
        {
            float t = 0f, start = cg.alpha;
            cg.blocksRaycasts = true;

            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                cg.alpha = Mathf.SmoothStep(start, target, t / duration);
                yield return null;
            }

            cg.alpha = target;
            cg.blocksRaycasts = target > 0.001f;
        }
    }
}