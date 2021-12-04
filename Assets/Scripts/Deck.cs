using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Deck {
    [SerializeField] private List<Card> cardList = new List<Card>();

    public List<Card> CardList { get { return cardList; } }
    
    [SerializeField] private List<Card> discardList = new List<Card>();

    public List<Card> DiscardList { get { return discardList; } }

    public Deck () { }

    public Deck (List<Card> cards) {
        cardList = new List<Card>(cards);
    }

    public Card Draw() {
        if(cardList.Count == 0){
            Reshuffle();
        }
        Card ret = cardList[0];
        cardList.RemoveAt(0);
        if (ret != null)
            return ret;
        else
            return null;
    }

    public void Shuffle() {
        List<Card> tempList = new List<Card>(cardList);
        cardList.Clear();
        int drawnCard;
        while (tempList.Count > 0) {
            drawnCard = Random.Range(0, tempList.Count);
            cardList.Add(tempList[drawnCard]);
            tempList.RemoveAt(drawnCard);
        }

        if(AudioManager.audioMgr != null) AudioManager.audioMgr.PlayUISFX("Shuffle");
    }

    //Shuffles the discard pile into the draw pile
    public void Reshuffle(){
        cardList.AddRange(discardList);
        Shuffle();
    }

    public void AddCard(Card card) {

        if (cardList.Count >= 1)
        {
            card.Color = cardList[0].Color;

        }

        cardList.Add(card);
        Shuffle();
    }

    public void AddDeck(Deck deck) {
        foreach (Card c in deck.CardList)
            cardList.Add(c);
        Shuffle();
    }
}
