using UnityEngine;
using UnityEditor;

/// <summary>
/// Gera os prefabs de gameplay seguindo a identidade visual da Etapa 5.
/// Colliders, tags e escalas de raiz são idênticos aos originais — apenas
/// o visual muda. Rodar de novo substitui o conteúdo mantendo os GUIDs
/// (nenhuma referência de cena/spawner quebra).
/// </summary>
public static class CreatePrefabs
{
    const string PrefabFolder = "Assets/_Project/Prefabs";

    [MenuItem("FragmentoZero/Create Obstacle Prefabs")]
    public static void Create()
    {
        if (!AssetDatabase.IsValidFolder(PrefabFolder))
            AssetDatabase.CreateFolder("Assets/_Project", "Prefabs");

        var meteor  = MakeMeteor();
        var drone   = MakeDrone();
        var crystal = MakeCrystal();
        var bullet  = MakeBullet();

        // Wiring só é necessário na primeira geração (GUIDs persistem depois)
        var spawner = Object.FindFirstObjectByType<ObstacleSpawner>();
        if (spawner != null)
        {
            var so = new SerializedObject(spawner);
            so.FindProperty("meteorPrefab").objectReferenceValue  = meteor;
            so.FindProperty("dronePrefab").objectReferenceValue   = drone;
            so.FindProperty("crystalPrefab").objectReferenceValue = crystal;
            so.ApplyModifiedProperties();
            EditorUtility.SetDirty(spawner);
        }

        var powerFire = Object.FindFirstObjectByType<PowerFire>();
        if (powerFire != null)
        {
            var pfSO = new SerializedObject(powerFire);
            pfSO.FindProperty("bulletPrefab").objectReferenceValue = bullet;
            pfSO.ApplyModifiedProperties();
            EditorUtility.SetDirty(powerFire);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());

        Debug.Log("FragmentoZero: prefabs (re)gerados com a identidade visual da Etapa 5.");
    }

    // ── METEORO: rocha escura dessaturada com crateras e fissura de magma ───
    static GameObject MakeMeteor()
    {
        var go = new GameObject("Meteor");
        go.tag = "Obstacle";
        AddKinematicBody(go);

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = Circle();
        sr.color  = new Color(0.38f, 0.36f, 0.34f);   // rocha neutra dessaturada
        go.transform.localScale = new Vector3(0.7f, 0.7f, 1f);

        AddChildSprite(go, "Crater1", Circle(), new Color(0.20f, 0.19f, 0.18f, 0.9f),
            new Vector3(-0.18f, 0.15f, 0f), new Vector3(0.32f, 0.32f, 1f), 1);
        AddChildSprite(go, "Crater2", Circle(), new Color(0.22f, 0.21f, 0.20f, 0.85f),
            new Vector3(0.2f, -0.12f, 0f), new Vector3(0.22f, 0.22f, 1f), 1);

        // Fissura de magma — o Fogo destrói rocha (afinidade visual)
        var crack = AddChildSprite(go, "MagmaCrack", Square(), new Color(1f, 0.30f, 0.05f, 0.65f),
            new Vector3(0.02f, 0.05f, 0f), new Vector3(0.55f, 0.07f, 1f), 1);
        crack.transform.localRotation = Quaternion.Euler(0f, 0f, -28f);

        var col = go.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius    = 0.5f;

        go.AddComponent<Meteor>();
        return SaveAndDestroy(go, "Meteor");
    }

