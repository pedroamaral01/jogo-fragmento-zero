using UnityEngine;

/// <summary>
/// Base dos construtos da Mente Matriz. Ciclo: entra pela direita
/// (invulnerável), luta com fases por fração de HP, morre anunciando
/// BossDefeated. Visual e comportamento ficam nas subclasses
/// (BuildVisual/FightUpdate) — tudo procedural, sem prefabs.
/// </summary>
public abstract class BossBase : MonoBehaviour, IDamageable, IPlayerHazard
{
    protected float arenaX     = 6.3f;
    protected float enterSpeed = 4f;

    /// <summary>Frações de HP que iniciam as fases seguintes (decrescentes).</summary>
    protected float[] phaseThresholds = { 0.5f };

    /// <summary>Renderer principal — usado no flash de dano.</summary>
    protected SpriteRenderer body;

    public string DisplayName { get; protected set; } = "CONSTRUTO";
    public int  MaxHp      { get; private set; }
    public int  Hp         { get; private set; }
    public int  Phase      { get; private set; } = 1;
    public bool IsEntering { get; private set; } = true;

    float hitFlashTimer;
    Color bodyColor;

    /// <summary>Chamado pelo BossDirector logo após criar o componente.</summary>
    public void Setup(int hp)
    {
        MaxHp = hp;
        Hp    = hp;
    }

    void Start()
    {
        BuildVisual();
        if (body != null) bodyColor = body.color;
    }

    void Update()
    {
        if (GameManager.Instance == null || !GameManager.Instance.IsGameplayActive) return;

        if (IsEntering)
        {
            transform.position += Vector3.left * enterSpeed * Time.deltaTime;
            if (transform.position.x <= arenaX)
            {
                IsEntering = false;
                OnFightStart();
            }
            return;
        }

        FightUpdate();
        UpdateHitFlash();
    }

    public void TakeDamage(int amount)
    {
        if (IsEntering || Hp <= 0) return;

        Hp -= amount;
        hitFlashTimer = 0.08f;
        if (body != null) body.color = Color.white;

        if (Hp <= 0)
        {
            Die();
            return;
        }

        // Fase = 1 + quantos thresholds a fração de HP já cruzou
        float fraction    = (float)Hp / MaxHp;
        int   targetPhase = 1;
        foreach (var threshold in phaseThresholds)
            if (fraction <= threshold) targetPhase++;

        if (targetPhase != Phase)
        {
            Phase = targetPhase;
            OnPhaseChanged();
            GameEvents.RaiseBossPhaseChanged(this, Phase);
        }
    }

    void Die()
    {
        GameEvents.RaiseBossDefeated(this);
        ScreenEffects.Instance?.TriggerShake(0.45f, 0.7f);
        ScreenEffects.Instance?.TriggerFlash(0.5f);
        Destroy(gameObject);
    }

    public void OnHitPlayer() => PlayerController.Instance?.TakeHit();

    void UpdateHitFlash()
    {
        if (hitFlashTimer <= 0f || body == null) return;
        hitFlashTimer -= Time.deltaTime;
        if (hitFlashTimer <= 0f) body.color = bodyColor;
    }

    /// <summary>Subclasse pode retintar o corpo (ex.: fase enraivecida).</summary>
    protected void SetBodyColor(Color color)
    {
        bodyColor = color;
        if (body != null && hitFlashTimer <= 0f) body.color = color;
    }

    // ── Contrato das subclasses ─────────────────────────────────────────────

    protected abstract void BuildVisual();
    protected abstract void FightUpdate();
    protected virtual  void OnFightStart()   { }
    protected virtual  void OnPhaseChanged() { }

    // ── Helpers comuns ──────────────────────────────────────────────────────

    protected float PlayerY =>
        PlayerController.Instance != null ? PlayerController.Instance.transform.position.y : 0f;

    protected Vector2 AimAtPlayer(Vector3 from)
    {
        if (PlayerController.Instance == null) return Vector2.left;
        return ((Vector2)(PlayerController.Instance.transform.position - from)).normalized;
    }

    protected void MoveTowardsY(float targetY, float speed)
    {
        var pos = transform.position;
        pos.y = Mathf.MoveTowards(pos.y, targetY, speed * Time.deltaTime);
        transform.position = pos;
    }

    protected SpriteRenderer AddShape(string name, Sprite sprite, Color color,
        Vector3 localPos, Vector3 localScale, int sortingOrder = 2)
    {
        var go = new GameObject(name);
        go.transform.SetParent(transform, false);
        go.transform.localPosition = localPos;
        go.transform.localScale    = localScale;
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite       = sprite;
        sr.color        = color;
        sr.sortingOrder = sortingOrder;
        return sr;
    }
}
