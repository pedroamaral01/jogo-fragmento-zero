using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class Bullet : MonoBehaviour
{
    [SerializeField] float      speed       = 20f;
    [SerializeField] float      lifetime    = 1.17f;  // 70 frames / 60 fps
    [SerializeField] GameObject hitEffectPrefab;

    float life;

    void Start()
    {
        life = lifetime;
        GetComponent<CircleCollider2D>().isTrigger = true;
    }

    void Update()
    {
        transform.position += Vector3.right * speed * Time.deltaTime;
        life -= Time.deltaTime;
        if (life <= 0f || transform.position.x > 10f) Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Obstacle")) return;

        // Interface cobre obstáculos, chefes e projéteis destrutíveis
        var damageable = other.GetComponent<IDamageable>();
        if (damageable == null) return;

        damageable.TakeDamage(1);

        if (hitEffectPrefab != null)
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
