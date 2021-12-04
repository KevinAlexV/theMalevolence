using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** <summary>Card effect: Restore HP to a target character or alter their Corruption.</summary> */
[System.Serializable]
public class VitalityEffect : CardEffect {

    /** <summary>Which part of the character's vitality to affect</summary> */
    [SerializeField] private Enums.VitalityType vitalityType;
    /** <summary>The value to change the character's vitality</summary> */
    [SerializeField] private int value;

    private int val;

    /** <summary>Applies the effect onto the relevant targets</summary> */
    public override IEnumerator ApplyEffect () {
        ApplyModification();

        if (targets != null)
        { 
            foreach (Character c in targets) {
                switch (vitalityType) {
                    //Modify the character's health
                    case Enums.VitalityType.Health:
                        c.Health += val;
                        //Clamp value to min and max values
                        if (c.Health < 0) c.Health = 0;
                        if (c.Health > c.data.health) c.Health = c.data.health;
                        //Inform the player what just happened
                        //If value is positive, Health was restored, color = green
                        //If value is negative, Health was lost, color = white
                        if (val > 0)
                            yield return CombatUIManager.Instance.DisplayMessage($"{c.data.name} restored {val} HP");
                        else
                            yield return CombatUIManager.Instance.DisplayMessage($"{c.data.name} lost {val} HP");
                        break;
                    //Modify the character's corruption
                    case Enums.VitalityType.Corruption:
                        c.Corruption += val;
                        //Clamp value to min and max values
                        if (c.Corruption < 0) c.Corruption = 0;
                        if (c.Corruption > 100) c.Corruption = 100;
                        //Inform the player what just happened
                        //If value is positive, Corruption was gained, color = purple
                        //If value is negative, Corruption was cleansed, color = purple
                        if (val < 0)
                            yield return CombatUIManager.Instance.DisplayMessage($"{c.data.name} cleansed {val} Corruption");
                        else
                            yield return CombatUIManager.Instance.DisplayMessage($"{c.data.name} gained {val} Corruption");
                        break;
                }
            }
        }
        yield return null;
    }

    /** <summary>Takes the modification from the MODIFY effect and increases "value" value</summary> */
    public override void ApplyModification () {
        val = value;
        if (modifyingValue != 0) {
            switch (modification) {
                case Enums.Modifier.Add:
                    val += modifyingValue;
                    break;
                case Enums.Modifier.Subtract:
                    val -= modifyingValue;
                    break;
                case Enums.Modifier.Multiply:
                    val *= modifyingValue;
                    break;
                case Enums.Modifier.Divide:
                    val /= modifyingValue;
                    break;
            }
        }
    }
}