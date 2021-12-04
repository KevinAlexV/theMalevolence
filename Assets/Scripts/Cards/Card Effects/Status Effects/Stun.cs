using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stun: StatusEffect {
    private Character watchedCharacter;
    public Stun(Character target){
        watchedCharacter = target;
        target.Action = Enums.Action.Stunned;
    }
}