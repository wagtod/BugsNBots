using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingController : MonoBehaviour {

    public GameObject loadingScreenObject;
    public Slider slider;

    private AsyncOperation _async;

    //private void Start()
    //{
    //    LoadForm(SceneToLoad);
    //}

    public void LoadForm(string loadMe)
    {
        StartCoroutine(LoadingScreen(loadMe));
    }

    private IEnumerator LoadingScreen(string loadMe)
    {
        loadingScreenObject.SetActive(true);
        _async = SceneManager.LoadSceneAsync(loadMe);
        _async.allowSceneActivation = false;

        while (! _async.isDone)
        {
            slider.value = _async.progress;
            if (_async.progress == 0.9f)
            {
                slider.value = 1f;
                _async.allowSceneActivation = true;
            }
            yield return null;
        }
    }
	
}
