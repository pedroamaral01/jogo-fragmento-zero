using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Anúncios rápidos no topo da tela (evolução, poderes desbloqueados).
/// Fila com fade in/out; mostra um por vez.
/// </summary>
public class ToastUI : MonoBehaviour
{
    const float ShowSecs = 2.4f;
    const float FadeIn   = 0.25f;
    const float FadeOut  = 0.5f;

    readonly Queue<(string message, Color color)> pending = new Queue<(string, Color)>();

    TMP_Text    text;
    CanvasGroup group;
    float       timer;

    void Awake()
    {
        var canvas = UiFactory.CreateCanvas(transform, "ToastCanvas", 40);
        text = UiFactory.Text(canvas.transform, "Toast", "",
            26f, new Vector2(0.5f, 1f), new Vector2(0, -130), new Vector2(900, 44), Color.white);
        group = text.gameObject.AddComponent<CanvasGroup>();
        group.alpha = 0f;
    }

    void OnEnable()
    {
        GameEvents.EvolutionChanged  += OnEvolutionChanged;
        GameEvents.PowerUnlocked     += OnPowerUnlocked;
        GameEvents.DifficultyChanged += OnDifficultyChanged;
        GameEvents.StateChanged      += OnStateChanged;
    }

    void OnDisable()
    {
        GameEvents.EvolutionChanged  -= OnEvolutionChanged;
        GameEvents.PowerUnlocked     -= OnPowerUnlocked;
        GameEvents.DifficultyChanged -= OnDifficultyChanged;
        GameEvents.StateChanged      -= OnStateChanged;
    }

    void OnDifficultyChanged(int tier)
        => Enqueue($"⚠ A FENDA ESTÁ MAIS INSTÁVEL — NÍVEL {tier}", new Color(1f, 0.45f, 0.3f));

    void OnEvolutionChanged(int level, string stageName)
        => Enqueue($"✦ EVOLUÇÃO — {stageName.ToUpper()} ✦", new Color(1f, 0.85f, 0.2f));

    void OnPowerUnlocked(PowerBase power)
        => Enqueue($"NOVO PODER: {power.DisplayName} [{power.Key}]", power.ThemeColor);

    void OnStateChanged(GameState previous, GameState next)
    {
        // Fila não sobrevive à saída do gameplay
        if (next == GameState.Menu || next == GameState.GameOver)
        {
            pending.Clear();
            timer = 0f;
            group.alpha = 0f;
        }
    }

    void Enqueue(string message, Color color) => pending.Enqueue((message, color));

    void Update()
    {
        if (timer > 0f)
        {
            timer -= Time.deltaTime;
            float shown = ShowSecs - timer;
            float alpha = 1f;
            if (shown < FadeIn)        alpha = shown / FadeIn;
            else if (timer < FadeOut)  alpha = timer / FadeOut;
            group.alpha = Mathf.Clamp01(alpha);
            return;
        }

        group.alpha = 0f;

        if (pending.Count > 0 && GameManager.Instance != null && GameManager.Instance.IsGameplayActive)
        {
            var (message, color) = pending.Dequeue();
            text.text  = message;
            text.color = color;
            timer      = ShowSecs;
        }
    }
}
