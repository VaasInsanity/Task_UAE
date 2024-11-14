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
    public GameObject PauseScreen;
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
    public int CurrentLevel;
    string LevelKey = "CurrentLevel";
    public LoadCards LevelLoader;
    public LevelData[] AllLevels;
    public int LoadedLevel;
    [SerializeField]
    AudioManager audioManager;
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
        totalMatchesRequired = gameSprites.Count;
        maxScore = totalMatchesRequired * 10;
        gameActive = true;
        remainingTime = timeLimit;
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
        audioManager.Play("Click");
        if (!isFirstGuess)
        {
            FirstGuess(cardIndex);
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
        cards[firstGuessIndex].GetComponent<Animation>().Play("CardEntry");
        cards[firstGuessIndex].GetComponent<CardData>().DisableDefaultImage();
        cards[firstGuessIndex].GetComponent<CardData>().CharacterImage.sprite = gameSprites[firstGuessIndex];
    }

    private void SecondGuess(int index)
    {
        isSecondGuess = true;
        secondGuessIndex = index;
        secondGuessCardName = gameSprites[index].name;
        cards[secondGuessIndex].GetComponent<Animation>().Play("CardEntry");
        cards[secondGuessIndex].GetComponent<CardData>().DisableDefaultImage();
        cards[secondGuessIndex].GetComponent<CardData>().CharacterImage.sprite = gameSprites[secondGuessIndex];
    }

    private IEnumerator CheckMatch()
    {
        

        if (firstGuessCardName == secondGuessCardName)
        {
            yield return new WaitForSeconds(0.5f);
            DisableMatchedCards();
            correctGuesses++;
            IncreaseScore();
            CheckGameCompletion();
            audioManager.Play("CorrectMatched");
        }
        else
        {
            yield return new WaitForSeconds(0.8f);
            ResetUnmatchedCards();
            audioManager.Play("WrongMatched");
        }

        yield return new WaitForSeconds(0.4f);
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
        cards[firstGuessIndex].GetComponent<CardData>().ResetUnmatched();
        cards[secondGuessIndex].GetComponent<CardData>().ResetUnmatched();
    }

    private void CheckGameCompletion()
    {
        if (correctGuesses == totalMatchesRequired / 2)
        {
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
        score += 10;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = "Score: " + score.ToString();
        if (TargetGuess != null)
            TargetGuess.text = totalMatchesRequired.ToString();
        if (CurrentGuess != null)
            CurrentGuess.text = totalGuesses.ToString();
    }
    private void AssignStarsBasedOnScore()
    {
        float efficiencyRatio = (float)totalMatchesRequired / totalGuesses;
        foreach (GameObject star in starImages)
        {
            star.SetActive(false);
        }

        if (efficiencyRatio >= 0.9f)
        {
            StarCount = 3;
        }
        else if (efficiencyRatio >= 0.7f)
        {
            StarCount = 2;
        }
        else
        {
            StarCount = 1;
        }
    }
    IEnumerator ShowFinishScreen()
    {
        FinishScreen.SetActive(true);
        audioManager.Play("Finish");
        AssignStarsBasedOnScore();
        ResetStarUI();
        yield return new WaitForSeconds(0.3f);
        for (int i = 0; i < starImages.Length; i++)
        {
            if (i < StarCount)
                starImages[i].SetActive(true);
            else
                starImages[i].SetActive(false);
            yield return new WaitForSeconds(0.3f);
        }
        
        CurrentLevel = PlayerPrefs.GetInt(LevelKey);
        if(LoadedLevel < CurrentLevel)
        {
            CurrentLevel = LoadedLevel;
            CurrentLevel++;
        }
        else
        {
            AllLevels[CurrentLevel].locked = false;
            AllLevels[CurrentLevel].SaveLevelData();
            if (CurrentLevel < 7) //Currently 7 levels added only
                CurrentLevel++;
            PlayerPrefs.SetInt(LevelKey, CurrentLevel);
        }
    }
    private void UpdateTimer()
    {
        remainingTime -= Time.deltaTime;
        UpdateTimerUI();

        if (remainingTime <= 0)
        {
            remainingTime = 0;
            EndGame(false);
        }
    }

    private void UpdateTimerUI()
    {
        if (timerText != null)
            timerText.text = "Time: " + Mathf.CeilToInt(remainingTime).ToString();
    }

    private void EndGame(bool win)
    {
        gameActive = false;

        if (win)
        {
            if (FinishScreen != null)
                FinishScreen.SetActive(true);
        }
        else
        {
            if (FailedScreen != null)
                FailedScreen.SetActive(true);
            audioManager.Play("WrongMatched");
        }
    }

    public void ResetGameplay()
    {
        foreach (var card in cards)
        {
            Destroy(card.gameObject);
        }

        cards.Clear();
        gameSprites.Clear();

        score = 0;
        correctGuesses = 0;
        totalGuesses = 0;
        remainingTime = timeLimit;
        isFirstGuess = isSecondGuess = false;
        firstGuessIndex = 0;
        secondGuessIndex = 0;
        UpdateScoreUI();
        UpdateTimerUI();
        

        foreach (var card in cards)
        {
            Destroy(card.gameObject);
        }

        cards.Clear();
        
    }
    private void ResetStarUI()
    {
        foreach (GameObject star in starImages)
        {
            star.SetActive(false);
        }
    }
    public void PlayNextLevel()
    {
        LevelLoader.LoadGameData(CurrentLevel);
        ResetGameplay();
        FinishScreen.SetActive(false);
        FailedScreen.SetActive(false);
        PauseScreen.SetActive(false);
    }
    public void RetryCurrentLevel()
    {
        CurrentLevel = PlayerPrefs.GetInt(LevelKey);
        LevelLoader.LoadGameData(CurrentLevel);
        ResetGameplay();
        FinishScreen.SetActive(false);
        FailedScreen.SetActive(false);
        PauseScreen.SetActive(false);
    }
    public void BackToHome()
    {
        ResetGameplay();
        FinishScreen.SetActive(false);
        FailedScreen.SetActive(false);
    }
    public void PauseGame()
    {
        PauseScreen.SetActive(true);
        audioManager.Play("Click");
        Time.timeScale = 0;
    }
    public void ResumeGame()
    {
        PauseScreen.SetActive(false);
        Time.timeScale = 1;
    }
    public void RestartGame()
    {
        PauseScreen.SetActive(false);
        Time.timeScale = 1;
        gameActive = false;
        ResetGameplay();
        RetryCurrentLevel();
    }
    public void GiveUp()
    {
        PauseScreen.SetActive(false);
        gameActive = false;
        Time.timeScale = 1;
        ResetGameplay();
    }
}
