using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityCharacter : EnemyCharacter {

    [SerializeField] private Animator platformAnimator;

    [SerializeField]
    private Card firstCard;
    private bool firstTurn = true;

    private void Awake () {
        _health = data.currentHealth;
        _corruption = data.currentCorruption;
        Action = Enums.Action.Attack; Debug.Log($"{data.Deck.CardList.Count} cards will be created...");

        int count = 0;
        //Establish card color for decks
        foreach (Card c in data.Deck.CardList) {
            count++;
            Debug.Log($"{count}: {c.name} given color.");
            c.Color = data.color;
        }
        Debug.Log($"{count}: {firstCard.name} given color.");
        firstCard.Color = data.color;

        deck = data.Deck;

        animator = platformAnimator;
    }

    public override IEnumerator GetTurn () {
        deck.Shuffle();
        if (Action != Enums.Action.Stunned && Health > 0) {
                if (deck.CardList.Count == 0) deck.Reshuffle();
            if (firstTurn) {
                CardToPlay = firstCard;
                firstTurn = false;
            } else
                CardToPlay = deck.Draw();
            yield return CombatUIManager.Instance.RevealCard(CardToPlay); //Should extend this time when not testing
            Debug.Log($"{name} playing card {CardToPlay.Name}");
            CombatUIManager.Instance.DisplayMessage($"{name} plays {CardToPlay.Name}");
            yield return CardToPlay.Activate();
        }
    }
}
