public class ExistentialDread : StatusEffect {
    private Character watchedCharacter;
    private Character ownerCharacter;

    public ExistentialDread(Character target, Character owner){
        watchedCharacter = target;
        ownerCharacter = owner;
        GameManager.manager.onPhaseChange += DamageOverTime;
        watchedCharacter.onStatChange += EndEffect;
        ownerCharacter.onStatChange += EndEffect;
    }

    public void DamageOverTime(Enums.GameplayPhase phase){
        if(phase == Enums.GameplayPhase.Draw){
            watchedCharacter.Health -= new Damage(1,6,0).Value;
        }
    }
    public void EndEffect(string statName, ref int oldValue, ref int newValue){
        if(statName == "health" && newValue == 0){
            GameManager.manager.onPhaseChange -= DamageOverTime;
            watchedCharacter.onStatChange -= EndEffect;
            ownerCharacter.onStatChange -= EndEffect;
        }
    }
}