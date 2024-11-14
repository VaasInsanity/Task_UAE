using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameplayController : MonoBehaviour
{

    [Header("Card Settings")]
    [SerializeField] private Sprite cardBackground;
    [SerializeField] private Sprite[] cardSprites;
    private List<Sprite> gameSprites = new List<Sprite>();

    [Header("Game State Tracking")]
    private List<Button> cards = new List<Button>();
    private bool isFirstGuess, isSecondGuess;
    private int totalGuesses, correctGuesses, totalMatchesRequired;
    private int firstGuessIndex, secondGuessIndex;
    private string firstGuessCardName, secondGuessCardName;


    private void Awake()
    {
        LoadCardSprites();
    }

    private void Start()
    {
        InitializeCards();
        AddCardListeners();
        SetupGameCards();
        ShuffleCards(gameSprites);
        totalMatchesRequired = gameSprites.Count / 2;
    }

    private void LoadCardSprites()
    {
        // Path to be replaced when actual UI elements added
        cardSprites = Resources.LoadAll<Sprite>("Sprites/PlaceHolders");
    }

    private void InitializeCards()
    {
        GameObject[] cardObjects = GameObject.FindGameObjectsWithTag("FlipCard");
        foreach (GameObject obj in cardObjects)
        {
            Button button = obj.GetComponent<Button>();
            button.image.sprite = cardBackground;
            cards.Add(button);
        }
    }

    private void SetupGameCards()
    {
        int cardCount = cards.Count;
        int spriteIndex = 0;

        for (int i = 0; i < cardCount; i++)
        {
            if (spriteIndex == cardCount / 2)
                spriteIndex = 0;
            gameSprites.Add(cardSprites[spriteIndex]);
            spriteIndex++;
        }
    }

    private void AddCardListeners()
    {
        foreach (Button card in cards)
            card.onClick.AddListener(HandleCardClick);
    }

    private void HandleCardClick()
    {
        int cardIndex = int.Parse(EventSystem.current.currentSelectedGameObject.name);

        if (!isFirstGuess)
        {
            FirstGuess(cardIndex);
        }
        else if (!isSecondGuess)
        {
            SecondGuess(cardIndex);
            totalGuesses++;
            StartCoroutine(CheckMatch());
        }
    }

    private void FirstGuess(int index)
    {
        isFirstGuess = true;
        firstGuessIndex = index;
        firstGuessCardName = gameSprites[index].name;
        cards[firstGuessIndex].image.sprite = gameSprites[firstGuessIndex];
    }

    private void SecondGuess(int index)
    {
        isSecondGuess = true;
        secondGuessIndex = index;
        secondGuessCardName = gameSprites[index].name;
        cards[secondGuessIndex].image.sprite = gameSprites[secondGuessIndex];
    }

    private IEnumerator CheckMatch()
    {
        yield return new WaitForSeconds(1f);

        if (firstGuessCardName == secondGuessCardName)
        {
            DisableMatchedCards();
            correctGuesses++;
            CheckGameCompletion();
        }
        else
        {
            ResetUnmatchedCards();
        }

        yield return new WaitForSeconds(0.5f);
        isFirstGuess = isSecondGuess = false;
    }

    private void DisableMatchedCards()
    {
        cards[firstGuessIndex].interactable = false;
        cards[secondGuessIndex].interactable = false;
    }

    private void ResetUnmatchedCards()
    {
        cards[firstGuessIndex].image.sprite = cardBackground;
        cards[secondGuessIndex].image.sprite = cardBackground;
    }

    private void CheckGameCompletion()
    {
        if (correctGuesses == totalMatchesRequired)
        {
            //finish game menu to be added here
        }
    }

    private void ShuffleCards(List<Sprite> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            Sprite temp = list[i];
            int randomIndex = Random.Range(i, list.Count);
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }
}
