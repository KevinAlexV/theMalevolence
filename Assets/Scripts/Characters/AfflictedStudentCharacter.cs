using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AfflictedStudentCharacter : EnemyCharacter {
    
    public override IEnumerator GetTurn () {
        if (Action != Enums.Action.Stunned) {
            if (deck.CardList.Count == 0) deck.Reshuffle();
            CardToPlay = deck.Draw();
            yield return CombatUIManager.Instance.RevealCard(CardToPlay); //Should extend this time when not testing
            Debug.Log($"{name} playing card {CardToPlay.Name}");
            animator.SetTrigger("Serve");
            CombatUIManager.Instance.DisplayMessage($"{name} plays {CardToPlay.Name}");
            yield return CardToPlay.Activate();
        }
    }

    protected override void OnDeath () {
        GameManager.manager.foes.Remove(this);
        Destroy(GetComponent<Targetable>());
        Destroy(gameObject);
    }
}
