/// <summary>
/// Estados de alto nível do jogo. A transição é controlada exclusivamente
/// pelo GameManager (única fonte de verdade), que anuncia mudanças via GameEvents.
/// </summary>
public enum GameState
{
    Menu,       // Menu principal / classificação indicativa
    Running,    // Corrida normal (spawner ativo, score acumulando)
    BossFight,  // Chefão em cena (spawner pausado, velocidade travada)
    Paused,     // Pausa — Time.timeScale = 0, lembra o estado anterior
    GameOver    // Energia zerou — aguarda restart
}
