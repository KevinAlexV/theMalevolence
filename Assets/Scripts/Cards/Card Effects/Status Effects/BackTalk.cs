using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackTalk : StatusEffect {
    
    private Character watchedCharacter;
    private int bonus = 0;
    public BackTalk(Character c){
        watchedCharacter = c;
        GameManager.manager.onPhaseChange += ExtraAttack;
        watchedCharacter.onStatChange += AddDamage;
    }

    public void AddDamage(string statName, ref int oldValue, ref int newValue){
        if(statName == "corruption" && oldValue < newValue){
            bonus = newValue - oldValue;
        }
    }

    public void ExtraAttack(Enums.GameplayPhase phase){
        if(phase != Enums.GameplayPhase.Resolve) return;
        var index = GameManager.manager.turns.IndexOf(watchedCharacter);
        GameManager.manager.turns.Insert(index + 1, watchedCharacter);
        watchedCharacter.Action = (watchedCharacter.Action != Enums.Action.Stunned) ? Enums.Action.Attack : Enums.Action.Stunned;
        watchedCharacter.onTurnEnd += WaitToSecondAttack;
        GameManager.manager.onPhaseChange -= ExtraAttack;
    }

    public void WaitToSecondAttack(){
        watchedCharacter.onTurnEnd -= WaitToSecondAttack;
        watchedCharacter.onAttack += BonusDamage;
    }

    public void BonusDamage(Character target, ref Damage d){
        d = new Damage(watchedCharacter.data.basicAttack);
        d.bonus += Mathf.CeilToInt(bonus / 5);
        watchedCharacter.onStatChange -= AddDamage;
        watchedCharacter.onAttack -= BonusDamage;
    }

    /*
        New Targetting system:
        - Targetable.getTargetable requires a Character source
        - Character.Target requires a character source, returns a Character that is the target
        - Character.onTarget events can see the source and change the target
    
    */
}