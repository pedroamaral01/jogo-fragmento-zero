using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public static class SetupPowerBars
{
    [MenuItem("FragmentoZero/Setup Power Bars")]
    static void Setup()
    {
        var hud = Object.FindFirstObjectByType<HUDController>();
        if (hud == null) { Debug.LogError("HUDController não encontrado na cena!"); return; }

        var canvasT = hud.GetComponent<Canvas>().transform;

        // Remove painel anterior se já existir
        var old = canvasT.Find("PowerBars");
        if (old != null) Object.DestroyImmediate(old.gameObject);

        // Painel raiz
        var panel = MakeRect(canvasT, "PowerBars",
            new Vector2(0, 0), new Vector2(0, 0),
            new Vector2(10, 50), new Vector2(160, 56));

        // --- Fogo (linha 0) ---
        var fireBg   = MakeBarBG(panel.transform, "FireBG",   0);
        var fireFill = MakeBarFill(fireBg.transform, "FireFill", new Color(1f, 0.6f, 0.1f));
        var fireLbl  = MakeLabel(panel.transform, "FireLabel", "A  FOGO ►", 0);

        // --- Gelo (linha 1) ---
        var iceBg   = MakeBarBG(panel.transform, "IceBG",   1);
        var iceFill = MakeBarFill(iceBg.transform, "IceFill", new Color(0.2f, 0.9f, 1f));
        var iceLbl  = MakeLabel(panel.transform, "IceLabel", "D  GELO [OK]", 1);

        // Conectar ao HUDController
        var so = new SerializedObject(hud);
        so.FindProperty("fireCooldownFill").objectReferenceValue = fireFill.GetComponent<Image>();
        so.FindProperty("iceDurationFill").objectReferenceValue  = iceFill.GetComponent<Image>();
        so.FindProperty("fireLabel").objectReferenceValue        = fireLbl.GetComponent<TMP_Text>();
        so.FindProperty("iceLabel").objectReferenceValue         = iceLbl.GetComponent<TMP_Text>();
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(hud);

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

        Debug.Log("FragmentoZero: Power Bars criadas e conectadas ao HUDController!");
    }

    // ── Helpers ─────────────────────────────────────────────────────────────

    static GameObject MakeRect(Transform parent, string name,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 pos, Vector2 size)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
        return go;
    }

    static GameObject MakeBarBG(Transform parent, string name, int row)
    {
        var go = MakeRect(parent, name,
            new Vector2(0, 1), new Vector2(0, 1),
            new Vector2(0, -row * 26f), new Vector2(110, 10));
        var img = go.AddComponent<Image>();
        img.color = new Color(0.05f, 0.05f, 0.05f, 0.75f);
        return go;
    }

    static GameObject MakeBarFill(Transform parent, string name, Color color)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        var img = go.AddComponent<Image>();
        img.color      = color;
        img.type       = Image.Type.Filled;
        img.fillMethod = Image.FillMethod.Horizontal;
        img.fillAmount = 1f;
        return go;
    }

    static GameObject MakeLabel(Transform parent, string name, string text, int row)
    {
        var go = MakeRect(parent, name,
            new Vector2(0, 1), new Vector2(0, 1),
            new Vector2(114, -row * 26f - 1f), new Vector2(50, 12));
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text     = text;
        tmp.fontSize = 8f;
        tmp.color    = Color.white;
        return go;
    }
}
