using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CardDraftPool : ScriptableObject
{
    public List<Card> cards;

    public List<Card> GetShuffled(){
        List<Card> tempList = new List<Card>(cards);
        List<Card> shuffledList = new List<Card>();
        int drawnCard;
        while (tempList.Count > 0) {
            drawnCard = Random.Range(0, tempList.Count);
            shuffledList.Add(tempList[drawnCard]);
            tempList.RemoveAt(drawnCard);
        }
        return shuffledList;
    }
}
