using UnityEngine;
using System;
using System.Collections.Generic;

namespace GeoPlaySample.InfiniteRunner
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        [SerializeField] private GameAttributes gameAttributes;

        [Header("Global Speed Settings")]
        [SerializeField] private float initialBaseSpeed = 5f;
        [SerializeField] private float maxBaseSpeed = 15f;
        [SerializeField] private float speedIncreaseInterval = 10f;
        [SerializeField] private float speedIncrement = 0.5f;

        [Header("Obstacle Settings")]
        [SerializeField] private float baseObstacleSpeed = 8f;

        [Header("References")]
        [SerializeField] private ScoreManager scoreManager;
        [SerializeField] private GameplayUI gameplayUI;

        public float CurrentSpeedMultiplier { get; private set; } = 1f;
        public float CurrentBaseSpeed { get; private set; }
        public float BaseObstacleSpeed => baseObstacleSpeed;
        public float MaxBaseSpeed => maxBaseSpeed;

        // Events for speed changes
        public static event Action<float> OnSpeedChanged;
        public static event Action<float> OnBaseSpeedChanged;

        private float speedIncreaseTimer;
        private bool isGameRunning = false;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            CurrentBaseSpeed = initialBaseSpeed;
            StartGame();
        }

        private void OnEnable()
        {
            Player.OnObstacleCollision += OnPlayerCollide;
        }

        private void OnDisable()
        {
            Player.OnObstacleCollision += OnPlayerCollide;
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePauseGame();
            }

            if (!isGameRunning) return;

            speedIncreaseTimer += Time.deltaTime;
            if (speedIncreaseTimer >= speedIncreaseInterval)
            {
                IncreaseSpeed();
                speedIncreaseTimer = 0f;
            }
        }

        public void StartGame()
        {
            isGameRunning = true;
            ResetSpeed();
        }

        public void PauseGame()
        {
            PauseManager.PauseGame();
            isGameRunning = false;
            OnPauseGame();
        }

        public void ResumeGame()
        {
            PauseManager.ResumeGame();
            isGameRunning = true;
            OnResumeGame();
        }

        public void GameOver()
        {
            isGameRunning = false;

            PauseGame();
            gameplayUI.OnGameOver(scoreManager.CurrentScore);
            AnalyticsHandler.Log("game_over", new Dictionary<string, object> { { "player_score", scoreManager.CurrentScore } });
        }

        public void TogglePauseGame()
        {
            if (isGameRunning)
                PauseGame();
            else
                ResumeGame();
        }

        private void IncreaseSpeed()
        {
            CurrentBaseSpeed = Mathf.Min(CurrentBaseSpeed + speedIncrement, maxBaseSpeed);

            // Calculate the speed multiplier based on how much we've increased from initial
            CurrentSpeedMultiplier = CurrentBaseSpeed / initialBaseSpeed;

            OnBaseSpeedChanged?.Invoke(CurrentBaseSpeed);
            OnSpeedChanged?.Invoke(CurrentSpeedMultiplier);

            //Debug.Log($"Speed Increased: Base={CurrentBaseSpeed}, Multiplier={CurrentSpeedMultiplier}");
        }

        public void SetSpeedMultiplier(float multiplier)
        {
            CurrentSpeedMultiplier = multiplier;
            OnSpeedChanged?.Invoke(CurrentSpeedMultiplier);
        }

        public void ResetSpeed()
        {
            CurrentBaseSpeed = initialBaseSpeed;
            CurrentSpeedMultiplier = 1f;
            speedIncreaseTimer = 0f;
            OnBaseSpeedChanged?.Invoke(CurrentBaseSpeed);
            OnSpeedChanged?.Invoke(CurrentSpeedMultiplier);
        }

        public float GetTotalSpeed()
        {
            return CurrentBaseSpeed * CurrentSpeedMultiplier;
        }

        private void OnPauseGame()
        {
            if (gameplayUI != null)
                gameplayUI.OnPauseGame();
        }

        private void OnResumeGame()
        {
            if (gameplayUI != null)
                gameplayUI.OnResumeGame();
        }

        private void OnPlayerCollide(Obstacle obstacle)
        {
            GameOver();
        }

        // Game over screen buttons
        public void GotoMainMenu()
        {
            Loading.Show();
            StartCoroutine(Utils.LoadSceneAsync(gameAttributes.MenuSceneName.ToString(), Loading.Hide));
        }
        
        public void ReplayGame()
        {
            Loading.Show();
            StartCoroutine(Utils.LoadSceneAsync(gameAttributes.GameSceneName.ToString(), Loading.Hide));
        }

        public void GotoLeaderBoard()
        { 
            // Leaderboard logic goes here
        }
    }
}