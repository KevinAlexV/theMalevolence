using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackSubstitution : StatusEffect {
    private Character watchedCharacter;
    private Character substitutionCharacter;
    public AttackSubstitution(Character c, Character target){
        watchedCharacter = target;
        substitutionCharacter = c;
        target.onTargeted += Substitute;
    }

    public void Substitute(Character source, ref Character target){
        target.onTargeted -= Substitute;
        if((source as EnemyCharacter) != null){
            target = substitutionCharacter;
        }
    }
}