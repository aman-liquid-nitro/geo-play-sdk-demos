using System;

namespace GeoPlaySample.InfiniteRunner
{
    public interface IInputHandler
    {
        event Action OnJumpInput;
        event Action OnDuckInput;
        event Action OnDuckRelease;
        event Action OnIdleInput;

        void Update();
    }
}