using UnityEngine;

public class Meteor : ObstacleBase
{
    protected override void Awake()
    {
        base.Awake();

        // O prefab referencia um sprite built-in do Unity (só para autoria no
        // editor); em runtime trocamos pelo RuntimeSprites.Circle, que é
        // garantidamente 1 unidade de mundo por unidade de escala — o mesmo
        // sprite que o player usa. Sem isso o meteoro renderiza bem menor do
        // que localScale sugere, desequilibrando o tamanho contra o player.
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null) sr.sprite = RuntimeSprites.Circle;
    }

    protected override float GetSlowFactor()
    {
        return PowerIce.Instance != null && PowerIce.Instance.IsActive ? 0.25f : 1f;
    }
}
