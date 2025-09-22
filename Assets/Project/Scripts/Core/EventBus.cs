using System;
using UnityEngine;

namespace WhaleShark.Core
{
    public static class EventBus
    {
        public static event Action<int> ScoreChanged;
        public static void RaiseScoreChanged(int v) => ScoreChanged?.Invoke(v);

        public static event Action<Vector3> Hit; // 피격 위치 등
        public static void RaiseHit(Vector3 pos) => Hit?.Invoke(pos);

        public static event Action<bool> PauseToggled;
        public static void RaisePause(bool on) => PauseToggled?.Invoke(on);

        public static event Action<string> Toast;
        public static void RaiseToast(string message) => Toast?.Invoke(message);

        public static event Action PlayerDied;
        public static void RaisePlayerDied() => PlayerDied?.Invoke();

        public static event Action<int> HealthChanged;
        public static void RaiseHealthChanged(int health) => HealthChanged?.Invoke(health);
    }
}