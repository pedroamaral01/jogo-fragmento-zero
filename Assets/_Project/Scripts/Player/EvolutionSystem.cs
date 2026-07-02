using UnityEngine;

/// <summary>
/// Evolução do Fragmento Zero: acumula toda energia absorvida (cristais e
/// recompensas de kill) e, ao cruzar marcos, sobe de estágio — mutação
/// visual + desbloqueio do próximo poder na ordem Fogo→Gelo→Raio→Gravidade.
/// </summary>
public class EvolutionSystem : MonoBehaviour
{
    [System.Serializable]
    public class Stage
    {
        public string name;
        public float  energyToReach;
        public Color  bodyColor;
        public float  scale;
    }

    [SerializeField] Stage[] stages;

    public int   Level         { get; private set; }
    public float TotalAbsorbed { get; private set; }

    public Stage Current => stages[Level];
    public bool  IsMaxed => Level >= stages.Length - 1;

    public float ProgressToNext => IsMaxed
        ? 1f
        : Mathf.InverseLerp(stages[Level].energyToReach,
                            stages[Level + 1].energyToReach, TotalAbsorbed);

    PowerBase[] powerOrder;

    void Awake()
    {
        if (stages == null || stages.Length == 0)
        {
            stages = new[]
            {
                // Escalas reduzidas: o corpo é o sprite RuntimeSprites.Circle (1 unidade
                // de diâmetro em escala 1) — em 1.0 o player ficava maior que qualquer
                // obstáculo. 0.72 deixa o Fragmento Zero comparável a um meteoro (0.7).
                new Stage { name = "Centelha",           energyToReach = 0f,   bodyColor = ArtPalette.Cyan,            scale = 0.72f },
                new Stage { name = "Fragmento Desperto", energyToReach = 60f,  bodyColor = new Color(0.3f, 1f, 0.8f),  scale = 0.81f },
                new Stage { name = "Núcleo Instável",    energyToReach = 150f, bodyColor = new Color(1f, 0.9f, 0.35f), scale = 0.90f },
                new Stage { name = "Avatar Elemental",   energyToReach = 280f, bodyColor = new Color(0.8f, 0.5f, 1f),  scale = 1.00f },
            };
        }

        // PlayerController.Awake adiciona este componente por último,
        // então todos os poderes já existem aqui.
        powerOrder = new PowerBase[]
        {
            GetComponent<PowerFire>(),
            GetComponent<PowerIce>(),
            GetComponent<PowerLightning>(),
            GetComponent<PowerGravity>(),
        };

        ApplyStage(announce: false);
    }

    void OnEnable()
    {
        GameEvents.CrystalCollected += OnCrystalCollected;
        GameEvents.EnemyKilled      += OnEnemyKilled;
    }

    void OnDisable()
    {
        GameEvents.CrystalCollected -= OnCrystalCollected;
        GameEvents.EnemyKilled      -= OnEnemyKilled;
    }

    void OnCrystalCollected(Vector3 pos, float energy, float score) => Absorb(energy);
    void OnEnemyKilled(ObstacleBase enemy, Vector3 pos) => Absorb(GameConfig.I.killEnergyReward);

    void Absorb(float amount)
    {
        TotalAbsorbed += amount;
        while (!IsMaxed && TotalAbsorbed >= stages[Level + 1].energyToReach)
        {
            Level++;
            ApplyStage(announce: true);
        }
    }

    void ApplyStage(bool announce)
    {
        var stage = stages[Level];
        PlayerController.Instance?.ApplyEvolutionVisual(stage.bodyColor, stage.scale);

        for (int i = 0; i < powerOrder.Length; i++)
        {
            var power = powerOrder[i];
            if (power == null) continue;

            bool shouldUnlock = i <= Level;
            if (shouldUnlock && !power.IsUnlocked)
            {
                power.Unlock();
                if (announce) GameEvents.RaisePowerUnlocked(power);
            }
            else if (!shouldUnlock && power.IsUnlocked)
            {
                power.Lock();
            }
        }

        if (announce) GameEvents.RaiseEvolutionChanged(Level, stage.name);
    }
}
