using UnityEngine;

/// <summary>
/// Gelo — congela o tempo por alguns segundos (tecla D):
/// meteoros/cristais a 25% da velocidade, drones parados.
/// Consultado globalmente via Instance (Meteor, Drone, Crystal, Starfield).
/// </summary>
public class PowerIce : PowerBase
{
    public static PowerIce Instance { get; private set; }

    [SerializeField] float durationSecs = 4f;
    [SerializeField] ParticleSystem activateParticles;

    float durationTimer;

    public override string  DisplayName => "GELO";
    public override KeyCode Key         => KeyCode.D;
    public override Color   ThemeColor  => new Color(0.2f, 0.9f, 1f);

    public bool  IsActive      => durationTimer > 0f;
    public float TimeLeft      => durationTimer;
    public float SlowFactor    => IsActive ? 0.25f : 1f;
    public bool  IsDroneFrozen => IsActive;

    // Sem cooldown extra: pronto de novo assim que o efeito termina (como no VS)
    public override bool IsReady => base.IsReady && !IsActive;

    void Awake()
    {
        Instance = this;
        if (energyCost <= 0f) energyCost = 22f;
    }

    protected override void Update()
    {
        base.Update();
        if (durationTimer > 0f) durationTimer -= Time.deltaTime;
    }

    protected override void Activate()
    {
        durationTimer = durationSecs;
        if (activateParticles != null) activateParticles.Play();
    }

    public override float HudFillRatio =>
        IsActive ? durationTimer / durationSecs : (IsReady ? 1f : 0f);

    public override string HudLabel
    {
        get
        {
            if (IsActive) return $"{Key}  {DisplayName} {durationTimer:F1}s";
            return IsReady ? $"{Key}  {DisplayName} [OK]" : $"{Key}  {DisplayName} (cd)";
        }
    }
}
