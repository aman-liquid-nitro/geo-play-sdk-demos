using System;

namespace GeoPlaySample.InfiniteRunner
{
    public class PauseManager
    {
        public static void PauseGame() => PauseEvents.TriggerPauseToggle(true);
        public static void ResumeGame() => PauseEvents.TriggerPauseToggle(false);
    }

    public static class PauseEvents
    {
        public static event Action<bool> OnPauseToggle;

        public static void TriggerPauseToggle(bool isPaused)
        {
            OnPauseToggle?.Invoke(isPaused);
        }
    }
}
