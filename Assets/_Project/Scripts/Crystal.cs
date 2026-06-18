using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Crystal : MonoBehaviour
{
    [SerializeField] float energyReward   = 18f;
    [SerializeField] float scoreReward    = 50f;
    [SerializeField] float rotationSpeed  = 120f;
    [SerializeField] GameObject collectEffectPrefab;

    const float DespawnX = -10f;

    void Start()
    {
        GetComponent<CircleCollider2D>().isTrigger = true;
    }

    void Update()
    {
        if (GameManager.Instance.State != GameManager.GameState.Playing) return;

        float slow = PowerIce.Instance != null && PowerIce.Instance.IsActive ? 0.25f : 1f;
        transform.position += Vector3.left * GameManager.Instance.Speed * slow * Time.deltaTime;
        transform.Rotate(0f, 0f, rotationSpeed * Time.deltaTime);

        if (transform.position.x < DespawnX) Destroy(gameObject);
    }

    public void OnCollected()
    {
        PlayerController.Instance?.ModifyEnergy(energyReward);
        GameManager.Instance?.AddScore(scoreReward);
        ScreenEffects.Instance?.TriggerFlash(0.08f);

        if (collectEffectPrefab != null)
            Instantiate(collectEffectPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
