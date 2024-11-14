using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadCards : MonoBehaviour
{
    [SerializeField]
    private Transform gridPanel;

    [SerializeField]
    private GameObject card;

    public int cardAmount = 8;

    private void Awake()
    {
        for(int i = 0; i < cardAmount; i++)
        {
            GameObject flipCard = Instantiate(card);
            flipCard.name = "" + i;
            flipCard.transform.SetParent(gridPanel);
        }
    }
}
