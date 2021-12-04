using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Card", menuName = "Card")]
[System.Serializable]
public class Card : ScriptableObject {
    //[Header("Card Info")]
    [SerializeField] private string cardName;
    [TextArea]
    [SerializeField] private string cardDescription;
    [SerializeField] private string cardCorruptionPassDescription;
    [SerializeField] private string cardCorruptionFailDescription;
    [SerializeField] private string cardFlavor;
    [SerializeField] private Enums.Character cardCharacter;
    [SerializeField] private Color cardColor;
    [SerializeField] private bool exiled;
    [SerializeField] private bool bossCard;
    [SerializeField] private Enums.Target bossCorTargets;

    //[Header("Card Art")]
    [SerializeField] private Sprite cardFront;
    [SerializeField] private Sprite cardBack;
    [SerializeField] private Sprite[] cardIcons = new Sprite[4];
    [SerializeField] private string cardAnimation = "";

    public List<CardEffectsMaker> cardEffects = new List<CardEffectsMaker>();
    public List<CardEffectsMaker> cardCorPass = new List<CardEffectsMaker>();

    internal void ChangeOwner (Enums.Character characterType) {
        cardCharacter = characterType;
    }

    public List<CardEffectsMaker> cardCorFail = new List<CardEffectsMaker>();

    public string Name { get { return cardName; } }
    public string Description { get { return cardDescription; } }
    public string Flavor { get { return cardFlavor; } }
    public Enums.Character Character { get { return cardCharacter; } }
    public Color Color
    {
        get { return cardColor; }
        set{cardColor = value;}
    }
    public string CorruptionPassDescription { get { return cardCorruptionPassDescription; } }
    public string CorruptionFailDescription { get { return cardCorruptionFailDescription; } }

    public bool Exiled { get { return exiled; } }
    public bool BossCard { get { return bossCard; } }
    public Enums.Target BossCorTargets { get { return bossCorTargets; } }

    public Sprite FrontArt { get { return cardFront; } }
    public Sprite BackArt { get { return cardBack; } }
    public Sprite[] Icons { get { return cardIcons; } }

    public Character AllyTarget { get; set; }
    public Character SecondAllyTarget { get; set; }
    public Character EnemyTarget { get; set; }

    private void SetList (List<CardEffect> effectsList, List<CardEffectsMaker> makerList) {
        for (int i = 0; i < makerList.Count; i++) {
            switch (makerList[i].effectType) {
                case Enums.CardEffects.Afflict:
                    effectsList.Add(makerList[i].afflictEffect);
                    break;
                case Enums.CardEffects.Attack:
                    effectsList.Add(makerList[i].attackEffect);
                    break;
                case Enums.CardEffects.Cleanse:
                    effectsList.Add(makerList[i].cleanseEffect);
                    break;
                case Enums.CardEffects.Draw:
                    effectsList.Add(makerList[i].drawEffect);
                    break;
                case Enums.CardEffects.Insert:
                    effectsList.Add(makerList[i].insertEffect);
                    break;
                case Enums.CardEffects.Modify:
                    effectsList.Add(makerList[i].modifyEffect);
                    break;
                case Enums.CardEffects.Reshuffle:
                    effectsList.Add(makerList[i].reshuffleEffect);
                    break;
                case Enums.CardEffects.Summon:
                    effectsList.Add(makerList[i].summonEffect);
                    break;
                case Enums.CardEffects.Vitality:
                    effectsList.Add(makerList[i].vitalityEffect);
                    break;
                case Enums.CardEffects.Solve:
                    effectsList.Add(makerList[i].solveEffect);
                    break;
            }
        }
    }

    public void AddCardEffectMaker (int listNo) {
        if (listNo == 0) {
            cardEffects.Add(new CardEffectsMaker(this));
        } else if (listNo == 1)
            cardCorPass.Add(new CardEffectsMaker(this));
        else if (listNo == 2)
            cardCorFail.Add(new CardEffectsMaker(this));
    }

    public CardEffect GetEffect (int index, int listNo) {
        if (listNo == 0)
            return cardEffects[index].GetEffect();
        else if (listNo == 1)
            return cardCorPass[index].GetEffect();
        else if (listNo == 2)
            return cardCorFail[index].GetEffect();
        else
            return null;
    }

    public CardEffectsMaker GetEffectMaker (int index, int listNo) {
        if (listNo == 0)
            return cardEffects[index];
        else if (listNo == 1)
            return cardCorPass[index];
        else if (listNo == 2)
            return cardCorFail[index];
        else
            return null;
    }

