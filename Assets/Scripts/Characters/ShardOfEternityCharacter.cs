using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShardOfEternityCharacter : EnemyCharacter {
    public override void Awake () {
        base.Awake();
        CardToPlay = data.Deck.CardList[0];
    }

    public override IEnumerator GetTurn () {
        Debug.Log($"{name} playing card {CardToPlay.Name}");
        CombatUIManager.Instance.DisplayMessage($"{name} plays {CardToPlay.Name}");
        yield return CardToPlay.Activate();
    }

    protected override void OnDeath () {
        GameManager.manager.foes.Remove(this);
        Destroy(GetComponent<Targetable>());
        Destroy(gameObject);
    }
}
