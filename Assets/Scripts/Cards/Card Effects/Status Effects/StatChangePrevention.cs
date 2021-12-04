using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatChangePrevention : StatusEffect {
    private string watchedStat;
    private Character watchedCharacter;
    public StatChangePrevention(Character c, string stat, Enums.StatChangeEnum changeEnum = Enums.StatChangeEnum.Any){
        watchedStat = stat;
        watchedCharacter = c;
        switch(changeEnum){
            case Enums.StatChangeEnum.Increase:
                watchedCharacter.onStatChange += PreventStatIncrease;
                break;
            case Enums.StatChangeEnum.Decrease:
                watchedCharacter.onStatChange += PreventStatDecrease;
                break;
            default:
                watchedCharacter.onStatChange += PreventStatChange;
                break;
        }
    }

    public void PreventStatDecrease(string statName, ref int oldValue, ref int newValue){
        if(statName == watchedStat && newValue < oldValue){
            newValue = oldValue;
            watchedCharacter.onStatChange -= PreventStatDecrease;
            //Workaround to update ui
            if(statName == "health"){
                watchedCharacter.Health = watchedCharacter.Health;
            } else {
               watchedCharacter.Corruption = watchedCharacter.Corruption; 
            }
        }
    }

    public void PreventStatIncrease(string statName, ref int oldValue, ref int newValue){
        if(statName == watchedStat && newValue > oldValue){
            newValue = oldValue;
            watchedCharacter.onStatChange -= PreventStatIncrease;
            if(statName == "health"){
                watchedCharacter.Health = watchedCharacter.Health;
            } else {
               watchedCharacter.Corruption = watchedCharacter.Corruption; 
            }
        }
    }

    public void PreventStatChange(string statName, ref int oldValue, ref int newValue){
        if(statName == watchedStat){
            newValue = oldValue;
            watchedCharacter.onStatChange -= PreventStatChange;
        }
    }
}