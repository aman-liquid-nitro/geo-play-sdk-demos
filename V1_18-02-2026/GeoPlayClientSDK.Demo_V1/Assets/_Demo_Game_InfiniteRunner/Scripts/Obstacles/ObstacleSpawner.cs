using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

namespace GeoPlaySample.InfiniteRunner
{
    public class ObstacleSpawner : MonoBehaviour, IPausable
    {
        [System.Serializable]
        public class ObstaclePoolConfig
        {
            public Obstacle obstaclePrefab;
            public int defaultCapacity = 10;
            public int maxSize = 20;
        }

        [System.Serializable]
        public class ObstaclePosition
        {
            public ObstacleHeight height;
            public Transform spawnPoint;
            public ObstaclePoolConfig[] obstaclePoolConfigs;
        }

        [Header("Spawn Settings")]
        [SerializeField] private float initialSpawnDelay = 3f;
        [SerializeField] private float minSpawnInterval = 1.5f;
        [SerializeField] private float maxSpawnInterval = 3f;
        [SerializeField] private AnimationCurve spawnRateCurve = AnimationCurve.Linear(0, 1, 1, 2);

        [Header("Obstacle Positions")]
        [SerializeField] private ObstaclePosition[] obstaclePositions;

        private bool isSpawning = false;
        private bool isPaused = false;
        private Coroutine spawningCoroutine;
        private Dictionary<ObstacleHeight, ObstaclePosition> positionLookup;
        private Dictionary<Obstacle, IObjectPool<Obstacle>> obstaclePools;
        private Dictionary<ObstacleHeight, List<IObjectPool<Obstacle>>> heightToPoolsMap;

        void OnEnable()
        {
            GameManager.OnBaseSpeedChanged += OnGameSpeedChanged;
        }

        void OnDisable()
        {
            GameManager.OnBaseSpeedChanged -= OnGameSpeedChanged;
        }

        void Start()
        {
            InitializePools();
            BuildPositionLookup();
            StartSpawning();
        }

        private void InitializePools()
        {
            obstaclePools = new Dictionary<Obstacle, IObjectPool<Obstacle>>();
            heightToPoolsMap = new Dictionary<ObstacleHeight, List<IObjectPool<Obstacle>>>();

            // Initialize height mapping
            foreach (ObstacleHeight height in System.Enum.GetValues(typeof(ObstacleHeight)))
            {
                heightToPoolsMap[height] = new List<IObjectPool<Obstacle>>();
            }

            // Create pools for all obstacle prefabs
            foreach (var position in obstaclePositions)
            {
                foreach (var poolConfig in position.obstaclePoolConfigs)
                {
                    if (poolConfig.obstaclePrefab != null && !obstaclePools.ContainsKey(poolConfig.obstaclePrefab))
                    {
                        var pool = CreatePoolForObstacle(poolConfig);
                        obstaclePools[poolConfig.obstaclePrefab] = pool;
                        heightToPoolsMap[position.height].Add(pool);
                    }
                }
            }
        }
        private IObjectPool<Obstacle> CreatePoolForObstacle(ObstaclePoolConfig config)
        {
            return new ObjectPool<Obstacle>(
                createFunc: () => CreateObstacle(config.obstaclePrefab),
                actionOnGet: (obstacle) => OnGetObstacle(obstacle),
                actionOnRelease: (obstacle) => OnReleaseObstacle(obstacle),
                actionOnDestroy: (obstacle) => OnDestroyObstacle(obstacle),
                collectionCheck: true,
                defaultCapacity: config.defaultCapacity,
                maxSize: config.maxSize
            );
        }

        private Obstacle CreateObstacle(Obstacle prefab)
        {
            Obstacle obstacle = Instantiate(prefab);
            obstacle.SetPool(obstaclePools[prefab]);
            return obstacle;
        }

        private void OnGetObstacle(Obstacle obstacle)
        {
            obstacle.ResetObstacle();
            obstacle.gameObject.SetActive(true);
            obstacle.StartMoving();
        }

        private void OnReleaseObstacle(Obstacle obstacle)
        {
            obstacle.gameObject.SetActive(false);
            obstacle.StopMoving();
        }

