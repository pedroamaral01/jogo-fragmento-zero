using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Dono do estado global (GameState), do score e da velocidade da corrida.
/// Toda transição de estado passa por SetState, que anuncia via GameEvents.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // Sobrevive ao reload de cena: quando true (restart), pula o menu e inicia direto.
    static bool autoStartRun;

    public GameState State { get; private set; } = GameState.Menu;

    /// <summary>Gameplay rodando de fato (mundo se move, spawns, input do player).</summary>
    public bool IsGameplayActive => State == GameState.Running || State == GameState.BossFight;

    public float Score { get; private set; }
    public float Speed { get; private set; }

    GameState stateBeforePause = GameState.Running;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        Time.timeScale = 1f;
        Speed = GameConfig.I.baseSpeed;
    }

    void OnEnable()  => GameEvents.CrystalCollected += OnCrystalCollected;
    void OnDisable() => GameEvents.CrystalCollected -= OnCrystalCollected;

    void OnCrystalCollected(Vector3 pos, float energy, float score) => AddScore(score);

    void Start()
    {
        if (autoStartRun) { autoStartRun = false; StartRun(); }
    }

    void Update()
    {
        // Score = distância percorrida; só acumula (e acelera) na corrida normal.
        // Durante BossFight a velocidade fica travada — a arena "segura" o ritmo.
        if (State == GameState.Running)
        {
            var cfg = GameConfig.I;
            Score += Speed * cfg.scorePerSpeed * Time.deltaTime;
            Speed  = cfg.baseSpeed + Score * cfg.speedPerScore;
        }
    }

    public void AddScore(float amount) => Score += amount;

    // ── Transições ──────────────────────────────────────────────────────────

    public void StartRun()
    {
        if (State != GameState.Menu) return;
        SetState(GameState.Running);
    }

    public void EnterBossFight()
    {
        if (State != GameState.Running) return;
        SetState(GameState.BossFight);
    }

    public void ExitBossFight()
    {
        if (State != GameState.BossFight) return;
        SetState(GameState.Running);
    }

    public void TogglePause()
    {
        if (State == GameState.Paused)
        {
            Time.timeScale = 1f;
            SetState(stateBeforePause);
        }
        else if (IsGameplayActive)
        {
            stateBeforePause = State;
            Time.timeScale   = 0f;
            SetState(GameState.Paused);
        }
    }

    public void TriggerGameOver()
    {
        if (State == GameState.GameOver) return;
        SetState(GameState.GameOver);
    }

    public void RestartRun()
    {
        autoStartRun   = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitToMenu()
    {
        autoStartRun   = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    void SetState(GameState next)
    {
        if (State == next) return;
        GameState previous = State;
        State = next;
        GameEvents.RaiseStateChanged(previous, next);
    }
}
