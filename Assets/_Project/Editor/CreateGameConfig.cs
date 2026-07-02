using UnityEngine;
using UnityEditor;

public static class CreateGameConfig
{
    const string AssetPath = "Assets/_Project/Resources/GameConfig.asset";

    [MenuItem("FragmentoZero/Create Game Config Asset")]
    public static void Create()
    {
        if (!AssetDatabase.IsValidFolder("Assets/_Project/Resources"))
            AssetDatabase.CreateFolder("Assets/_Project", "Resources");

        if (AssetDatabase.LoadAssetAtPath<GameConfig>(AssetPath) != null)
        {
            Debug.Log("GameConfig.asset já existe — nada a fazer.");
            return;
        }

        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<GameConfig>(), AssetPath);
        AssetDatabase.SaveAssets();
        Debug.Log($"GameConfig criado em {AssetPath}");
    }
}
