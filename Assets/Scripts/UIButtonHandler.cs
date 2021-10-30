using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class UIButtonHandler : Singleton<UIButtonHandler>
{
    [SerializeField]
    private AudioMixer _masterAudioMixer;

    [SerializeField]
    private Slider _effectsVolumeSlider;

    [SerializeField]
    private Slider _musicVolumeSlider;

    [SerializeField]
    private GameObject _mouseBuildPrefab;

    [SerializeField]
    private GameObject _turretButtonGO;

    [SerializeField]
    private GameObject _shieldButtonGO;

    [SerializeField]
    private GameObject _ammoPlusButtonGO;

    [SerializeField]
    private GameObject _bfgButtonGO;

    [SerializeField]
    private GameObject _freezeButtonGO;

    [SerializeField]
    private GameObject _speedButtonGO;

    [SerializeField]
    private GameObject _winButtonGO;

    [SerializeField]
    private GameObject _escMenu;

    [SerializeField]
    private GameObject _gameOverPanel;

    public Animator FadeAnimator;

    public bool IsGamePaused { get; set; }
    public bool IsTurretClicked { get; set; }

    private GameObject _mouseBuild;
    private Sprite _turretHoverSprite;
    private Button _turretButton;
    private Button TurretButton
    {
        get
        {
            if (_turretButton == null)
            {
                _turretButton = _turretButtonGO.GetComponent<Button>();
            }
            return _turretButton;
        }
    }

    private Button _shieldsButton;
    private Button ShieldsButton
    {
        get
        {
            if (_shieldsButton == null)
            {
                _shieldsButton = _shieldButtonGO.GetComponent<Button>();
            }
            return _shieldsButton;
        }
    }
    private Button _ammoPlusButton;
    private Button AmmoPlusButton
    {
        get
        {
            if (_ammoPlusButton == null)
            {
                _ammoPlusButton = _ammoPlusButtonGO.GetComponent<Button>();
            }
            return _ammoPlusButton;
        }
    }
    private Button _bfgButton;
    private Button BfgButton
    {
        get
        {
            if (_bfgButton == null)
            {
                _bfgButton = _bfgButtonGO.GetComponent<Button>();
            }
            return _bfgButton;
        }
    }
    private Button _freezeButton;
    private Button FreezeButton
    {
        get
        {
            if (_freezeButton == null)
            {
                _freezeButton = _freezeButtonGO.GetComponent<Button>();
            }
            return _freezeButton;
        }
    }
    private Button _speedButton;
    private Button SpeedButton
    {
        get
        {
            if (_speedButton == null)
            {
                _speedButton = _speedButtonGO.GetComponent<Button>();
            }
            return _speedButton;
        }
    }
    private Button _winButton;
    private Button WinButton
    {
        get
        {
            if (_winButton == null)
            {
                _winButton = _winButtonGO.GetComponent<Button>();
            }
            return _winButton;
        }
    }

    private void Start()
    {
        if (LevelController.GameSettings == null)
        {
            LevelController.GameSettings = GameSettings.LoadFromDisk();
        }
        var go = (GameObject)Resources.Load("Prefabs\\TurretSprite");
        var sr = go.GetComponent<SpriteRenderer>();
        _turretHoverSprite = sr.sprite;
        _ammoPlusButton = _ammoPlusButtonGO.GetComponent<Button>();
        _bfgButton = _bfgButtonGO.GetComponent<Button>();
        _freezeButton = _freezeButtonGO.GetComponent<Button>();
        _speedButton = _speedButtonGO.GetComponent<Button>();
        _winButton = _winButtonGO.GetComponent<Button>();
        SetEffectsVolume(LevelController.GameSettings.EffectsVolumeLevel);
        SetMusicVolume(LevelController.GameSettings.MusicVolumeLevel);
    }

    private void Awake()
    {
        IsGamePaused = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsGamePaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        IsGamePaused = false;
        _escMenu.SetActive(false);
        Time.timeScale = 1f;
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

    private void Pause()
    {
        IsGamePaused = true;
        _escMenu.SetActive(true);
        Time.timeScale = 0f;
    }

    internal void HandleEnergyChanged(object sender, EventArgs e)
    {
        var sea = (Stat.StatEventArgs)e;
        AbleTurretButton(sea.CurrentValue);
        AbleShieldButton(sea.CurrentValue);
        AbleFreezeButton(sea.CurrentValue);
        AbleAmmoPlusButton(sea.CurrentValue);
        AbleBfgButton(sea.CurrentValue);
        AbleSpeedButton(sea.CurrentValue);
        AbleWinButton(sea.CurrentValue);
    }
    
    private void AbleTurretButton(float currentEngergy)
    {
        TurretButton.interactable = currentEngergy >= TurretController.TurretCost;
    }

    private void AbleShieldButton(float currentEngergy)
    {
        ShieldsButton.interactable = (currentEngergy >= LevelController.Instance.GeneratorCost && !LevelController.Instance.HasShields);
    }
    private void AbleFreezeButton(float currentEngergy)
    {
        FreezeButton.interactable = (currentEngergy >= LevelController.Instance.GeneratorCost && !LevelController.Instance.HasFreeze);
    }
    private void AbleAmmoPlusButton(float currentEngergy)
    {
        AmmoPlusButton.interactable = (currentEngergy >= LevelController.Instance.GeneratorCost && !LevelController.Instance.HasAmmoPlus);
    }
    private void AbleBfgButton(float currentEngergy)
    {
        BfgButton.interactable = (currentEngergy >= LevelController.Instance.GeneratorCost && !LevelController.Instance.HasBfg);
    }
    private void AbleSpeedButton(float currentEngergy)
    {
        SpeedButton.interactable = (currentEngergy >= LevelController.Instance.GeneratorCost && !LevelController.Instance.HasSpeed);
    }
    private void AbleWinButton(float currentEngergy)
    {
        WinButton.interactable = (currentEngergy >= LevelController.Instance.GeneratorCost 
                                    && LevelController.Instance.HasShields
                                    && LevelController.Instance.HasAmmoPlus
                                    && LevelController.Instance.HasFreeze
                                    && LevelController.Instance.HasSpeed
                                    && LevelController.Instance.HasBfg
                                    && !LevelController.Instance.HasWin);
        //WinButton.interactable = true;  //debug only
    }

    public void TurretButtonClicked()
    {
        IsTurretClicked = true;
        if (_mouseBuild == null)
        {
            _mouseBuild = Instantiate(_mouseBuildPrefab, transform.position, Quaternion.identity);
            var mhScript = _mouseBuild.GetComponent<MouseHover>();
            mhScript.SetHoverIcon(_turretHoverSprite);
        }
    }

    public void ShieldsButtonClicked()
    {
        if (LevelController.Instance.TryBuildShieldGenerator())
        {
            FadeTextColor(_shieldButtonGO, 0.25f);
        }
    }

    private void FadeTextColor(GameObject uiGO, float alpha)
    {
        var uiText = uiGO.transform.Find("Text").GetComponent<Text>();
        uiText.color = new Color(uiText.color.r, uiText.color.g, uiText.color.b, alpha);
    }

    public void AmmoPlusButtonClicked()
    {
        if (LevelController.Instance.TryBuildAmmoPlusGenerator()) FadeTextColor(_ammoPlusButtonGO, 0.25f);
    }

    public void BFGButtonClicked()
    {
        if (LevelController.Instance.TryBuildBFGGenerator()) FadeTextColor(_bfgButtonGO, 0.25f);
    }

    public void FreezeButtonClicked()
    {
        if (LevelController.Instance.TryBuildFreezeGenerator()) FadeTextColor(_freezeButtonGO, 0.25f);
    }

    public void SpeedButtonClicked()
    {
        if (LevelController.Instance.TryBuildSpeedGenerator()) FadeTextColor(_speedButtonGO, 0.25f);
    }

    public void WinButtonClicked()
    {
        if (LevelController.Instance.TryBuildWinGenerator()) FadeTextColor(_winButtonGO, 0.25f);
    }

    public void MenuButtonClicked()
    {
        StartCoroutine(LoadSceneWithAmimation("MainMenu", "Exit"));
        Time.timeScale = 1f;
    }

    public void QuitButtonClicked()
    {
        LevelController.StopGame();
    }

    public IEnumerator LoadSceneWithAmimation(string scene, string animTrigger)
    {
        FadeAnimator.SetTrigger(animTrigger);
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(scene, LoadSceneMode.Single);
    }


    public void Reset()
    {
        IsTurretClicked = false;
        if (_mouseBuild != null)
        {
            Destroy(_mouseBuild);
            _mouseBuild = null;
        }
    }

    internal bool ClickInProgress()
    {
        //Add other click checks as ORs
        return IsTurretClicked || false;
    }

    public void DisplayGameOver(string message)
    {
        _gameOverPanel.SetActive(true);
        Text messageText = _gameOverPanel.GetComponentInChildren<Text>();
        messageText.text = message;
    }
}
