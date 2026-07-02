using UnityEngine;

/// <summary>
/// Dono do tier de dificuldade da corrida. Classe pura (não-MonoBehaviour),
/// criada e alimentada pelo GameManager — sem estado global mutável.
///
/// O tier sobe por marcos de distância (GameConfig.tierDistances) e por
/// bônus concedidos ao derrotar chefões (AddBonusTier). Cada tier:
/// - libera padrões de spawn mais agressivos (SpawnPattern.minTier)
/// - encurta o respiro entre padrões (SpawnGapMultiplier)
/// - soma velocidade extra (SpeedBonus)
/// </summary>
public class DifficultyDirector
{
    readonly float[] tierDistances;

    int   bonusTiers;
    float lastScore;

    public int Tier { get; private set; }

    public DifficultyDirector(float[] tierDistances)
    {
        this.tierDistances = tierDistances ?? new float[0];
    }

    public void Tick(float score)
    {
        lastScore = score;

        int tier = bonusTiers;
        foreach (var distance in tierDistances)
            if (score >= distance) tier++;

        if (tier != Tier)
        {
            Tier = tier;
            GameEvents.RaiseDifficultyChanged(Tier);
        }
    }

    /// <summary>Chefão derrotado → a Fenda fica permanentemente mais instável.</summary>
    public void AddBonusTier()
    {
        bonusTiers++;
        Tick(lastScore);
    }

    /// <summary>Multiplicador dos intervalos de spawn (~12% mais denso por tier, com piso).</summary>
    public float SpawnGapMultiplier => Mathf.Max(0.45f, Mathf.Pow(0.88f, Tier));

    /// <summary>Velocidade extra somada à curva base.</summary>
    public float SpeedBonus => Tier * GameConfig.I.tierSpeedBonus;
}
