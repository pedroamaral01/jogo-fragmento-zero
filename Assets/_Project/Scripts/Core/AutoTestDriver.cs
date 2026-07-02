using System;
using System.IO;
using UnityEngine;

/// <summary>
/// Piloto automático de validação visual. Só ativa com o argumento de linha
/// de comando -autotest: pula menu/tutorial, joga sozinho (tiros, energia,
/// evolução acelerada — o que invoca chefão) e salva screenshots do
/// framebuffer em momentos-chave. Inerte em execução normal.
/// </summary>
public static class AutoTestBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void Init()
    {
        if (Array.IndexOf(Environment.GetCommandLineArgs(), "-autotest") < 0) return;

        var go = new GameObject("AutoTestDriver");
        UnityEngine.Object.DontDestroyOnLoad(go);
        go.AddComponent<AutoTestDriver>();
    }
}

public class AutoTestDriver : MonoBehaviour
{
    string shotDir;
    float  t;
    int    nextShot;
    float  fireTimer;
    float  energyTimer;
    int    crystalsRaised;

    // (momento, nome) — cobre rating, corrida, evolução, boss e pós-boss
    static readonly (float when, string name)[] Shots =
    {
        (0.7f,  "shot0_rating"),
        (5f,    "shot1_corrida"),
        (12f,   "shot2_evolucao"),
        (17f,   "shot3_boss"),
        (24f,   "shot4_late"),
    };

    const float QuitAt = 28f;

    void Awake()
    {
        shotDir = Path.Combine(Application.persistentDataPath, "autotest");
        var args = Environment.GetCommandLineArgs();
        int i = Array.IndexOf(args, "-shotdir");
        if (i >= 0 && i + 1 < args.Length) shotDir = args[i + 1];
        Directory.CreateDirectory(shotDir);

        PlayerPrefs.SetInt("TutorialDone", 1);   // tutorial espera teclas reais
    }

    void Update()
    {
        t += Time.unscaledDeltaTime;
        var gm = GameManager.Instance;
        if (gm == null) return;

        if (t > 1.0f && gm.State == GameState.Menu) gm.StartRun();

        if (gm.IsGameplayActive) Autoplay();

        if (nextShot < Shots.Length && t >= Shots[nextShot].when)
        {
            ScreenCapture.CaptureScreenshot(
                Path.Combine(shotDir, Shots[nextShot].name + ".png"));
            nextShot++;
        }

        if (t > QuitAt) Application.Quit();
    }

    void Autoplay()
    {
        var player = PlayerController.Instance;
        if (player == null) return;

        // Atira continuamente (mostra balas, carga descendo, kills)
        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0f)
        {
            fireTimer = 0.45f;
            player.GetComponent<PowerFire>()?.TryActivate();
        }

        // Não deixa a energia zerar durante a sessão de captura
        energyTimer -= Time.deltaTime;
        if (energyTimer <= 0f)
        {
            energyTimer = 2f;
            player.ModifyEnergy(30f);
        }

        // Evolução acelerada via coletas fantasma → dispara o mini-chefão
        if (t > 6f && crystalsRaised < 12 && Time.frameCount % 30 == 0)
        {
            crystalsRaised++;
            GameEvents.RaiseCrystalCollected(player.transform.position, 18f, 50f);
        }

        // Diagnóstico: estado do chefe a cada ~2s + teste de dano direto
        if (Time.frameCount % 120 == 0)
        {
            var boss = FindFirstObjectByType<BossBase>();
            if (boss != null)
                Debug.Log($"[autotest] boss={boss.DisplayName} hp={boss.Hp}/{boss.MaxHp} " +
                          $"phase={boss.Phase} x={boss.transform.position.x:F2} entering={boss.IsEntering}");
        }
        if (!damageProbeDone && t > 15f)
        {
            damageProbeDone = true;
            var boss = FindFirstObjectByType<BossBase>();
            if (boss != null)
            {
                int before = boss.Hp;
                boss.TakeDamage(3);
                Debug.Log($"[autotest] damage probe: hp {before} -> {boss.Hp}");
            }
        }
    }

    bool damageProbeDone;
}
