using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Orquestra os encontros com construtos da Mente Matriz:
/// - Evolução Nv1 → mini-chefão; Nv2 → Guardião de Magma
/// - Depois disso, encontros alternados por distância, com HP escalando
/// - Spawner pausa automaticamente (estado BossFight); ao vencer, a corrida
///   continua com tier de dificuldade extra + recompensa
/// Vive na raiz persistente RuntimeSystems (reset por sceneLoaded).
/// </summary>
public class BossDirector : MonoBehaviour
{
    enum BossKind { DroneAlfa, MagmaGuardian }

    const float SpawnDelaySecs      = 2.2f;
    const float DistanceBetweenBoss = 650f;
    const int   DroneAlfaBaseHp     = 14;
    const int   GuardianBaseHp      = 26;
    const float HpScalePerEncounter = 0.35f;

    const float RewardEnergy = 35f;
    const float RewardScore  = 200f;

    BossBase activeBoss;
    BossKind pendingKind;
    bool     hasPending;
    float    pendingTimer;
    float    nextDistanceTrigger = -1f;
    int      encounterCount;

    void OnEnable()
    {
        GameEvents.EvolutionChanged += OnEvolutionChanged;
        GameEvents.BossDefeated     += OnBossDefeated;
        SceneManager.sceneLoaded    += OnSceneLoaded;
    }

    void OnDisable()
    {
        GameEvents.EvolutionChanged -= OnEvolutionChanged;
        GameEvents.BossDefeated     -= OnBossDefeated;
        SceneManager.sceneLoaded    -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Nova corrida: estado zerado (o boss antigo morreu com a cena)
        activeBoss          = null;
        hasPending          = false;
        nextDistanceTrigger = -1f;
        encounterCount      = 0;
    }

    void OnEvolutionChanged(int level, string stageName)
    {
        if (level == 1) Schedule(BossKind.DroneAlfa);
        else if (level == 2) Schedule(BossKind.MagmaGuardian);
        else if (level >= 3 && nextDistanceTrigger < 0f && GameManager.Instance != null)
            nextDistanceTrigger = GameManager.Instance.Score + DistanceBetweenBoss;
    }

    void Schedule(BossKind kind)
    {
        if (activeBoss != null || hasPending) return;
        pendingKind  = kind;
        pendingTimer = SpawnDelaySecs;
        hasPending   = true;
    }

    void Update()
    {
        var gm = GameManager.Instance;
        if (gm == null) return;

        // Encontros recorrentes por distância (após a evolução máxima)
        if (!hasPending && activeBoss == null &&
            nextDistanceTrigger > 0f && gm.State == GameState.Running &&
            gm.Score >= nextDistanceTrigger)
        {
            Schedule(encounterCount % 2 == 0 ? BossKind.DroneAlfa : BossKind.MagmaGuardian);
        }

        if (!hasPending || gm.State != GameState.Running) return;

        pendingTimer -= Time.deltaTime;
        if (pendingTimer > 0f) return;

        hasPending = false;
        SpawnBoss(pendingKind);
    }

    void SpawnBoss(BossKind kind)
    {
        GameManager.Instance.EnterBossFight();

        var go = new GameObject(kind.ToString());
        go.tag = "Obstacle";
        go.transform.position = new Vector3(10.5f, 0f, 0f);

        BossBase boss = kind == BossKind.DroneAlfa
            ? go.AddComponent<BossDroneAlfa>()
            : go.AddComponent<BossMagmaGuardian>();

        int baseHp  = kind == BossKind.DroneAlfa ? DroneAlfaBaseHp : GuardianBaseHp;
        float scale = 1f + HpScalePerEncounter * encounterCount;
        boss.Setup(Mathf.RoundToInt(baseHp * scale));

        activeBoss = boss;
        encounterCount++;
        GameEvents.RaiseBossSpawned(boss);
    }

    void OnBossDefeated(BossBase boss)
    {
        activeBoss = null;

        var gm = GameManager.Instance;
        if (gm == null) return;

        gm.ExitBossFight();
        gm.Difficulty.AddBonusTier();       // a Fenda fica permanentemente pior
        gm.AddScore(RewardScore);
        PlayerController.Instance?.ModifyEnergy(RewardEnergy);

        nextDistanceTrigger = gm.Score + DistanceBetweenBoss;
    }
}
