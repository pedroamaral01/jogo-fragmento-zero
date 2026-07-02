using UnityEngine;

/// <summary>
/// Projétil de chefão, montado 100% em runtime. Machuca o player no toque,
/// é destrutível por tiros de fogo (1 HP) e é desacelerado pelo Gelo —
/// a resposta tática pedida pelo GDD (\"o gelo contém o magma\").
/// </summary>
public class BossProjectile : MonoBehaviour, IPlayerHazard, IDamageable
{
    const float IceSlow = 0.35f;

    Vector2 velocity;
    float   life = 7f;

    public static BossProjectile Spawn(Vector3 position, Vector2 velocity, Color color, float diameter = 0.4f)
    {
        var go = new GameObject("BossProjectile");
        go.tag = "Obstacle";
        go.transform.position   = position;
        go.transform.localScale = new Vector3(diameter, diameter, 1f);

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite       = RuntimeSprites.Circle;
        sr.color        = color;
        sr.sortingOrder = 3;

        var col = go.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius    = 0.5f;

        var rb = go.AddComponent<Rigidbody2D>();
        rb.bodyType     = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;

        var projectile = go.AddComponent<BossProjectile>();
        projectile.velocity = velocity;
        return projectile;
    }

    void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsGameplayActive) return;

        float slow = PowerIce.Instance != null && PowerIce.Instance.IsActive ? IceSlow : 1f;
        transform.position += (Vector3)(velocity * slow * Time.deltaTime);

        life -= Time.deltaTime;
        if (life <= 0f ||
            transform.position.x < -11f ||
            Mathf.Abs(transform.position.y) > 6.5f)
        {
            Destroy(gameObject);
        }
    }

    public void OnHitPlayer()
    {
        PlayerController.Instance?.TakeHit();
        Destroy(gameObject);
    }

    public void TakeDamage(int amount)
    {
        ScreenEffects.Instance?.TriggerFlash(0.1f);
        Destroy(gameObject);
    }
}
