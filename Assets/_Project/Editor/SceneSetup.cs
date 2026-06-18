using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public static class SceneSetup
{
    [MenuItem("FragmentoZero/Setup Scene")]
    static void Setup()
    {
        // ---- GameManager ----
        var gm = new GameObject("GameManager");
        gm.AddComponent<GameManager>();

        // ---- LaneSystem ----
        var ls = new GameObject("LaneSystem");
        ls.AddComponent<LaneSystem>();

        // ---- ObstacleSpawner ----
        var sp = new GameObject("ObstacleSpawner");
        sp.AddComponent<ObstacleSpawner>();

        // ---- ScreenEffects ----
        var se = new GameObject("ScreenEffects");
        se.AddComponent<ScreenEffects>();

        // ---- Player ----
        var player = new GameObject("Player");
        player.transform.position = new Vector3(-5f, 0f, 0f);
        player.AddComponent<PlayerController>();
        player.AddComponent<PowerFire>();
        player.AddComponent<PowerIce>();
        var rb = player.AddComponent<Rigidbody2D>();
        rb.bodyType    = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        var col = player.AddComponent<CircleCollider2D>();
        col.radius = 0.35f;
        col.isTrigger = true;
        // Simple visual: white circle sprite
        var sr = player.AddComponent<SpriteRenderer>();
        sr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
        sr.color = new Color(0.2f, 0.9f, 1f);
        sr.sortingLayerName = "Default";

        // ---- Trail ----
        var trail = player.AddComponent<TrailRenderer>();
        trail.time = 0.2f;
        trail.startWidth = 0.3f;
        trail.endWidth = 0f;
        trail.sortingLayerName = "Default";
        var mat = new Material(Shader.Find("Sprites/Default"));
        trail.material = mat;
        trail.startColor = new Color(0.2f, 0.9f, 1f, 1f);
        trail.endColor   = new Color(0.2f, 0.9f, 1f, 0f);

        // ---- HUD Canvas ----
        var canvasGO = new GameObject("HUD");
        var canvas   = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 10;
        canvasGO.AddComponent<CanvasScaler>();
        canvasGO.AddComponent<GraphicRaycaster>();
        canvasGO.AddComponent<HUDController>();

        // Score text
        var scoreTxtGO = CreateTMP(canvasGO.transform, "ScoreText", "0 m",
            new Vector2(0, 1), new Vector2(0, 1), new Vector2(10, -10), new Vector2(200, 30));

        // Energy bar (image)
        var energyBarGO = new GameObject("EnergyFill");
        energyBarGO.transform.SetParent(canvasGO.transform, false);
        var ebRT = energyBarGO.AddComponent<RectTransform>();
        ebRT.anchorMin = new Vector2(0, 0);
        ebRT.anchorMax = new Vector2(0, 0);
        ebRT.anchoredPosition = new Vector2(10, 10);
        ebRT.sizeDelta = new Vector2(120, 14);
        var ebImg = energyBarGO.AddComponent<Image>();
        ebImg.color = new Color(0f, 1f, 1f);
        ebImg.type = Image.Type.Filled;
        ebImg.fillMethod = Image.FillMethod.Horizontal;
        ebImg.fillAmount = 0.6f;

        // Game Over panel
        var govGO = new GameObject("GameOverPanel");
        govGO.transform.SetParent(canvasGO.transform, false);
        var govRT = govGO.AddComponent<RectTransform>();
        govRT.anchorMin = Vector2.zero;
        govRT.anchorMax = Vector2.one;
        govRT.offsetMin = Vector2.zero;
        govRT.offsetMax = Vector2.zero;
        var govImg = govGO.AddComponent<Image>();
        govImg.color = new Color(0, 0, 0, 0.75f);
        govGO.SetActive(false);

        var govTxt = CreateTMP(govGO.transform, "GameOverText", "GAME OVER",
            new Vector2(0.5f, 0.6f), new Vector2(0.5f, 0.6f), Vector2.zero, new Vector2(300, 60));

        var govScore = CreateTMP(govGO.transform, "GameOverScore", "0 metros",
            new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(300, 40));

        var govInstr = CreateTMP(govGO.transform, "GameOverInstructions", "Pressione R para reiniciar",
            new Vector2(0.5f, 0.4f), new Vector2(0.5f, 0.4f), Vector2.zero, new Vector2(300, 30));

        // Wire HUDController references
        var hud = canvasGO.GetComponent<HUDController>();
        var so = new SerializedObject(hud);
        so.FindProperty("energyFill").objectReferenceValue = ebImg;
        so.FindProperty("scoreText").objectReferenceValue = scoreTxtGO;
        so.FindProperty("gameOverPanel").objectReferenceValue = govGO;
        so.FindProperty("gameOverScore").objectReferenceValue = govScore;
        so.FindProperty("gameOverInstructions").objectReferenceValue = govInstr;
        so.ApplyModifiedProperties();

        // Wire ScreenEffects references
        var seComp = se.GetComponent<ScreenEffects>();
        var seSO = new SerializedObject(seComp);
        seSO.FindProperty("mainCamera").objectReferenceValue = Camera.main;
        seSO.ApplyModifiedProperties();

        // Save scene
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

        Debug.Log("FragmentoZero: cena configurada! Pressione Play para testar.");
    }

    static TMP_Text CreateTMP(Transform parent, string name, string text,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 position, Vector2 size)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = anchorMin;
        rt.anchorMax = anchorMax;
        rt.anchoredPosition = position;
        rt.sizeDelta = size;
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = 18;
        tmp.color = Color.white;
        return tmp;
    }
}
