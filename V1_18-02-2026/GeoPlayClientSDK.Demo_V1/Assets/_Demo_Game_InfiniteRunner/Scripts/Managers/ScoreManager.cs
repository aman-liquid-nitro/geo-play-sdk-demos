using TMPro;
using UnityEngine;

namespace GeoPlaySample.InfiniteRunner
{
    public class ScoreManager : MonoBehaviour, IPausable
    {
        [Header("Score Settings")]
        [SerializeField] private float baseScoreRate = 10f;
        [SerializeField] private TextMeshProUGUI scoreText;
        [SerializeField] private TextMeshProUGUI highScoreText;

        private float currentScore;
        private int highScore;
        private float scoreMultiplier = 1f;
        private bool isPaused;

        public int CurrentScore => Mathf.FloorToInt(currentScore);
        public int HighScore => highScore;

        public static event System.Action<int> OnScoreChanged;
        public static event System.Action<int> OnNewHighScore;

        void OnEnable()
        {
            GameManager.OnSpeedChanged += OnSpeedChanged;
        }

        void OnDisable()
        {
            GameManager.OnSpeedChanged -= OnSpeedChanged;
        }

        void Start()
        {
            LoadHighScore();
            UpdateScoreDisplay();
        }

        void Update()
        {
            if (isPaused) return;

            if (GameManager.Instance != null && GameManager.Instance.GetTotalSpeed() > 0)
            {
                float scoreIncrease = baseScoreRate * GameManager.Instance.GetTotalSpeed() * Time.deltaTime;
                currentScore += scoreIncrease;
                CheckHighScore();
                UpdateScoreDisplay();
            }
        }

        private void OnSpeedChanged(float newMultiplier)
        {
            scoreMultiplier = newMultiplier;
        }

        private void UpdateScoreDisplay()
        {
            if (scoreText != null)
            {
                scoreText.text = $"Score: {Mathf.FloorToInt(currentScore)}";
            }

            if (highScoreText != null)
            {
                highScoreText.text = $"Best: {highScore}";
            }
        }

        private void CheckHighScore()
        {
            int currentIntScore = Mathf.FloorToInt(currentScore);
            if (currentIntScore > highScore)
            {
                highScore = currentIntScore;
                SaveHighScore();
                OnNewHighScore?.Invoke(highScore);
            }
        }

        public void AddBonusScore(int bonus)
        {
            currentScore += bonus * scoreMultiplier;
            CheckHighScore();
            UpdateScoreDisplay();
            OnScoreChanged?.Invoke(CurrentScore);
        }

        public void ResetScore()
        {
            currentScore = 0;
            scoreMultiplier = 1f;
            UpdateScoreDisplay();
            OnScoreChanged?.Invoke(0);
        }

        private void LoadHighScore()
        {
            highScore = PlayerPrefs.GetInt("HighScore", 0);
        }

        private void SaveHighScore()
        {
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }

        public void TogglePause(bool isPaused)
        {
            this.isPaused = isPaused;
        }
    }
}