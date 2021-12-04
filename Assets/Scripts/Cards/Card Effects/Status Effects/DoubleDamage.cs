public class DoubleDamage : StatusEffect {
    private Character watchedCharacter;

    public DoubleDamage(Character target){
        watchedCharacter = target;
        watchedCharacter.onAttack += BoostDamage;
        GameManager.manager.onPhaseChange += EndEffect;
    }

    public void BoostDamage(Character target, ref Damage damage){
        var newDamage = new Damage(
            (int)(watchedCharacter.data.basicAttack.DieNumber * 2.5),
            (int)(watchedCharacter.data.basicAttack.DieSize * 2.5),
            (int)(watchedCharacter.data.basicAttack.DieBonus * 2.5)
        );
        damage = newDamage;
    }

    public void EndEffect(Enums.GameplayPhase phase){
        if(phase == Enums.GameplayPhase.Draw){
            watchedCharacter.onAttack -= BoostDamage;
            GameManager.manager.onPhaseChange -= EndEffect;
        }
    }

}