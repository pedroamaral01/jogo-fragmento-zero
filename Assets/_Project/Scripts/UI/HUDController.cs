using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// HUD de gameplay ancorada no canto superior esquerdo, 100% construída em
/// runtime. Todas as barras animam suavemente (o preenchimento persegue o
/// valor real) e piscam quando o recurso está criticamente baixo.
/// Score/velocidade ficam no canto superior direito.
/// </summary>
public class HUDController : MonoBehaviour
{
    static readonly Color EnergyHigh = ArtPalette.Cyan;       // #66FCF1
    static readonly Color EnergyMid  = new Color(1f, 0.85f, 0.25f);
    static readonly Color EnergyLow  = ArtPalette.AlertRed;   // #FF003C
    static readonly Color BarBg      = new Color(0.03f, 0.04f, 0.07f, 0.85f);
    static readonly Color LabelDim   = new Color(0.65f, 0.65f, 0.65f);

    // Coluna esquerda: x do centro das barras (largura BarW a partir de x=14)
    const float BarW = 232f;
    const float BarX = 14f + BarW / 2f;

    const float FillLerpSpeed = 9f;
    const float PulseSpeed    = 9f;

    class Bar
    {
        public GameObject root;
        public Image      fill;
        public TMP_Text   label;
        public float      shown;   // valor exibido — persegue o alvo (lerp)
    }

    Bar energyBar;
    readonly List<(PowerBase power, Bar bar)> powerBars = new List<(PowerBase, Bar)>();

    TMP_Text scoreText;
    TMP_Text speedText;
    TMP_Text evolutionLabel;
    Image    evolutionFill;

    PlayerController player;
    EvolutionSystem  evolution;
    Canvas           hudCanvas;

    void Awake() => hudCanvas = GetComponent<Canvas>();

    void OnEnable()
    {
        GameEvents.StateChanged  += OnStateChanged;
        GameEvents.PowerUnlocked += OnPowerUnlocked;
    }

    void OnDisable()
    {
        GameEvents.StateChanged  -= OnStateChanged;
        GameEvents.PowerUnlocked -= OnPowerUnlocked;
    }

    void Start()
    {
        player = PlayerController.Instance;
        if (player != null) evolution = player.GetComponent<EvolutionSystem>();

        // Elementos legados da cena foram substituídos pela HUD em runtime
        HideLegacy("PowerBars");
        HideLegacy("EnergyFill");
        HideLegacy("ScoreText");

        BuildTopRight();
        BuildEnergyBar();
        BuildPowerBars();
        BuildEvolutionDisplay();
        RefreshVisibility(GameManager.Instance.State);
    }

    void HideLegacy(string childName)
    {
        var legacy = transform.Find(childName);
        if (legacy != null) legacy.gameObject.SetActive(false);
    }

    void OnStateChanged(GameState previous, GameState next) => RefreshVisibility(next);

    void OnPowerUnlocked(PowerBase power) => BuildPowerBars();

    // HUD de gameplay não aparece no menu (o canvas desliga, o controller vive)
    void RefreshVisibility(GameState state)
    {
        if (hudCanvas != null) hudCanvas.enabled = state != GameState.Menu;
    }

    void Update()
    {
        if (GameManager.Instance.IsGameplayActive)
            RefreshHUD();
    }

    // ── Atualização por frame ───────────────────────────────────────────────

    void RefreshHUD()
    {
        if (player == null) return;

        // Energia: gradiente ciano → amarelo → vermelho + pulso quando crítica
        float maxEnergy = GameConfig.I.maxEnergy;
        float pct       = player.Energy / maxEnergy;
        bool  critical  = pct < GameConfig.I.lowEnergyWarning;

        Color energyColor = pct > 0.5f
            ? Color.Lerp(EnergyMid, EnergyHigh, (pct - 0.5f) * 2f)
            : Color.Lerp(EnergyLow, EnergyMid, pct * 2f);

        UpdateBar(energyBar, pct, energyColor,
            $"ENERGIA  {Mathf.FloorToInt(player.Energy)}", critical, ready: true);

        foreach (var (power, bar) in powerBars)
        {
            if (power == null) continue;
            UpdateBar(bar, power.HudFillRatio, power.ThemeColor,
                power.HudLabel, power.HudCritical, power.IsReady);
        }

        if (scoreText != null) scoreText.text = $"{Mathf.FloorToInt(GameManager.Instance.Score)} m";
        if (speedText != null) speedText.text = $"{GameManager.Instance.Speed:F1}x";

        if (evolution != null && evolutionLabel != null)
        {
            evolutionLabel.text      = $"NV{evolution.Level}  {evolution.Current.name.ToUpper()}";
            evolutionLabel.color     = evolution.Current.bodyColor;
            evolutionFill.fillAmount = Mathf.Lerp(evolutionFill.fillAmount,
                                                  evolution.ProgressToNext,
                                                  FillLerpSpeed * Time.deltaTime);
            evolutionFill.color      = evolution.Current.bodyColor;
        }
    }

