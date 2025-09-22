using System;
using System.IO;
using UnityEngine;

namespace WhaleShark.Core
{
    [Serializable]
    public class SaveData
    {
        public float bgm = 0.8f, se = 1f, sensitivity = 1f;
        public int highScore = 0;
        public bool fullscreen = true;
        public string playerName = "";
        public int level = 1;
        public bool tutorialCompleted = false;
    }

    public static class SaveService
    {
        static string PathFile => Path.Combine(Application.persistentDataPath, "save.json");
        public static SaveData Data { get; private set; } = new SaveData();

        public static void Load()
        {
            try
            {
                if (File.Exists(PathFile))
                    Data = JsonUtility.FromJson<SaveData>(File.ReadAllText(PathFile));
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Save load failed: {e.Message}");
                Data = new SaveData();
            }

            ApplyRuntime();
        }

        public static void Save()
        {
            try
            {
                var json = JsonUtility.ToJson(Data, true);
                File.WriteAllText(PathFile, json);
                Debug.Log($"Game saved to: {PathFile}");
            }
            catch (Exception e)
            {
                Debug.LogWarning($"Save write failed: {e.Message}");
            }
        }

        static void ApplyRuntime()
        {
            Screen.fullScreen = Data.fullscreen;
            if (AudioManager.I != null)
            {
                AudioManager.SetBGMVolume(Data.bgm);
                AudioManager.SetSEVolume(Data.se);
            }
        }

        public static void UpdateHighScore(int score)
        {
            if (score > Data.highScore)
            {
                Data.highScore = score;
                Save();
            }
        }
    }
}