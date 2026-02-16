using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GeoPlaySample.InfiniteRunner
{
    public class GameplayUI : MonoBehaviour
    {
        [SerializeField] private Button pauseButton;
        [SerializeField] private Button playButton;
        [SerializeField] private GameObject pausedPanel;

        [Header("Game Over")]
        [SerializeField] private GameObject gameOverPanel;
        [SerializeField] private TextMeshProUGUI gameOverScoreText;
        [SerializeField] private Button leaderboardButton;

        private void Start()
        {
            // Show unpaused game state on start
            OnPause(false);
            HideGameOverPanel();
        }

        public void OnPauseGame()
        {
            OnPause(true);
        }

        public void OnResumeGame()
        {
            OnPause(false);
        }
        
        public void OnGameOver(int score)
        {
            ShowGameOverPanel(score);
        }

        private void OnPause(bool isPaused)
        {
            pauseButton.gameObject.SetActive(!isPaused);
            playButton.gameObject.SetActive(isPaused);
            pausedPanel.gameObject.SetActive(isPaused);
        }

        private void ShowGameOverPanel(int score)
        {
            if (gameOverPanel != null)
                gameOverPanel.SetActive(true);

            gameOverScoreText.text = $"Score: {score}";
        }

        private void HideGameOverPanel()
        {
            if (gameOverPanel != null)
                gameOverPanel.SetActive(false);
        }

        private void UpdateLeadearBoardButtonState(GameConfig config)
        {
            if (leaderboardButton == null) return;
            Feature leaderboardFeatureConfig = config.featuresConfig.features.FirstOrDefault(t => t.id == "leaderboard");
            leaderboardButton.gameObject.SetActive(leaderboardFeatureConfig == null || leaderboardFeatureConfig.enabled);
        }

        public void OnSetConfig(GameConfig config)
        {
            UpdateLeadearBoardButtonState(config);
        }
    }
}
