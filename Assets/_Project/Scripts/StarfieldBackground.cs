using UnityEngine;

// Alternativa simples ao Particle System — replicates the original 100-star scrolling background.
// Coloque num GameObject vazio na cena e atribua um material emissivo branco em starMaterial.
public class StarfieldBackground : MonoBehaviour
{
    [SerializeField] int   starCount    = 100;
    [SerializeField] float minSpeed     = 0.2f;
    [SerializeField] float maxSpeed     = 0.8f;
    [SerializeField] float minAlpha     = 0.4f;
    [SerializeField] float maxAlpha     = 0.9f;
    [SerializeField] float minRadius    = 0.04f;
    [SerializeField] float maxRadius    = 0.12f;
    [SerializeField] Material starMaterial;

    struct Star { public Vector3 pos; public float speed, alpha, radius; }

    Star[]           stars;
    MaterialPropertyBlock mpb;
    Mesh             mesh;
    MeshFilter       mf;
    MeshRenderer     mr;

    const float BoundsX = 9f;
    const float BoundsY = 4f;

    void Start()
    {
        stars = new Star[starCount];
        for (int i = 0; i < starCount; i++) ResetStar(ref stars[i], randomX: true);

        mesh = new Mesh { name = "Starfield" };
        mf   = gameObject.AddComponent<MeshFilter>();
        mr   = gameObject.AddComponent<MeshRenderer>();
        mf.mesh    = mesh;
        mr.material = starMaterial;
        mr.sortingLayerName = "Background";
        mpb = new MaterialPropertyBlock();
    }

    void Update()
    {
        float dt      = Time.deltaTime;
        bool  iceOn   = PowerIce.Instance != null && PowerIce.Instance.IsActive;
        float iceSlow = iceOn ? 0.3f : 1f;

        var verts  = new Vector3[starCount * 4];
        var uvs    = new Vector2[starCount * 4];
        var colors = new Color[starCount * 4];
        var tris   = new int[starCount * 6];

        for (int i = 0; i < starCount; i++)
        {
            ref Star s = ref stars[i];
            s.pos.x -= s.speed * iceSlow * dt;
            if (s.pos.x < -BoundsX) ResetStar(ref s, randomX: false);

            float r = s.radius;
            var c   = new Color(1f, 1f, 1f, s.alpha);
            int vi  = i * 4;
            verts[vi + 0] = s.pos + new Vector3(-r, -r, 0);
            verts[vi + 1] = s.pos + new Vector3( r, -r, 0);
            verts[vi + 2] = s.pos + new Vector3( r,  r, 0);
            verts[vi + 3] = s.pos + new Vector3(-r,  r, 0);
            uvs[vi + 0]   = new Vector2(0, 0);
            uvs[vi + 1]   = new Vector2(1, 0);
            uvs[vi + 2]   = new Vector2(1, 1);
            uvs[vi + 3]   = new Vector2(0, 1);
            for (int k = 0; k < 4; k++) colors[vi + k] = c;
            int ti = i * 6;
            tris[ti+0] = vi; tris[ti+1] = vi+2; tris[ti+2] = vi+1;
            tris[ti+3] = vi; tris[ti+4] = vi+3; tris[ti+5] = vi+2;
        }

        mesh.vertices  = verts;
        mesh.uv        = uvs;
        mesh.colors    = colors;
        mesh.triangles = tris;
    }

    void ResetStar(ref Star s, bool randomX)
    {
        s.pos    = new Vector3(randomX ? Random.Range(-BoundsX, BoundsX) : BoundsX,
                               Random.Range(-BoundsY, BoundsY), 1f);
        s.speed  = Random.Range(minSpeed, maxSpeed);
        s.alpha  = Random.Range(minAlpha, maxAlpha);
        s.radius = Random.Range(minRadius, maxRadius);
    }
}
