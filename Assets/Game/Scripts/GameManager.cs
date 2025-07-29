using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI Panels")]
    public GameObject startPanel;
    public GameObject gamePanel;
    public GameObject gameOverPanel;

    [Header("Buttons")]
    public Button startButton;
    public Button tryAgainButton;

    [Header("References")]
    public GameScore gameTimer;
    public GameObject player;
    public ObjectSpawner FruitSpawner;

    private Vector3 playerStartPosition;
    private Quaternion playerStartRotation;

    public bool GameRunning = false;

    void Awake()
    {
        if (Instance == null) Instance = this;

        playerStartPosition = player.transform.position;
        playerStartRotation = player.transform.rotation;
        
        startPanel.SetActive(true);
        gamePanel.SetActive(false);
        gameOverPanel.SetActive(false);
    }

    void Start()
    {
        startButton.onClick.AddListener(StartGame);
        tryAgainButton.onClick.AddListener(RestartGame);
    }

    public void StartGame()
    {
        startPanel.SetActive(false);
        gamePanel.SetActive(true);
        gameOverPanel.SetActive(false);

        player.gameObject.SetActive(true);
        gameTimer.StartScoring();
        GameRunning = true;
    }

    public void GameOver()
    {
        gameTimer.StopScoring();

        gameOverPanel.SetActive(true);
        gamePanel.SetActive(false);

        GameRunning = false;
    }

    public void RestartGame()
    {
        player.gameObject.SetActive(false);
        player.transform.position = playerStartPosition;
        player.transform.rotation = playerStartRotation;

        // Optionally reset player state here (health, velocity, etc.)
        player.GetComponent<Ninja>().ResetPlayer();

        FruitSpawner.OnRestart();
        gameTimer.StartScoring();
        gamePanel.SetActive(true);
        gameOverPanel.SetActive(false);
        GameRunning = true;
    }
}