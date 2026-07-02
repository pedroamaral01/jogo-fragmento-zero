using UnityEngine;
using TMPro;

/// <summary>
/// Tela de Game Over: distância final, recorde persistente e opções
/// de reinício rápido (R) ou volta ao menu (M).
/// </summary>
public class GameOverController : MonoBehaviour
{
    GameObject panel;
    TMP_Text   scoreText;
    TMP_Text   recordText;

    void Awake()
    {
        var canvas = UiFactory.CreateCanvas(transform, "GameOverCanvas", 55);
        BuildPanel(canvas.transform);
        panel.SetActive(false);
    }

    void OnEnable()  => GameEvents.StateChanged += OnStateChanged;
    void OnDisable() => GameEvents.StateChanged -= OnStateChanged;

    void OnStateChanged(GameState previous, GameState next)
    {
        if (next == GameState.GameOver) Show();
        else panel.SetActive(false);
    }

    void Show()
    {
        int  meters    = Mathf.FloorToInt(GameManager.Instance.Score);
        bool newRecord = HighScore.Submit(meters);

        scoreText.text  = $"{meters} metros percorridos";
        recordText.text = newRecord
            ? "★ NOVO RECORDE! ★"
            : $"Recorde: {HighScore.Best} m";
        recordText.color = newRecord
            ? new Color(1f, 0.85f, 0.2f)
            : new Color(0.7f, 0.7f, 0.7f);

        panel.SetActive(true);
    }

    void Update()
    {
        if (GameManager.Instance == null ||
            GameManager.Instance.State != GameState.GameOver) return;

        if (Input.GetKeyDown(KeyCode.R)) GameManager.Instance.RestartRun();
        if (Input.GetKeyDown(KeyCode.M)) GameManager.Instance.QuitToMenu();
    }

    void BuildPanel(Transform canvas)
    {
        panel = UiFactory.FullscreenPanel(canvas, "GameOverPanel", new Color(0.08f, 0f, 0.02f, 0.85f));
        var t = panel.transform;

        UiFactory.Text(t, "Title", "GAME OVER",
            56f, new Vector2(0.5f, 0.5f), new Vector2(0, 140), new Vector2(600, 80),
            new Color(1f, 0.3f, 0.25f));

        scoreText = UiFactory.Text(t, "Score", "",
            26f, new Vector2(0.5f, 0.5f), new Vector2(0, 70), new Vector2(600, 40), Color.white);

        recordText = UiFactory.Text(t, "Record", "",
            20f, new Vector2(0.5f, 0.5f), new Vector2(0, 30), new Vector2(600, 32), Color.white);

        UiFactory.Button(t, "RestartButton", "CORRER DE NOVO  [R]",
            new Vector2(0.5f, 0.5f), new Vector2(0, -50), new Vector2(340, 50),
            () => GameManager.Instance.RestartRun());

        UiFactory.Button(t, "MenuButton", "MENU PRINCIPAL  [M]",
            new Vector2(0.5f, 0.5f), new Vector2(0, -114), new Vector2(340, 50),
            () => GameManager.Instance.QuitToMenu());
    }
}
