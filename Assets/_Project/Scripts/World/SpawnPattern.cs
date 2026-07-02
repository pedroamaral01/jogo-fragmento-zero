using UnityEngine;

/// <summary>
/// Padrão de spawn declarativo. Cada coluna é uma string com um char por
/// faixa (índice 0 = topo): 'M' meteoro, 'D' drone, 'C' cristal, '.' vazio.
/// Ex.: "MM.M" = parede de meteoros com brecha na faixa 2.
///
/// A biblioteca em código serve de fonte única; minTier controla quando o
/// padrão entra na rotação e weight a frequência relativa.
/// </summary>
[System.Serializable]
public class SpawnPattern
{
    public string   name;
    public int      minTier;
    public float    weight      = 1f;
    public bool     allowMirror = true;   // espelha as faixas verticalmente
    public float    columnGap   = 0.55f;  // segundos entre colunas
    public string[] columns;

    public static readonly SpawnPattern[] Library =
    {
        // ── Tier 0: aquecimento ─────────────────────────────────────────────
        new SpawnPattern { name = "meteoro solo",        minTier = 0, weight = 3.0f,
                           columns = new[] { "M..." } },
        new SpawnPattern { name = "par de meteoros",     minTier = 0, weight = 2.0f,
                           columns = new[] { "M.M." } },
        new SpawnPattern { name = "cristal solo",        minTier = 0, weight = 2.5f,
                           columns = new[] { ".C.." } },
        new SpawnPattern { name = "trilha de cristais",  minTier = 0, weight = 1.5f, columnGap = 0.4f,
                           columns = new[] { "C...", "C...", "C..." } },
        new SpawnPattern { name = "diagonal de cristais", minTier = 0, weight = 1.2f, columnGap = 0.35f,
                           columns = new[] { "C...", ".C..", "..C.", "...C" } },
        new SpawnPattern { name = "drone patrulha",      minTier = 0, weight = 1.5f,
                           columns = new[] { "..D." } },

        // ── Tier 1: pressão ─────────────────────────────────────────────────
        new SpawnPattern { name = "parede com brecha",   minTier = 1, weight = 2.0f,
                           columns = new[] { "MM.M" } },
        new SpawnPattern { name = "corredor",            minTier = 1, weight = 1.5f, columnGap = 0.5f,
                           columns = new[] { "M..M", "M..M", "M..M" } },
        new SpawnPattern { name = "escolta",             minTier = 1, weight = 1.5f,
                           columns = new[] { "D..C" } },

        // ── Tier 2: hordas ──────────────────────────────────────────────────
        new SpawnPattern { name = "zigue-zague",         minTier = 2, weight = 1.5f, columnGap = 0.45f,
                           columns = new[] { "M...", "..M.", "M...", "..M." } },
        new SpawnPattern { name = "esquadrão de drones", minTier = 2, weight = 1.2f,
                           columns = new[] { "D.D." } },
        new SpawnPattern { name = "parede recompensa",   minTier = 2, weight = 1.0f,
                           columns = new[] { "MMCM" } },

        // ── Tier 3+: caos coreografado ──────────────────────────────────────
        new SpawnPattern { name = "paredes alternadas",  minTier = 3, weight = 1.2f, columnGap = 0.6f,
                           columns = new[] { "MM.M", "M.MM" } },
        new SpawnPattern { name = "muralha de drones",   minTier = 3, weight = 1.0f,
                           columns = new[] { "DD.D" } },
    };
}
