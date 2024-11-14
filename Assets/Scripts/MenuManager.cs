using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    GameObject MainMenu, LevelsMenu, GameplayScreen;
    [SerializeField]
    LevelData[] AllLevels;
    [Header("Level UI")]
    GameLevel[] gameLevel;
    bool datasaved;
    public int CurrentLevel;
    string LevelKey = "CurrentLevel";
    [SerializeField]
    AudioManager audioManager;
    private void Awake()
    {
        audioManager.Play("BackgroundMusic");
        CurrentLevel = PlayerPrefs.GetInt(LevelKey, 0);
        LoadandApplyLevel();
    }


    public void LoadandApplyLevel()
    {
        if (PlayerPrefs.HasKey("LvlDataSaved"))
            datasaved = true;
        for (int i = 0; i < AllLevels.Length; i++)
        {
            if (!datasaved)
            {
                PlayerPrefs.SetInt("LvlDataSaved", 1);
                AllLevels[i].SaveLevelData();
            }
            else
                AllLevels[i].LoadLevelData();
        }
    }

    public void ShowLevelsScreen()
    {
        MainMenu.SetActive(false);
        LevelsMenu.SetActive(true);
    }

    public void ShowMainMenu()
    {
        MainMenu.SetActive(true);
        GameplayScreen.SetActive(false);
        LevelsMenu.SetActive(false);
    }
    public void LoadGameplayScreen()
    {
        GameplayScreen.SetActive(true);
        LevelsMenu.SetActive(false);
    }
}
