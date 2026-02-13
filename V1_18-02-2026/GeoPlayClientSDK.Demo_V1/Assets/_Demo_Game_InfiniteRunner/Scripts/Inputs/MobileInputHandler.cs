using System;
using UnityEngine;

namespace GeoPlaySample.InfiniteRunner
{
    public class MobileInputHandler : IInputHandler
    {
        public event Action OnJumpInput;
        public event Action OnDuckInput;
        public event Action OnDuckRelease;
        public event Action OnIdleInput;

        private float swipeThreshold;
        private float duckDuration;

        private Vector2 touchStartPos;
        private bool isSwiping;
        private bool isDucking;
        private float duckTimer;

        public MobileInputHandler(float swipeThreshold = 50f, float duckDuration = 1.5f)
        {
            this.swipeThreshold = swipeThreshold;
            this.duckDuration = duckDuration;
        }

        public void Update()
        {
            // Handle timed duck
            if (isDucking)
            {
                duckTimer -= Time.deltaTime;
                if (duckTimer <= 0)
                {
                    OnDuckRelease?.Invoke();
                    isDucking = false;
                }
            }

            // Handle touch input
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);

                switch (touch.phase)
                {
                    case TouchPhase.Began:
                        touchStartPos = touch.position;
                        isSwiping = true;
                        break;

                    case TouchPhase.Moved:
                    case TouchPhase.Stationary:
                        if (isSwiping)
                        {
                            float swipeDelta = touch.position.y - touchStartPos.y;

                            if (swipeDelta > swipeThreshold)
                            {
                                // Swipe Up - Jump
                                OnJumpInput?.Invoke();
                                isSwiping = false;

                                // Cancel duck if jumping
                                if (isDucking)
                                {
                                    OnDuckRelease?.Invoke();
                                    isDucking = false;
                                }
                            }
                            else if (swipeDelta < -swipeThreshold/* && !isDucking*/)
                            {
                                // Swipe Down - Duck for specific duration
                                OnDuckInput?.Invoke();
                                isDucking = true;
                                duckTimer = duckDuration;
                                isSwiping = false;
                            }
                        }
                        break;

                    case TouchPhase.Ended:
                    case TouchPhase.Canceled:
                        isSwiping = false;
                        break;
                }
            }

            // Call idle when not ducking
            if (!isDucking && !isSwiping)
            {
                OnIdleInput?.Invoke();
            }

            // Optional: Mouse input for testing in editor
#if UNITY_EDITOR
            HandleMouseInput();
#endif
        }

        private void HandleMouseInput()
        {
            if (Input.GetMouseButtonDown(0))
            {
                touchStartPos = Input.mousePosition;
                isSwiping = true;
            }
            else if (Input.GetMouseButton(0) && isSwiping)
            {
                float swipeDelta = Input.mousePosition.y - touchStartPos.y;

                if (swipeDelta > swipeThreshold)
                {
                    OnJumpInput?.Invoke();
                    isSwiping = false;

                    if (isDucking)
                    {
                        OnDuckRelease?.Invoke();
                        isDucking = false;
                    }
                }
                else if (swipeDelta < -swipeThreshold/* && !isDucking*/)
                {
                    OnDuckInput?.Invoke();
                    isDucking = true;
                    duckTimer = duckDuration;
                    isSwiping = false;
                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                isSwiping = false;
            }
        }
    }
}