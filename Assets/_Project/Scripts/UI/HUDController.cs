using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDController : MonoBehaviour
{
    [Header("Energia")]
    [SerializeField] Image    energyFill;
    [SerializeField] TMP_Text energyLabel;
    [SerializeField] Color    colorNormal = new Color(0f, 1f, 1f);
    [SerializeField] Color    colorLow    = new Color(1f, 0.27f, 0.27f);

    [Header("Pontuação")]
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text speedText;

    // Slots de poder são construídos em runtime a partir dos PowerBase do player —
    // a HUD não conhece poderes concretos.
    class PowerSlot
    {
        public PowerBase power;
        public GameObject root;
        public Image fill;
        public TMP_Text label;
    }

    readonly List<PowerSlot> slots = new List<PowerSlot>();

    PlayerController player;
    Canvas           hudCanvas;

    void Awake() => hudCanvas = GetComponent<Canvas>();

    void OnEnable()  => GameEvents.StateChanged += OnStateChanged;
    void OnDisable() => GameEvents.StateChanged -= OnStateChanged;

    void Start()
    {
        player = PlayerController.Instance;

        // Painel legado da cena (barras fixas Fogo/Gelo) foi substituído pelos slots
        var legacy = transform.Find("PowerBars");
        if (legacy != null) legacy.gameObject.SetActive(false);

        BuildPowerSlots();
        RefreshVisibility(GameManager.Instance.State);
    }

    void OnStateChanged(GameState previous, GameState next)
    {
        RefreshVisibility(next);
    }

    // HUD de gameplay não aparece no menu (o canvas desliga, o controller continua vivo)
    void RefreshVisibility(GameState state)
    {
        if (hudCanvas != null) hudCanvas.enabled = state != GameState.Menu;
    }

    void Update()
    {
        // Valores contínuos (energia/score/cooldown) mudam todo frame — polling é adequado
        if (GameManager.Instance.IsGameplayActive)
            RefreshHUD();
    }

    void RefreshHUD()
    {
        if (player == null) return;

        float pct = player.Energy / GameConfig.I.maxEnergy;

        if (energyFill  != null) energyFill.fillAmount = pct;
        if (energyLabel != null) energyLabel.text = $"ENERGIA {Mathf.FloorToInt(player.Energy)}";
        if (energyFill  != null) energyFill.color = pct < GameConfig.I.lowEnergyWarning ? colorLow : colorNormal;

        if (scoreText != null) scoreText.text = $"{Mathf.FloorToInt(GameManager.Instance.Score)} m";
        if (speedText != null) speedText.text = $"{GameManager.Instance.Speed:F1}x";

        foreach (var slot in slots)
        {
            if (slot.power == null) continue;
            slot.fill.fillAmount = slot.power.HudFillRatio;
            slot.label.text      = slot.power.HudLabel;
            slot.label.color     = slot.power.IsReady ? Color.white : new Color(0.65f, 0.65f, 0.65f);
        }
    }

    // ── Slots de poder ──────────────────────────────────────────────────────

    public void BuildPowerSlots()
    {
        foreach (var slot in slots)
            if (slot.root != null) Destroy(slot.root);
        slots.Clear();

        if (player == null) return;

        int row = 0;
        foreach (var power in player.GetComponents<PowerBase>())
        {
            if (!power.IsUnlocked) continue;
            slots.Add(CreateSlot(power, row++));
        }
    }

    PowerSlot CreateSlot(PowerBase power, int row)
    {
        float y = 50f + row * 26f;

        var root = UiFactory.Rect(transform, $"PowerSlot_{power.DisplayName}",
            Vector2.zero, new Vector2(10, y), new Vector2(240, 22)).gameObject;
        var rootRt = root.GetComponent<RectTransform>();
        rootRt.pivot = new Vector2(0f, 0f);

        // Fundo da barra
        var bgRt = UiFactory.Rect(root.transform, "BarBG",
            new Vector2(0f, 0.5f), new Vector2(55, 0), new Vector2(110, 10));
        bgRt.gameObject.AddComponent<Image>().color = new Color(0.05f, 0.05f, 0.05f, 0.75f);

        // Preenchimento (cooldown/duração) na cor do poder
        var fillGo = new GameObject("Fill");
        fillGo.transform.SetParent(bgRt, false);
        var fillRt = fillGo.AddComponent<RectTransform>();
        fillRt.anchorMin = Vector2.zero;
        fillRt.anchorMax = Vector2.one;
        fillRt.offsetMin = Vector2.zero;
        fillRt.offsetMax = Vector2.zero;
        var fill = fillGo.AddComponent<Image>();
        fill.color      = power.ThemeColor;
        fill.type       = Image.Type.Filled;
        fill.fillMethod = Image.FillMethod.Horizontal;

        var label = UiFactory.Text(root.transform, "Label", power.HudLabel,
            11f, new Vector2(0f, 0.5f), new Vector2(186, 0), new Vector2(140, 16),
            Color.white, TextAlignmentOptions.Left);

        return new PowerSlot { power = power, root = root, fill = fill, label = label };
    }
}
