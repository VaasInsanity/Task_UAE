using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadCards : MonoBehaviour
{
    [SerializeField]
    private Transform gridPanel;

    [SerializeField]
    private GameObject card;

    public int cardAmount = 8;
    [SerializeField]
    GameplayController gameplayController;

    private void Awake()
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
            yield return new WaitForSeconds(0.2f);
            flipCard.GetComponent<Button>().interactable = true;
        }
        gameplayController.StartGame();
    }
}
