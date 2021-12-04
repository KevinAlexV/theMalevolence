using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PartyCharacter : Character
{
    bool cFirst25 = true, cFirst50 = true, cFirst75 = true, cFirstPlay = true;

    public override void Start()
    {
        base.Start();

        if (data.color != null && highlight != null)
            highlight.GetComponent<ParticleSystem>().startColor = data.color;
    }

    //overriden to update character display, and to return cards to then hand if the player plays multiple
    public override Card CardToPlay
    {
        get
        {
            return _cardToPlay;
        }
        set {
            var newCard = value;
            //If the player plays a card while another card has yet to be played, return the first to the hand
            if (_cardToPlay != null && newCard != null){
                GameManager.manager.PlaceCardInHand(CardToPlay);
            }
            _cardToPlay = newCard;
            //Update the Character's action
            if(_cardToPlay != null){
                Action = Enums.Action.Card;
            } else {
                Action = Enums.Action.Attack;
            }
        }
    }

    //Pull current characters basic attack (can create new one and save to the data object for specific chars)
    public IEnumerator BasicAttack(Damage damage = null){
        yield return Targetable.GetTargetable(Enums.TargetType.Foes, this, $"{data.name} attacks: Select a foe!", 1);
        Character target = (Character)Targetable.currentTargets[0];
        damage = damage == null ? data.basicAttack : damage;
        InvokeAttackHandler(target, ref damage);

        animator.SetTrigger("Attack");
        AudioManager.audioMgr.PlayCharacterSFX(SFX, "Attack");

        target.Health -= damage.Value;
    }

    //Executes the character's turn, where they either play their card or attack
    public override IEnumerator GetTurn(){
        Debug.Log($"<color=orange>{data.name}'s turn</color>");
        InvokeTurnStartHandler();
        if(Defeated)
        {
            Debug.Log($"<Color=darkred>{data.name} has been defeated and cannot continue the fight</color>");
        }
        else if(Action == Enums.Action.Card && CardToPlay != null)
        {
            Debug.Log($"<color=green>{data.name} playing card {CardToPlay.Name}</color>");
            //CombatUIManager.Instance.DisplayMessage($"{name} plays {CardToPlay.Name}");
            yield return CombatUIManager.Instance.RevealCard(CardToPlay);
            //Execute the selected card from the dropzone.
            yield return CardToPlay.Activate();
        }
        else if(Action == Enums.Action.Attack || Action == Enums.Action.Silenced)
        {
            yield return BasicAttack();
        }
        InvokeTurnEndHandler();
        yield return new WaitForSeconds(1f);
    }

    public void checkVoicelines()
    {


        if (this.Corruption >= 25 && this.Corruption <= 50 && cFirst25 == true)
        {
            AudioManager.audioMgr.PlayVoiceline(this.gameObject, $"{this.data.characterType}-25Corruption");
            cFirst25 = false;
        }
        else if (this.Corruption >= 50 && this.Corruption <= 75 && cFirst50 == true)
        {
            AudioManager.audioMgr.PlayVoiceline(this.gameObject, $"{this.data.characterType}-50Corruption");
            cFirst50 = false;
        }
        else if (this.Corruption >= 75 && cFirst75 == true)
        {
            AudioManager.audioMgr.PlayVoiceline(this.gameObject, $"{this.data.characterType}-75Corruption");
            cFirst75 = false;
        }
        else if (this.Corruption >= 75 && cFirst75 == true)
        {
            AudioManager.audioMgr.PlayVoiceline(this.gameObject, $"{this.data.characterType}-75Corruption");
            cFirst75 = false;
        }

        if (cFirstPlay == true && this.data.characterType == Enums.Character.Jock)
        {
            AudioManager.audioMgr.PlayVoiceline(this.gameObject, $"{this.data.characterType}-IntroChoices");
            cFirstPlay = false;
        }

    }


}
