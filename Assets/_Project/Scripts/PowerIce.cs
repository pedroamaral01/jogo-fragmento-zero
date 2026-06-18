using UnityEngine;

public class PowerIce : MonoBehaviour
{
    public static PowerIce Instance { get; private set; }

    [SerializeField] float durationSecs = 4f;
    [SerializeField] float energyCost   = 22f;
    [SerializeField] ParticleSystem activateParticles;

    float timer;

    public bool  IsActive   => timer > 0f;
    public float TimeLeft   => timer;
    public float SlowFactor => IsActive ? 0.25f : 1f;
    public bool  IsDroneFrozen => IsActive;

    public bool IsReady => timer <= 0f && PlayerController.Instance != null
                                       && PlayerController.Instance.Energy >= energyCost;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Update()
    {
        if (timer > 0f) timer -= Time.deltaTime;
    }

    public void TryActivate()
    {
        if (!IsReady) return;
        PlayerController.Instance.ModifyEnergy(-energyCost);
        timer = durationSecs;
        if (activateParticles != null) activateParticles.Play();
    }
}
