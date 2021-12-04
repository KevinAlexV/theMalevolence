using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunDelayed : StatusEffect {
    private Character watchedCharacter;
    public StunDelayed(Character target){
        watchedCharacter = target;
        GameManager.manager.onPhaseChange += SetStun;
    }
    public void SetStun(Enums.GameplayPhase phase){
        if(phase != Enums.GameplayPhase.Planning) return;
        watchedCharacter.Action = Enums.Action.Stunned;
        GameManager.manager.onPhaseChange -= SetStun;
    }
}