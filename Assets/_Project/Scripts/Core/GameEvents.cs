using System;
using UnityEngine;

/// <summary>
/// Canal central de eventos discretos do jogo (Observer pattern).
///
/// Regra de uso: assinantes registram em OnEnable e removem em OnDisable —
/// isso garante que objetos destruídos (inclusive em reload de cena) nunca
/// fiquem pendurados nos eventos estáticos.
///
/// Valores contínuos (energia, score) NÃO passam por aqui: mudam todo frame,
/// então a HUD os lê por polling com referências cacheadas.
/// </summary>
public static class GameEvents
{
    /// <summary>(estadoAnterior, estadoNovo)</summary>
    public static event Action<GameState, GameState> StateChanged;

    /// <summary>Inimigo levou dano (mesmo sem morrer).</summary>
    public static event Action<ObstacleBase> EnemyDamaged;

    /// <summary>Inimigo destruído por dano (posição do inimigo).</summary>
    public static event Action<ObstacleBase, Vector3> EnemyKilled;

    /// <summary>Cristal coletado (posição, energia, score).</summary>
    public static event Action<Vector3, float, float> CrystalCollected;

    /// <summary>Player levou dano de obstáculo.</summary>
    public static event Action PlayerHit;

    /// <summary>Um poder foi ativado com sucesso.</summary>
    public static event Action<PowerBase> PowerActivated;

    /// <summary>O Fragmento evoluiu (nível, nome do estágio).</summary>
    public static event Action<int, string> EvolutionChanged;

    /// <summary>Um poder foi desbloqueado pela evolução.</summary>
    public static event Action<PowerBase> PowerUnlocked;

    /// <summary>Tier de dificuldade da Fenda subiu.</summary>
    public static event Action<int> DifficultyChanged;

    /// <summary>Um chefão entrou na arena.</summary>
    public static event Action<BossBase> BossSpawned;

    /// <summary>Chefão mudou de fase.</summary>
    public static event Action<BossBase, int> BossPhaseChanged;

    /// <summary>Chefão foi destruído.</summary>
    public static event Action<BossBase> BossDefeated;

    public static void RaiseStateChanged(GameState previous, GameState next)
        => StateChanged?.Invoke(previous, next);

    public static void RaiseEnemyDamaged(ObstacleBase enemy)
        => EnemyDamaged?.Invoke(enemy);

    public static void RaiseEnemyKilled(ObstacleBase enemy, Vector3 position)
        => EnemyKilled?.Invoke(enemy, position);

    public static void RaiseCrystalCollected(Vector3 position, float energy, float score)
        => CrystalCollected?.Invoke(position, energy, score);

    public static void RaisePlayerHit()
        => PlayerHit?.Invoke();

    public static void RaisePowerActivated(PowerBase power)
        => PowerActivated?.Invoke(power);

    public static void RaiseEvolutionChanged(int level, string stageName)
        => EvolutionChanged?.Invoke(level, stageName);

    public static void RaisePowerUnlocked(PowerBase power)
        => PowerUnlocked?.Invoke(power);

    public static void RaiseDifficultyChanged(int tier)
        => DifficultyChanged?.Invoke(tier);

    public static void RaiseBossSpawned(BossBase boss)
        => BossSpawned?.Invoke(boss);

    public static void RaiseBossPhaseChanged(BossBase boss, int phase)
        => BossPhaseChanged?.Invoke(boss, phase);

    public static void RaiseBossDefeated(BossBase boss)
        => BossDefeated?.Invoke(boss);
}
