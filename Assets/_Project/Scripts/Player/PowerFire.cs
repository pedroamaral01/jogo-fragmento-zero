using UnityEngine;

/// <summary>
/// Fogo — dispara projétil que destrói obstáculos (tecla A).
/// Consome CARGA DE FOGO própria (não a energia/vida): a carga regenera
/// devagar e é restaurada pelos cristais de fogo laranja — abundantes na
/// arena dos chefões.
/// </summary>
public class PowerFire : PowerBase
{
    [SerializeField] GameObject     bulletPrefab;
    [SerializeField] Transform      muzzlePoint;
    [SerializeField] ParticleSystem muzzleParticles;

    [Header("Carga de Fogo")]
    [SerializeField] float maxCharge   = 100f;
    [SerializeField] float chargeCost  = 6f;
    [SerializeField] float regenPerSec = 4f;

    public float Charge    { get; private set; }
    public float MaxCharge => maxCharge;

    public override string  DisplayName => "FOGO";
    public override KeyCode Key         => KeyCode.A;
    public override Color   ThemeColor  => new Color(1f, 0.6f, 0.1f);

    void Awake()
    {
        if (cooldownSecs <= 0f) cooldownSecs = 0.37f;
        Charge = maxCharge;
    }

    void OnEnable()  => GameEvents.FireCrystalCollected += OnFireCrystal;
    void OnDisable() => GameEvents.FireCrystalCollected -= OnFireCrystal;

    void OnFireCrystal(Vector3 pos, float amount) => RestoreCharge(amount);

    public void RestoreCharge(float amount) =>
        Charge = Mathf.Min(maxCharge, Charge + amount);

    // Custo sai da carga, não da energia
    protected override bool HasResources => Charge >= chargeCost;
    protected override void PayCost()    => Charge -= chargeCost;

    public override bool IsReady => base.IsReady && bulletPrefab != null;

    protected override void Update()
    {
        base.Update();
        if (GameManager.Instance != null && GameManager.Instance.IsGameplayActive)
            Charge = Mathf.Min(maxCharge, Charge + regenPerSec * Time.deltaTime);
    }

    protected override void Activate()
    {
        Vector3 spawnPos = muzzlePoint != null
            ? muzzlePoint.position
            : transform.position + Vector3.right * 0.6f;

        Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
        if (muzzleParticles != null) muzzleParticles.Play();
    }

    // ── HUD: barra = carga (desce ao atirar, sobe com regen/cristais) ──────
    public override float HudFillRatio => Charge / maxCharge;
    public override string HudLabel    => $"{Key}  {DisplayName}  {Mathf.FloorToInt(Charge)}";
    public override bool  HudCritical  => Charge < chargeCost;
}
