using UnityEngine;

namespace GeoPlaySample.InfiniteRunner
{
    public class Pausable : MonoBehaviour
    {
        private IPausable[] _pausables;

        private void OnEnable()
        {
            // Get all IPausable components on this GameObject
            _pausables = GetComponents<IPausable>();

            if (_pausables.Length > 0)
            {
                PauseEvents.OnPauseToggle += OnPauseToggled;
            }
        }

        private void OnDisable()
        {
            if (_pausables.Length > 0)
            {
                PauseEvents.OnPauseToggle -= OnPauseToggled;
            }
        }

        private void OnPauseToggled(bool isPaused)
        {
            foreach (var pausable in _pausables)
            {
                pausable?.TogglePause(isPaused);
            }
        }
    }
}
