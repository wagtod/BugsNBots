using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class Stat
{
    public event EventHandler StatCurrentValueChanged;

    public class StatEventArgs : EventArgs
    {
        public float CurrentValue;
    }

    /// <summary>
    /// A reference to the bar that this stat is controlling
    /// </summary>
    [SerializeField]
    private BarScript _bar;

    /// <summary>
    /// The max value of the stat
    /// </summary>
    [SerializeField]
    private float _maxVal;

    /// <summary>
    /// The current value of the stat
    /// </summary>
    [SerializeField]
    private float _currentVal;

    /// <summary>
    /// A Property for accessing and setting the current value
    /// </summary>
    public float CurrentValue
    {
        get
        {
            return _currentVal;
        }
        set
        {
            //Clamps the current value between 0 and max
            this._currentVal = Mathf.Clamp(value, 0, MaxVal);

            //Updates the bar
            if (Bar != null)        //Null means no bar in use.
            {
                Bar.Value = _currentVal;
            }
            EventHandler eh = StatCurrentValueChanged;
            if (eh != null)
            {
                eh(this, new StatEventArgs() { CurrentValue = _currentVal });
            }
        }
    }

    /// <summary>
    /// A proprty for accessing the max value
    /// </summary>
    public float MaxVal
    {
        get
        {
            return _maxVal;
        }
        set
        {
            //Updates the bar's max value
            if (Bar != null)    //If null, no bar is in use.
            {
                Bar.MaxValue = value;
            }

            //Sets the max value
            this._maxVal = value;
        }
    }

    public BarScript Bar
    {
        get
        {
            return _bar;
        }
    }

    /// <summary>
    /// Initializes the stat
    /// This function needs to be called in awake
    /// </summary>
    public void Initialize()
    {
        //Updates the bar
        this.MaxVal = _maxVal;
        this.CurrentValue = _currentVal;
    }
}

