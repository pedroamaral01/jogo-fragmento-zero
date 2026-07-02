using UnityEngine;

public class ObstacleBase : MonoBehaviour, IDamageable, IPlayerHazard
{
    [SerializeField] protected int   maxHp         = 1;
    [SerializeField] protected float speedMultMin  = 1f;
    [SerializeField] protected float speedMultMax  = 1.43f;  // 1 + 1.5 / 3.5 ≈ original variance

    [SerializeField] GameObject deathEffectPrefab;

    protected int   hp;
    protected float baseSpeedMult;

    const float DespawnX = -10f;

    protected virtual void Awake()
    {
        hp            = maxHp;
        baseSpeedMult = Random.Range(speedMultMin, speedMultMax);
    }

    /// <summary>Spawner define um multiplicador único por padrão — formações coesas.</summary>
    public void OverrideSpeedMultiplier(float multiplier) => baseSpeedMult = multiplier;

    void Update()
    {
        if (!GameManager.Instance.IsGameplayActive) return;

        float vx = GameManager.Instance.Speed * baseSpeedMult * GetSlowFactor();
        transform.position += Vector3.left * vx * Time.deltaTime;

        if (transform.position.x < DespawnX) Destroy(gameObject);

        OnObstacleUpdate();
    }

    protected virtual float GetSlowFactor() => 1f;
    protected virtual void  OnObstacleUpdate() { }

    public virtual void OnHitPlayer() => PlayerController.Instance?.TakeHit();

    public virtual void TakeDamage(int amount)
    {
        hp -= amount;
        GameEvents.RaiseEnemyDamaged(this);

        if (hp <= 0)
        {
            // Consequências (recompensa, flash, som) são responsabilidade dos assinantes
            GameEvents.RaiseEnemyKilled(this, transform.position);
            if (deathEffectPrefab != null)
                Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