    // ── DRONE: carcaça cinza industrial, olho e mira laser vermelhos ────────
    static GameObject MakeDrone()
    {
        var go = new GameObject("Drone");
        go.tag = "Obstacle";
        AddKinematicBody(go);

        // Casco (renderer raiz — Drone.cs tinta ao congelar)
        var body = go.AddComponent<SpriteRenderer>();
        body.sprite = Square();
        body.color  = new Color(0.24f, 0.30f, 0.38f);  // Estruturas IA (variante clara)
        go.transform.localScale = new Vector3(0.75f, 0.5f, 1f);

        var col = go.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size      = new Vector2(1f, 1f);

        // Blindagem superior escura (leitura angular)
        AddChildSprite(go, "Armor", Square(), new Color(0.122f, 0.157f, 0.20f),
            new Vector3(0f, 0.32f, 0f), new Vector3(1.05f, 0.4f, 1f), 0);

        // Olho vermelho com brilho
        AddChildSprite(go, "EyeGlow", Circle(), new Color(1f, 0f, 0.235f, 0.45f),
            new Vector3(-0.28f, 0.05f, 0f), new Vector3(0.8f, 1.1f, 1f), 1);
        var eye = AddChildSprite(go, "Eye", Circle(), new Color(1f, 0f, 0.235f),
            new Vector3(-0.28f, 0.05f, 0f), new Vector3(0.42f, 0.6f, 1f), 2);

        // Mira laser — assinatura visual da IA (telegrafamento da faixa)
        AddChildSprite(go, "Laser", Square(), new Color(1f, 0f, 0.235f, 0.35f),
            new Vector3(-3.1f, 0.05f, 0f), new Vector3(5.5f, 0.05f, 1f), 0);

        // Antena
        AddChildSprite(go, "Antenna", Square(), new Color(0.55f, 0.60f, 0.68f),
            new Vector3(0.25f, 0.62f, 0f), new Vector3(0.06f, 0.5f, 1f), 0);

        // Espinhos de gelo — ativados pelo Drone.cs quando congelado
        var spikes = new GameObject("IceSpikes");
        spikes.transform.SetParent(go.transform, false);
        AddChildSprite(spikes, "Spike1", Square(), new Color(0.8f, 0.95f, 1f, 0.9f),
            new Vector3(-0.3f, 0.4f, 0f), new Vector3(0.28f, 0.28f, 1f), 3, rotationZ: 45f);
        AddChildSprite(spikes, "Spike2", Square(), new Color(0.85f, 0.97f, 1f, 0.9f),
            new Vector3(0.25f, 0.5f, 0f), new Vector3(0.2f, 0.2f, 1f), 3, rotationZ: 45f);
        AddChildSprite(spikes, "Spike3", Square(), new Color(0.8f, 0.95f, 1f, 0.9f),
            new Vector3(0.42f, -0.35f, 0f), new Vector3(0.24f, 0.24f, 1f), 3, rotationZ: 45f);
        spikes.SetActive(false);

        var droneComp = go.AddComponent<Drone>();

        var prefab = SaveAndDestroy(go, "Drone");

        // Wiring dos renderers no prefab salvo
        var saved = prefab.GetComponent<Drone>();
        var so = new SerializedObject(saved);
        so.FindProperty("bodyRenderer").objectReferenceValue    = prefab.GetComponent<SpriteRenderer>();
        so.FindProperty("eyeRenderer").objectReferenceValue     = prefab.transform.Find("Eye").GetComponent<SpriteRenderer>();
        so.FindProperty("iceSpikesObject").objectReferenceValue = prefab.transform.Find("IceSpikes").gameObject;
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(prefab);
        return prefab;
    }

    // ── CRISTAL: diamante ciano com núcleo branco e halo ────────────────────
    static GameObject MakeCrystal()
    {
        var go = new GameObject("Crystal");
        go.tag = "Crystal";
        AddKinematicBody(go);

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = Square();
        sr.color  = new Color(0.40f, 0.988f, 0.945f);  // Ciano Base #66FCF1
        go.transform.localRotation = Quaternion.Euler(0f, 0f, 45f);
        go.transform.localScale    = new Vector3(0.45f, 0.45f, 1f);

        // Halo (tintado junto com o corpo pelo Crystal.SetKind)
        AddChildSprite(go, "Glow", Circle(), new Color(0.40f, 0.988f, 0.945f, 0.35f),
            Vector3.zero, new Vector3(2.8f, 2.8f, 1f), -1);

        // Núcleo branco (permanece branco em qualquer variante)
        AddChildSprite(go, "Core", Square(), new Color(1f, 1f, 1f, 0.95f),
            Vector3.zero, new Vector3(0.45f, 0.45f, 1f), 1);

        var col = go.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius    = 0.5f;

        go.AddComponent<Crystal>();
        return SaveAndDestroy(go, "Crystal");
    }

    // ── BALA: projétil de Fogo Elemental #FF4500 ────────────────────────────
    static GameObject MakeBullet()
    {
        var go = new GameObject("Bullet");
        AddKinematicBody(go);

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite       = Circle();
        sr.color        = new Color(1f, 0.271f, 0f);   // Fogo Elemental
        sr.sortingOrder = 2;
        go.transform.localScale = new Vector3(0.25f, 0.25f, 1f);

        AddChildSprite(go, "Glow", Circle(), new Color(1f, 0.45f, 0.1f, 0.5f),
            Vector3.zero, new Vector3(2.4f, 2.4f, 1f), 1);

        var col = go.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius    = 0.5f;

        go.AddComponent<Bullet>();
        return SaveAndDestroy(go, "Bullet");
    }

    // ── Helpers ─────────────────────────────────────────────────────────────

    static void AddKinematicBody(GameObject go)
    {
        var rb = go.AddComponent<Rigidbody2D>();
        rb.bodyType     = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
    }

    static GameObject AddChildSprite(GameObject parent, string name, Sprite sprite,
        Color color, Vector3 localPos, Vector3 localScale, int sortingOrder,
        float rotationZ = 0f)
    {
        var go = new GameObject(name);
        go.transform.SetParent(parent.transform, false);
        go.transform.localPosition = localPos;
        go.transform.localScale    = localScale;
        go.transform.localRotation = Quaternion.Euler(0f, 0f, rotationZ);
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite           = sprite;
        sr.color            = color;
        sr.sortingLayerName = "Default";
        sr.sortingOrder     = sortingOrder;
        return go;
    }

    static GameObject SaveAndDestroy(GameObject go, string name)
    {
        string path = $"{PrefabFolder}/{name}.prefab";
        var prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        Object.DestroyImmediate(go);
        return prefab;
    }

    // Prefabs devem referenciar sprites persistidos — built-ins do Unity
    static Sprite Circle() =>
        AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");

    static Sprite Square() =>
        AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
}
