using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GeoPlaySample.InfiniteRunner
{
    public static class Utils
    {
        public static IEnumerator LoadSceneAsync(string sceneName, System.Action onDone = null)
        {
            AsyncOperation sceneLoadOp = SceneManager.LoadSceneAsync(sceneName);
            sceneLoadOp.completed += (op) => { onDone?.Invoke(); };
            sceneLoadOp.allowSceneActivation = false;

            while (sceneLoadOp.progress < 0.9f)
                yield return null;

            sceneLoadOp.allowSceneActivation = true;
        }
    }
}