using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** <summary>Special card effect: Reshuffles a number of cards back into the deck.</summary> */
[System.Serializable]
public class ReshuffleEffect : CardEffect {

    public override IEnumerator ApplyEffect () {
        foreach (Character c in targets) {
            Enums.Character charType = c.data.characterType;
            Deck deck;
            GameManager.manager.decks.TryGetValue(charType, out deck);
            //Get all cards from discard with same chartype
            deck.Reshuffle();
            GameManager.manager.updateDiscardPile(charType);
        }
        yield return null;
    }
}
