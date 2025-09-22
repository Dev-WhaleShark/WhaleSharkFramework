using System.Collections;
using UnityEngine;
using WhaleShark.Core;

namespace WhaleShark.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIWindow : MonoBehaviour
    {
        CanvasGroup cg;

        void Awake()
        {
            cg = GetComponent<CanvasGroup>();
            HideImmediate();
        }

        public virtual void Show()
        {
            gameObject.SetActive(true);
            StartCoroutine(TweenUtil.FadeCanvasGroup(cg, 1f, 0.15f));
        }

        public virtual void Hide()
        {
            StartCoroutine(HideRoutine());
        }

        IEnumerator HideRoutine()
        {
            yield return TweenUtil.FadeCanvasGroup(cg, 0f, 0.12f);
            gameObject.SetActive(false);
        }

        public void HideImmediate()
        {
            cg.alpha = 0;
            gameObject.SetActive(false);
        }

        public void SetInteractable(bool on)
        {
            cg.interactable = cg.blocksRaycasts = on;
        }
    }
}