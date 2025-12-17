using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [Header("Audio")]
    public AudioSource backgroundMusic;

    [Header("Panels")]
    public GameObject startPanel;
    public GameObject gamePanel;
    public GameObject winPanel;
    public GameObject losePanel;

    [Header("UI Text (TMP)")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI cubesLeftText;
    public TextMeshProUGUI winTimeText;
    public TextMeshProUGUI bestTimeText;
    public TextMeshProUGUI loseReasonText;

    [Header("Game Settings")]
    public float gameTimeLimit = 120f; // 2 minutes
    public bool IsGameRunning => gameRunning;

    float currentTime;
    int totalCollectables;
    int collectedCount;
    bool gameRunning;

    const string BEST_TIME_KEY = "BEST_TIME";

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        Time.timeScale = 1f;

        totalCollectables = GameObject.FindGameObjectsWithTag("Collectable").Length;
        collectedCount = 0;

        currentTime = gameTimeLimit;

        ShowStartPanel();

    }

    void Update()
    {
        if (!gameRunning) return;

        currentTime -= Time.deltaTime;
        UpdateTimerUI();

        if (currentTime <= 0f)
        {
            LoseGame("Time’s up!");
        }

    }

    // -------------------------
    // GAME FLOW
    // -------------------------

    public void StartGame()
    {
        startPanel.SetActive(false);
        winPanel.SetActive(false);
        losePanel.SetActive(false);

        gamePanel.SetActive(true);

        gameRunning = true;
        currentTime = gameTimeLimit;
        UpdateCollectableUI();
        UpdateTimerUI();
        if (backgroundMusic && !backgroundMusic.isPlaying)
        {
            backgroundMusic.Play();
        }
    }

    public void OnCollectablePicked()
    {
        collectedCount++;
        UpdateCollectableUI();

        if (collectedCount >= totalCollectables)
        {
            WinGame();
        }
    }

    public void OnPlayerFellIntoVoid()
    {
        LoseGame("You fell into the void!");
    }

    void WinGame()
    {
        if (backgroundMusic && backgroundMusic.isPlaying)
        {
            backgroundMusic.Stop();
        }

        gameRunning = false;
        gamePanel.SetActive(false);
        winPanel.SetActive(true);

        float timeUsed = gameTimeLimit - currentTime;
        winTimeText.text = $"Time: {timeUsed:F2}s";

        float bestTime = PlayerPrefs.GetFloat(BEST_TIME_KEY, float.MaxValue);
        if (timeUsed < bestTime)
        {
            bestTime = timeUsed;
            PlayerPrefs.SetFloat(BEST_TIME_KEY, bestTime);
        }

        bestTimeText.text =
            bestTime == float.MaxValue ? "Best: --" : $"Best: {bestTime:F2}s";
    }

    void LoseGame(string reason)
    {
        if (backgroundMusic && backgroundMusic.isPlaying)
        {
            backgroundMusic.Stop();
        }

        if (!gameRunning) return;

        gameRunning = false;
        gamePanel.SetActive(false);
        losePanel.SetActive(true);

        loseReasonText.text = reason;
    }

    // -------------------------
    // UI HELPERS
    // -------------------------

    void UpdateTimerUI()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60f);
        int seconds = Mathf.FloorToInt(currentTime % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
    }

    void UpdateCollectableUI()
    {
        int left = totalCollectables - collectedCount;
        cubesLeftText.text = $"Cubes Left: {left}";
    }

    // -------------------------
    // BUTTON CALLBACKS
    // -------------------------

    public void Retry()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    void ShowStartPanel()
    {
        startPanel.SetActive(true);
        gamePanel.SetActive(false);
        winPanel.SetActive(false);
        losePanel.SetActive(false);
        gameRunning = false;
    }
}
