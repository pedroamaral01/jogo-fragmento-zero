using UnityEngine;
using UnityEngine.UI;

public class ScreenEffects : MonoBehaviour
{
    public static ScreenEffects Instance { get; private set; }

    [SerializeField] Camera mainCamera;

    // Full-screen UI Image — set color to (1, 0.6, 0, 0) initially
    [SerializeField] Image flashImage;

    // Radial-gradient UI Image for ice vignette — set color to (0, 0.6, 1, 0) initially
    [SerializeField] Image iceVignette;

    float shakeMagnitude;
    float shakeTimer;
    Vector3 cameraOrigin;

    float flashAlpha;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        if (mainCamera == null) mainCamera = Camera.main;
        cameraOrigin = mainCamera.transform.position;
    }

    void Update()
    {
        UpdateShake();
        UpdateFlash();
        UpdateIceVignette();
    }

    void UpdateShake()
    {
        if (shakeTimer > 0f)
        {
            shakeTimer -= Time.deltaTime;
            mainCamera.transform.position =
                cameraOrigin + (Vector3)(Random.insideUnitCircle * shakeMagnitude);
        }
        else
        {
            mainCamera.transform.position = cameraOrigin;
        }
    }

    void UpdateFlash()
    {
        flashAlpha *= Mathf.Pow(0.85f, Time.deltaTime * 60f); // frame-rate independent decay
        SetImageAlpha(flashImage, flashAlpha);
    }

    void UpdateIceVignette()
    {
        if (iceVignette == null) return;
        bool  active = PowerIce.Instance != null && PowerIce.Instance.IsActive;
        float target = active ? 0.45f : 0f;
        var   c      = iceVignette.color;
        iceVignette.color = new Color(c.r, c.g, c.b,
            Mathf.MoveTowards(c.a, target, Time.deltaTime * 2f));
    }

    public void TriggerShake(float magnitude, float duration)
    {
        shakeMagnitude = magnitude;
        shakeTimer     = Mathf.Max(shakeTimer, duration);
    }

    public void TriggerFlash(float alpha)
    {
        flashAlpha = Mathf.Max(flashAlpha, alpha);
        SetImageAlpha(flashImage, flashAlpha);
    }

    static void SetImageAlpha(Image img, float a)
    {
        if (img == null) return;
        var c = img.color;
        img.color = new Color(c.r, c.g, c.b, a);
    }
}
