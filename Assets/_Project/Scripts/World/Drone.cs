using UnityEngine;

public class Drone : ObstacleBase
{
    [SerializeField] SpriteRenderer bodyRenderer;
    [SerializeField] SpriteRenderer eyeRenderer;
    [SerializeField] GameObject     iceSpikesObject;   // child GameObject with ice spike visuals

    // Identidade visual: carcaça = Estruturas IA; olho = Alerta/Erro IA
    [SerializeField] Color colorBody       = new Color(0.24f, 0.30f, 0.38f);
    [SerializeField] Color colorBodyFrozen = new Color(0.62f, 0.80f, 0.92f);
    [SerializeField] Color colorEye        = new Color(1f, 0f, 0.235f);
    [SerializeField] Color colorEyeFrozen  = new Color(0.75f, 0.90f, 1f);

    protected override void Awake()
    {
        maxHp = 2;
        base.Awake();

        // Ver comentário em Meteor.Awake: sprite built-in do editor -> sprite
        // 1-unidade do RuntimeSprites, para bater o tamanho real com o player.
        if (bodyRenderer != null) bodyRenderer.sprite = RuntimeSprites.Square;
    }

    // Frozen drones stop completely
    protected override float GetSlowFactor()
    {
        return PowerIce.Instance != null && PowerIce.Instance.IsDroneFrozen ? 0f : 1f;
    }

    protected override void OnObstacleUpdate()
    {
        bool frozen = PowerIce.Instance != null && PowerIce.Instance.IsDroneFrozen;

        if (bodyRenderer != null) bodyRenderer.color = frozen ? colorBodyFrozen : colorBody;
        if (eyeRenderer  != null) eyeRenderer.color  = frozen ? colorEyeFrozen  : colorEye;
        if (iceSpikesObject != null) iceSpikesObject.SetActive(frozen);
    }
}
