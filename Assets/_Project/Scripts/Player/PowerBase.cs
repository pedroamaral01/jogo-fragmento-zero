using UnityEngine;

/// <summary>
/// Contrato comum dos poderes elementais (Template Method):
/// TryActivate cobra energia, arma o cooldown e delega o efeito a Activate().
/// A HUD é genérica — pergunta ao poder como se apresentar (HudFillRatio/HudLabel),
/// então novos poderes não exigem mudanças na interface.
/// </summary>
public abstract class PowerBase : MonoBehaviour
{
    // Componentes de cena mantêm valores serializados; poderes adicionados em
    // runtime definem defaults no próprio Awake (guardando contra zero).
    [SerializeField] protected float energyCost;
    [SerializeField] protected float cooldownSecs;

    protected float cooldownTimer;

    public abstract string  DisplayName { get; }
    public abstract KeyCode Key         { get; }
    public abstract Color   ThemeColor  { get; }

    /// <summary>Controlado pelo sistema de evolução (F2.4).</summary>
    public bool IsUnlocked { get; private set; } = true;

    public float EnergyCost => energyCost;

    public float CooldownRatio =>
        cooldownSecs <= 0f || cooldownTimer <= 0f ? 1f : 1f - cooldownTimer / cooldownSecs;

    public virtual bool IsReady => IsUnlocked && cooldownTimer <= 0f && HasResources;

    /// <summary>Recurso disponível? Padrão: energia do player. Fogo usa carga própria.</summary>
    protected virtual bool HasResources =>
        PlayerController.Instance != null &&
        PlayerController.Instance.Energy >= energyCost;

    protected virtual void PayCost() =>
        PlayerController.Instance.ModifyEnergy(-energyCost);

    protected virtual void Update()
    {
        if (cooldownTimer > 0f) cooldownTimer -= Time.deltaTime;
    }

    public bool TryActivate()
    {
        if (!IsReady) return false;

        PayCost();
        cooldownTimer = cooldownSecs;
        Activate();
        GameEvents.RaisePowerActivated(this);
        return true;
    }

    /// <summary>Efeito concreto do poder.</summary>
    protected abstract void Activate();

    public void Unlock() => IsUnlocked = true;
    public void Lock()   => IsUnlocked = false;

    // ── Apresentação na HUD ─────────────────────────────────────────────────

    public virtual float HudFillRatio => CooldownRatio;

    public virtual string HudLabel =>
        IsReady ? $"{Key}  {DisplayName} ►" : $"{Key}  {DisplayName} (cd)";

    /// <summary>Recurso criticamente baixo — a HUD pisca a barra.</summary>
    public virtual bool HudCritical => false;
}
