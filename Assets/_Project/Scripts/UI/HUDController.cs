using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HUDController : MonoBehaviour
{
    [Header("Energia")]
    [SerializeField] Image    energyFill;
    [SerializeField] TMP_Text energyLabel;
    [SerializeField] Color    colorNormal = new Color(0f, 1f, 1f);
    [SerializeField] Color    colorLow    = new Color(1f, 0.27f, 0.27f);

    [Header("Pontuação")]
    [SerializeField] TMP_Text scoreText;
    [SerializeField] TMP_Text speedText;

    [Header("Poder: Fogo")]
    [SerializeField] TMP_Text fireLabel;
    [SerializeField] Image    fireCooldownFill;

    [Header("Poder: Gelo")]
    [SerializeField] TMP_Text iceLabel;
    [SerializeField] Image    iceDurationFill;

    [Header("Game Over")]
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] TMP_Text   gameOverScore;
    [SerializeField] TMP_Text   gameOverInstructions;

    bool gameOverShown;

    void Update()
    {
        if (!gameOverShown && GameManager.Instance.State == GameState.GameOver)
        {
            ShowGameOver();
            gameOverShown = true;
        }

        if (GameManager.Instance.IsGameplayActive)
            RefreshHUD();
    }

    void RefreshHUD()
    {
        var player = PlayerController.Instance;
        if (player == null) return;

        float pct = player.Energy / 100f;

        if (energyFill  != null) energyFill.fillAmount = pct;
        if (energyLabel != null) energyLabel.text = $"ENERGIA {Mathf.FloorToInt(player.Energy)}";
        if (energyFill  != null) energyFill.color = player.Energy < 25f ? colorLow : colorNormal;

        if (scoreText != null) scoreText.text = $"{Mathf.FloorToInt(GameManager.Instance.Score)} m";
        if (speedText != null) speedText.text = $"{GameManager.Instance.Speed:F1}x";

        RefreshFireHUD(player);
        RefreshIceHUD(player);
    }

    void RefreshFireHUD(PlayerController player)
    {
        var fire = player.GetComponent<PowerFire>();
        if (fire == null) return;

        if (fireCooldownFill != null) fireCooldownFill.fillAmount = fire.CooldownRatio;
        if (fireLabel != null)
            fireLabel.text = fire.IsReady ? "A  FOGO ►" : "A  FOGO (cd)";
    }

    void RefreshIceHUD(PlayerController player)
    {
        var ice = player.GetComponent<PowerIce>();
        if (ice == null) return;

        float fillPct = ice.IsActive ? ice.TimeLeft / 4f : (ice.IsReady ? 1f : 0f);
        if (iceDurationFill != null) iceDurationFill.fillAmount = fillPct;
        if (iceLabel != null)
        {
            if (ice.IsActive) iceLabel.text = $"D  GELO {ice.TimeLeft:F1}s";
            else if (ice.IsReady) iceLabel.text = "D  GELO [OK]";
            else iceLabel.text = "D  GELO (cd)";
        }
    }

    void ShowGameOver()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
        if (gameOverScore != null)
            gameOverScore.text = $"{Mathf.FloorToInt(GameManager.Instance.Score)} metros";
        if (gameOverInstructions != null)
            gameOverInstructions.text = "Pressione R para reiniciar";
    }
}
