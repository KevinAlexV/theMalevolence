using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marked : StatusEffect {
    private Character watchedCharacter;
    private int currentCountdown;
    public Marked (Character target) {
        watchedCharacter = target;
        target.Marked = true;
    }
}