        private void OnDestroyObstacle(Obstacle obstacle)
        {
            if (obstacle != null)
                Destroy(obstacle.gameObject);
        }

        private void BuildPositionLookup()
        {
            positionLookup = new Dictionary<ObstacleHeight, ObstaclePosition>();
            foreach (var position in obstaclePositions)
            {
                if (!positionLookup.ContainsKey(position.height))
                {
                    positionLookup.Add(position.height, position);
                }
                else
                {
                    Debug.LogWarning($"Duplicate position found for height: {position.height}");
                }
            }

            // Validate that all required heights are present
            if (!positionLookup.ContainsKey(ObstacleHeight.Low))
                Debug.LogError("Low position is missing from obstacle positions array!");
            if (!positionLookup.ContainsKey(ObstacleHeight.Mid))
                Debug.LogError("Mid position is missing from obstacle positions array!");
            if (!positionLookup.ContainsKey(ObstacleHeight.High))
                Debug.LogError("High position is missing from obstacle positions array!");
        }

        private void OnGameSpeedChanged(float newBaseSpeed)
        {
            // Adjust spawn intervals based on game speed
            float normalizedSpeed = newBaseSpeed / GameManager.Instance.MaxBaseSpeed;
            float spawnRateMultiplier = spawnRateCurve.Evaluate(normalizedSpeed);
        }

        public void StartSpawning()
        {
            if (isSpawning || isPaused) return;

            isSpawning = true;
            spawningCoroutine = StartCoroutine(SpawnRoutine());
        }

        public void StopSpawning()
        {
            if (!isSpawning) return;

            isSpawning = false;
            if (spawningCoroutine != null)
            {
                StopCoroutine(spawningCoroutine);
                spawningCoroutine = null;
            }
        }

        private IEnumerator SpawnRoutine()
        {
            // Initial delay before starting to spawn obstacles
            yield return new WaitForSeconds(initialSpawnDelay);

            while (isSpawning && !isPaused)
            {
                SpawnRandomObstacle();

                // Adjust spawn interval based on current game speed
                // As speed increases, we want obstacles to spawn less frequently to maintain spacing
                float speedFactor = GameManager.Instance != null ? GameManager.Instance.GetTotalSpeed() : 1f;

                // Use the spawn rate curve to determine how spawn intervals should scale with speed
                float normalizedSpeed = GameManager.Instance.CurrentBaseSpeed / GameManager.Instance.MaxBaseSpeed;
                float spawnIntervalMultiplier = spawnRateCurve.Evaluate(normalizedSpeed);

                // Increase spawn intervals as speed increases to maintain proper spacing
                float adjustedMinInterval = minSpawnInterval * spawnIntervalMultiplier;
                float adjustedMaxInterval = maxSpawnInterval * spawnIntervalMultiplier;

                float randomInterval = Random.Range(adjustedMinInterval, adjustedMaxInterval);
                yield return new WaitForSeconds(randomInterval);
            }
        }

        private void SpawnRandomObstacle()
        {
            // Randomly select which position to spawn obstacle at
            ObstacleHeight randomHeight = (ObstacleHeight)Random.Range(0, 3);
            SpawnObstacleAtHeight(randomHeight);
        }

        private void SpawnObstacleAtHeight(ObstacleHeight height)
        {
            if (positionLookup.TryGetValue(height, out ObstaclePosition selectedPosition))
            {
                // Spawn obstacle at selected position
                if (selectedPosition.obstaclePoolConfigs.Length > 0 && selectedPosition.spawnPoint != null)
                {
                    // Randomly select an obstacle type from this height's available pools
                    int configIndex = Random.Range(0, selectedPosition.obstaclePoolConfigs.Length);
                    ObstaclePoolConfig selectedConfig = selectedPosition.obstaclePoolConfigs[configIndex];

                    if (selectedConfig.obstaclePrefab != null && obstaclePools.ContainsKey(selectedConfig.obstaclePrefab))
                    {
                        IObjectPool<Obstacle> pool = obstaclePools[selectedConfig.obstaclePrefab];
                        Obstacle obstacle = pool.Get();

                        // Position the obstacle
                        obstacle.transform.position = selectedPosition.spawnPoint.position;
                        obstacle.transform.rotation = selectedPosition.spawnPoint.rotation;

                        // Apply current pause state to new obstacle
                        if (isPaused)
                        {
                            obstacle.TogglePause(true);
                        }
                    }
                    else
                    {
                        Debug.LogWarning($"Cannot spawn obstacle at {height} - prefab or pool not found");
                    }
                }
                else
                {
                    Debug.LogWarning($"Cannot spawn obstacle at {height} - check spawn point and prefabs are assigned");
                }
            }
            else
            {
                Debug.LogWarning($"Position not found for height: {height}");
            }
        }

