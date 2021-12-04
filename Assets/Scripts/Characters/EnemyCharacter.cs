using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCharacter : Character
{
    protected Deck deck;

    public override Card CardToPlay {
        get {
            return _cardToPlay;
        }

        set {
            var newCard = value;
            if(_cardToPlay != null){
                deck.DiscardList.Add(_cardToPlay);
            }
            _cardToPlay = newCard;
            if(_cardToPlay != null){
                Action = Enums.Action.Card;
            } else {
                Action = Enums.Action.Attack;
            }
        }
    }
    public override void Awake(){
        _health = data.health;
        _corruption = data.corruption;
        Action = Enums.Action.Attack;

        Debug.Log($"{data.Deck.CardList.Count} cards will be created...");

        int count = 0;
        //Establish card color for decks
        foreach (Card c in data.Deck.CardList)
        {
            count++;
            Debug.Log($"{count}: {c.name} given color.");
            c.Color = data.color;
        }

        deck = data.Deck;

        animator = GetComponentInChildren<Animator>();
        //Any initialization in deck order
    }

    public override void Start()
    {
        base.Start();

        if (data.color != null && highlight != null)
            highlight.GetComponent<ParticleSystem>().startColor = Color.white;
    }

    public override IEnumerator GetTurn(){
        Character target;
    
        do {
            target = GameManager.manager.party[Random.Range(0, 4)];
            Debug.Log("Picking target");
        } while (target.Defeated == true);
        target = target.Targeted(this); //let the target know it has been targetted, and allow it to reassign the arget if it can
        int dmg = data.basicAttack.Value;
        Debug.Log($"Boss is attacking {target.data.name} for {dmg} HP!");
        StartCoroutine(CombatUIManager.Instance.DisplayMessage($"Boss attacks {target.data.name} for {dmg} HP!"));

        //Damage health
        target.Health -= dmg;
        CombatUIManager.Instance.SetDamageText(dmg, target.transform);
        yield return new WaitForSeconds(0.25f);
        //Increase corruption
        target.Corruption += dmg * 2;
        CombatUIManager.Instance.SetDamageText(dmg * 2, target.transform, new Color32(139, 0, 139, 0));
        yield return new WaitForSeconds(1f);
    }
}
