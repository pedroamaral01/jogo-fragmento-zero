using UnityEngine;

public class LaneSystem : MonoBehaviour
{
    public static LaneSystem Instance { get; private set; }

    [SerializeField] int   laneCount = 4;
    [SerializeField] float topY      =  2.25f;
    [SerializeField] float bottomY   = -2.25f;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    public int LaneCount => laneCount;

    public float GetLaneY(int lane)
    {
        float t = laneCount <= 1 ? 0.5f : (float)lane / (laneCount - 1);
        return Mathf.Lerp(topY, bottomY, t);
    }
}
