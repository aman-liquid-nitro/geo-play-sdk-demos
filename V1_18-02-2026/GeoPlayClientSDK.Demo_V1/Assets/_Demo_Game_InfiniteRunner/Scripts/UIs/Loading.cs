using TMPro;
using UnityEngine;

namespace GeoPlaySample.InfiniteRunner
{
    public class Loading : MonoBehaviour
    {
        [SerializeField] private GameObject loadingCanvasObject;
        [SerializeField] private TextMeshProUGUI loadingText;

        private static Loading instance;
        private static bool applicationIsQuitting = false;

        public static bool IsShowing
        {
            get
            {
                if (instance == null || instance.loadingCanvasObject == null)
                    return false;
                return instance.loadingCanvasObject.activeSelf;
            }
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
                return;
            }

            if (loadingCanvasObject != null)
                loadingCanvasObject.SetActive(false);
        }

        private void OnDestroy()
        {
            if (instance == this && !applicationIsQuitting)
            {
                instance = null;
            }
        }

        private void OnApplicationQuit()
        {
            applicationIsQuitting = true;
        }

        public static void Show()
        {
            Show("Loading...");
        }

        public static void Hide()
        {
            if (instance == null)
            {
                Debug.LogWarning("Loading instance is null. Cannot hide loading screen.");
                return;
            }

            if (instance.loadingCanvasObject != null)
                instance.loadingCanvasObject.SetActive(false);
            else
                Debug.LogError("Loading Canvas Object is not assigned in the inspector!");
        }

        public static void Show(string loadingMessage)
        {
            if (instance == null)
            {
                Debug.LogWarning("Loading instance is null. Cannot show loading screen.");
                return;
            }

            if (instance.loadingText != null)
                instance.loadingText.text = loadingMessage;

            if (instance.loadingCanvasObject != null)
                instance.loadingCanvasObject.SetActive(true);
            else
                Debug.LogError("Loading Canvas Object is not assigned in the inspector!");
        }
    }
}