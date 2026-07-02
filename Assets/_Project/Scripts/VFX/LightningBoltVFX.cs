using UnityEngine;

/// <summary>
/// Relâmpago procedural entre dois pontos: LineRenderer segmentado com
/// jitter perpendicular, esmaece e se autodestrói. Totalmente gerado em
/// runtime — nenhum asset necessário.
/// </summary>
public class LightningBoltVFX : MonoBehaviour
{
    const int   Segments = 7;
    const float Jitter   = 0.3f;
    const float LifeSecs = 0.25f;

    static Material sharedMaterial;

    LineRenderer line;
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

        line = gameObject.AddComponent<LineRenderer>();
        line.material      = sharedMaterial;
        line.positionCount = Segments + 1;
        line.startWidth    = 0.09f;
        line.endWidth      = 0.03f;
        line.sortingOrder  = 5;
        line.startColor    = color;
        line.endColor      = color;

        Vector3 dir  = to - from;
        Vector3 perp = Vector3.Cross(dir, Vector3.forward).normalized;

        for (int i = 0; i <= Segments; i++)
        {
            float   t      = (float)i / Segments;
            Vector3 point  = Vector3.Lerp(from, to, t);
            if (i > 0 && i < Segments)
                point += perp * Random.Range(-Jitter, Jitter);
            line.SetPosition(i, point);
        }
    }

    void Update()
    {
        life -= Time.deltaTime;
        if (life <= 0f) { Destroy(gameObject); return; }

        float alpha = life / LifeSecs;
        var   c     = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
        line.startColor = c;
        line.endColor   = c;
    }
}
