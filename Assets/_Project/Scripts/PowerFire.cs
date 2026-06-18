using UnityEngine;

public class PowerFire : MonoBehaviour
{
    [SerializeField] GameObject bulletPrefab;
    [SerializeField] float      cooldownSecs  = 0.37f;
    [SerializeField] float      energyCost    = 4f;
    [SerializeField] Transform  muzzlePoint;
    [SerializeField] ParticleSystem muzzleParticles;

    float timer;

    public float CooldownRatio => timer <= 0f ? 1f : 1f - (timer / cooldownSecs);
    public bool  IsReady       => timer <= 0f && bulletPrefab != null
                                              && PlayerController.Instance != null
                                              && PlayerController.Instance.Energy >= energyCost;

    void Update()
    {
        if (timer > 0f) timer -= Time.deltaTime;
    }

    public void TryShoot()
    {
        if (!IsReady) return;
        PlayerController.Instance.ModifyEnergy(-energyCost);
        timer = cooldownSecs;

        Vector3 spawnPos = muzzlePoint != null
            ? muzzlePoint.position
            : transform.position + Vector3.right * 0.6f;

        Instantiate(bulletPrefab, spawnPos, Quaternion.identity);
        if (muzzleParticles != null) muzzleParticles.Play();
    }
}
