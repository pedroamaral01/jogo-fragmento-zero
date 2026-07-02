using UnityEngine;

/// <summary>
/// Áudio da demo, 100% orientado a eventos: nenhum sistema de gameplay
/// conhece esta classe (exceto a UI, para o clique de botão). Clips são
/// sintetizados uma vez no Awake (SfxSynth) — zero assets.
/// </summary>
public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    AudioSource music;
    AudioSource sfx;

    AudioClip shoot, freeze, zap, gravity;
    AudioClip hit, explosion, pickup, pickupFire, hurt;
    AudioClip unlock, evolve, tierUp;
    AudioClip bossSpawn, bossPhase, bossDie;
    AudioClip gameOver, click;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;

        music = gameObject.AddComponent<AudioSource>();
        music.loop        = true;
        music.volume      = 0.28f;
        music.playOnAwake = false;

        sfx = gameObject.AddComponent<AudioSource>();
        sfx.playOnAwake = false;
        sfx.volume      = 0.8f;

        // Poderes
        shoot   = SfxSynth.Sweep("shoot", 0.12f, 880f, 220f, 0.30f, square: true);
        freeze  = SfxSynth.Sweep("freeze", 0.5f, 1300f, 250f, 0.28f);
        zap     = SfxSynth.NoiseBurst("zap", 0.22f, 0.40f, 0.45f);
        gravity = SfxSynth.Sweep("gravity", 0.4f, 200f, 700f, 0.26f);

        // Combate / coleta
        hit        = SfxSynth.NoiseBurst("hit", 0.12f, 0.32f, 0.30f);
        explosion  = SfxSynth.NoiseBurst("explosion", 0.45f, 0.48f, 0.12f);
        pickup     = SfxSynth.Arpeggio("pickup", new[] { 660f, 990f }, 0.07f, 0.28f);
        pickupFire = SfxSynth.Arpeggio("pickupFire", new[] { 440f, 587f }, 0.07f, 0.28f);
        hurt       = SfxSynth.Sweep("hurt", 0.3f, 300f, 70f, 0.45f, square: true);

        // Progressão
        unlock = SfxSynth.Arpeggio("unlock", new[] { 523f, 659f, 784f }, 0.09f, 0.32f);
        evolve = SfxSynth.Arpeggio("evolve", new[] { 440f, 554f, 659f, 880f }, 0.11f, 0.36f);
        tierUp = SfxSynth.Sweep("tierUp", 0.35f, 150f, 90f, 0.36f, square: true);

        // Chefões
        bossSpawn = SfxSynth.Wobble("bossSpawn", 0.9f, 70f, 5f, 0.5f);
        bossPhase = SfxSynth.Wobble("bossPhase", 0.5f, 95f, 8f, 0.45f);
        bossDie   = SfxSynth.NoiseBurst("bossDie", 0.9f, 0.55f, 0.08f);

        // Fluxo
        gameOver = SfxSynth.Sweep("gameOver", 0.9f, 330f, 55f, 0.4f);
        click    = SfxSynth.Sweep("click", 0.05f, 1000f, 800f, 0.22f);

        music.clip = SfxSynth.AmbientLoop();
        music.Play();
    }

    void OnEnable()
    {
        GameEvents.PowerActivated   += OnPowerActivated;
        GameEvents.EnemyDamaged     += OnEnemyDamaged;
        GameEvents.EnemyKilled      += OnEnemyKilled;
        GameEvents.CrystalCollected += OnCrystalCollected;
        GameEvents.FireCrystalCollected += OnFireCrystalCollected;
        GameEvents.PlayerHit        += OnPlayerHit;
        GameEvents.PowerUnlocked    += OnPowerUnlocked;
        GameEvents.EvolutionChanged += OnEvolutionChanged;
        GameEvents.DifficultyChanged += OnDifficultyChanged;
        GameEvents.BossSpawned      += OnBossSpawned;
        GameEvents.BossPhaseChanged += OnBossPhaseChanged;
        GameEvents.BossDefeated     += OnBossDefeated;
        GameEvents.StateChanged     += OnStateChanged;
    }

    void OnDisable()
    {
        GameEvents.PowerActivated   -= OnPowerActivated;
        GameEvents.EnemyDamaged     -= OnEnemyDamaged;
        GameEvents.EnemyKilled      -= OnEnemyKilled;
        GameEvents.CrystalCollected -= OnCrystalCollected;
        GameEvents.FireCrystalCollected -= OnFireCrystalCollected;
        GameEvents.PlayerHit        -= OnPlayerHit;
        GameEvents.PowerUnlocked    -= OnPowerUnlocked;
        GameEvents.EvolutionChanged -= OnEvolutionChanged;
        GameEvents.DifficultyChanged -= OnDifficultyChanged;
        GameEvents.BossSpawned      -= OnBossSpawned;
        GameEvents.BossPhaseChanged -= OnBossPhaseChanged;
        GameEvents.BossDefeated     -= OnBossDefeated;
        GameEvents.StateChanged     -= OnStateChanged;
    }

    void OnPowerActivated(PowerBase power)
    {
        switch (power)
        {
            case PowerFire:      Play(shoot);   break;
            case PowerIce:       Play(freeze);  break;
            case PowerLightning: Play(zap);     break;
            case PowerGravity:   Play(gravity); break;
        }
    }

    void OnEnemyDamaged(ObstacleBase enemy)                  => Play(hit, 0.6f);
    void OnEnemyKilled(ObstacleBase enemy, Vector3 pos)      => Play(explosion);
    void OnCrystalCollected(Vector3 pos, float e, float s)   => Play(pickup);
    void OnFireCrystalCollected(Vector3 pos, float charge)   => Play(pickupFire);
    void OnPlayerHit()                                       => Play(hurt);
    void OnPowerUnlocked(PowerBase power)                    => Play(unlock);
    void OnEvolutionChanged(int level, string name)          => Play(evolve);
    void OnDifficultyChanged(int tier)                       => Play(tierUp, 0.7f);
    void OnBossSpawned(BossBase boss)                        => Play(bossSpawn);
    void OnBossPhaseChanged(BossBase boss, int phase)        => Play(bossPhase);
    void OnBossDefeated(BossBase boss)                       => Play(bossDie);

    void OnStateChanged(GameState previous, GameState next)
    {
        if (next == GameState.GameOver) Play(gameOver);
        // música continua no menu/pausa — só muda a presença
        music.volume = next == GameState.Paused ? 0.12f : 0.28f;
    }

    /// <summary>Clique de botão — chamado pela UiFactory.</summary>
    public void PlayClick() => Play(click, 0.8f);

    void Play(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        sfx.pitch = Random.Range(0.94f, 1.06f);   // variação leve evita fadiga
        sfx.PlayOneShot(clip, volume);
    }
}
