using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Loader
{
    #region Public

    public enum Scene
    {
        Menu,
        Gallery,
        PhotoViewer,
        Loader
    }

    public static void Load(Scene scene, float time = 0f)
    {
        OnLoaderCallback = () =>
        {
            var loadingGameObject = new GameObject("Loading Game Object").AddComponent<LoaderMonoBehaviour>();
            loadingGameObject.StartCoroutine(LoadSceneAsync(scene, time));
        };
        SceneManager.LoadScene(Scene.Loader.ToString());
    }

    public static float GetLoadingProgress()
    {
        if (loadingAsyhcOperation != null)
        {
            return loadingAsyhcOperation.progress;
        }
        else
        {
            return 0f;
        }
    }
    
    public static void LoaderCallback()
    {
        OnLoaderCallback?.Invoke();
        OnLoaderCallback = null;
    }
    
    #endregion
    
    private class LoaderMonoBehaviour : MonoBehaviour { }

    private static Action OnLoaderCallback;
    private static AsyncOperation loadingAsyhcOperation;

    private static IEnumerator LoadSceneAsync(Scene scene, float time)
    {
        yield return new WaitForSeconds(time);
        
        loadingAsyhcOperation = SceneManager.LoadSceneAsync(scene.ToString());

        while (loadingAsyhcOperation.isDone)
        {
            yield return null;
        }
    }
}
