using UnityEngine;

/// <summary>Persistência do recorde de distância (PlayerPrefs).</summary>
public static class HighScore
{
    const string Key = "HighScoreMeters";

    public static int Best => PlayerPrefs.GetInt(Key, 0);

    /// <summary>Registra a distância; retorna true se virou novo recorde.</summary>
    public static bool Submit(int meters)
    {
        if (meters <= Best) return false;
        PlayerPrefs.SetInt(Key, meters);
        PlayerPrefs.Save();
        return true;
    }
}
