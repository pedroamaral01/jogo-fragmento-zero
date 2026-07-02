using UnityEngine;

/// <summary>Fogo — dispara projétil que destrói obstáculos (tecla A).</summary>
public class PowerFire : PowerBase
{
    [SerializeField] GameObject     bulletPrefab;
    [SerializeField] Transform      muzzlePoint;
    [SerializeField] ParticleSystem muzzleParticles;

    public override string  DisplayName => "FOGO";
    public override KeyCode Key         => KeyCode.A;
    public override Color   ThemeColor  => new Color(1f, 0.6f, 0.1f);

    void Awake()
    {
        if (energyCost   <= 0f) energyCost   = 4f;
        if (cooldownSecs <= 0f) cooldownSecs = 0.37f;
    }

    public override bool IsReady => base.IsReady && bulletPrefab != null;

    protected override void Activate()
    {
        Vector3 spawnPos = muzzlePoint != null
            ? muzzlePoint.position
            : transform.position + Vector3.right * 0.6f;

        Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
        if (muzzleParticles != null) muzzleParticles.Play();
    }
}
