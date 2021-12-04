using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Character : MonoBehaviour, ITurnExecutable, ITargetable
{
    protected GameObject SFX;
    protected GameObject highlight;

    [SerializeField]
    protected int _health;
    public int Health
    {
        get{
            return _health;
        }
        set{
            var newValue = Mathf.Max(Mathf.Min(value, data.health), 0);

            if (newValue == 0)
            {
                Defeated = true;

                CombatUIManager.Instance.SetDamageText(_health - newValue, transform);

                try { animator.SetTrigger("Death"); } catch (System.Exception e) { Debug.Log("Character error: No animation controller set"); }

                AudioManager.audioMgr.PlayCharacterSFX(SFX, "Death");
            }
            else if(_health > newValue)
            {
                CombatUIManager.Instance.SetDamageText(_health - newValue, transform);
                try { animator.SetTrigger("Hit"); } catch (System.Exception e) { Debug.Log("Character error: No animation controller set"); }

                AudioManager.audioMgr.PlayCharacterSFX(SFX, "Hurt");

            }
            else
            {
                CombatUIManager.Instance.SetDamageText(newValue - _health, transform, Color.green);

                AudioManager.audioMgr.PlayUISFX("Heal");
            }

            if(Defeated && newValue != 0){
                Defeated = false;
            }

            if (onStatChange != null)
            {
                onStatChange("health", ref _health, ref newValue);
            }
            
            _health = newValue;
            
        }
    }

    [SerializeField]
    protected int _discardPile;
    public int Discarded
    {
        get
        {
            return _discardPile;
        }
        set
        {
            if (onStatChange != null)
            {
                onStatChange("discard", ref _discardPile, ref value);
            }

            _discardPile = value;
        }
    }

    [SerializeField]
    protected int _corruption;
    public int Corruption
    {
        get{
            return _corruption;
        }
        set{
            if(onStatChange != null){
                onStatChange("corruption", ref _corruption, ref value);
            }

            //Play sounds based on corruption
            if (_corruption < value) {
                try { animator.SetTrigger("Corrupt"); } catch (System.Exception e) { Debug.Log("Character error: No animation controller set"); }
                AudioManager.audioMgr.PlayUISFX("CorruptionGain");
            }
            else {
                AudioManager.audioMgr.PlayUISFX("CorruptionCleanse");
            }

            CombatUIManager.Instance.SetDamageText(value - _corruption, transform, new Color32(139, 0, 139, 0));
            _corruption = value;
        }
    }

    private bool _defeated = false;
    public bool Defeated
    {
        get
        {
            return _defeated;
        }
        set
        {
            _defeated = value;
            if(_defeated){
                AudioManager.audioMgr.PlayCharacterSFX(SFX, "Death");
                OnDeath();
                GameManager.manager.CheckGameOver();
            }
        }
    }

    
    public bool Incapacitated {
        get {
            return Defeated || Action == Enums.Action.Stunned || Action == Enums.Action.Silenced;
        }
    }

    public CharacterData data;

    [SerializeField]
    protected Card _cardToPlay = null;
    public virtual Card CardToPlay
    {
        get
        {
            return _cardToPlay;
        }
        set {
            var newCard = value;
            _cardToPlay = newCard;
            if(_cardToPlay != null){
                Action = Enums.Action.Card;
            } else {
                Action = Enums.Action.Attack;
            }
        }
    }

    private Enums.Action _action = Enums.Action.Attack;

    public Enums.Action Action {
        get{
            return _action;
        }
        set{
            var newAction = value;
            if((onActionChange != null)){
                onActionChange(_action, newAction);
            }
            _action = newAction;
        }
    }

    public bool Marked { get; set; }

    protected Animator animator;
    public Animator Animator { get { return animator; } }

    //Character Events
    public delegate void StatChangeHandler(string statName, ref int oldValue, ref int newValue); //we should make some static statName strings to prevent bugs
    public event StatChangeHandler onStatChange;

    public delegate void ActionChangeHandler(Enums.Action prev, Enums.Action newAction);
    public ActionChangeHandler onActionChange;

    //Modifying corruptionValueForCheck will change the int the random roll is compared to
    public delegate void CorruptionCheckAttemptHandler(ref int corruptionValueForCheck);
    public event CorruptionCheckAttemptHandler onCorruptionCheckAttempt;

    public delegate void CorruptionCheckResultHandler(bool passed);
    public event CorruptionCheckResultHandler onCorruptionCheckResult;
    public delegate void TurnHandler();
    public event TurnHandler onTurnStart;
    public event TurnHandler onTurnEnd;

    public delegate void TargetedHandler(Character source, ref Character target);
    public event TargetedHandler onTargeted;

    public delegate void AttackHandler(Character target, ref Damage d);
    public event AttackHandler onAttack;


    //Event wrappers to allow events to be invoked in child classes

    protected void InvokeStatChangeHandler(string statName, ref int oldValue, ref int newValue){
        onStatChange?.Invoke(statName, ref oldValue, ref newValue);
	}

    protected void InvokeActionChangeHandler(Enums.Action prev, Enums.Action newAction){
        onActionChange?.Invoke(prev, newAction);
	}
    protected void InvokeCorruptionCheckAttemptHandler(ref int corruptionValueForCheck){
        onCorruptionCheckAttempt?.Invoke(ref corruptionValueForCheck);
	}

    protected void InvokeCorruptionCheckResultHandler(bool passed){
        onCorruptionCheckResult?.Invoke(passed);
	}
    protected void InvokeTurnStartHandler(){
        onTurnStart?.Invoke();
	}
    protected void InvokeTurnEndHandler(){
        onTurnEnd?.Invoke();
	}

    protected void InvokeTargetedHandler(Character source, ref Character target){
        onTargeted?.Invoke(source, ref target);
	}

    protected void InvokeAttackHandler(Character target, ref Damage d){
        onAttack?.Invoke(target, ref d);
	}

    //Perform a corruption check and return the result (passed or failed, true/false).
    public bool CorruptionCheck(){
        int corruptionValue = Corruption;

        if (onCorruptionCheckAttempt != null){
            onCorruptionCheckAttempt(ref corruptionValue);
        }

        int corruptionCheck = Random.Range(1, 100);
        
        bool result = corruptionCheck > corruptionValue;

        if(onCorruptionCheckResult != null){
            onCorruptionCheckResult(result);
        }

        return result;
    }

    

    /*
        New Targetting system:
        - Targetable.getTargetable requires a Character source
        - Character.Targeted requires a character source, returns a Character that is the target
        - Character.onTarget events can see the source and change the target
    
    */

    //Called whenever a character is targetted. Returns the actual target, which will most likely be that character
    public Character Targeted(Character source){
        Character target = this;
        if(onTargeted != null){
            onTargeted(source, ref target);
        }
        return target;
    }

    public virtual void Awake()
    {
        _health = data.currentHealth;
        _corruption = data.currentCorruption;
        Action = Enums.Action.Attack;
        animator = GetComponentInChildren<Animator>();
    }

    public virtual void Start()
    {
        Debug.Log($"Creating {this.gameObject.name}");

        SFX = GameManager.manager.getChildGameObject(this.gameObject, "CharacterSFX");
        highlight = GameManager.manager.getChildGameObject(this.gameObject, "Highlight");

    }

    public void toggleHighlight()
    {
        try
        {
            if (highlight.active)
                highlight.SetActive(false);
            else if (Defeated == false)
                highlight.SetActive(true);
        }
        catch { Debug.Log($"<color=red>Error: {this.name} does not contain a ParticleSystem highlight component. Cannot toggle (Character.cs)</color>"); }
    }

    //Called once a resolve phase ends, reseting the character's status
    public virtual void EndResolvePhase(){
        GameManager.manager.Discard(CardToPlay);
        _cardToPlay = null;
        Action = Enums.Action.Attack;
        
    }

    protected virtual void OnDeath() { return; }

    //Temporary implementation of character's turn
    public abstract IEnumerator GetTurn();
}
