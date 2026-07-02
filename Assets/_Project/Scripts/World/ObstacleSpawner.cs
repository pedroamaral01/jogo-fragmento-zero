using UnityEngine;

/// <summary>
/// Director de spawn procedural: executa SpawnPatterns coluna a coluna,
/// escolhidos por peso e filtrados pelo tier de dificuldade.
/// (Mantém o nome ObstacleSpawner: o componente e as refs de prefab já
/// vivem na cena.)
/// </summary>
public class ObstacleSpawner : MonoBehaviour
{
    [SerializeField] GameObject meteorPrefab;
    [SerializeField] GameObject dronePrefab;
    [SerializeField] GameObject crystalPrefab;
    [SerializeField] float      spawnX = 9f;

    [Tooltip("Respiro (s) entre um padrão e o próximo — encolhe com o tier")]
    [SerializeField] float patternGapSecs = 1.1f;

    SpawnPattern current;
    int   columnIndex;
    bool  mirrored;
    float patternSpeedMult = 1f;
    float timer = 1f;   // pequena espera inicial

    void Update()
    {
        // Só trabalha na corrida normal — BossFight pausa a geração
        if (GameManager.Instance.State != GameState.Running) return;

        timer -= Time.deltaTime;
        if (timer > 0f) return;

        if (current == null) PickPattern();

        SpawnColumn(current.columns[columnIndex]);
        columnIndex++;

        float gapMult = GameManager.Instance.Difficulty.SpawnGapMultiplier;

        if (columnIndex >= current.columns.Length)
        {
            current = null;
            timer   = patternGapSecs * gapMult;
        }
        else
        {
            timer = current.columnGap * gapMult;
        }
    }

    void PickPattern()
    {
        int tier = GameManager.Instance.Difficulty.Tier;

        // Seleção ponderada entre os padrões liberados para o tier atual
        float total = 0f;
        foreach (var p in SpawnPattern.Library)
            if (p.minTier <= tier) total += p.weight;

        float roll = Random.value * total;
        foreach (var p in SpawnPattern.Library)
        {
            if (p.minTier > tier) continue;
            roll -= p.weight;
            if (roll <= 0f) { current = p; break; }
        }
        if (current == null) current = SpawnPattern.Library[0];

        mirrored    = current.allowMirror && Random.value < 0.5f;
        columnIndex = 0;

        // Um multiplicador por padrão (não por entidade) mantém formações coesas
        patternSpeedMult = Random.Range(1f, 1.25f);
    }

    void SpawnColumn(string column)
    {
        int lanes = Mathf.Min(column.Length, LaneSystem.Instance.LaneCount);

        for (int lane = 0; lane < lanes; lane++)
        {
            char code = column[mirrored ? column.Length - 1 - lane : lane];

            GameObject prefab = code switch
            {
                'M' => meteorPrefab,
                'D' => dronePrefab,
                'C' => crystalPrefab,
                _   => null
            };
            if (prefab == null) continue;

            float y  = LaneSystem.Instance.GetLaneY(lane);
            var   go = Instantiate(prefab, new Vector3(spawnX, y, 0f), Quaternion.identity);

            if (go.TryGetComponent<ObstacleBase>(out var obstacle))
                obstacle.OverrideSpeedMultiplier(patternSpeedMult);
            else if (go.TryGetComponent<Crystal>(out var crystal))
                crystal.OverrideSpeedMultiplier(patternSpeedMult);
        }
    }
}
