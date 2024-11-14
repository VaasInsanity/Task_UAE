using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadCards : MonoBehaviour
{
    [SerializeField]
    private Transform gridPanel;
    [SerializeField]
    private GridLayoutGroup gridLayout;
    [SerializeField]
    private GameObject card;

    public int cardAmount = 8;
    [SerializeField]
    GameplayController gameplayController;
    [SerializeField]
    MenuManager menuManager;
    public LevelData[] AllLevels;
    public int LevelIndex;
    [SerializeField]
    AudioManager audioManager;
    private void Awake()
    {
        if(gridLayout == null)
            gridLayout = gridPanel.GetComponent<GridLayoutGroup>();
    }

    public void LoadGameData(int levelIndex)
    {
        LevelIndex = levelIndex;
        gridLayout.constraint = AllLevels[LevelIndex].constraint;
        gridLayout.constraintCount = AllLevels[LevelIndex].constraintCount;
        gridLayout.cellSize = AllLevels[LevelIndex].cellSize;
        cardAmount = AllLevels[LevelIndex].cardAmount;
        gameplayController.timeLimit = AllLevels[LevelIndex].timeLimit;
        gameplayController.LoadedLevel = levelIndex;
        LoadLevelCards();
        menuManager.LoadGameplayScreen();
    }
    
    private void LoadLevelCards()
    {
        StartCoroutine(InstansiateCards());
    }
    IEnumerator InstansiateCards()
    {
        for (int i = 0; i < cardAmount; i++)
        {
            GameObject flipCard = Instantiate(card);
            flipCard.name = "" + i;
            flipCard.transform.SetParent(gridPanel);
            audioManager.Play("Card");
            yield return new WaitForSeconds(0.2f);
            flipCard.GetComponent<Button>().interactable = true;
        }
        gameplayController.StartGame();
    }
}
