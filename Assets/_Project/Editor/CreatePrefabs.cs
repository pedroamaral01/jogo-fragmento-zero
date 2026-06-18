using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using System.IO;

public static class CreatePrefabs
{
    const string PrefabFolder = "Assets/_Project/Prefabs";

    [MenuItem("FragmentoZero/Create Obstacle Prefabs")]
    static void Create()
    {
        if (!AssetDatabase.IsValidFolder(PrefabFolder))
            AssetDatabase.CreateFolder("Assets/_Project", "Prefabs");

        var meteor  = MakeMeteor();
        var drone   = MakeDrone();
        var crystal = MakeCrystal();
        var bullet  = MakeBullet();

        // Wire to ObstacleSpawner in scene
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

        // Wire bulletPrefab to PowerFire on Player
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

        Debug.Log("FragmentoZero: prefabs criados e atribuídos ao ObstacleSpawner!");
    }

    // ── METEOR ──────────────────────────────────────────────────────────────
    static GameObject MakeMeteor()
    {
        var go = new GameObject("Meteor");
        go.tag = "Obstacle";

        var rb = go.AddComponent<Rigidbody2D>();
        rb.bodyType     = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite           = GetCircleSprite();
        sr.color            = new Color(1f, 0.45f, 0.1f);   // orange
        sr.sortingLayerName = "Default";
        go.transform.localScale = new Vector3(0.7f, 0.7f, 1f);

        var col = go.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius    = 0.5f;

        go.AddComponent<Meteor>();

        var prefab = Save(go, "Meteor");
        Object.DestroyImmediate(go);
        return prefab;
    }

    // ── DRONE ───────────────────────────────────────────────────────────────
    static GameObject MakeDrone()
    {
        var go = new GameObject("Drone");
        go.tag = "Obstacle";

        var rb = go.AddComponent<Rigidbody2D>();
        rb.bodyType     = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;

        // Body (parent SpriteRenderer)
        var body = go.AddComponent<SpriteRenderer>();
        body.sprite           = GetSquareSprite();
        body.color            = new Color(1f, 0f, 1f);      // magenta
        body.sortingLayerName = "Default";
        go.transform.localScale = new Vector3(0.75f, 0.5f, 1f);

        // Collider on parent
        var col = go.AddComponent<BoxCollider2D>();
        col.isTrigger = true;
        col.size      = new Vector2(1f, 1f);

        // Eye child
        var eyeGO = new GameObject("Eye");
        eyeGO.transform.SetParent(go.transform, false);
        eyeGO.transform.localPosition = new Vector3(0.15f, 0.1f, 0f);
        eyeGO.transform.localScale    = new Vector3(0.45f, 0.45f, 1f);
        var eye = eyeGO.AddComponent<SpriteRenderer>();
        eye.sprite           = GetCircleSprite();
        eye.color            = new Color(1f, 0.27f, 0.27f); // red
        eye.sortingLayerName = "Default";
        eye.sortingOrder     = 1;

        // Drone script wired via SerializedObject after save
        var droneComp = go.AddComponent<Drone>();

        var prefab = Save(go, "Drone");

        // Wire bodyRenderer and eyeRenderer on the saved prefab
        var dronePrefabComp = prefab.GetComponent<Drone>();
        var so = new SerializedObject(dronePrefabComp);
        so.FindProperty("bodyRenderer").objectReferenceValue = prefab.GetComponent<SpriteRenderer>();
        so.FindProperty("eyeRenderer").objectReferenceValue  = prefab.transform.Find("Eye").GetComponent<SpriteRenderer>();
        so.ApplyModifiedProperties();
        EditorUtility.SetDirty(prefab);

        Object.DestroyImmediate(go);
        return prefab;
    }

    // ── CRYSTAL ─────────────────────────────────────────────────────────────
    static GameObject MakeCrystal()
    {
        var go = new GameObject("Crystal");
        go.tag = "Crystal";

        var rb = go.AddComponent<Rigidbody2D>();
        rb.bodyType     = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;

        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite           = GetSquareSprite();
        sr.color            = new Color(0f, 1f, 1f);         // cyan
        sr.sortingLayerName = "Default";
        // Diamond shape: rotate 45° and scale
        go.transform.localRotation = Quaternion.Euler(0f, 0f, 45f);
        go.transform.localScale    = new Vector3(0.45f, 0.45f, 1f);

        // Crystal.cs requires CircleCollider2D
        var col = go.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius    = 0.5f;

        go.AddComponent<Crystal>();

        var prefab = Save(go, "Crystal");
        Object.DestroyImmediate(go);
        return prefab;
    }

    // ── BULLET ──────────────────────────────────────────────────────────────
    static GameObject MakeBullet()
    {
        var go = new GameObject("Bullet");
        var rb = go.AddComponent<Rigidbody2D>();
        rb.bodyType     = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite           = GetCircleSprite();
        sr.color            = new Color(1f, 0.85f, 0.1f); // yellow-orange
        sr.sortingLayerName = "Default";
        sr.sortingOrder     = 2;
        go.transform.localScale = new Vector3(0.25f, 0.25f, 1f);
        var col = go.AddComponent<CircleCollider2D>();
        col.isTrigger = true;
        col.radius    = 0.5f;
        go.AddComponent<Bullet>();
        var prefab = Save(go, "Bullet");
        Object.DestroyImmediate(go);
        return prefab;
    }

    // ── Helpers ─────────────────────────────────────────────────────────────
    static GameObject Save(GameObject go, string name)
    {
        string path = $"{PrefabFolder}/{name}.prefab";
        var prefab = PrefabUtility.SaveAsPrefabAsset(go, path);
        return prefab;
    }

    static Sprite GetCircleSprite() =>
        AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");

    static Sprite GetSquareSprite() =>
        AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/UISprite.psd");
}
