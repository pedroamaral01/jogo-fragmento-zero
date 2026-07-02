using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Tela de classificação indicativa + menu principal.
/// Vive na raiz persistente (RuntimeUI); constrói sua UI por código e
/// aparece sempre que o jogo está no estado Menu.
/// </summary>
public class MenuController : MonoBehaviour
{
    const float RatingSeconds = 3f;

    GameObject ratingPanel;
    GameObject menuPanel;
    TMP_Text   highScoreText;

    float ratingTimer;
    bool  ratingShown;   // selo só aparece uma vez por sessão

    void Awake()
    {
        var canvas = UiFactory.CreateCanvas(transform, "MenuCanvas", 50);
        BuildRatingPanel(canvas.transform);
        BuildMenuPanel(canvas.transform);
        ratingPanel.SetActive(false);
        menuPanel.SetActive(false);
    }

    void OnEnable()
    {
        GameEvents.StateChanged      += OnStateChanged;
        SceneManager.sceneLoaded     += OnSceneLoaded;
    }

    void OnDisable()
    {
        GameEvents.StateChanged      -= OnStateChanged;
        SceneManager.sceneLoaded     -= OnSceneLoaded;
    }

    void Start() => Refresh();

    void OnSceneLoaded(Scene s, LoadSceneMode m) => Refresh();

    void OnStateChanged(GameState previous, GameState next) => Refresh();

    void Refresh()
    {
        bool inMenu = GameManager.Instance != null &&
                      GameManager.Instance.State == GameState.Menu;

        if (!inMenu)
        {
            ratingPanel.SetActive(false);
            menuPanel.SetActive(false);
            return;
        }

        if (!ratingShown)
        {
            ratingTimer = RatingSeconds;
            ratingPanel.SetActive(true);
            menuPanel.SetActive(false);
        }
        else
        {
            ShowMenu();
        }
    }

    void Update()
    {
        if (ratingPanel.activeSelf)
        {
            ratingTimer -= Time.deltaTime;
            if (ratingTimer <= 0f || Input.anyKeyDown)
            {
                ratingShown = true;
                ShowMenu();
            }
            return;
        }

        if (menuPanel.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Return)) StartGame();
            if (Input.GetKeyDown(KeyCode.Escape)) QuitGame();
        }
    }

    void ShowMenu()
    {
        ratingPanel.SetActive(false);
        menuPanel.SetActive(true);

        int record = HighScore.Best;
        highScoreText.text = record > 0 ? $"RECORDE: {record} m" : "";
    }

    void StartGame()
    {
        menuPanel.SetActive(false);
        GameManager.Instance.StartRun();
    }

    void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // ── Construção da UI ────────────────────────────────────────────────────

    void BuildRatingPanel(Transform canvas)
    {
        ratingPanel = UiFactory.FullscreenPanel(canvas, "RatingPanel", Color.black);
        var t = ratingPanel.transform;

        // ClassInd 10 anos = selo azul
        UiFactory.RatingSeal(t, "10", new Color(0.0f, 0.45f, 0.85f), new Vector2(0, 80), 140f);

        UiFactory.Text(t, "RatingTitle", "CLASSIFICAÇÃO INDICATIVA",
            26f, new Vector2(0.5f, 0.5f), new Vector2(0, -40), new Vector2(700, 40), Color.white);

        UiFactory.Text(t, "RatingDesc", "Não recomendado para menores de 10 anos\nViolência fantasiosa",
            18f, new Vector2(0.5f, 0.5f), new Vector2(0, -100), new Vector2(700, 60),
            new Color(0.8f, 0.8f, 0.8f));

        UiFactory.Text(t, "RatingEsrb", "ESRB: E10+  •  Fantasy Violence",
            14f, new Vector2(0.5f, 0.5f), new Vector2(0, -160), new Vector2(700, 30),
            new Color(0.55f, 0.55f, 0.55f));
    }

    void BuildMenuPanel(Transform canvas)
    {
        menuPanel = UiFactory.FullscreenPanel(canvas, "MenuPanel", UiFactory.Dark);
        var t = menuPanel.transform;

        UiFactory.Text(t, "Title", "FRAGMENTO ZERO",
            64f, new Vector2(0.5f, 0.5f), new Vector2(0, 160), new Vector2(900, 90), UiFactory.Cyan);

        UiFactory.Text(t, "Subtitle", "— MODO FENDA INFINITA —",
            22f, new Vector2(0.5f, 0.5f), new Vector2(0, 100), new Vector2(700, 40),
            new Color(0.75f, 0.4f, 1f));

        highScoreText = UiFactory.Text(t, "HighScore", "",
            20f, new Vector2(0.5f, 0.5f), new Vector2(0, 52), new Vector2(500, 32),
            new Color(1f, 0.85f, 0.2f));

        UiFactory.Button(t, "PlayButton", "INICIAR CORRIDA  [ENTER]",
            new Vector2(0.5f, 0.5f), new Vector2(0, -20), new Vector2(340, 52), StartGame);

        UiFactory.Button(t, "QuitButton", "SAIR  [ESC]",
            new Vector2(0.5f, 0.5f), new Vector2(0, -90), new Vector2(340, 52), QuitGame);

        UiFactory.Text(t, "Controls",
            "↑ / ↓  mover entre faixas      A  poder de fogo      D  poder de gelo",
            15f, new Vector2(0.5f, 0f), new Vector2(0, 60), new Vector2(900, 30),
            new Color(0.6f, 0.6f, 0.6f));

        UiFactory.Text(t, "Credits", "Demo — Fragmento Zero © UFOP",
            12f, new Vector2(0.5f, 0f), new Vector2(0, 24), new Vector2(500, 24),
            new Color(0.4f, 0.4f, 0.4f));
    }
}
