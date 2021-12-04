using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardEffectsMaker {
    public Enums.CardEffects effectType;
    
    public CardEffect cardEffect = new CardEffect();
    public AfflictEffect afflictEffect = new AfflictEffect();
    public AttackEffect attackEffect = new AttackEffect();
    public CleanseEffect cleanseEffect = new CleanseEffect();
    public DrawEffect drawEffect = new DrawEffect();
    public InsertEffect insertEffect = new InsertEffect();
    public ModifierEffect modifyEffect = new ModifierEffect();
    public ReshuffleEffect reshuffleEffect = new ReshuffleEffect();
    public SolveEffect solveEffect = new SolveEffect();
    public SummonEffect summonEffect = new SummonEffect();
    public VitalityEffect vitalityEffect = new VitalityEffect();
    
    public CardEffectsMaker (Card c) {
        cardEffect.SetOwnerCard(c);
        afflictEffect.SetOwnerCard(c);
        attackEffect.SetOwnerCard(c);
        cleanseEffect.SetOwnerCard(c);
        drawEffect.SetOwnerCard(c);
        insertEffect.SetOwnerCard(c);
        modifyEffect.SetOwnerCard(c);
        reshuffleEffect.SetOwnerCard(c);
        solveEffect.SetOwnerCard(c);
        summonEffect.SetOwnerCard(c);
        vitalityEffect.SetOwnerCard(c);
    }

    public CardEffect GetEffect () {
        switch(effectType) {
            case Enums.CardEffects.Afflict:
                return afflictEffect;
            case Enums.CardEffects.Attack:
                return attackEffect;
            case Enums.CardEffects.Cleanse:
                return cleanseEffect;
            case Enums.CardEffects.Draw:
                return drawEffect;
            case Enums.CardEffects.Insert:
                return insertEffect;
            case Enums.CardEffects.Modify:
                return modifyEffect;
            case Enums.CardEffects.Reshuffle:
                return reshuffleEffect;
            case Enums.CardEffects.Solve:
                return solveEffect;
            case Enums.CardEffects.Summon:
                return summonEffect;
            case Enums.CardEffects.Vitality:
                return vitalityEffect;
            default:
                return cardEffect;
        }
    }

}
