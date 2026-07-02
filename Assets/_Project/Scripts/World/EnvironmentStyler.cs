using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Cenário procedural conforme a direção de arte (Etapa 5):
/// - céu = Vazio Cósmico (#0B0C10) aplicado à câmera
/// - nebulosas coloridas suaves em parallax lento (imprevisibilidade mágica)
/// - silhuetas angulares de ruínas da Mente Matriz nas bordas superior e
///   inferior (#1F2833), algumas com luz de alerta vermelha (#FF003C)
/// Vive na raiz persistente; camadas usam o sorting layer Background.
/// </summary>
public class EnvironmentStyler : MonoBehaviour
{
    class Drifter
    {
        public Transform transform;
        public float     speed;      // fator de parallax (fração da velocidade do jogo)
    }

    const float RespawnX = 14f;

    static readonly Color[] NebulaColors =
    {
        new Color(0.36f, 0.20f, 0.62f),   // roxo nebular
        new Color(0.16f, 0.28f, 0.66f),   // azul profundo
        new Color(0.14f, 0.45f, 0.50f),   // ciano apagado
    };

    readonly List<Drifter> drifters = new List<Drifter>();

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        ApplyCameraStyle();
    }

    void OnDisable() => SceneManager.sceneLoaded -= OnSceneLoaded;

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) => ApplyCameraStyle();

    static void ApplyCameraStyle()
    {
        var cam = Camera.main;
        if (cam == null) return;
        cam.clearFlags      = CameraClearFlags.SolidColor;
        cam.backgroundColor = ArtPalette.VoidBlack;
    }

    void Start()
    {
        BuildNebulas();
        BuildRuins();
    }

    void Update()
    {
        // Parallax segue o ritmo da corrida (para junto com pausa/menu via estado)
        float baseSpeed = GameManager.Instance != null && GameManager.Instance.IsGameplayActive
            ? GameManager.Instance.Speed
            : 1.2f;   // deriva sutil no menu

        foreach (var d in drifters)
        {
            d.transform.position += Vector3.left * baseSpeed * d.speed * Time.deltaTime;
            if (d.transform.position.x < -RespawnX)
            {
                var p = d.transform.position;
                d.transform.position = new Vector3(RespawnX, p.y, p.z);
            }
        }
    }

    // ── Nebulosas ───────────────────────────────────────────────────────────

    void BuildNebulas()
    {
        var root = new GameObject("Nebulas");
        root.transform.SetParent(transform, false);

        for (int i = 0; i < 7; i++)
        {
            var go = new GameObject($"Nebula{i}");
            go.transform.SetParent(root.transform, false);
            go.transform.position = new Vector3(
                Random.Range(-RespawnX, RespawnX), Random.Range(-4f, 4f), 10f);

            float scale = Random.Range(6f, 13f);
            go.transform.localScale = new Vector3(scale, scale * Random.Range(0.55f, 0.9f), 1f);

            var sr   = go.AddComponent<SpriteRenderer>();
            var tint = NebulaColors[i % NebulaColors.Length];
            sr.sprite           = RuntimeSprites.Glow;
            sr.color            = new Color(tint.r, tint.g, tint.b, Random.Range(0.05f, 0.13f));
            sr.sortingLayerName = "Background";
            sr.sortingOrder     = -60;

            drifters.Add(new Drifter { transform = go.transform, speed = Random.Range(0.02f, 0.06f) });
        }
    }

    // ── Ruínas da Mente Matriz ──────────────────────────────────────────────

    void BuildRuins()
    {
        var root = new GameObject("Ruins");
        root.transform.SetParent(transform, false);

        for (int i = 0; i < 12; i++)
        {
            bool top = i % 2 == 0;
            var go = new GameObject($"Ruin{i}");
            go.transform.SetParent(root.transform, false);
            go.transform.position = new Vector3(
                Random.Range(-RespawnX, RespawnX),
                (top ? 1f : -1f) * Random.Range(3.4f, 4.6f),
                5f);

            bool spire = Random.value < 0.35f;
            go.transform.localScale = spire
                ? new Vector3(Random.Range(0.25f, 0.5f), Random.Range(1.8f, 3.2f), 1f)
                : new Vector3(Random.Range(1.2f, 3f), Random.Range(1.2f, 2.4f), 1f);
            go.transform.localRotation =
                Quaternion.Euler(0f, 0f, spire ? 0f : Random.Range(-14f, 14f));

            var sr = go.AddComponent<SpriteRenderer>();
            sr.sprite           = RuntimeSprites.Square;
            sr.color            = new Color(ArtPalette.Structure.r, ArtPalette.Structure.g,
                                            ArtPalette.Structure.b, Random.Range(0.55f, 0.85f));
            sr.sortingLayerName = "Background";
            sr.sortingOrder     = -40;

            // Algumas ruínas ainda têm sensores ativos da IA
            if (Random.value < 0.35f) AddAlertLight(go.transform, top);

            drifters.Add(new Drifter { transform = go.transform, speed = Random.Range(0.10f, 0.22f) });
        }
    }

    static void AddAlertLight(Transform parent, bool top)
    {
        var go = new GameObject("AlertLight");
        go.transform.SetParent(parent, false);
        go.transform.localPosition = new Vector3(
            Random.Range(-0.3f, 0.3f), top ? -0.45f : 0.45f, 0f);
        go.transform.localScale = new Vector3(0.12f, 0.12f, 1f);

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite           = RuntimeSprites.Glow;
        sr.color            = new Color(ArtPalette.AlertRed.r, ArtPalette.AlertRed.g,
                                        ArtPalette.AlertRed.b, 0.85f);
        sr.sortingLayerName = "Background";
        sr.sortingOrder     = -39;
    }
}
