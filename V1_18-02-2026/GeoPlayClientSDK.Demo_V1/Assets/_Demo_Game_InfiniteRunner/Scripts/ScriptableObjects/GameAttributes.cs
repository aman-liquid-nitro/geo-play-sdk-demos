using UnityEngine;

namespace GeoPlaySample.InfiniteRunner
{
    [CreateAssetMenu(fileName = "NewGameAttributes", menuName = "SO/Game/Game Attributes")]
    public class GameAttributes : ScriptableObject
    {
        public SceneName MenuSceneName = SceneName.Menu;
        public SceneName GameSceneName = SceneName.Game;
    }

    public enum SceneName
    {
        Menu,
        Game,
        Game3D
    }
}
