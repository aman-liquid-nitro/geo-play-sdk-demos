using UnityEngine;

namespace GeoPlaySample.InfiniteRunner
{
    public class MenuUI : MonoBehaviour
    {
        [SerializeField] private UnityEngine.UI.Button storeButton;
        [SerializeField] private UnityEngine.UI.Button leaderboardButton;

        public void ToggleStoreButton(bool isEnabled)
        {
            storeButton.gameObject.SetActive(isEnabled);
        }

        public void ToggleLeaderBoardButton(bool isEnabled)
        {
            leaderboardButton.gameObject.SetActive(isEnabled);
        }
    }
}
