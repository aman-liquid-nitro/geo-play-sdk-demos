using UnityEngine;
using TMPro;

namespace GeoPlaySample.InfiniteRunner
{
    public class FPSCounter : MonoBehaviour
    {
        [Header("UI Reference")]
        [SerializeField] private TextMeshProUGUI fpsText;

        [Header("Settings")]
        [SerializeField] private float updateInterval = 0.5f;

        private float accum = 0.0f;
        private int frames = 0;
        private float timeLeft;

        private void Awake()
        {
            // Singleton pattern to prevent duplicates
            if (FindObjectsByType<FPSCounter>(FindObjectsSortMode.None).Length > 1)
            {
                Destroy(gameObject);
                return;
            }

            // Make this object persist across scenes
            DontDestroyOnLoad(gameObject);

            timeLeft = updateInterval;
        }

        private void Start()
        {
            if (fpsText == null)
            {
                Debug.LogError("FPS Text component is not assigned!");
            }
        }

        private void Update()
        {
            if (fpsText == null) return;

            timeLeft -= Time.deltaTime;
            accum += Time.timeScale / Time.deltaTime;
            frames++;

            // Update FPS display at specified interval
            if (timeLeft <= 0.0f)
            {
                float fps = accum / frames;
                string fpsString = fps.ToString("F0");

                fpsText.text = "FPS: " + fpsString;

                // Color coding based on FPS
                if (fps >= 60)
                    fpsText.color = Color.green;
                else if (fps >= 30)
                    fpsText.color = Color.yellow;
                else
                    fpsText.color = Color.red;

                timeLeft = updateInterval;
                accum = 0.0f;
                frames = 0;
            }
        }
    }
}