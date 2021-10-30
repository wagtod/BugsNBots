using System;
using UnityEngine;

public class ShieldController : MonoBehaviour {

    private int _shieldPoints;
    public int ShieldPoints
    {
        get { return _shieldPoints; }
        set {
            _shieldPoints = value;
            _shieldPoints = Mathf.Clamp(_shieldPoints, 0, _maxShieldPoints);
            SetBubbleAlpha();
        }
    }

    public int _shieldRegenRate;
    public int _maxShieldPoints;
    private float _shieldRegenTimer;
    private SpriteRenderer _shieldBubbleSR;


    void Start ()
    {
	}

    private void Awake()
    {
        _shieldBubbleSR = GetComponent<SpriteRenderer>();
        Init(0, 0);
    }

    void Update()
    {
        if (_shieldRegenRate > 0)
        {
            if (ShieldPoints < _maxShieldPoints)
            {
                _shieldRegenTimer += Time.deltaTime;
                if (_shieldRegenTimer >= 1f)
                {
                    ShieldPoints += _shieldRegenRate;
                    _shieldRegenTimer -= 1f;
                }
            }
            else
            {
                _shieldRegenTimer = 0f;
            }
        }
    }

    private void SetBubbleAlpha()
    {
        //Opacity of shield bubble is based on percentage of full
        float opacity = 0f;
        if (_maxShieldPoints > 0)
        {
            opacity = ShieldPoints / (float)_maxShieldPoints;
        }
        _shieldBubbleSR.color = new Color(1f, 1f, 1f, opacity); 
    }

    public void Init(int maxShieldPoints, int regenPointPerSecond)
    {
        if (maxShieldPoints > 0)
        {
            switch (LevelController.GameSettings.LastDifficulty)
            {
                case GameSettings.DifficultyLevels.Normal:
                    maxShieldPoints += 6;
                    break;
                case GameSettings.DifficultyLevels.Hard:
                    maxShieldPoints += 12;
                    break;
            }
        }

        _maxShieldPoints = maxShieldPoints;
        _shieldRegenRate = regenPointPerSecond;
        _shieldRegenTimer = 0;
        ShieldPoints = 0;
    }

}
