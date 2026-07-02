using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] GameObject meteorPrefab;
    [SerializeField] GameObject dronePrefab;
    [SerializeField] GameObject crystalPrefab;
    [SerializeField] float      spawnX = 9f;

    // Original: 60% obstacle, 40% crystal. Obstacle split: 60% meteor, 40% drone.
    [SerializeField] float obstacleChance = 0.6f;
    [SerializeField] float meteorChance   = 0.6f;
    [SerializeField] float extraCrystalChance = 0.35f;

    float frameAccum;

    void Update()
    {
        // Spawner só trabalha na corrida normal — pausa automática durante BossFight
        if (GameManager.Instance.State != GameState.Running) return;

        // Accumulate in "frames at 60fps" to match the original rate formula
        frameAccum += Time.deltaTime * 60f;
        float rate  = Mathf.Max(38f, 100f - GameManager.Instance.Score * 0.25f);

        if (frameAccum < rate) return;
        frameAccum = 0f;

        if (Random.value < obstacleChance) SpawnObstacle();
        else                               SpawnCrystal();

        if (Random.value < extraCrystalChance) SpawnCrystal();
    }

    void SpawnObstacle()
    {
        int   lane = Random.Range(0, LaneSystem.Instance.LaneCount);
        float y    = LaneSystem.Instance.GetLaneY(lane);
        var prefab = Random.value < meteorChance ? meteorPrefab : dronePrefab;
        Instantiate(prefab, new Vector3(spawnX, y, 0f), Quaternion.identity);
    }

    void SpawnCrystal()
    {
        int   lane = Random.Range(0, LaneSystem.Instance.LaneCount);
        float y    = LaneSystem.Instance.GetLaneY(lane);
        Instantiate(crystalPrefab, new Vector3(spawnX, y, 0f), Quaternion.identity);
    }
}
