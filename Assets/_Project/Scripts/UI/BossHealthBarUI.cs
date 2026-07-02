using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>Barra de vida do chefão no topo da tela, com nome e fases.</summary>
public class BossHealthBarUI : MonoBehaviour
{
    GameObject root;
    Image      fill;
    TMP_Text   nameText;
    BossBase   boss;

    void Awake()
    {
        var canvas = UiFactory.CreateCanvas(transform, "BossBarCanvas", 45);

        var rt = UiFactory.Rect(canvas.transform, "BossBar",
            new Vector2(0.5f, 1f), new Vector2(0, -46), new Vector2(560, 52));
        root = rt.gameObject;

        nameText = UiFactory.Text(root.transform, "Name", "",
            16f, new Vector2(0.5f, 1f), new Vector2(0, -8), new Vector2(560, 20),
            new Color(1f, 0.6f, 0.4f));

        var bgRt = UiFactory.Rect(root.transform, "BarBG",
            new Vector2(0.5f, 0f), new Vector2(0, 10), new Vector2(520, 13));
        bgRt.gameObject.AddComponent<Image>().color = new Color(0.05f, 0.05f, 0.05f, 0.85f);

        var fillGo = new GameObject("Fill");
        fillGo.transform.SetParent(bgRt, false);
        var fillRt = fillGo.AddComponent<RectTransform>();
        fillRt.anchorMin = Vector2.zero;
        fillRt.anchorMax = Vector2.one;
        fillRt.offsetMin = Vector2.zero;
        fillRt.offsetMax = Vector2.zero;
        fill = fillGo.AddComponent<Image>();
        fill.color      = new Color(1f, 0.4f, 0.2f);
        fill.type       = Image.Type.Filled;
        fill.fillMethod = Image.FillMethod.Horizontal;

        root.SetActive(false);
    }

    void OnEnable()
    {
        GameEvents.BossSpawned  += OnBossSpawned;
        GameEvents.BossDefeated += OnBossDefeated;
        GameEvents.StateChanged += OnStateChanged;
    }

    void OnDisable()
    {
        GameEvents.BossSpawned  -= OnBossSpawned;
        GameEvents.BossDefeated -= OnBossDefeated;
        GameEvents.StateChanged -= OnStateChanged;
    }

    void OnBossSpawned(BossBase spawned)
    {
        boss = spawned;
        nameText.text = spawned.DisplayName;
        fill.fillAmount = 1f;
        root.SetActive(true);
    }

    void OnBossDefeated(BossBase defeated)
    {
        boss = null;
        root.SetActive(false);
    }

    void OnStateChanged(GameState previous, GameState next)
    {
        if (next == GameState.Menu || next == GameState.GameOver)
        {
            boss = null;
            root.SetActive(false);
        }
    }

    void Update()
    {
        if (boss == null) return;
        fill.fillAmount = Mathf.Lerp(fill.fillAmount, (float)boss.Hp / boss.MaxHp,
                                     12f * Time.deltaTime);
    }
}
