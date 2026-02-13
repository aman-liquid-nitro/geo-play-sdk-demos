using UnityEngine;

namespace GeoPlaySample.InfiniteRunner
{
    public class InfiniteScrollBG : MonoBehaviour, IPausable
    {
        [SerializeField] private Transform segmentA;
        [SerializeField] private Transform segmentB;
        [SerializeField] private float baseScrollSpeed = 10f; // This should match your obstacle base speed

        private float segmentWidth;
        private bool isPaused;
        private float currentScrollSpeed;

        void OnEnable()
        {
            GameManager.OnSpeedChanged += OnSpeedChanged;
            GameManager.OnBaseSpeedChanged += OnBaseSpeedChanged;

            if (GameManager.Instance != null)
            {
                UpdateScrollSpeed();
            }
        }

        void OnDisable()
        {
            GameManager.OnSpeedChanged -= OnSpeedChanged;
            GameManager.OnBaseSpeedChanged -= OnBaseSpeedChanged;
        }

        public void TogglePause(bool isPaused)
        {
            this.isPaused = isPaused;
        }

        private void Start()
        {
            SpriteRenderer sr = segmentA.GetComponentInChildren<SpriteRenderer>();
            segmentWidth = sr.bounds.size.x;
            UpdateScrollSpeed();
        }

        private void Update()
        {
            if (isPaused) return;

            float move = currentScrollSpeed * Time.deltaTime;

            segmentA.position += Vector3.left * move;
            segmentB.position += Vector3.left * move;

            if (segmentA.position.x <= -segmentWidth)
            {
                segmentA.position += new Vector3(segmentWidth * 2f, 0f, 0f);
            }

            if (segmentB.position.x <= -segmentWidth)
            {
                segmentB.position += new Vector3(segmentWidth * 2f, 0f, 0f);
            }
        }

        private void OnSpeedChanged(float newMultiplier)
        {
            UpdateScrollSpeed();
        }

        private void OnBaseSpeedChanged(float newBaseSpeed)
        {
            UpdateScrollSpeed();
        }

        private void UpdateScrollSpeed()
        {
            if (GameManager.Instance != null)
            {
                // Use the same calculation as obstacles
                currentScrollSpeed = baseScrollSpeed * GameManager.Instance.CurrentSpeedMultiplier;
            }
            else
            {
                currentScrollSpeed = baseScrollSpeed;
            }
        }

        // Optional: Method to get current speed for debugging
        public float GetCurrentScrollSpeed()
        {
            return currentScrollSpeed;
        }
    }
}