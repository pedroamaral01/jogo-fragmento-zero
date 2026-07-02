using UnityEngine;

/// <summary>
/// Pausa: ESC alterna durante o gameplay; painel com atalhos e botões.
/// A física de pausa (Time.timeScale) é do GameManager — aqui só input e UI.
/// </summary>
public class PauseController : MonoBehaviour
{
    GameObject panel;

    void Awake()
    {
        var canvas = UiFactory.CreateCanvas(transform, "PauseCanvas", 60);
        BuildPanel(canvas.transform);
        panel.SetActive(false);
    }

    void OnEnable()  => GameEvents.StateChanged += OnStateChanged;
    void OnDisable() => GameEvents.StateChanged -= OnStateChanged;

    void OnStateChanged(GameState previous, GameState next)
        => panel.SetActive(next == GameState.Paused);

    void Update()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        if (Input.GetKeyDown(KeyCode.Escape) &&
            (gm.IsGameplayActive || gm.State == GameState.Paused))
            gm.TogglePause();

        if (gm.State != GameState.Paused) return;

        if (Input.GetKeyDown(KeyCode.R)) gm.RestartRun();
        if (Input.GetKeyDown(KeyCode.M)) gm.QuitToMenu();
    }

    void BuildPanel(Transform canvas)
    {
        panel = UiFactory.FullscreenPanel(canvas, "PausePanel", new Color(0f, 0f, 0f, 0.72f));
        var t = panel.transform;

        UiFactory.Text(t, "Title", "PAUSADO",
            48f, new Vector2(0.5f, 0.5f), new Vector2(0, 120), new Vector2(500, 70), UiFactory.Cyan);

        UiFactory.Button(t, "ResumeButton", "CONTINUAR  [ESC]",
            new Vector2(0.5f, 0.5f), new Vector2(0, 30), new Vector2(320, 48),
            () => GameManager.Instance.TogglePause());

        UiFactory.Button(t, "RestartButton", "REINICIAR  [R]",
            new Vector2(0.5f, 0.5f), new Vector2(0, -34), new Vector2(320, 48),
            () => GameManager.Instance.RestartRun());

        UiFactory.Button(t, "MenuButton", "MENU PRINCIPAL  [M]",
            new Vector2(0.5f, 0.5f), new Vector2(0, -98), new Vector2(320, 48),
            () => GameManager.Instance.QuitToMenu());
    }
}
