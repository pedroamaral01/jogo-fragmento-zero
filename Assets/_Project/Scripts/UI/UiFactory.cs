using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

/// <summary>
/// Fábrica de UI construída em runtime. O projeto gera toda a interface por
/// código (mesmo padrão dos editor-scripts do Vertical Slice), o que elimina
/// dependência de wiring manual na cena.
/// </summary>
public static class UiFactory
{
    public static readonly Color Cyan   = ArtPalette.Cyan;   // #66FCF1 — Ciano Base
    public static readonly Color Dark   = new Color(0.043f, 0.047f, 0.063f, 0.97f);   // Vazio Cósmico
    public static readonly Color Panel  = new Color(0.122f, 0.157f, 0.20f, 0.9f);     // Estruturas IA

    public static Canvas CreateCanvas(Transform parent, string name, int sortingOrder)
    {
        var go = new GameObject(name);
        if (parent != null) go.transform.SetParent(parent, false);

        var canvas = go.AddComponent<Canvas>();
        canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = sortingOrder;

        var scaler = go.AddComponent<CanvasScaler>();
        scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);
        scaler.matchWidthOrHeight  = 0.5f;

        go.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    public static void EnsureEventSystem(Transform parent)
    {
        if (Object.FindFirstObjectByType<EventSystem>() != null) return;

        var go = new GameObject("EventSystem");
        if (parent != null) go.transform.SetParent(parent, false);
        go.AddComponent<EventSystem>();
        // activeInputHandler = Both no projeto, então o módulo legado funciona
        go.AddComponent<StandaloneInputModule>();
    }

    /// <summary>Painel que cobre a tela inteira.</summary>
    public static GameObject FullscreenPanel(Transform parent, string name, Color color)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        go.AddComponent<Image>().color = color;
        return go;
    }

    public static RectTransform Rect(Transform parent, string name,
        Vector2 anchor, Vector2 pos, Vector2 size)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchor;
        rt.anchorMax = anchor;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        return rt;
    }

    public static TMP_Text Text(Transform parent, string name, string content,
        float fontSize, Vector2 anchor, Vector2 pos, Vector2 size,
        Color color, TextAlignmentOptions alignment = TextAlignmentOptions.Center)
    {
        var rt  = Rect(parent, name, anchor, pos, size);
        var tmp = rt.gameObject.AddComponent<TextMeshProUGUI>();
        tmp.text      = content;
        tmp.fontSize  = fontSize;
        tmp.color     = color;
        tmp.alignment = alignment;
        return tmp;
    }

    public static Button Button(Transform parent, string name, string label,
        Vector2 anchor, Vector2 pos, Vector2 size, System.Action onClick)
    {
        var rt  = Rect(parent, name, anchor, pos, size);
        var img = rt.gameObject.AddComponent<Image>();
        img.color = new Color(0f, 1f, 1f, 0.12f);

        var btn = rt.gameObject.AddComponent<Button>();
        var colors = btn.colors;
        colors.highlightedColor = new Color(0f, 1f, 1f, 0.45f);
        colors.pressedColor     = new Color(1f, 1f, 1f, 0.6f);
        btn.colors = colors;
        btn.onClick.AddListener(() =>
        {
            if (AudioManager.Instance != null) AudioManager.Instance.PlayClick();
            onClick?.Invoke();
        });

        Text(rt, "Label", label, 20f, new Vector2(0.5f, 0.5f), Vector2.zero, size, Color.white);
        return btn;
    }

    /// <summary>Quadrado colorido com um número — selo de classificação indicativa.</summary>
    public static void RatingSeal(Transform parent, string ratingLabel, Color sealColor, Vector2 pos, float side)
    {
        var rt = Rect(parent, "RatingSeal", new Vector2(0.5f, 0.5f), pos, new Vector2(side, side));
        rt.gameObject.AddComponent<Image>().color = sealColor;
        Text(rt, "Value", ratingLabel, side * 0.55f, new Vector2(0.5f, 0.5f), Vector2.zero,
             new Vector2(side, side), Color.white);
    }
}