        // Method to spawn specific obstacle type at specific position
        public void SpawnSpecificObstacle(ObstacleHeight height, int configIndex)
        {
            if (positionLookup.TryGetValue(height, out ObstaclePosition position))
            {
                if (configIndex >= 0 && configIndex < position.obstaclePoolConfigs.Length)
                {
                    var config = position.obstaclePoolConfigs[configIndex];
                    if (config.obstaclePrefab != null && obstaclePools.ContainsKey(config.obstaclePrefab))
                    {
                        IObjectPool<Obstacle> pool = obstaclePools[config.obstaclePrefab];
                        Obstacle obstacle = pool.Get();

                        obstacle.transform.position = position.spawnPoint.position;
                        obstacle.transform.rotation = position.spawnPoint.rotation;

                        if (isPaused)
                        {
                            obstacle.TogglePause(true);
                        }
                    }
                }
            }
        }

        // IPausable implementation
        public void TogglePause(bool isPaused)
        {
            this.isPaused = isPaused;

            // Pause/Resume spawning
            if (isPaused)
            {
                StopSpawning();
            }
            else
            {
                StartSpawning();
            }
        }

        // Public methods to adjust spawning dynamically
        public void SetSpawnIntervals(float minInterval, float maxInterval)
        {
            minSpawnInterval = minInterval;
            maxSpawnInterval = maxInterval;
        }

        public void SetInitialDelay(float delay)
        {
            initialSpawnDelay = delay;
        }

        public Vector3 GetSpawnPoint(ObstacleHeight height)
        {
            if (positionLookup.TryGetValue(height, out ObstaclePosition position))
            {
                return position.spawnPoint?.position ?? Vector3.zero;
            }
            return Vector3.zero;
        }

        public bool RequiresDuck(ObstacleHeight height)
        {
            return height == ObstacleHeight.High;
        }

        public ObstacleHeight[] GetAvailableHeights()
        {
            return new ObstacleHeight[] { ObstacleHeight.Low, ObstacleHeight.Mid, ObstacleHeight.High };
        }

        // Get random pool for a specific height (useful for testing)
        public IObjectPool<Obstacle> GetRandomPoolForHeight(ObstacleHeight height)
        {
            if (heightToPoolsMap.ContainsKey(height) && heightToPoolsMap[height].Count > 0)
            {
                int randomIndex = Random.Range(0, heightToPoolsMap[height].Count);
                return heightToPoolsMap[height][randomIndex];
            }
            return null;
        }

        // Clean up all pools when destroyed
        void OnDestroy()
        {
            if (obstaclePools != null)
            {
                foreach (var pool in obstaclePools.Values)
                {
                    pool?.Clear();
                }
            }
        }

        void OnValidate()
        {
            // Ensure values are valid in inspector
            minSpawnInterval = Mathf.Max(0.1f, minSpawnInterval);
            maxSpawnInterval = Mathf.Max(minSpawnInterval, maxSpawnInterval);
            initialSpawnDelay = Mathf.Max(0f, initialSpawnDelay);

            // Auto-assign heights if not set and we have the right number of positions
            if (obstaclePositions != null && obstaclePositions.Length == 3)
            {
                // Ensure each position has a unique height
                for (int i = 0; i < obstaclePositions.Length; i++)
                {
                    if (i == 0) obstaclePositions[i].height = ObstacleHeight.Low;
                    else if (i == 1) obstaclePositions[i].height = ObstacleHeight.Mid;
                    else if (i == 2) obstaclePositions[i].height = ObstacleHeight.High;
                }
            }
        }
    }
}