    void UpdateBar(Bar bar, float target, Color color, string text, bool critical, bool ready)
    {
        if (bar == null) return;

        // O preenchimento persegue o valor real — dá leitura de "subindo/descendo"
        bar.shown = Mathf.Lerp(bar.shown, Mathf.Clamp01(target), FillLerpSpeed * Time.deltaTime);
        bar.fill.fillAmount = bar.shown;

        // Crítico: barra pisca; indisponível: barra apagada
        float alpha = 0.85f;
        if (critical)    alpha = 0.35f + 0.5f * Mathf.Abs(Mathf.Sin(Time.time * PulseSpeed));
        else if (!ready) alpha = 0.35f;
        bar.fill.color = new Color(color.r, color.g, color.b, alpha);

        bar.label.text  = text;
        bar.label.color = critical ? EnergyLow : (ready ? Color.white : LabelDim);
    }

    // ── Construção ──────────────────────────────────────────────────────────

    void BuildTopRight()
    {
        scoreText = UiFactory.Text(transform, "ScoreDisplay", "0 m",
            26f, new Vector2(1f, 1f), new Vector2(-100, -26), new Vector2(180, 34),
            Color.white, TextAlignmentOptions.Right);

        speedText = UiFactory.Text(transform, "SpeedDisplay", "",
            14f, new Vector2(1f, 1f), new Vector2(-100, -52), new Vector2(180, 20),
            LabelDim, TextAlignmentOptions.Right);
    }

    void BuildEnergyBar()
    {
        energyBar = CreateBar("EnergyBar", -22f, 18f, 12f);
        energyBar.shown = 0.6f;
    }

    void BuildPowerBars()
    {
        foreach (var (_, bar) in powerBars)
            if (bar.root != null) Destroy(bar.root);
        powerBars.Clear();

        if (player == null) return;

        int row = 0;
        foreach (var power in player.GetComponents<PowerBase>())
        {
            if (!power.IsUnlocked) continue;

            var bar = CreateBar($"PowerBar_{power.DisplayName}", -48f - row * 21f, 13f, 10f);
            bar.shown = power.HudFillRatio;
            powerBars.Add((power, bar));
            row++;
        }
    }

    void BuildEvolutionDisplay()
    {
        evolutionLabel = UiFactory.Text(transform, "EvolutionLabel", "",
            12f, new Vector2(0f, 1f), new Vector2(BarX, -142f), new Vector2(BarW, 16),
            Color.white, TextAlignmentOptions.Left);

        var bgRt = UiFactory.Rect(transform, "EvolutionBarBG",
            new Vector2(0f, 1f), new Vector2(BarX, -156f), new Vector2(BarW, 5));
        bgRt.gameObject.AddComponent<Image>().color = BarBg;

        evolutionFill = CreateFill(bgRt, Color.white);
    }

    /// <summary>Barra padrão da coluna esquerda: fundo, preenchimento e rótulo interno.</summary>
    Bar CreateBar(string name, float y, float height, float fontSize)
    {
        var bgRt = UiFactory.Rect(transform, name,
            new Vector2(0f, 1f), new Vector2(BarX, y), new Vector2(BarW, height));
        var bg = bgRt.gameObject.AddComponent<Image>();
        bg.color = BarBg;

        var outline = bgRt.gameObject.AddComponent<Outline>();
        outline.effectColor    = new Color(0f, 0f, 0f, 0.9f);
        outline.effectDistance = new Vector2(1f, -1f);

        var fill = CreateFill(bgRt, Color.white);

        // Rótulo dentro da barra, alinhado à esquerda com margem
        var labelGo = new GameObject("Label");
        labelGo.transform.SetParent(bgRt, false);
        var labelRt = labelGo.AddComponent<RectTransform>();
        labelRt.anchorMin = Vector2.zero;
        labelRt.anchorMax = Vector2.one;
        labelRt.offsetMin = new Vector2(7f, 0f);
        labelRt.offsetMax = new Vector2(-7f, 0f);
        var label = labelGo.AddComponent<TextMeshProUGUI>();
        label.fontSize  = fontSize;
        label.color     = Color.white;
        label.alignment = TextAlignmentOptions.MidlineLeft;

        return new Bar { root = bgRt.gameObject, fill = fill, label = label };
    }

    static Image CreateFill(RectTransform parent, Color color)
    {
        var go = new GameObject("Fill");
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        var fill = go.AddComponent<Image>();
        // Image sem sprite ignora fillAmount — sprite branco é obrigatório
        fill.sprite     = RuntimeSprites.Square;
        fill.color      = color;
        fill.type       = Image.Type.Filled;
        fill.fillMethod = Image.FillMethod.Horizontal;
        fill.fillAmount = 1f;
        return fill;
    }
}
