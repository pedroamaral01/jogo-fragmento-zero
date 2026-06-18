using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance { get; private set; }

    [Header("Movement")]
    [SerializeField] float laneSmoothing  = 10f;

    [Header("Energy")]
    [SerializeField] float startEnergy    = 60f;
    [SerializeField] float drainPerFrame  = 0.02f;   // matches original
    [SerializeField] float hitDamage      = 25f;
    [SerializeField] float invincibleSecs = 1.17f;   // 70 frames / 60 fps

    [Header("References")]
    [SerializeField] SpriteRenderer body;
    [SerializeField] TrailRenderer  trail;
    [SerializeField] ParticleSystem hitParticles;

    [Header("Colors")]
    [SerializeField] Color colorNormal = new Color(0f, 1f, 1f);
    [SerializeField] Color colorIce    = new Color(0.53f, 0.93f, 1f);

    public float Energy      { get; private set; }
    public int   CurrentLane { get; private set; } = 2;
    public bool  IsInvincible => invTimer > 0f;

    float invTimer;
    float blinkAccum;

    PowerFire firePower;
    PowerIce  icePower;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        Energy = startEnergy;
        transform.position = new Vector3(-5f, LaneSystem.Instance.GetLaneY(CurrentLane), 0f);
        firePower = GetComponent<PowerFire>();
        icePower  = GetComponent<PowerIce>();

        var col = GetComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius    = 0.23f;
    }

    void Update()
    {
        if (GameManager.Instance.State != GameManager.GameState.Playing) return;

        HandleInput();
        SmoothToLane();
        Drain(drainPerFrame * Time.deltaTime * 60f);
        HandleInvincibility();
        UpdateVisuals();
    }

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.Space))
        {
            if (CurrentLane > 0) CurrentLane--;
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (CurrentLane < LaneSystem.Instance.LaneCount - 1) CurrentLane++;
        }
        if (Input.GetKeyDown(KeyCode.A)) firePower?.TryShoot();
        if (Input.GetKeyDown(KeyCode.D)) icePower?.TryActivate();
    }

    void SmoothToLane()
    {
        float ty = LaneSystem.Instance.GetLaneY(CurrentLane);
        float ny = Mathf.Lerp(transform.position.y, ty, laneSmoothing * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, ny, 0f);
    }

    void HandleInvincibility()
    {
        if (invTimer > 0f)
        {
            invTimer   -= Time.deltaTime;
            blinkAccum += Time.deltaTime;
            if (body != null) body.enabled = Mathf.FloorToInt(blinkAccum * 12f) % 2 == 0;
        }
        else
        {
            blinkAccum = 0f;
            if (body != null) body.enabled = true;
        }
    }

    void UpdateVisuals()
    {
        Color c = (icePower != null && icePower.IsActive) ? colorIce : colorNormal;
        if (body  != null) body.color = c;
        if (trail != null)
        {
            trail.startColor = c;
            trail.endColor   = new Color(c.r, c.g, c.b, 0f);
        }
    }

    public void TakeHit()
    {
        if (IsInvincible) return;
        Drain(hitDamage);
        invTimer   = invincibleSecs;
        blinkAccum = 0f;
        if (hitParticles != null) hitParticles.Play();
        if (ScreenEffects.Instance != null) ScreenEffects.Instance.TriggerShake(0.25f, 0.35f);
    }

    // Used by powers and obstacles — positive = gain, negative = spend
    public void ModifyEnergy(float delta)
    {
        Energy = Mathf.Clamp(Energy + delta, 0f, 100f);
        if (Energy <= 0f) GameManager.Instance.TriggerGameOver();
    }

    void Drain(float amount)
    {
        Energy -= amount;
        if (Energy <= 0f)
        {
            Energy = 0f;
            GameManager.Instance.TriggerGameOver();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
            other.GetComponent<ObstacleBase>()?.OnHitPlayer();
        else if (other.CompareTag("Crystal"))
            other.GetComponent<Crystal>()?.OnCollected();
    }
}
