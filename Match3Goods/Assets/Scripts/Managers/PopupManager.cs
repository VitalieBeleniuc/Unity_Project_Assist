using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    public GameObject winPopup;
    public GameObject lossPopup;
    public GameObject gameoverPopup;

    void Start()
    {
        winPopup.SetActive(false);
        lossPopup.SetActive(false);
        gameoverPopup.SetActive(false);
    }

    public void ShowWinPopup()
    {
        winPopup.SetActive(true);
    }
    public void HideWinPopup()
    {
        winPopup.SetActive(false);
    }
    public void ShowLossPopup()
    {
        lossPopup.SetActive(true);
    }
    public void HideLossPopup()
    {
        lossPopup.SetActive(false);
    }
    public void ShowOverPopup()
    {
        gameoverPopup.SetActive(true);
    }
    public void HideOverPopup()
    {
        gameoverPopup.SetActive(false);
    }
}
