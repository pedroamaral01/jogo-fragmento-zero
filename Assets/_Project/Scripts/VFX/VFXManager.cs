using UnityEngine;

/// <summary>
/// Dispara efeitos visuais em resposta a eventos de gameplay — mesma
/// filosofia do AudioManager: ninguém precisa conhecer VFX para tê-los.
/// </summary>
public class VFXManager : MonoBehaviour
{
    static readonly Color KillColor        = new Color(1f, 0.6f, 0.2f);
    static readonly Color CrystalColor     = new Color(0.2f, 1f, 1f);
    static readonly Color FireCrystalColor = new Color(1f, 0.6f, 0.15f);
    static readonly Color BossColor        = new Color(1f, 0.35f, 0.15f);

    void OnEnable()
    {
        GameEvents.EnemyKilled          += OnEnemyKilled;
        GameEvents.CrystalCollected     += OnCrystalCollected;
        GameEvents.FireCrystalCollected += OnFireCrystalCollected;
        GameEvents.BossDefeated         += OnBossDefeated;
    }

    void OnDisable()
    {
        GameEvents.EnemyKilled          -= OnEnemyKilled;
        GameEvents.CrystalCollected     -= OnCrystalCollected;
        GameEvents.FireCrystalCollected -= OnFireCrystalCollected;
        GameEvents.BossDefeated         -= OnBossDefeated;
    }

    void OnFireCrystalCollected(Vector3 position, float charge)
        => BurstVFX.Spawn(position, FireCrystalColor, 8, 3f, 0.4f);

    void OnEnemyKilled(ObstacleBase enemy, Vector3 position)
        => BurstVFX.Spawn(position, KillColor, 12, 4f, 0.5f);

    void OnCrystalCollected(Vector3 position, float energy, float score)
        => BurstVFX.Spawn(position, CrystalColor, 8, 3f, 0.4f);

    void OnBossDefeated(BossBase boss)
        => BurstVFX.Spawn(boss.transform.position, BossColor, 26, 6f, 0.9f);
}
