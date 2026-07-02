using UnityEngine;

/// <summary>
/// Fonte única de balanceamento global (economia de energia, curva de velocidade).
/// Carregado de Resources/GameConfig.asset; se o asset não existir, os defaults
/// de código (idênticos aos valores do Vertical Slice) entram em vigor.
///
/// Valores por-entidade (recompensa de um cristal, HP de um drone) continuam
/// nos prefabs — aqui ficam apenas os números que atravessam sistemas.
/// </summary>
[CreateAssetMenu(fileName = "GameConfig", menuName = "Fragmento Zero/Game Config")]
public class GameConfig : ScriptableObject
{
    [Header("Corrida")]
    public float baseSpeed     = 4.5f;
    public float speedPerScore = 0.003f;
    [Tooltip("Metros ganhos por segundo para cada unidade de Speed")]
    public float scorePerSpeed = 2.4f;

    [Header("Dificuldade")]
    [Tooltip("Velocidade máxima da corrida (teto da curva)")]
    public float maxSpeed = 11f;
    [Tooltip("Distâncias (m) em que o tier de dificuldade sobe")]
    public float[] tierDistances = { 250f, 600f, 1000f, 1500f, 2100f, 2800f };
    [Tooltip("Velocidade extra somada por tier")]
    public float tierSpeedBonus = 0.4f;

    [Header("Energia")]
    public float maxEnergy        = 100f;
    public float startEnergy      = 60f;
    public float drainPerSecond   = 1.2f;
    public float hitDamage        = 25f;
    public float invincibleSecs   = 1.17f;
    public float killEnergyReward = 6f;
    [Range(0f, 1f)] public float lowEnergyWarning = 0.25f;

    static GameConfig instance;

    public static GameConfig I
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<GameConfig>("GameConfig");
                if (instance == null) instance = CreateInstance<GameConfig>();
            }
            return instance;
        }
    }
}
