using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLevel : MonoBehaviour
{
    [SerializeField]
    GameObject Selector, LockIcon, AvailableEffect;
    public bool isAvailable;
    public bool AlreadyPlayed;
    public int LevelIndex;
    public Sprite PlayedSprite;
    public Image LevelImage;
    [SerializeField]
    LevelData myLevelData;
    public int CurrentLevel;
    string LevelKey = "CurrentLevel";


    public void OnEnable()
    {
        CurrentLevel = PlayerPrefs.GetInt(LevelKey);
        myLevelData.LoadLevelData();

        if (CurrentLevel == myLevelData.levelIndex)
            LevelAvailable();
        else if (CurrentLevel > myLevelData.levelIndex)
            LevelPlayed();
        else
            LevelLocked();
    }

    public void LevelLocked()
    {
        GetComponent<Button>().interactable = false;
        LockIcon.SetActive(true);
    }

    public void LevelAvailable()
    {
        GetComponent<Button>().interactable = true;
        isAvailable = true;
        LockIcon.SetActive(false);
        AvailableEffect.SetActive(true);
    }
    public void LevelPlayed()
    {
        GetComponent<Button>().interactable = true;
        isAvailable = true;
        LevelImage.sprite = PlayedSprite;
        LockIcon.SetActive(false);
        AvailableEffect.SetActive(false);
    }
}
