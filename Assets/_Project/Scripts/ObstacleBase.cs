using UnityEngine;

public class ObstacleBase : MonoBehaviour
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

    void Update()
    {
        if (GameManager.Instance.State != GameManager.GameState.Playing) return;

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
        ScreenEffects.Instance?.TriggerFlash(0.18f);

        if (hp <= 0)
        {
            PlayerController.Instance?.ModifyEnergy(6f);  // +6 energy reward for kill
            if (deathEffectPrefab != null)
                Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }
}
