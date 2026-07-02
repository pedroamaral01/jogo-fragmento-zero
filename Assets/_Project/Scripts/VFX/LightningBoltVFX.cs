using UnityEngine;

/// <summary>
/// Relâmpago procedural entre dois pontos: núcleo grosso + halo translúcido
/// (LineRenderer duplo) com jitter perpendicular, mais um flash circular no
/// ponto de impacto para deixar claro qual inimigo foi atingido. Esmaece e
/// se autodestrói. Totalmente gerado em runtime — nenhum asset necessário.
/// </summary>
public class LightningBoltVFX : MonoBehaviour
{
    const int   Segments = 9;
    const float Jitter   = 0.35f;
    const float LifeSecs = 0.35f;

    static Material sharedMaterial;

    LineRenderer core;
    LineRenderer glow;
    Transform    impactFlash;
    SpriteRenderer impactRenderer;
    float life = LifeSecs;
    Color baseColor;

    public static void Spawn(Vector3 from, Vector3 to, Color color)
    {
        var go = new GameObject("LightningBolt");
        go.AddComponent<LightningBoltVFX>().Init(from, to, color);
    }

    void Init(Vector3 from, Vector3 to, Color color)
    {
        baseColor = color;

        if (sharedMaterial == null)
            sharedMaterial = new Material(Shader.Find("Sprites/Default"));

        Vector3 dir  = to - from;
        Vector3 perp = Vector3.Cross(dir, Vector3.forward).normalized;

        var jaggedPoints = new Vector3[Segments + 1];
        for (int i = 0; i <= Segments; i++)
        {
            float   t     = (float)i / Segments;
            Vector3 point = Vector3.Lerp(from, to, t);
            if (i > 0 && i < Segments)
                point += perp * Random.Range(-Jitter, Jitter);
            jaggedPoints[i] = point;
        }

        // Halo largo e translúcido por baixo — dá volume elétrico ao traço
        glow = MakeLine("Glow", jaggedPoints, 0.32f, 0.10f, 3, color, 0.45f);
        // Núcleo fino e brilhante por cima — o "fio" nítido do relâmpago
        core = MakeLine("Core", jaggedPoints, 0.14f, 0.045f, 4, Color.Lerp(color, Color.white, 0.6f), 1f);

        // Flash circular no alvo — identifica claramente quem foi atingido
        var flashGo = new GameObject("Impact");
        flashGo.transform.SetParent(transform, false);
        flashGo.transform.position   = to;
        flashGo.transform.localScale = Vector3.one * 0.5f;
        impactRenderer = flashGo.AddComponent<SpriteRenderer>();
        impactRenderer.sprite       = RuntimeSprites.Glow;
        impactRenderer.color        = color;
        impactRenderer.sortingOrder = 6;
        impactFlash = flashGo.transform;
    }

    LineRenderer MakeLine(string name, Vector3[] points, float startWidth, float endWidth,
        int sortingOrder, Color color, float alphaMult)
    {
        var go = new GameObject(name);
        go.transform.SetParent(transform, false);
        var lr = go.AddComponent<LineRenderer>();
        lr.material      = sharedMaterial;
        lr.positionCount = points.Length;
        lr.startWidth    = startWidth;
        lr.endWidth      = endWidth;
        lr.sortingOrder  = sortingOrder;
        var c = new Color(color.r, color.g, color.b, alphaMult);
        lr.startColor = c;
        lr.endColor   = c;
        lr.SetPositions(points);
        return lr;
    }

    void Update()
    {
        life -= Time.deltaTime;
        if (life <= 0f) { Destroy(gameObject); return; }

        float t01   = life / LifeSecs;             // 1 → 0
        float alpha = Mathf.Clamp01(t01 * 1.6f);    // some quase no fim, sem fade linear longo

        SetLineAlpha(core, 1f * alpha);
        SetLineAlpha(glow, 0.45f * alpha);

        // Flash de impacto: pulso rápido que expande e esmaece
        float growT = 1f - t01;
        impactFlash.localScale = Vector3.one * Mathf.Lerp(0.5f, 1.6f, growT);
        impactRenderer.color   = new Color(baseColor.r, baseColor.g, baseColor.b, alpha * 0.9f);
    }

    static void SetLineAlpha(LineRenderer lr, float alpha)
    {
        var start = lr.startColor;
        var end   = lr.endColor;
        lr.startColor = new Color(start.r, start.g, start.b, alpha);
        lr.endColor   = new Color(end.r, end.g, end.b, alpha);
    }
}
