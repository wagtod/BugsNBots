using System;
using System.IO;
using UnityEngine;

[Serializable]
public class GameSettings
{
    public DifficultyLevels MaxDifficulty;
    public DifficultyLevels LastDifficulty;
    public float EffectsVolumeLevel;
    public float MusicVolumeLevel;

    private static string _pathAndFilename = Path.Combine(Application.persistentDataPath, "GameSettings.txt");

    public enum DifficultyLevels
    {
        Easy = 0,
        Normal = 1,
        Hard = 2
    }

    public GameSettings()
    {
        MaxDifficulty = DifficultyLevels.Easy;
        LastDifficulty = DifficultyLevels.Easy;
        EffectsVolumeLevel = 0.0f;
        MusicVolumeLevel = 0.0f;
    }

    public static void Persist(GameSettings gs)
    {
        using (StreamWriter streamWriter = File.CreateText(_pathAndFilename))
        {
            string json = JsonUtility.ToJson(gs);
            streamWriter.Write(json);
        }
    }

    public static GameSettings LoadFromDisk()
    {
        GameSettings retVal = new GameSettings();
        if (File.Exists(_pathAndFilename))
        {
            using (StreamReader streamReader = File.OpenText(_pathAndFilename))
            {
                string jsonString = streamReader.ReadToEnd();
                retVal = JsonUtility.FromJson<GameSettings>(jsonString);
            }
        }
        return retVal;
    }
}
