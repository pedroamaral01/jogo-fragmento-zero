using System;
using UnityEngine;

/// <summary>
/// Sintetizador de SFX em runtime: gera PCM e devolve AudioClips — o jogo
/// não depende de nenhum asset de áudio. Receitas simples (sweep, ruído,
/// arpejo, wobble) cobrem toda a paleta sonora da demo.
/// </summary>
public static class SfxSynth
{
    const int SampleRate = 44100;

    static readonly System.Random rng = new System.Random(12345);

    // ── Receitas ────────────────────────────────────────────────────────────

    /// <summary>Varredura de frequência com decaimento (tiros, stings).</summary>
    public static AudioClip Sweep(string name, float duration, float fromHz, float toHz,
                                  float volume = 0.5f, bool square = false)
    {
        int n = (int)(duration * SampleRate);
        var data = new float[n];
        double phase = 0;

        for (int i = 0; i < n; i++)
        {
            float t01 = (float)i / n;
            double freq = fromHz + (toHz - fromHz) * t01;
            phase += 2.0 * Math.PI * freq / SampleRate;

            float s = (float)Math.Sin(phase);
            if (square) s = Math.Sign(s) * 0.6f;

            data[i] = s * DecayEnv(t01) * volume;
        }
        return Make(name, data);
    }

    /// <summary>Explosões e impactos: ruído com filtro passa-baixa simples.</summary>
    public static AudioClip NoiseBurst(string name, float duration, float volume, float lowpass01)
    {
        int n = (int)(duration * SampleRate);
        var data = new float[n];
        float filtered = 0f;

        for (int i = 0; i < n; i++)
        {
            float t01   = (float)i / n;
            float white = (float)(rng.NextDouble() * 2.0 - 1.0);
            filtered += (white - filtered) * lowpass01;   // one-pole LP
            data[i] = filtered * DecayEnv(t01) * volume;
        }
        return Make(name, data);
    }

    /// <summary>Sequência de notas (coleta, desbloqueio, evolução).</summary>
    public static AudioClip Arpeggio(string name, float[] freqs, float noteDuration, float volume)
    {
        int perNote = (int)(noteDuration * SampleRate);
        int n = perNote * freqs.Length;
        var data = new float[n];
        double phase = 0;

        for (int i = 0; i < n; i++)
        {
            int   note = Mathf.Min(i / perNote, freqs.Length - 1);
            float t01  = (float)(i % perNote) / perNote;
            phase += 2.0 * Math.PI * freqs[note] / SampleRate;
            data[i] = (float)Math.Sin(phase) * DecayEnv(t01) * volume;
        }
        return Make(name, data);
    }

    /// <summary>Rugido grave com vibrato (chefões).</summary>
    public static AudioClip Wobble(string name, float duration, float baseHz,
                                   float wobbleHz, float volume)
    {
        int n = (int)(duration * SampleRate);
        var data = new float[n];
        double phase = 0;

        for (int i = 0; i < n; i++)
        {
            float t   = (float)i / SampleRate;
            float t01 = (float)i / n;
            double freq = baseHz * (1.0 + 0.25 * Math.Sin(2.0 * Math.PI * wobbleHz * t));
            phase += 2.0 * Math.PI * freq / SampleRate;

            // saw suavizado: fundamental + 2ª harmônica
            float s = (float)(Math.Sin(phase) + 0.5 * Math.Sin(phase * 2.0));
            data[i] = s * 0.66f * DecayEnv(t01) * volume;
        }
        return Make(name, data);
    }

    /// <summary>
    /// Drone ambiente em loop (12s): tríade grave com LFOs que completam
    /// ciclos inteiros no loop — emenda imperceptível.
    /// </summary>
    public static AudioClip AmbientLoop(float volume = 0.35f)
    {
        const float loopSecs = 12f;
        int n = (int)(loopSecs * SampleRate);
        var data = new float[n];

        // frequências inteiras → fase fecha exatamente no fim do loop
        float[] freqs = { 55f, 110f, 165f };
        float[] lfoHz = { 2f / loopSecs, 3f / loopSecs, 5f / loopSecs };

        for (int i = 0; i < n; i++)
        {
            float t = (float)i / SampleRate;
            float s = 0f;
            for (int v = 0; v < freqs.Length; v++)
            {
                float lfo = 0.55f + 0.45f * Mathf.Sin(2f * Mathf.PI * lfoHz[v] * t);
                s += Mathf.Sin(2f * Mathf.PI * freqs[v] * t) * lfo / freqs.Length;
            }
            data[i] = s * volume;
        }
        return Make("music_ambient", data);
    }

    // ── Infra ───────────────────────────────────────────────────────────────

    static float DecayEnv(float t01) => Mathf.Pow(1f - t01, 1.5f);

    static AudioClip Make(string name, float[] data)
    {
        var clip = AudioClip.Create(name, data.Length, 1, SampleRate, false);
        clip.SetData(data, 0);
        return clip;
    }
}
