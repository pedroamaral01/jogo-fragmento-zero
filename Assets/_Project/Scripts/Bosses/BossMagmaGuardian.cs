using UnityEngine;

/// <summary>
/// Chefão: Guardião de Magma. Colosso que flutua em senoide e cobre a tela
/// com leques de projéteis de magma — o Gelo os desacelera (resposta tática
/// do GDD). Três fases: leques maiores, mais rápidos e meteoros arremessados.
/// </summary>
public class BossMagmaGuardian : BossBase
{
    static readonly Color BodyColor  = new Color(1f, 0.40f, 0.05f);   // magma vivo
    static readonly Color CoreColor  = new Color(1f, 0.85f, 0.3f);
    static readonly Color MagmaColor = ArtPalette.Fire;               // #FF4500

    Transform shards;          // anel de rochas orbitando
    ObstacleSpawner spawner;   // fonte do prefab de meteoro
    float volleyTimer = 2f;
    float meteorTimer = 5f;

    void Awake()
    {
        DisplayName     = "GUARDIÃO DE MAGMA";
        phaseThresholds = new[] { 0.6f, 0.3f };
        arenaX          = 6.6f;
        enterSpeed      = 3f;
    }

    protected override void BuildVisual()
    {
        body = AddShape("Body", RuntimeSprites.Circle, BodyColor,
            Vector3.zero, new Vector3(2.6f, 2.6f, 1f));

        AddShape("Core", RuntimeSprites.Circle, CoreColor,
            Vector3.zero, new Vector3(1.2f, 1.2f, 1f), 3);

        // Anel de fragmentos de rocha girando ao redor do corpo
        var shardsGo = new GameObject("Shards");
        shardsGo.transform.SetParent(transform, false);
        shards = shardsGo.transform;
        for (int i = 0; i < 4; i++)
        {
            float angle = i * 90f * Mathf.Deg2Rad;
            AddShapeTo(shards, $"Shard{i}", new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0f) * 1.7f);
        }

        var col = gameObject.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius    = 1.3f;

        var rb = gameObject.AddComponent<Rigidbody2D>();
        rb.bodyType     = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
    }

    void AddShapeTo(Transform parent, string name, Vector3 localPos)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        go.transform.localPosition = localPos;
        go.transform.localScale    = new Vector3(0.45f, 0.45f, 1f);
        go.transform.localRotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 90f));
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite       = RuntimeSprites.Square;
        sr.color        = new Color(0.45f, 0.2f, 0.1f);
        sr.sortingOrder = 4;
    }

    protected override void OnFightStart()
    {
        spawner = FindFirstObjectByType<ObstacleSpawner>();
    }

    protected override void FightUpdate()
    {
        // Flutua em senoide cobrindo as faixas
        var pos = transform.position;
        pos.y = Mathf.Sin(Time.time * 0.7f) * 1.9f;
        transform.position = pos;

        if (shards != null)
            shards.Rotate(0f, 0f, 40f * Time.deltaTime);

        HandleVolley();
        if (Phase >= 2) HandleMeteorToss();
    }

    void HandleVolley()
    {
        volleyTimer -= Time.deltaTime;
        if (volleyTimer > 0f) return;

        (float interval, int count, float spread, float speed) = Phase switch
        {
            1 => (2.4f, 3, 24f, 4.5f),
            2 => (2.0f, 4, 36f, 5.0f),
            _ => (1.6f, 5, 48f, 5.6f),
        };
        volleyTimer = interval;

        Vector3 muzzle = transform.position + Vector3.left * 2f;
        Vector2 dir    = AimAtPlayer(muzzle);

        for (int i = 0; i < count; i++)
        {
            float t     = count == 1 ? 0f : (float)i / (count - 1) - 0.5f;   // -0.5..0.5
            float angle = t * spread;
            Vector2 v   = (Vector2)(Quaternion.Euler(0, 0, angle) * dir) * speed;
            BossProjectile.Spawn(muzzle, v, MagmaColor, 0.42f);
        }
    }

    void HandleMeteorToss()
    {
        meteorTimer -= Time.deltaTime;
        if (meteorTimer > 0f) return;
        meteorTimer = Phase >= 3 ? 3.5f : 5f;

        int tosses = Phase >= 3 ? 2 : 1;
        for (int i = 0; i < tosses; i++) TossMeteor();
    }

    void TossMeteor()
    {
        if (spawner == null || spawner.MeteorPrefab == null) return;

        int   lane = Random.Range(0, LaneSystem.Instance.LaneCount);
        float y    = LaneSystem.Instance.GetLaneY(lane);
        var   go   = Instantiate(spawner.MeteorPrefab, new Vector3(9f, y, 0f), Quaternion.identity);
        if (go.TryGetComponent<ObstacleBase>(out var obstacle))
            obstacle.OverrideSpeedMultiplier(1.6f);
    }

    protected override void OnPhaseChanged()
    {
        SetBodyColor(Phase >= 3 ? new Color(1f, 0.25f, 0.1f) : new Color(1f, 0.35f, 0.1f));
        ScreenEffects.Instance?.TriggerShake(0.3f, 0.4f);
    }
}
