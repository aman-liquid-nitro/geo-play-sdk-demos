using System;
using System.Linq;
using UnityEngine;

namespace GeoPlaySample.InfiniteRunner
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField] private GameAttributes gameAttributes;
        [SerializeField] private MenuUI menuUI;

        private void OnEnable()
        {
            GameManager.OnSetGameConfig += OnSetGameConfig;
        }

        private void OnDisable()
        {
            GameManager.OnSetGameConfig -= OnSetGameConfig;
        }

        private void Start()
        {
#if UNITY_IOS || UNITY_ANDROID
            Application.targetFrameRate = 60;
#endif
            GameManager.SetDefaultGameConfig();
        }

        public void StartGame()
        {
            Loading.Show();
            StartCoroutine(Utils.LoadSceneAsync(gameAttributes.GameSceneName.ToString(), Loading.Hide));

            AnalyticsHandler.Log("game_start");
        }

        private void ApplyMenuConfig()
        {
            if (menuUI == null)
                return;

            Feature leaderbordFeatureConfig = GameManager.GameConfig.featuresConfig.features.FirstOrDefault(t => t.id == "leaderboard");
            Feature storeFeatureConfig = GameManager.GameConfig.featuresConfig.features.FirstOrDefault(t => t.id == "store");
            menuUI.ToggleLeaderBoardButton(leaderbordFeatureConfig == null || leaderbordFeatureConfig.enabled);
            menuUI.ToggleStoreButton(storeFeatureConfig == null || storeFeatureConfig.enabled);
        }

        private void OnSetGameConfig(GameConfig config)
        {
            ApplyMenuConfig();
        }
    }
}
