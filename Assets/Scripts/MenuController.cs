using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEditor;
using System.Linq;

public class MenuController : MonoBehaviour {

    [SerializeField]
    private AudioMixer _masterAudioMixer;

    [SerializeField]
    private Slider _effectsVolumeSlider;

    [SerializeField]
    private Slider _musicVolumeSlider;

    [SerializeField]
    private Toggle _easyToggle;
    [SerializeField]
    private Toggle _normalToggle;
    [SerializeField]
    private Toggle _hardToggle;
    [SerializeField]
    private GameObject _messagePanel;
    
    public Animator FadeAnimator;

    public GameObject loadingScreenObject;
  
    private AsyncOperation _async;

     // Use this for initialization
    void Start ()
    {
        if (LevelController.GameSettings == null)
        {
            LevelController.GameSettings = GameSettings.LoadFromDisk();
        }
        SetEffectsVolume(LevelController.GameSettings.EffectsVolumeLevel);
        SetMusicVolume(LevelController.GameSettings.MusicVolumeLevel);
        switch (LevelController.GameSettings.LastDifficulty)
        {
            case GameSettings.DifficultyLevels.Easy:
                _easyToggle.isOn = true;
                break;
            case GameSettings.DifficultyLevels.Normal:
                _normalToggle.isOn = true;
                break;
            case GameSettings.DifficultyLevels.Hard:
                _hardToggle.isOn = true;
                break;
            default:
                break;
        }
	}
	

    // Update is called once per frame
    void Update ()
    {
        
    }

    public void HandleStart()
    {
        //SceneManager.LoadSceneAsync("Level01", LoadSceneMode.Single);
        //FadeAnimator.SetTrigger("Close");

        Debug.Log("HandleStart " + System.DateTime.Now.ToString());
        StartCoroutine(ShowSpinner());
        StartCoroutine(LoadSceneWithAmimation("Level01", "Close"));
    }
    public void HandleInstructions()
    {
        SceneManager.LoadScene("Instructions", LoadSceneMode.Single);
    }

    public void HandleQuit()
    {
        LevelController.StopGame();
    }

    public void EasyToggleChanged(bool isOn)
    {
        if (isOn)
        {
            LevelController.GameSettings.LastDifficulty = GameSettings.DifficultyLevels.Easy;
        }
    }

    public void NormalToggleChanged(bool isOn)
    {
        if (isOn)
        {
            if (GameSettings.DifficultyLevels.Normal <= LevelController.GameSettings.MaxDifficulty)
            {
                LevelController.GameSettings.LastDifficulty = GameSettings.DifficultyLevels.Normal;
            }
            else
            {
                _easyToggle.isOn = true;
                DisplayMessageBox("Beat the game on Easy to unlock Normal.");
            }
        }
    }

    private void DisplayMessageBox(string message)
    {
        _messagePanel.SetActive(true);
        List<Text> texts = new List<Text>();
        _messagePanel.gameObject.GetComponentsInChildren<Text>(texts);
        Text messageText = texts.Where(t => t.name == "MessageText").FirstOrDefault();
        messageText.text = message;
    }

    public void HandleMessageOkClick()
    {
        _messagePanel.SetActive(false);
    }

    public void HardToggleChanged(bool isOn)
    {
        if (isOn)
        {
            if (GameSettings.DifficultyLevels.Hard <= LevelController.GameSettings.MaxDifficulty)
            {
                LevelController.GameSettings.LastDifficulty = GameSettings.DifficultyLevels.Hard;
            }
            else
            {
                _easyToggle.isOn = true;
                DisplayMessageBox("Beat the game on Normal to unlock Hard.");
            }
        }
    }

    public void SetEffectsVolume(float level)
    {
        _effectsVolumeSlider.value = level;
        LevelController.GameSettings.EffectsVolumeLevel = level;
        _masterAudioMixer.SetFloat("EffectsVolume", level);
    }

    public void SetMusicVolume(float level)
    {
        _musicVolumeSlider.value = level;
        LevelController.GameSettings.MusicVolumeLevel = level;
        _masterAudioMixer.SetFloat("MusicVolume", level);
    }

    private IEnumerator LoadSceneWithAmimation(string scene, string animTrigger)
    {
        FadeAnimator.SetTrigger(animTrigger);
        yield return new WaitForSeconds(1.5f);

        //Debug.Log("Wait is over " + System.DateTime.Now.ToString());
        //_async = SceneManager.LoadSceneAsync(scene);
        //_async.allowSceneActivation = true;

        StartCoroutine(LoadingScreen(scene));
        //SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }

    private IEnumerator ShowSpinner()
    {
        loadingScreenObject.SetActive(true);
        yield return null;
    }

    private IEnumerator LoadingScreen(string loadMe)
    {
        Application.backgroundLoadingPriority = ThreadPriority.Low;
        _async = SceneManager.LoadSceneAsync(loadMe);
        _async.allowSceneActivation = false;

        while (!_async.isDone)
        {
            if (_async.progress == 0.9f)
            {
                _async.allowSceneActivation = true;
            }
            yield return null;
        }
        Debug.Log("New scene load is done " + System.DateTime.Now.ToString());

    }


}
