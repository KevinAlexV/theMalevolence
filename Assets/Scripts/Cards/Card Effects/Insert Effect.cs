using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** <summary>Special card effect: Inserts a card into the target's deck.</summary> */
[System.Serializable]
public class InsertEffect : CardEffect {
    /** <summary>The card to insert into the target's deck</summary> */
    [SerializeField] private Card toInsert;

    /** <summary>Applies the effect onto the relevant targets</summary> */
    public override IEnumerator ApplyEffect () {
        //If target is "None", that means put the card in the "toInsert"'s chosen character
        if (target == Enums.Target.None) {
            targets.Clear();
            Character owner;
            GameManager.manager.characters.TryGetValue(toInsert.Character, out owner);
            targets.Add(owner);
        }
        //Apply effect on each target
        foreach (Character c in targets) {
            //Pull a deck
            Deck deck;
            GameManager.manager.decks.TryGetValue(c.data.characterType, out deck);
            //Change the card's owner to the selected character and add to their deck
            toInsert.ChangeOwner(c.data.characterType);
            deck.AddCard(toInsert);
            //Inform the player what just happened
            yield return CombatUIManager.Instance.DisplayMessage($"Added {toInsert.Name} to {c.data.name}'s deck");
        }
        yield return new WaitForSeconds(1f);
    }
}