    //Play the card
    public IEnumerator Activate () {
        if (bossCard)
            yield return DesignateTargets();

        if (!cardAnimation.Equals("")) {
            Character c;
            GameManager.manager.characters.TryGetValue(cardCharacter, out c);
            c.Animator.SetTrigger(cardAnimation);
        }


        for (int i = 0; i < cardEffects.Count; i++){
            var effect = cardEffects[i].GetEffect();
            effect.SetOwnerCard(this);
            yield return effect.ApplyEffect();
        }

        if (cardCorPass.Count > 0 || cardCorFail.Count > 0) {
            Character character;
            GameManager.manager.characters.TryGetValue(cardCharacter, out character);
            if (bossCard) {
                yield return BossCorruptionCheck();
            } else {
                if (character.CorruptionCheck())
                    for (int i = 0; i < cardCorPass.Count; i++) {
                        var effect = cardCorPass[i].GetEffect();
                        effect.SetOwnerCard(this);
                        yield return effect.ApplyEffect();
                    } 
                else
                    for (int i = 0; i < cardCorFail.Count; i++) {
                        var effect = cardCorFail[i].GetEffect();
                        effect.SetOwnerCard(this);
                        yield return effect.ApplyEffect();
                    }
            }
        }
        yield return null;
    }


    public IEnumerator DesignateTargets() {
        AllyTarget = null;
        EnemyTarget = null;

        //Somewhere here, targets are not being designated for the card. Are there missing references?
        Debug.Log("Designating targets");

        for (int i = 0; i < cardEffects.Count; i++) {
            //cardEffect.card was not being set properly, this is a workaround
            var effect = cardEffects[i].GetEffect();
            effect.SetOwnerCard(this);
            yield return effect.DesignateTarget();
        }

        for (int i = 0; i < cardCorPass.Count; i++) {
            //cardEffect.card was not being set properly, this is a workaround
            var effect = cardCorPass[i].GetEffect();
            effect.SetOwnerCard(this);
            if (!bossCard)
                yield return effect.DesignateTarget();
        }

        for (int i = 0; i < cardCorFail.Count; i++) {
            var effect = cardCorFail[i].GetEffect();
            effect.SetOwnerCard(this);
            if (!bossCard)
                yield return effect.DesignateTarget();
        }
    }

    public IEnumerator BossCorruptionCheck () {
        List<Character> targets = new List<Character>();
        switch (bossCorTargets) {
            case Enums.Target.Ally:
                if (AllyTarget == null) {
                    int targ;
                    do {
                        targ = Random.Range(0, GameManager.manager.party.Count);
                    } while (GameManager.manager.party[targ].Defeated);
                    AllyTarget = GameManager.manager.party[targ];
                }
                targets.Add(AllyTarget);
                break;
            case Enums.Target.All_Ally:
                targets.AddRange(GameManager.manager.party);
                break;
            case Enums.Target.After_Self:
                targets.Add((Character)GameManager.manager.turns[3]);
                targets.Add((Character)GameManager.manager.turns[4]);
                break;
            case Enums.Target.Before_Self:
                targets.Add((Character)GameManager.manager.turns[0]);
                targets.Add((Character)GameManager.manager.turns[1]);
                break;
        }

        foreach (Character c in targets) {
            if (c.CorruptionCheck())
                for (int i = 0; i < cardCorPass.Count; i++) {
                    var effect = cardCorPass[i].GetEffect();
                    effect.SetOwnerCard(this);
                    effect.ResetTargets();
                    switch (effect.Target) {
                        case Enums.Target.Self:
                            effect.AddTarget(c);
                            break;
                        case Enums.Target.Enemy:
                            effect.AddTarget(GameManager.manager.foes[0]);
                            break;
                    }
                    yield return effect.ApplyEffect();
                } else
                for (int i = 0; i < cardCorFail.Count; i++) {
                    var effect = cardCorFail[i].GetEffect();
                    effect.SetOwnerCard(this);
                    effect.ResetTargets();
                    switch (effect.Target) {
                        case Enums.Target.Self:
                            effect.AddTarget(c);
                            break;
                        case Enums.Target.Enemy:
                            effect.AddTarget(GameManager.manager.foes[0]);
                            break;
                    }
                    yield return effect.ApplyEffect();
                }
        }
    }

    public void Exile() {
        exiled = true;
    }

    /*
        CardEffect Resolution and Targeting
    */
    // public IEnumerator ActivateEffect(){
    //     foreach(CardEffect effect in cardEffects){
    //         yield return effect.Activate();
    //     }
    // }

    // public IEnumerator AccquireTargets(){
    //     foreach(CardEffect effect in cardEffects){
    //         yield return effect.AccquireTarget();
    //     }
    // }
}
