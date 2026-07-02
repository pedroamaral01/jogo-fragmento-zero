using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Raio — ataque em cadeia (tecla S): salta do player para o inimigo mais
/// próximo e segue encadeando até maxTargets, limpando hordas.
/// </summary>
public class PowerLightning : PowerBase
{
    [SerializeField] int   maxTargets  = 4;
    [SerializeField] float chainRadius = 5.5f;   // alcance entre um elo e o próximo
    [SerializeField] int   damage      = 2;

    // Janela visível da pista (spawn em x=9, despawn em x=-10)
    const float MinX = -8f, MaxX = 9f;

    public override string  DisplayName => "RAIO";
    public override KeyCode Key         => KeyCode.S;
    public override Color   ThemeColor  => new Color(1f, 0.95f, 0.3f);

    void Awake()
    {
        if (energyCost   <= 0f) energyCost   = 25f;
        if (cooldownSecs <= 0f) cooldownSecs = 6f;
    }

    protected override void Activate()
    {
        Vector3 from = transform.position;

        foreach (var target in FindChainTargets())
        {
            Vector3 hitPos = target.transform.position;
            LightningBoltVFX.Spawn(from, hitPos, ThemeColor);
            target.TakeDamage(damage);

            // Feedback de impacto próprio (não depende do inimigo morrer) —
            // deixa claro quem foi atingido mesmo quando o alvo sobrevive.
            BurstVFX.Spawn(hitPos, ThemeColor, 14, 5.5f, 0.4f);

            from = hitPos;
        }
    }

    List<ObstacleBase> FindChainTargets()
    {
        var candidates = new List<ObstacleBase>();
        foreach (var obstacle in FindObjectsByType<ObstacleBase>(FindObjectsSortMode.None))
        {
            float x = obstacle.transform.position.x;
            if (x >= MinX && x <= MaxX) candidates.Add(obstacle);
        }

        // Encadeia sempre para o inimigo mais próximo do elo anterior
        var chain = new List<ObstacleBase>();
        Vector3 from = transform.position;

        while (chain.Count < maxTargets && candidates.Count > 0)
        {
            ObstacleBase nearest = null;
            float bestSqr = chainRadius * chainRadius;

            foreach (var c in candidates)
            {
                float sqr = (c.transform.position - from).sqrMagnitude;
                if (sqr < bestSqr) { bestSqr = sqr; nearest = c; }
            }

            if (nearest == null) break;

            chain.Add(nearest);
            candidates.Remove(nearest);
            from = nearest.transform.position;
        }

        return chain;
    }
}
