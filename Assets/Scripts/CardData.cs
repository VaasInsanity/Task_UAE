using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardData : MonoBehaviour
{
    public Image CharacterImage;
    public GameObject ImageToReplace;
    public GameObject MatchedEffects;

    public void DisableDefaultImage()
    {
        ImageToReplace.SetActive(false);
        CharacterImage.gameObject.SetActive(true);
    }
    public void CardsMatched()
    {
        MatchedEffects.SetActive(true);
    }
    public void ResetUnmatched()
    {
        ImageToReplace.SetActive(true);
        CharacterImage.gameObject.SetActive(false);
    }
}
