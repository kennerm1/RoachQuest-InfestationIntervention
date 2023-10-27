using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    public TextMeshProUGUI roachText;

    // instance
    public static GameUI instance;

    void Awake()
    {
        instance = this;
    }

    public void UpdateRoachText(int roach)
    {
        roachText.text = "<b>Roaches:</b> " + roach;
    }

}
