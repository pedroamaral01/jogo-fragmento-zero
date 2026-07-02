using UnityEngine;

/// <summary>
/// Explosão de partículas procedural: fragmentos que voam para fora,
/// desaceleram, encolhem e somem. Zero assets, zero prefabs.
/// </summary>
public class BurstVFX : MonoBehaviour
{
    struct Particle
    {
        public Transform      transform;
        public SpriteRenderer renderer;
        public Vector3        velocity;
        public float          spin;
    }

    Particle[] particles;
    float life;
    float maxLife;
    Color color;

    public static void Spawn(Vector3 position, Color color,
                             int count = 10, float speed = 3.5f, float life = 0.45f)
    {
        var go  = new GameObject("BurstVFX");
        go.transform.position = position;
        go.AddComponent<BurstVFX>().Init(color, count, speed, life);
    }

    void Init(Color burstColor, int count, float speed, float duration)
    {
        color   = burstColor;
        life    = duration;
        maxLife = duration;

        particles = new Particle[count];
        for (int i = 0; i < count; i++)
        {
            var frag = new GameObject("p");
            frag.transform.SetParent(transform, false);
            float size = Random.Range(0.08f, 0.2f);
            frag.transform.localScale = new Vector3(size, size, 1f);

            var sr = frag.AddComponent<SpriteRenderer>();
            sr.sprite       = RuntimeSprites.Square;
            sr.color        = color;
            sr.sortingOrder = 6;

            float   angle = Random.Range(0f, Mathf.PI * 2f);
            float   spd   = speed * Random.Range(0.4f, 1.1f);
            particles[i] = new Particle
            {
                transform = frag.transform,
                renderer  = sr,
                velocity  = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * spd,
                spin      = Random.Range(-360f, 360f),
            };
        }
    }

    void Update()
    {
        life -= Time.deltaTime;
        if (life <= 0f) { Destroy(gameObject); return; }

        float t01   = life / maxLife;                 // 1 → 0
        float alpha = t01;
        var   c     = new Color(color.r, color.g, color.b, alpha);

        for (int i = 0; i < particles.Length; i++)
        {
            ref var p = ref particles[i];
            p.velocity *= 1f - 2.5f * Time.deltaTime;             // arrasto
            p.transform.position += p.velocity * Time.deltaTime;
            p.transform.Rotate(0f, 0f, p.spin * Time.deltaTime);
            p.transform.localScale = Vector3.one * 0.14f * t01;
            p.renderer.color = c;
        }
    }
}
