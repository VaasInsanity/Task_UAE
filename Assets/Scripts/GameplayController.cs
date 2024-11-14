using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
public class GameplayController : MonoBehaviour
{

    [Header("Card Settings")]
    [SerializeField] private Sprite cardBackground;
    [SerializeField] private Sprite[] cardSprites;
    private List<Sprite> gameSprites = new List<Sprite>();
    [Space(20)]
    [Header("Game State Tracking")]
    private List<Button> cards = new List<Button>();
    private bool isFirstGuess, isSecondGuess;
    private int totalGuesses, correctGuesses, totalMatchesRequired;
    private int firstGuessIndex, secondGuessIndex;
    private string firstGuessCardName, secondGuessCardName;
    [Space(20)]
    [Header("UI Data")]
    public TextMeshProUGUI TargetGuess;
    public TextMeshProUGUI CurrentGuess;



    private void Awake()
    {
        LoadCardSprites();
    }

    public void StartGame()
    {
        InitializeCards();
        AddCardListeners();
        SetupGameCards();
        ShuffleCards(gameSprites);
        totalMatchesRequired = gameSprites.Count / 2;
        TargetGuess.text = totalMatchesRequired.ToString();
    }

    private void LoadCardSprites()
    {
        // Path to be replaced when actual UI elements added
        cardSprites = Resources.LoadAll<Sprite>("Sprites/Characters");
    }

    private void InitializeCards()
    {
        GameObject[] cardObjects = GameObject.FindGameObjectsWithTag("FlipCard");
        foreach (GameObject obj in cardObjects)
        {
            Button button = obj.GetComponent<Button>();
            //button.image.sprite = cardBackground;
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
        Debug.Log("FirstF");
        if (!isFirstGuess)
        {
            FirstGuess(cardIndex);
            Debug.Log("FirstF");
        }
        else if (!isSecondGuess)
        {
            SecondGuess(cardIndex);
            totalGuesses++;
            CurrentGuess.text = totalGuesses.ToString();
            StartCoroutine(CheckMatch());
        }
    }

    private void FirstGuess(int index)
    {
        isFirstGuess = true;
        firstGuessIndex = index;
        firstGuessCardName = gameSprites[index].name;
        //cards[firstGuessIndex].image.sprite = gameSprites[firstGuessIndex];
        cards[firstGuessIndex].GetComponent<Animation>().Play("CardEntry");
        cards[firstGuessIndex].GetComponent<CardData>().DisableDefaultImage(); //Changed
        cards[firstGuessIndex].GetComponent<CardData>().CharacterImage.sprite = gameSprites[firstGuessIndex]; //Changed
        Debug.Log("First");
    }

    private void SecondGuess(int index)
    {
        isSecondGuess = true;
        secondGuessIndex = index;
        secondGuessCardName = gameSprites[index].name;
        //cards[secondGuessIndex].image.sprite = gameSprites[secondGuessIndex];
        cards[secondGuessIndex].GetComponent<Animation>().Play("CardEntry");
        cards[secondGuessIndex].GetComponent<CardData>().DisableDefaultImage(); //Changed
        cards[secondGuessIndex].GetComponent<CardData>().CharacterImage.sprite = gameSprites[secondGuessIndex]; //Changed
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
        cards[firstGuessIndex].GetComponent<CardData>().CardsMatched();
        cards[secondGuessIndex].interactable = false;
        cards[secondGuessIndex].GetComponent<CardData>().CardsMatched();
    }

    private void ResetUnmatchedCards()
    {
        //cards[firstGuessIndex].image.sprite = cardBackground;
        //cards[secondGuessIndex].image.sprite = cardBackground;
        cards[firstGuessIndex].GetComponent<CardData>().ResetUnmatched();
        cards[secondGuessIndex].GetComponent<CardData>().ResetUnmatched();
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
