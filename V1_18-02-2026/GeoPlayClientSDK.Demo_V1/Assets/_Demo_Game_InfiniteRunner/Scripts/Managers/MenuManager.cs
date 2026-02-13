using UnityEngine;

namespace GeoPlaySample.InfiniteRunner
{
    public class MenuManager : MonoBehaviour
    {
        [SerializeField] private GameAttributes gameAttributes;

#if UNITY_IOS || UNITY_ANDROID
        private void Start()
        {
            Application.targetFrameRate = 60;
        }
#endif

        public void StartGame()
        {
            Loading.Show();
            StartCoroutine(Utils.LoadSceneAsync(gameAttributes.GameSceneName.ToString(), Loading.Hide));

            AnalyticsHandler.Log("game_start");
        }
    }
}
