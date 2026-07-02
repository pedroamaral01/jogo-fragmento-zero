using UnityEngine;

/// <summary>
/// Mini-chefão: Drone Alfa. Persegue a faixa do player e dispara projéteis
/// teleguiados no momento do tiro. Fase 2 (≤50% HP): mais rápido e tiro duplo.
/// </summary>
public class BossDroneAlfa : BossBase
{
    // Construto da Mente Matriz: carcaça industrial + vermelho de alerta
    static readonly Color BodyColor    = ArtPalette.StructureLit;
    static readonly Color EnragedColor = new Color(0.45f, 0.14f, 0.20f);
    static readonly Color ShotColor    = ArtPalette.AlertRed;

    float shootTimer = 1.2f;

    void Awake()
    {
        DisplayName     = "DRONE ALFA";
        phaseThresholds = new[] { 0.5f };
    }

    protected override void BuildVisual()
    {
        body = AddShape("Body", RuntimeSprites.Square, BodyColor,
            Vector3.zero, new Vector3(1.7f, 1.1f, 1f));

        AddShape("EyeGlow", RuntimeSprites.Glow, new Color(1f, 0f, 0.235f, 0.5f),
            new Vector3(-0.55f, 0.12f, 0f), new Vector3(0.9f, 0.9f, 1f), 2);
        AddShape("Eye", RuntimeSprites.Circle, ArtPalette.AlertRed,
            new Vector3(-0.55f, 0.12f, 0f), new Vector3(0.42f, 0.42f, 1f), 3);

        AddShape("Antenna", RuntimeSprites.Square, new Color(0.7f, 0.7f, 0.9f),
            new Vector3(0f, 0.7f, 0f), new Vector3(0.08f, 0.4f, 1f), 1);

        var col = gameObject.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size      = new Vector2(1.7f, 1.1f);

        var rb = gameObject.AddComponent<Rigidbody2D>();
        rb.bodyType     = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
    }

    protected override void FightUpdate()
    {
        bool enraged = Phase >= 2;

        // Persegue a faixa do player
        MoveTowardsY(PlayerY, enraged ? 3.2f : 2f);

        shootTimer -= Time.deltaTime;
        if (shootTimer > 0f) return;
        shootTimer = enraged ? 1.0f : 1.6f;

        Vector3 muzzle = transform.position + Vector3.left * 1.1f;
        Vector2 dir    = AimAtPlayer(muzzle);
        const float projectileSpeed = 5.5f;

        FireAt(muzzle, dir * projectileSpeed);
        if (enraged)
        {
            FireAt(muzzle, (Vector2)(Quaternion.Euler(0, 0,  14f) * dir) * projectileSpeed);
            FireAt(muzzle, (Vector2)(Quaternion.Euler(0, 0, -14f) * dir) * projectileSpeed);
        }
    }

    void FireAt(Vector3 from, Vector2 velocity)
        => BossProjectile.Spawn(from, velocity, ShotColor, 0.36f);

    protected override void OnPhaseChanged()
    {
        SetBodyColor(EnragedColor);
        ScreenEffects.Instance?.TriggerShake(0.2f, 0.3f);
    }
}
