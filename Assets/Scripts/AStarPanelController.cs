using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AStarPanelController : MonoBehaviour
{
    public Text GCostText;
    public int GCost { get; set; }
    public Text HCostText;
    public int HCost { get; set; }
    public Text FCostText;
    //start here tod: hide the canvas until it is used.
    void Start()
    {
        GCost = 0;
        HCost = 0;
    }

    void Update()
    {
        GCostText.text = $"G:{GCost}";
        HCostText.text = $"H:{HCost}";
        FCostText.text = $"F:{FCost()}";
    }

    private int FCost()
    {
        return GCost + HCost;
    }
}
