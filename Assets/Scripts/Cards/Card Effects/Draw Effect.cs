using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** <summary>Special card effect: Allows the character to draw an additional card.</summary> */
[System.Serializable]
public class DrawEffect : CardEffect {
    
    /** <summary>The number of cards to draw.</summary> */
    [SerializeField] private int cardsToDraw;
    /** <summary>Draw cards from the discard pile.</summary> */
    [SerializeField] private bool fromDiscard;
    /** <summary>Discard cards in hand to discard pile. Overrides fromDiscard.</summary> */
    [SerializeField] private bool toDiscard;

    private int cards;

    /** <summary>Applies the effect onto the relevant targets</summary> */
    public override IEnumerator ApplyEffect () {
        ApplyModification();

        //Sends a number of numbers from the player's hand into the discard pile
        if (toDiscard) {
            for (int i = 0; i < cards; i++) {
                //Tell game manager to discard a card
            }
        }
        //Draws a number of cards from the discard pile into the player's hand
        else if (fromDiscard) {
            Deck discards = new Deck();
            Deck charDeck;
            GameManager.manager.decks.TryGetValue(Enums.Character.Goth, out charDeck);
            if (charDeck.DiscardList.Count > 0)
                discards.CardList.AddRange(charDeck.DiscardList);
            GameManager.manager.decks.TryGetValue(Enums.Character.Jock, out charDeck);
            if (charDeck.DiscardList.Count > 0)
                discards.CardList.AddRange(charDeck.DiscardList);
            GameManager.manager.decks.TryGetValue(Enums.Character.Nerd, out charDeck);
            if (charDeck.DiscardList.Count > 0)
                discards.CardList.AddRange(charDeck.DiscardList);
            GameManager.manager.decks.TryGetValue(Enums.Character.Popular, out charDeck);
            if (charDeck.DiscardList.Count > 0)
                discards.CardList.AddRange(charDeck.DiscardList);

            discards.Shuffle();
            for (int i = 0; i < cards; i++) {

                Card drawn = discards.Draw();
                if (drawn != null)
                { 
                    GameManager.manager.PlaceCardInHand(drawn);
                    GameManager.manager.decks.TryGetValue(drawn.Character, out charDeck);
                    charDeck.DiscardList.Remove(drawn);
                }
            }
        } 
        //Draw a number of cards from your decks
        else {
            for (int i = 0; i < cards; i++) {
                //Tell the game manager to enter the draw phase, then return to resolve phase
                yield return GameManager.manager.ExecuteDrawPhase();
                GameManager.manager.phase = Enums.GameplayPhase.Resolve;
            }
        }
        yield return new WaitForSeconds(1f);
    }

    /** <summary>Takes the modification from the MODIFY effect and increases "cardsToDraw" value</summary> */
    public override void ApplyModification () {
        cards = cardsToDraw;
        if (modifyingValue != 0) {
            switch (modification) {
                case Enums.Modifier.Add:
                    cards += modifyingValue;
                    break;
                case Enums.Modifier.Subtract:
                    cards -= modifyingValue;
                    break;
                case Enums.Modifier.Multiply:
                    cards *= modifyingValue;
                    break;
                case Enums.Modifier.Divide:
                    cards /= modifyingValue;
                    break;
            }
        }
    }
}
