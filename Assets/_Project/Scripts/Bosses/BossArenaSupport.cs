using UnityEngine;

/// <summary>
/// Suporte de recursos durante a luta de chefão: cristais de fogo laranja
/// em bom ritmo (mantêm o jogador atirando) e cristais azuis de energia
/// raramente — recuperar vida na arena deve ser difícil.
/// </summary>
public class BossArenaSupport : MonoBehaviour
{
    const float FireIntervalSecs   = 4f;
    const float EnergyIntervalSecs = 15f;
    const float FirstFireDelay     = 2f;
    const float FirstEnergyDelay   = 9f;
    const float SpawnX             = 9f;

    float fireTimer;
    float energyTimer;
    ObstacleSpawner spawner;

    void Update()
    {
        var gm = GameManager.Instance;
        if (gm == null || gm.State != GameState.BossFight)
        {
            // Fora da arena: re-arma os primeiros spawns da próxima luta
            fireTimer   = FirstFireDelay;
            energyTimer = FirstEnergyDelay;
            return;
        }

        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0f)
        {
            fireTimer = FireIntervalSecs;
            SpawnCrystal(Crystal.Kind.Fire);
        }

        energyTimer -= Time.deltaTime;
        if (energyTimer <= 0f)
        {
            energyTimer = EnergyIntervalSecs;
            SpawnCrystal(Crystal.Kind.Energy);
        }
    }

    void SpawnCrystal(Crystal.Kind kind)
    {
        if (spawner == null) spawner = FindFirstObjectByType<ObstacleSpawner>();
        if (spawner == null || spawner.CrystalPrefab == null) return;

        int   lane = Random.Range(0, LaneSystem.Instance.LaneCount);
        float y    = LaneSystem.Instance.GetLaneY(lane);
        var   go   = Instantiate(spawner.CrystalPrefab, new Vector3(SpawnX, y, 0f), Quaternion.identity);

        if (go.TryGetComponent<Crystal>(out var crystal))
            crystal.SetKind(kind);
    }
}
