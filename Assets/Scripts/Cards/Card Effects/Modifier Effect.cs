using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** <summary>Special card effect: Changes another effects value based on a set of factors.</summary> */
[System.Serializable]
public class ModifierEffect : CardEffect {

    /** <summary>The desired effect on a card effect's value.</summary> */
    [SerializeField] private Enums.Modifier modifierEffect;
    /** <summary>The desired factor to influence a card effect's value.</summary> */
    [SerializeField] private Enums.ModifierFactors modifierFactor;
    /** <summary>The magnitude of the modifier based on the factor.</summary> */
    [SerializeField] private int modifierPerFactor;
    /** <summary>The required value of a factor to produce the desired effect.</summary> */
    [SerializeField] private int perFactorValue;
    /** <summary>The index number of the card effect to modify.</summary> */
    [SerializeField] private int effectIndex;
    
    public override IEnumerator ApplyEffect () {
        int value = 0;
        Character target;

        switch (modifierFactor)
        {
            case Enums.ModifierFactors.Cards_Played:
                int cardsPlayed = 0;
                foreach (Character c in GameManager.manager.party)
                    if (c.Action == Enums.Action.Card)
                        cardsPlayed++;
                value = (cardsPlayed / perFactorValue) * modifierPerFactor;
                break;
            case Enums.ModifierFactors.Corruption:
                if (targets.Count > 0)
                    target = targets[0];
                else
                    GameManager.manager.characters.TryGetValue(card.Character, out target);
                Debug.Log(target.data.name + ": Corruption = " + target.Corruption);
                value = (target.Corruption / perFactorValue) * modifierPerFactor;
                break;
            case Enums.ModifierFactors.Hand_Size:
                break;
            case Enums.ModifierFactors.Health:
                if (targets.Count > 0)
                    target = targets[0];
                else
                    GameManager.manager.characters.TryGetValue(card.Character, out target);
                int hp = target.data.health - target.Health;
                value = (hp / perFactorValue) * modifierPerFactor;
                break;
            case Enums.ModifierFactors.Marked:
                Debug.Log(targets[0] + " is marked: " + targets[0].Marked);
                if (targets[0].Marked)
                    value = 1;
                break;
            case Enums.ModifierFactors.Discards:
                Deck deck;
                for (int i = 0; i < 4; i++) {
                    GameManager.manager.decks.TryGetValue(GameManager.manager.party[i].data.characterType, out deck);
                    value += deck.DiscardList.Count;
                }
                Debug.Log("Discards: " + value);
                break;
            case Enums.ModifierFactors.Enemies:
                value = GameManager.manager.foes.Count;
                break;
        }

        CardEffect effect = card.cardEffects[effectIndex].GetEffect();
        effect.SetModification(value, modifierEffect);
        yield return null;
    }
}
