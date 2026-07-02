using UnityEngine;

/// <summary>
/// Cria a raiz de UI de runtime uma única vez por sessão.
/// A raiz é DontDestroyOnLoad: sobrevive a reloads de cena (restart),
/// e os controllers reagem a sceneLoaded/StateChanged para se atualizarem.
/// </summary>
public static class UIBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Init()
    {
        var root = new GameObject("RuntimeUI");
        Object.DontDestroyOnLoad(root);

        UiFactory.EnsureEventSystem(root.transform);

        root.AddComponent<MenuController>();
        root.AddComponent<PauseController>();
        root.AddComponent<GameOverController>();
        root.AddComponent<ToastUI>();
        root.AddComponent<TutorialController>();
    }
}
