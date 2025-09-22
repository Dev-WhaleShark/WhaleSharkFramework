using System.Collections;
using UnityEngine;

namespace WhaleShark.Core
{
    public static class TweenUtil
    {
        public static IEnumerator FadeCanvasGroup(CanvasGroup cg, float target, float dur)
        {
            float t = 0f, start = cg.alpha;

            while (t < dur)
            {
                t += Time.unscaledDeltaTime;
                cg.alpha = Mathf.SmoothStep(start, target, t / dur);
                yield return null;
            }

            cg.alpha = target;
        }

        public static IEnumerator Move(Transform tr, Vector3 target, float dur)
        {
            float t = 0f;
            Vector3 start = tr.position;

            while (t < dur)
            {
                t += Time.deltaTime;
                tr.position = Vector3.LerpUnclamped(start, target, Smooth(t / dur));
                yield return null;
            }

            tr.position = target;
        }

        public static IEnumerator Scale(Transform tr, Vector3 target, float dur)
        {
            float t = 0f;
            Vector3 start = tr.localScale;

            while (t < dur)
            {
                t += Time.deltaTime;
                tr.localScale = Vector3.LerpUnclamped(start, target, Smooth(t / dur));
                yield return null;
            }

            tr.localScale = target;
        }

        public static IEnumerator PunchScale(Transform tr, float amp = 0.1f, float dur = 0.12f)
        {
            Vector3 baseS = tr.localScale;
            float t = 0f;

            while(t < dur)
            {
                t += Time.deltaTime;
                float p = 1f - (t / dur);
                tr.localScale = baseS * (1f + Mathf.Sin(t * 50f) * amp * p);
                yield return null;
            }

            tr.localScale = baseS;
        }

        public static IEnumerator Fade(SpriteRenderer sr, float target, float dur)
        {
            float t = 0f;
            Color start = sr.color;
            Color targetColor = new Color(start.r, start.g, start.b, target);

            while (t < dur)
            {
                t += Time.deltaTime;
                sr.color = Color.LerpUnclamped(start, targetColor, Smooth(t / dur));
                yield return null;
            }

            sr.color = targetColor;
        }

        static float Smooth(float x) => Mathf.SmoothStep(0f, 1f, x);

        // Easing functions
        public static float EaseOutCubic(float t) => 1f - Mathf.Pow(1f - t, 3f);
        public static float EaseInCubic(float t) => t * t * t;
        public static float EaseInOutCubic(float t) => t < 0.5f ? 4f * t * t * t : 1f - Mathf.Pow(-2f * t + 2f, 3f) / 2f;
    }
}