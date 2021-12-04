public class AttackBuff : StatusEffect {
    private Character watchedCharacter;

    public AttackBuff(Character target, int dieNumber, int dieSize, int dieBonus){
        var newDamage = new Damage(
            target.data.basicAttack.DieNumber + dieNumber,
            target.data.basicAttack.DieSize + dieSize,
            target.data.basicAttack.DieBonus + dieBonus
            );
        target.data.basicAttack = newDamage;
    }
}