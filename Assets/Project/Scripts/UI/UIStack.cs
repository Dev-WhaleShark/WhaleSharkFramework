using System.Collections.Generic;
using UnityEngine;

namespace WhaleShark.UI
{
    public class UIStack : MonoBehaviour
    {
        public static UIStack I;
        Stack<UIWindow> stack = new Stack<UIWindow>();
        public KeyCode backKey = KeyCode.Escape;

        void Awake()
        {
            I = this;
            DontDestroyOnLoad(gameObject);
        }

        void Update()
        {
            if(Input.GetKeyDown(backKey) && stack.Count > 0)
                Pop();
        }

        public static void Push(UIWindow w)
        {
            if (I.stack.Count > 0)
                I.stack.Peek().SetInteractable(false);

            I.stack.Push(w);
            w.Show();
            w.SetInteractable(true);
        }

        public static void Pop()
        {
            if (I.stack.Count == 0) return;

            var top = I.stack.Pop();
            top.SetInteractable(false);
            top.Hide();

            if (I.stack.Count > 0)
                I.stack.Peek().SetInteractable(true);
        }

        public static void Clear()
        {
            while(I.stack.Count > 0)
            {
                var w = I.stack.Pop();
                w.Hide();
            }
        }
    }
}