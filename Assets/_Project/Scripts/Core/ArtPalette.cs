using UnityEngine;

/// <summary>
/// Paleta oficial da identidade visual (Etapa 5 — Direção de Arte).
/// Contraste central: opressão mecânica (neutros escuros dessaturados)
/// vs. energia mágica livre (emissão neon vibrante).
/// </summary>
public static class ArtPalette
{
    /// <summary>#0B0C10 — Vazio Cósmico: preto profundo azulado (60% dos fundos).</summary>
    public static readonly Color VoidBlack = new Color(0.043f, 0.047f, 0.063f);

    /// <summary>#1F2833 — Estruturas IA: cinza industrial azulado (cenário, drones).</summary>
    public static readonly Color Structure = new Color(0.122f, 0.157f, 0.20f);

    /// <summary>Variante mais clara das estruturas, para leitura sobre o fundo.</summary>
    public static readonly Color StructureLit = new Color(0.24f, 0.30f, 0.38f);

    /// <summary>#66FCF1 — Ciano Base: energia primordial do Fragmento e HUD.</summary>
    public static readonly Color Cyan = new Color(0.40f, 0.988f, 0.945f);

    /// <summary>#FF4500 — Fogo Elemental: poder de fogo e mutação destrutiva.</summary>
    public static readonly Color Fire = new Color(1f, 0.271f, 0f);

    /// <summary>#FF003C — Alerta/Erro IA: lasers de drones, perigos, pontos fracos.</summary>
    public static readonly Color AlertRed = new Color(1f, 0f, 0.235f);
}
