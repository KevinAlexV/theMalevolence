public class LustForDestruction : StatusEffect {
    public LustForDestruction(Character target, Damage option1, Damage option2){
        bool allyDead = false;
        foreach(Character c in GameManager.manager.party){
            allyDead = allyDead || c.Defeated;
        }

        if(!allyDead){
            target.Health -= option1.Value;
        } else {
            target.Health -= option2.Value;
        }
    }
}