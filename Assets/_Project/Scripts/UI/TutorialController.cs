using UnityEngine;
using TMPro;

/// <summary>
/// Tutorial contextual da primeira corrida. Passos bloqueantes congelam o
/// tempo (timeScale 0) até a tecla pedida; passos informativos são banners
/// temporários. Concluído uma vez, não aparece mais (PlayerPrefs).
/// </summary>
public class TutorialController : MonoBehaviour
{
    /// <summary>Pausa/HUD consultam para não competir com o congelamento do tutorial.</summary>
    public static bool IsBlocking { get; private set; }

    const string DoneKey = "TutorialDone";

    class Step
    {
        public float     triggerAt;      // segundos de corrida
        public string    message;
        public KeyCode[] completeKeys;   // null = passo informativo
        public float     autoSecs;       // duração do passo informativo
    }

    Step[] steps;
    int    index;
    float  runTimer;
    float  infoTimer;
    bool   done;

    GameObject panel;
    TMP_Text   text;

    void Awake()
    {
        steps = new[]
        {
            new Step { triggerAt = 1.0f, message = "USE  ↑ / ↓  PARA TROCAR DE FAIXA",
                       completeKeys = new[] { KeyCode.UpArrow, KeyCode.DownArrow, KeyCode.Space } },
            new Step { triggerAt = 5.0f, message = "CRISTAIS RECARREGAM SUA ENERGIA — ELA DRENA SEM PARAR!",
                       autoSecs = 4f },
            new Step { triggerAt = 9.0f, message = "PRESSIONE  [A]  PARA DISPARAR FOGO",
                       completeKeys = new[] { KeyCode.A } },
        };

        var canvas = UiFactory.CreateCanvas(transform, "TutorialCanvas", 58);
        panel = UiFactory.Rect(canvas.transform, "TutorialPanel",
            new Vector2(0.5f, 0f), new Vector2(0, 150), new Vector2(820, 64)).gameObject;
        panel.AddComponent<UnityEngine.UI.Image>().color = new Color(0f, 0.05f, 0.12f, 0.85f);

        text = UiFactory.Text(panel.transform, "Text", "",
            22f, new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(800, 60), UiFactory.Cyan);

        panel.SetActive(false);
        done = PlayerPrefs.GetInt(DoneKey, 0) == 1;
    }

    void OnEnable()  => GameEvents.StateChanged += OnStateChanged;
    void OnDisable() => GameEvents.StateChanged -= OnStateChanged;

    void OnStateChanged(GameState previous, GameState next)
    {
        // Nova corrida vinda do menu: reinicia a sequência (se ainda não concluída)
        if (next == GameState.Running && previous == GameState.Menu)
        {
            done     = PlayerPrefs.GetInt(DoneKey, 0) == 1;
            index    = 0;
            runTimer = 0f;
        }

        if (next == GameState.Menu || next == GameState.GameOver)
            HideStep();
    }

    void Update()
    {
        if (done) return;

        var gm = GameManager.Instance;
        if (gm == null || gm.State != GameState.Running)
        {
            if (!IsBlocking) return;
            // corrida terminou no meio de um bloqueio (não deve ocorrer) — solta
            HideStep();
            return;
        }

        if (IsBlocking)
        {
            foreach (var key in steps[index].completeKeys)
            {
                if (Input.GetKeyDown(key))
                {
                    CompleteStep();
                    break;
                }
            }
            return;
        }

        if (panel.activeSelf)  // passo informativo em exibição
        {
            infoTimer -= Time.deltaTime;
            if (infoTimer <= 0f) CompleteStep();
            return;
        }

        runTimer += Time.deltaTime;
        if (index < steps.Length && runTimer >= steps[index].triggerAt)
            ShowStep();
    }

    void ShowStep()
    {
        var step = steps[index];
        panel.SetActive(true);
        text.text = step.message;

        if (step.completeKeys != null)
        {
            IsBlocking     = true;
            Time.timeScale = 0f;
        }
        else
        {
            infoTimer = step.autoSecs;
        }
    }

    void CompleteStep()
    {
        HideStep();
        index++;
        if (index >= steps.Length)
        {
            done = true;
            PlayerPrefs.SetInt(DoneKey, 1);
            PlayerPrefs.Save();
        }
    }

    void HideStep()
    {
        panel.SetActive(false);
        if (IsBlocking)
        {
            IsBlocking = false;
            // só devolve o tempo se o jogo não estiver pausado por outro motivo
            if (GameManager.Instance == null || GameManager.Instance.State != GameState.Paused)
                Time.timeScale = 1f;
        }
    }
}
