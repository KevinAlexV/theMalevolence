using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Silence : StatusEffect {
    private Character watchedCharacter;
    public Silence (Character target) {
        watchedCharacter = target;
        target.Action = Enums.Action.Silenced;
    }
}