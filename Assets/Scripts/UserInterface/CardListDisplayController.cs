using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ScrollRect))]
[RequireComponent(typeof(Mask))]
public class CardListDisplayController : MonoBehaviour
{
    private ScrollRect scrollRect;
    public List<CardDisplayController> DisplayedCards = new List<CardDisplayController>();

    public float cardsPerRow = 0;
    private float cardHeight = 90;
    private float cardWidth = 60;

    public CharacterData tempCharacter;

    // Start is called before the first frame update
    void Start()
    {
        scrollRect = GetComponent<ScrollRect>();
    }

    public void DisplayCards() {
        for(int i = 0; i < DisplayedCards.Count; i++){
            PositionCard(DisplayedCards[i], i);
        }
    }

    private void PositionCard(CardDisplayController card, int index){
        RectTransform cardRectTransform = card.GetComponent<RectTransform>();
        var cardHolder = scrollRect.content;
        cardRectTransform.SetParent(cardHolder.transform);
        var offset = (index % cardsPerRow);
        cardRectTransform.offsetMin = new Vector2(offset * cardWidth * 2, cardHolder.sizeDelta.y + Mathf.Floor(index / cardsPerRow) * cardHeight);
        cardRectTransform.offsetMax = new Vector2(0, 0);
        cardRectTransform.sizeDelta = new Vector2(cardWidth, cardHeight); 
    }

    //Adds a card to the list and updates the display
    public void AddCard(CardDisplayController card) {
        DisplayedCards.Add(card);
        card.GetComponent<Draggable>().followMouse = false;
        DisplayCards();
    }

    //Removes a card from the list and updates the display
    public void RemoveCard(CardDisplayController card){
        DisplayedCards.Remove(card);
        Destroy(card.gameObject);
        DisplayCards();
    }

    void Update(){
        if(cardsPerRow == 0){
            cardsPerRow = scrollRect.content.rect.width / (cardWidth);
            foreach(Card c in tempCharacter.Deck.CardList){
                AddCard(CardDisplayController.CreateCard(c));
            }
        }
    }
}
