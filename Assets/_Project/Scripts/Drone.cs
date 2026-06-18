using UnityEngine;

public class Drone : ObstacleBase
{
    [SerializeField] SpriteRenderer bodyRenderer;
    [SerializeField] SpriteRenderer eyeRenderer;
    [SerializeField] GameObject     iceSpikesObject;   // child GameObject with ice spike visuals

    [SerializeField] Color colorBody       = new Color(1f, 0f, 1f);
    [SerializeField] Color colorBodyFrozen = new Color(0.67f, 0.87f, 1f);
    [SerializeField] Color colorEye        = new Color(1f, 0.27f, 0.27f);
    [SerializeField] Color colorEyeFrozen  = new Color(0.8f, 0.93f, 1f);

    protected override void Awake()
    {
        maxHp = 2;
        base.Awake();
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
