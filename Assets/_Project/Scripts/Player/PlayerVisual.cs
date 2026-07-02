using UnityEngine;

/// <summary>
/// Visual do Fragmento Zero conforme o conceito final da Etapa 5:
/// orbe de plasma com núcleo branco brilhante, halo suave e anel de
/// energia pulsante. As camadas seguem a cor do corpo (evolução/gelo)
/// e o piscar de invencibilidade.
/// </summary>
public class PlayerVisual : MonoBehaviour
{
    SpriteRenderer body;
    SpriteRenderer glow;
    SpriteRenderer ring;
    SpriteRenderer core;

    void Start()
    {
        body = GetComponent<SpriteRenderer>();
        if (body != null) body.sprite = RuntimeSprites.Circle;   // orbe nítido

        glow = CreateLayer("Glow", RuntimeSprites.Glow, 2.3f, -1);
        ring = CreateLayer("Ring", RuntimeSprites.Ring, 1.3f, 1);
        core = CreateLayer("Core", RuntimeSprites.Circle, 0.42f, 2);
        core.color = new Color(1f, 1f, 1f, 0.95f);
    }

    void LateUpdate()
    {
        if (body == null) return;

        // Segue o piscar de invencibilidade do corpo
        bool visible = body.enabled;
        glow.enabled = visible;
        ring.enabled = visible;
        core.enabled = visible;
        if (!visible) return;

        // Camadas herdam a cor atual (evolução muda, gelo tinge)
        Color c = body.color;
        glow.color = new Color(c.r, c.g, c.b, 0.40f);
        ring.color = new Color(c.r, c.g, c.b, 0.85f);

        // Respiração sutil do anel e do halo — energia viva
        float pulse = Mathf.Sin(Time.time * 4f);
        ring.transform.localScale = Vector3.one * (1.3f + 0.055f * pulse);
        glow.transform.localScale = Vector3.one * (2.3f + 0.12f  * pulse);
    }

    SpriteRenderer CreateLayer(string name, Sprite sprite, float scale, int sortingOrder)
    {
        var go = new GameObject(name);
        go.transform.SetParent(transform, false);
        go.transform.localScale = Vector3.one * scale;
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite       = sprite;
        sr.sortingOrder = sortingOrder;
        return sr;
    }
}
