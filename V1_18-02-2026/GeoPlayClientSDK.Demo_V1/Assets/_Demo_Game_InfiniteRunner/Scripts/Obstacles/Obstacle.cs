using UnityEngine;
using UnityEngine.Pool;

namespace GeoPlaySample.InfiniteRunner
{
    public class Obstacle : MonoBehaviour, IPausable
    {
        [Header("Obstacle Settings")]
        [SerializeField] private ObstacleHeight obstacleHeight = ObstacleHeight.Mid;
        [SerializeField] private float speedVariation = 1f;

        public ObstacleHeight Height => obstacleHeight;
        public bool RequiresDuck => obstacleHeight == ObstacleHeight.High;

        private float currentSpeed;
        private bool isMoving = true;
        private bool isPaused = false;
        private IObjectPool<Obstacle> objectPool;

        void OnEnable()
        {
            GameManager.OnSpeedChanged += OnSpeedChanged;
            GameManager.OnBaseSpeedChanged += OnBaseSpeedChanged;

            // Always recalculate speed when enabled (when taken from pool)
            CalculateCurrentSpeed();
        }

        void OnDisable()
        {
            GameManager.OnSpeedChanged -= OnSpeedChanged;
            GameManager.OnBaseSpeedChanged -= OnBaseSpeedChanged;
        }

        void Update()
        {
            if (isMoving && !isPaused)
            {
                transform.Translate(Vector3.left * currentSpeed * Time.deltaTime);
            }
        }

        private void CalculateCurrentSpeed()
        {
            if (GameManager.Instance != null)
            {
                // Use global obstacle speed from GameManager + individual variation
                float baseSpeed = GameManager.Instance.BaseObstacleSpeed;
                currentSpeed = (baseSpeed + Random.Range(0, speedVariation)) * GameManager.Instance.CurrentSpeedMultiplier;
            }
            else
            {
                // Fallback if GameManager isn't available
                currentSpeed = 5f + Random.Range(-speedVariation, speedVariation);
            }
        }

        private void OnSpeedChanged(float newMultiplier)
        {
            // Update speed immediately when game speed changes
            CalculateCurrentSpeed();
        }

        private void OnBaseSpeedChanged(float newBaseSpeed)
        {
            // Update speed immediately when base speed changes
            CalculateCurrentSpeed();
        }

        public void SetPool(IObjectPool<Obstacle> pool)
        {
            objectPool = pool;
        }

        public void StopMoving()
        {
            isMoving = false;
        }

        public void StartMoving()
        {
            isMoving = true;
        }

        // IPausable implementation
        public void TogglePause(bool isPaused)
        {
            this.isPaused = isPaused;
        }

        void OnBecameInvisible()
        {
            if (transform.position.x < -10f && objectPool != null)
            {
                objectPool.Release(this);
            }
        }

        public void ResetObstacle()
        {
            isMoving = true;
            isPaused = false;
            // Speed will be recalculated in OnEnable when taken from pool
        }

        // Debug method to check current speed
        public float GetCurrentSpeed()
        {
            return currentSpeed;
        }
    }

    public enum ObstacleHeight { Low, Mid, High }
}