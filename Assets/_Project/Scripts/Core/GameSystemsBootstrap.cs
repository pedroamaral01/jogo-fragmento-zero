using UnityEngine;

/// <summary>
/// Raiz persistente dos sistemas de gameplay que não vivem na cena
/// (BossDirector, AudioManager). Criada uma vez por sessão; os sistemas
/// se resetam via sceneLoaded/StateChanged.
/// </summary>
public static class GameSystemsBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Init()
    {
        var root = new GameObject("RuntimeSystems");
        Object.DontDestroyOnLoad(root);

        root.AddComponent<BossDirector>();
        root.AddComponent<BossArenaSupport>();
        root.AddComponent<AudioManager>();
        root.AddComponent<VFXManager>();
        root.AddComponent<EnvironmentStyler>();
    }
}
