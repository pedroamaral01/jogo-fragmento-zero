using UnityEngine;

public class Meteor : ObstacleBase
{
    protected override float GetSlowFactor()
    {
        return PowerIce.Instance != null && PowerIce.Instance.IsActive ? 0.25f : 1f;
    }
}
