using UnityEngine;

/// <summary>
/// Gravidade — ímã de energia (tecla F): por alguns segundos todos os
/// cristais da pista são puxados na direção do player.
/// Consultado pelos cristais via Instance (mesmo padrão do PowerIce).
/// </summary>
public class PowerGravity : PowerBase
{
    public static PowerGravity Instance { get; private set; }

    [SerializeField] float durationSecs = 5f;
    [SerializeField] float pullSpeed    = 9f;

    float durationTimer;

    public override string  DisplayName => "GRAV";
    public override KeyCode Key         => KeyCode.F;
    public override Color   ThemeColor  => new Color(0.75f, 0.4f, 1f);

    public bool  IsActive  => durationTimer > 0f;
    public float PullSpeed => pullSpeed;

    public override bool IsReady => base.IsReady && !IsActive;

    void Awake()
    {
        Instance = this;
        if (energyCost   <= 0f) energyCost   = 18f;
        if (cooldownSecs <= 0f) cooldownSecs = 8f;
    }

    protected override void Update()
    {
        base.Update();
        if (durationTimer > 0f) durationTimer -= Time.deltaTime;
    }

    protected override void Activate()
    {
        durationTimer = durationSecs;
    }

    // Ativa: barra drena com a duração; senão: enche com o cooldown
    public override float HudFillRatio =>
        IsActive ? durationTimer / durationSecs : CooldownRatio;

    public override string HudLabel
    {
        get
        {
            if (IsActive) return $"{Key}  {DisplayName} {durationTimer:F1}s";
            return IsReady ? $"{Key}  {DisplayName} [OK]" : $"{Key}  {DisplayName} (cd)";
        }
    }
}
