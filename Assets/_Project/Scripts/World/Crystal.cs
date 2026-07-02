using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Crystal : MonoBehaviour
{
    [SerializeField] float energyReward   = 18f;
    [SerializeField] float scoreReward    = 50f;
    [SerializeField] float rotationSpeed  = 120f;
    [SerializeField] GameObject collectEffectPrefab;

    const float DespawnX = -10f;

    float speedMult = 1f;

    /// <summary>Mesmo multiplicador dos obstáculos do padrão — colunas coesas.</summary>
    public void OverrideSpeedMultiplier(float multiplier) => speedMult = multiplier;

    void Start()
    {
        GetComponent<CircleCollider2D>().isTrigger = true;
    }

    void Update()
    {
        if (!GameManager.Instance.IsGameplayActive) return;

        bool magnetized = PowerGravity.Instance != null && PowerGravity.Instance.IsActive
                          && PlayerController.Instance != null;

        if (magnetized)
        {
            // Gravidade ativa: o cristal é puxado direto para o player
            transform.position = Vector3.MoveTowards(transform.position,
                PlayerController.Instance.transform.position,
                PowerGravity.Instance.PullSpeed * Time.deltaTime);
        }
        else
        {
            float slow = PowerIce.Instance != null && PowerIce.Instance.IsActive ? 0.25f : 1f;
            transform.position += Vector3.left * GameManager.Instance.Speed * speedMult * slow * Time.deltaTime;
        }

        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

        if (transform.position.x < DespawnX) Destroy(gameObject);
    }

    public void OnCollected()
    {
        // Energia, score e feedback são aplicados pelos assinantes do evento
        GameEvents.RaiseCrystalCollected(transform.position, energyReward, scoreReward);

        if (collectEffectPrefab != null)
            Instantiate(collectEffectPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
