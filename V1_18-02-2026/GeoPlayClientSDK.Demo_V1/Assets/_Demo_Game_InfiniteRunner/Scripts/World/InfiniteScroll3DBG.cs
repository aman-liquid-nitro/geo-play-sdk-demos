using System.Collections.Generic;
using UnityEngine;

namespace GeoPlaySample.InfiniteRunner
{
    public class InfiniteScroll3DBG : MonoBehaviour, IPausable
    {
        [SerializeField] private List<Transform> segments = new List<Transform>();
        [SerializeField] private float segmentWidth = 20f; // Manually set the width of each segment
        [SerializeField] private float baseScrollSpeed = 10f; // This should match your obstacle base speed

        private bool isPaused;
        private float currentScrollSpeed;
        private List<Vector3> startPositions = new List<Vector3>();

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
            // Record the starting positions for reference
            startPositions.Clear();
            foreach (Transform segment in segments)
            {
                startPositions.Add(segment.position);
            }

            // Sort segments by x position
            segments.Sort((a, b) => a.position.x.CompareTo(b.position.x));
            UpdateScrollSpeed();
        }

        private void Update()
        {
            if (isPaused) return;

            float move = currentScrollSpeed * Time.deltaTime;

            // Move all segments
            foreach (Transform segment in segments)
            {
                segment.position += Vector3.left * move;
            }

            // Check and reposition segments that have moved too far left
            for (int i = 0; i < segments.Count; i++)
            {
                if (segments[i].position.x <= startPositions[0].x - segmentWidth)
                {
                    // Find the rightmost segment
                    Transform rightmostSegment = segments[0];
                    for (int j = 1; j < segments.Count; j++)
                    {
                        if (segments[j].position.x > rightmostSegment.position.x)
                        {
                            rightmostSegment = segments[j];
                        }
                    }

                    // Move this segment to the right of the rightmost segment
                    segments[i].position = new Vector3(
                        rightmostSegment.position.x + segmentWidth,
                        segments[i].position.y,
                        segments[i].position.z
                    );
                }
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
                currentScrollSpeed = baseScrollSpeed * GameManager.Instance.CurrentSpeedMultiplier;
            }
            else
            {
                currentScrollSpeed = baseScrollSpeed;
            }
        }

        // Add a segment at runtime
        public void AddSegment(Transform newSegment)
        {
            if (!segments.Contains(newSegment))
            {
                segments.Add(newSegment);
                startPositions.Add(newSegment.position);
            }
        }

        // Remove a segment
        public void RemoveSegment(Transform segmentToRemove)
        {
            int index = segments.IndexOf(segmentToRemove);
            if (index >= 0)
            {
                segments.RemoveAt(index);
                startPositions.RemoveAt(index);
            }
        }

        // Reset all segments
        public void ResetSegments()
        {
            for (int i = 0; i < segments.Count; i++)
            {
                if (i < startPositions.Count)
                {
                    segments[i].position = startPositions[i];
                }
            }
        }

        public float GetCurrentScrollSpeed()
        {
            return currentScrollSpeed;
        }

        public void SetSegmentWidth(float newWidth)
        {
            segmentWidth = newWidth;
        }

        public float GetSegmentWidth()
        {
            return segmentWidth;
        }
    }
}