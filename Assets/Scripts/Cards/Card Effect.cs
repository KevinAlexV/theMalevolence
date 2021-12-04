using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** <summary>The base class for all other card effects.</summary> */
[System.Serializable]
public class CardEffect {
    [SerializeField] protected Enums.Target target;
    protected List<Character> targets;
    protected Card card;

    protected int modifyingValue;
    protected Enums.Modifier modification;

    public Enums.Target Target { get { return target; } }

    public void SetOwnerCard (Card c) {
        card = c;
    }

    //Depending on the card, find the target for the card
    public IEnumerator DesignateTarget() {
        targets = new List<Character>();

        //Toggle the 'action' button for each character display prefab
        GameManager.manager.togglePartyButton(false);

        if (!card.BossCard)
            Debug.Log("<color=blue>CardEffect.cs</color>: find target");
        Character c;
        GameManager.manager.characters.TryGetValue(card.Character, out c);

        switch (target) {
            case Enums.Target.Self:
                targets.Add(c);
                break;
            case Enums.Target.Ally:
                if (card.AllyTarget == null) {
                    if (card.BossCard) {
                        int targ;
                        do {
                            targ = Random.Range(0, GameManager.manager.party.Count);
                        } while (GameManager.manager.party[targ].Defeated);
                        card.AllyTarget = GameManager.manager.party[targ];
                    } else {
                        yield return Targetable.GetTargetable(Enums.TargetType.Allies, c, "Select Ally", 1);
                        card.AllyTarget = ((Character)Targetable.currentTargets[0]).Targeted(GameManager.manager.characters[card.Character]);
                    }
                }
                targets.Add(card.AllyTarget);
                break;
            case Enums.Target.Enemy:
                if (card.EnemyTarget == null) {
                    if (card.BossCard) {
                        int targ;
                        do {
                            targ = Random.Range(0, GameManager.manager.foes.Count);
                        } while (GameManager.manager.foes[targ].Defeated);
                        card.EnemyTarget = GameManager.manager.party[targ];
                    } else {
                        yield return Targetable.GetTargetable(Enums.TargetType.Foes, c, "Select Enemy", 1);
                        card.EnemyTarget = ((Character)Targetable.currentTargets[0]).Targeted(GameManager.manager.characters[card.Character]);
                    }
                }
                targets.Add(card.EnemyTarget);
                break;
            case Enums.Target.All_Ally: //When all allies are targetted, effects that trigger of a single character being targetted shouldn't trigger
                targets = new List<Character>(GameManager.manager.party);
                break;
            case Enums.Target.All_Enemy: //Same with all enemies
                targets = new List<Character>(GameManager.manager.foes);
                break;
            case Enums.Target.Before_Self:
                targets.Add((Character)GameManager.manager.turns[0]);
                targets.Add((Character)GameManager.manager.turns[1]);
                break;
            case Enums.Target.After_Self:
                targets.Add((Character)GameManager.manager.turns[3]);
                targets.Add((Character)GameManager.manager.turns[4]);
                break;
            case Enums.Target.Second_Ally:
                if (card.SecondAllyTarget == null) {
                    if (card.BossCard) {
                        int targ;
                        do {
                            targ = Random.Range(0, 4);
                        } while (GameManager.manager.party[targ] != card.AllyTarget);
                        card.SecondAllyTarget = GameManager.manager.party[targ];
                    } else {
                        do {
                            yield return Targetable.GetTargetable(Enums.TargetType.Allies, c, "Select Another Ally", 1);
                        } while ((Character)Targetable.currentTargets[0] != card.AllyTarget);
                        card.SecondAllyTarget = (Character)Targetable.currentTargets[0];
                    }
                }
                targets.Add(card.SecondAllyTarget);
                break;
        }
        if(targets != null)
        { 
            //foreach (Character t in targets)
            //    if (t.Defeated)
            //        targets.Remove(t);
        }
        GameManager.manager.togglePartyButton(GameManager.manager.actionsEnabled);
    }

    public virtual IEnumerator ApplyEffect() {
        //Tell game manager to skip the caster's turn
        yield return null;
    }

    public void SetModification (int value, Enums.Modifier mod) {
        modifyingValue = value;
        modification = mod;
    }

    public virtual void ApplyModification () {
        return;
    }

    public void AddTarget(Character newTarget)
    {
        if (targets == null)
            targets = new List<Character>();

        targets.Add(newTarget);
    }
    public void ResetTargets()
    {
        if(targets != null)
            targets.Clear();

    }

    /*
        CardEffect Resolution and Targeting
    */
    // public IEnumerator Activate(){
    //     //Effect resolution goes here
    // }

    // public IEnumerator AccquireTarget(){
    //     yield return Targetable.GetTargetable(Enums.TargetType.Any, "Targetting ui msg", 1);
    //     this.targets = Targetable.currentTargets;
    // }
}
