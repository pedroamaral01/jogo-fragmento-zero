using UnityEngine;

/// <summary>
/// Sprites gerados proceduralmente em runtime (1×1 unidade) — permitem
/// construir visuais (chefes, projéteis) sem nenhum asset de arte.
/// </summary>
public static class RuntimeSprites
{
    static Sprite circle;
    static Sprite square;
    static Sprite glow;
    static Sprite ring;

    public static Sprite Circle
    {
        get
        {
            if (circle == null) circle = MakeCircle(64);
            return circle;
        }
    }

    public static Sprite Square
    {
        get
        {
            if (square == null) square = MakeSquare(8);
            return square;
        }
    }

    /// <summary>Halo radial suave (alpha cai com o quadrado da distância).</summary>
    public static Sprite Glow
    {
        get
        {
            if (glow == null) glow = MakeGlow(96);
            return glow;
        }
    }

    /// <summary>Anel fino (aura do Fragmento).</summary>
    public static Sprite Ring
    {
        get
        {
            if (ring == null) ring = MakeRing(96);
            return ring;
        }
    }

    static Sprite MakeCircle(int size)
    {
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        float center = (size - 1) * 0.5f;
        float radius = size * 0.5f - 1f;

        var pixels = new Color[size * size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float dist  = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                float alpha = Mathf.Clamp01((radius - dist) / 1.5f);   // borda suavizada
                pixels[y * size + x] = new Color(1f, 1f, 1f, alpha);
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size),
                             new Vector2(0.5f, 0.5f), size);
    }

    static Sprite MakeGlow(int size)
    {
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        float center = (size - 1) * 0.5f;
        float radius = size * 0.5f;

        var pixels = new Color[size * size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float d     = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                float t     = Mathf.Clamp01(1f - d / radius);
                float alpha = t * t;   // decaimento suave
                pixels[y * size + x] = new Color(1f, 1f, 1f, alpha);
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size),
                             new Vector2(0.5f, 0.5f), size);
    }

    static Sprite MakeRing(int size)
    {
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        float center = (size - 1) * 0.5f;
        float outer  = size * 0.5f - 1f;
        float inner  = outer * 0.82f;

        var pixels = new Color[size * size];
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                float d = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                float alpha = Mathf.Clamp01((outer - d) / 1.5f) *
                              Mathf.Clamp01((d - inner) / 1.5f);
                pixels[y * size + x] = new Color(1f, 1f, 1f, alpha);
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size),
                             new Vector2(0.5f, 0.5f), size);
    }

    static Sprite MakeSquare(int size)
    {
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        var pixels = new Color[size * size];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = Color.white;
        tex.SetPixels(pixels);
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, size, size),
                             new Vector2(0.5f, 0.5f), size);
    }
}
