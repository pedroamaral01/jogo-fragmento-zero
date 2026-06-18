using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public enum GameState { Playing, Dead }
    public GameState State { get; private set; } = GameState.Playing;

    public float Score { get; private set; }
    public float Speed { get; private set; } = 4.5f;

    const float BASE_SPEED = 4.5f;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Update()
    {
        if (State == GameState.Dead)
        {
            if (Input.GetKeyDown(KeyCode.R))
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            return;
        }

        // Original: score += speed * 0.04 * dt  (dt = 1 at 60 fps)
        Score += Speed * 0.04f * Time.deltaTime * 60f;
        Speed  = BASE_SPEED + Score * 0.003f;
    }

    public void AddScore(float amount) => Score += amount;

    public void TriggerGameOver()
    {
        if (State == GameState.Dead) return;
        State = GameState.Dead;
    }
}
