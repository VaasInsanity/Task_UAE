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
    public TextMeshProUGUI scoreText;
    public GameObject FinishScreen;
    public GameObject FailedScreen;
    [Space(20)]
    [Header("Score Data")]
    private int score = 0;
    [SerializeField] private GameObject[] starImages;
    private int maxScore, StarCount;
    [Header("Timer Settings")]
    [SerializeField] public float timeLimit = 30f;
    [SerializeField] private TextMeshProUGUI timerText;
    private float remainingTime;
    public bool gameActive;


    private void Awake()
    {
        LoadCardSprites();
        remainingTime = timeLimit;
    }

    public void StartGame()
    {
        InitializeCards();
        AddCardListeners();
        SetupGameCards();
        ShuffleCards(gameSprites);
        totalMatchesRequired = gameSprites.Count / 2;
        maxScore = totalMatchesRequired * 10;
        gameActive = true;
        remainingTime = timeLimit; // Set the initial timer value from the current level
        UpdateTimerUI();
        UpdateScoreUI();
    }
    private void Update()
    {
        if (gameActive)
        {
            UpdateTimer();
        }
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
            IncreaseScore();
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
            StartCoroutine(ShowFinishScreen());
            gameActive = false;
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
    private void IncreaseScore()
    {
        score += 10; // Increment score by 10 (or any other desired value)
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score.ToString();
        if (TargetGuess != null)
            TargetGuess.text = totalMatchesRequired.ToString();
    }
    private void AssignStarsBasedOnScore()
    {
        float scorePercentage = (float)score / maxScore;
        int starCount = 0;

        if (scorePercentage >= 0.8f) // 80% and above
            starCount = 3;
        else if (scorePercentage >= 0.5f) // 50-79%
            starCount = 2;
        else // Below 50%
            starCount = 1;
        Debug.Log("Score Percentage " + scorePercentage.ToString());
        StarCount = starCount;
    }
    IEnumerator ShowFinishScreen()
    {
        FinishScreen.SetActive(true);
        AssignStarsBasedOnScore();
        yield return new WaitForSeconds(0.3f);
        for (int i = 0; i < starImages.Length; i++)
        {
            if (i < StarCount)
                starImages[i].SetActive(true);
            else
                starImages[i].SetActive(false);
            yield return new WaitForSeconds(0.3f);
        }
    }
    private void UpdateTimer()
    {
        remainingTime -= Time.deltaTime;
        UpdateTimerUI();

        if (remainingTime <= 0)
        {
            remainingTime = 0;
            EndGame(false); // End the game with fail screen
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
            timerText.text = "Time: " + Mathf.CeilToInt(remainingTime).ToString();
    }

    private void EndGame(bool win)
    {
        gameActive = false; // Stop the timer and game actions

        if (win)
        {
            if (FinishScreen != null)
                FinishScreen.SetActive(true);
        }
        else
        {
            if (FailedScreen != null)
                FailedScreen.SetActive(true);
        }
    }

}
