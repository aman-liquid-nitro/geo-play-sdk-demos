using System;
using UnityEngine;

namespace GeoPlaySample.InfiniteRunner
{
    public class KeyboardInputHandler : IInputHandler
    {
        public event Action OnJumpInput;
        public event Action OnDuckInput;
        public event Action OnDuckRelease;
        public event Action OnIdleInput;

        private bool wasDucking;

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                OnJumpInput?.Invoke();
                wasDucking = false;
            }
            else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
            {
                if (!wasDucking)
                {
                    OnDuckInput?.Invoke();
                    wasDucking = true;
                }
            }
            else
            {
                if (wasDucking)
                {
                    OnDuckRelease?.Invoke();
                    wasDucking = false;
                }
                OnIdleInput?.Invoke();
            }
        }
    }
}