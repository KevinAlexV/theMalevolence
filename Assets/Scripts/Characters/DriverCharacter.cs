using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriverCharacter : EnemyCharacter
{
    public Card siezureChoiceHelp;
    public Card siezureChoiceIgnore;

    public IEnumerator Siezure(){

        siezureChoiceHelp.Color = Color.black;
        siezureChoiceIgnore.Color = Color.black;

        var choices = new List<Card>();
        choices.Add(siezureChoiceHelp);
        choices.Add(siezureChoiceIgnore);
        yield return CombatUIManager.Instance.DisplayChoice(choices);
        yield return choices[0].DesignateTargets(); //These two cards target all allies
        animator.SetTrigger("Flail");
        yield return choices[0].Activate();
    }

    public IEnumerator CrazedFlailing() {
        animator.SetTrigger("Flail");
        var index = GameManager.manager.turns.IndexOf(this);
        for(int i = 0; i < index; i++){
            Character partyMemberTarget = GameManager.manager.turns[i] as PartyCharacter;
            if(partyMemberTarget != null){
                var damage = new Damage(2, 6, 3).Value;
                partyMemberTarget.Health -= damage;
                CombatUIManager.Instance.SetDamageText(damage, partyMemberTarget.transform);
                yield return new WaitForSeconds(0.25f);
            }
        }
        yield return new WaitForSeconds(0.5f);
        foreach(Character partyMember in GameManager.manager.party){
            if(!partyMember.CorruptionCheck()){
                new StunDelayed(partyMember);
                yield return CombatUIManager.Instance.DisplayMessage($"{partyMember.data.name} will be stunned next turn!", 1.5f);
            } else {
                yield return CombatUIManager.Instance.DisplayMessage($"{partyMember.data.name} avoids the flailing!", 1.5f);
            }
        }
        yield return new WaitForSeconds(0.5f);
    }

    public IEnumerator SlamAndBreak() {
        animator.SetTrigger("Slam");
        //Deal 3d6 to character with must HP
        Character target = GameManager.manager.party[0].Targeted(this);
        for(int i = 1; i < GameManager.manager.party.Count; i++){
            if(target.Health < GameManager.manager.party[i].Health){
                target = GameManager.manager.party[i];
            }
        }
        target = target.Targeted(this);
        var slamDamage = new Damage(3, 6, 0).Value;
        target.Health -= slamDamage;
        CombatUIManager.Instance.SetDamageText(slamDamage, target.transform);
        yield return new WaitForSeconds(1f);

        foreach(Character partyMember in GameManager.manager.party){
            if(!partyMember.CorruptionCheck()){
                var breakDamage = new Damage(1, 6, 0).Value;
                partyMember.Health -= breakDamage;
                CombatUIManager.Instance.SetDamageText(breakDamage, partyMember.transform);
                yield return new WaitForSeconds(0.25f);
            }
        }
        
    }

    public IEnumerator HailToTheBusDriver() {
        animator.SetTrigger("Hail");
        //Each character gains 10 corruption
        CardToPlay.DesignateTargets(); //This card targets all allies, so it can use the card system
        CardToPlay.Activate();


        foreach(Character partyMember in GameManager.manager.party){
            if(!partyMember.CorruptionCheck()){
                var cardsToRemove = new List<CardDisplayController>();
                foreach(CardDisplayController cardDisplay in GameManager.manager.hand.DisplayedCards){
                    if(cardDisplay.CardData.Character == partyMember.data.characterType){
                        cardsToRemove.Add(cardDisplay);
                    }
                }

                foreach(CardDisplayController cardDisplay in cardsToRemove){
                    GameManager.manager.Discard(cardDisplay.CardData);
                    GameManager.manager.RemoveCardFromHand(cardDisplay);
                }
                yield return CombatUIManager.Instance.DisplayMessage($"Discarding {partyMember.name}'s cards from your hand!", 2f);
            } else {
                partyMember.Corruption += 10;
                CombatUIManager.Instance.SetDamageText(10, partyMember.transform, new Color32(139, 0, 139, 0));
                yield return CombatUIManager.Instance.DisplayMessage($"{partyMember.name} gained {10} Corruption");
            }
        }
    }
    /* Cannot implement ravings of a madman, need some method of creating multiple instances of the same card.
    public IEnumerator RavingsOfAMadman(){
        //Character with the least corruption gains 10
        Character target = GameManager.manager.party[0];
        for(int i = 1; i < GameManager.manager.party.Count; i++){
            if(target.Corruption > GameManager.manager.party[i].Corruption){
                target = GameManager.manager.party[i];
            }
        }

        target.Corruption += 10;
        CombatUIManager.Instance.SetDamageText(10, target.transform, new Color32(139, 0, 139, 0));
        yield return CombatUIManager.Instance.DisplayMessage($"{target.name} gained {10} Corruption");

        foreach(Character partyMember in GameManager.manager.party){
            if(!partyMember.CorruptionCheck()){
                Card c = 
            }
        }
    }*/

    public override IEnumerator GetTurn(){
        Debug.Log(animator);
        if(Action != Enums.Action.Stunned){

            if (deck.CardList.Count == 0){
                deck.Reshuffle();
            }

            CardToPlay = deck.Draw();

            yield return CombatUIManager.Instance.RevealCard(CardToPlay); //Should extend this time when not testing
            switch(CardToPlay.Name){
                case "Siezure":
                    yield return Siezure();
                    break;
                case "Crazed Flailing":
                    yield return CrazedFlailing();
                    break;
                case "Slam and Break":
                    yield return SlamAndBreak();
                    break;
                case "Hail to The Bus Driver":
                    yield return HailToTheBusDriver();
                    break;
                default:
                    throw new System.Exception("Driver tried to play unknown card of name: " + CardToPlay.Name);

            }
            deck.DiscardList.Add(CardToPlay);
        }
    }
}
