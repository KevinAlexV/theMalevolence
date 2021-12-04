using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptionSubstitution : StatusEffect {
    private Character watchedCharacter;
    private Character substitutionCharacter;
    public CorruptionSubstitution(Character c, Character target){
        watchedCharacter = target;
        substitutionCharacter = c;
        target.onCorruptionCheckAttempt += Substitute;
        GameManager.manager.onPhaseChange += TurnEnd;
    }

    public void Substitute(ref int value){
        value = substitutionCharacter.Corruption;
    }

    public void TurnEnd(Enums.GameplayPhase phase){
        if(phase == Enums.GameplayPhase.Draw){
            watchedCharacter.onCorruptionCheckAttempt -= Substitute;
        }
    }
}