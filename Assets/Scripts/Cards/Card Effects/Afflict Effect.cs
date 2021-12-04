using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** <summary>Special Card effect: Afflicts the target with a status effect.</summary> */
[System.Serializable]
public class AfflictEffect : CardEffect {
    [SerializeField] private Enums.StatusEffects statusEffect;
    private StatusEffect effect;

    public override IEnumerator ApplyEffect () {

        if (targets != null) { 

            foreach (Character target in targets) {
                switch (statusEffect) {
                    case Enums.StatusEffects.CorruptionShield:
                        effect = new StatChangePrevention(target, "corruption", Enums.StatChangeEnum.Increase);
                        break;
                    case Enums.StatusEffects.HealthShield:
                        effect = new StatChangePrevention(target, "health", Enums.StatChangeEnum.Decrease);
                        break;
                    case Enums.StatusEffects.CorruptionSubstitution:
                        effect = new CorruptionSubstitution(GameManager.manager.characters[card.Character], target);
                        break;
                    case Enums.StatusEffects.ExtraCard:
                        effect = new ExtraAction(target, Enums.Action.Card);
                        break;
                    case Enums.StatusEffects.Protected:
                        effect = new AttackSubstitution(GameManager.manager.characters[card.Character], target);
                        break;
                    case Enums.StatusEffects.BackTalk:
                        effect = new BackTalk(GameManager.manager.characters[card.Character]);
                        break;
                    case Enums.StatusEffects.DoubleDamage:
                        effect = new DoubleDamage(target);
                        break;
                    case Enums.StatusEffects.Stun:
                        effect = new Stun(target);
                        break;
                    case Enums.StatusEffects.StunDelayed:
                        effect = new StunDelayed(target);
                        break;
                    case Enums.StatusEffects.AttackBuff:
                        effect = new AttackBuff(target, 1, 0, 0);
                        break;
                    case Enums.StatusEffects.ExistentialDread:
                        effect = new ExistentialDread(target, GameManager.manager.characters[card.Character]);
                        break;
                    case Enums.StatusEffects.LustForDestruction:
                        effect = new LustForDestruction(target, new Damage(2, 6, 0), new Damage(3, 6, 0));
                        break;
                    case Enums.StatusEffects.Mark:
                        break;
                    case Enums.StatusEffects.WeakenArmor:
                        break;
                    case Enums.StatusEffects.WeakenWeapon:
                        break;
                    case Enums.StatusEffects.DiscardCharCards:
                            var cardsToRemove = new List<CardDisplayController>();
                            foreach (CardDisplayController cardDisplay in GameManager.manager.hand.DisplayedCards) {
                                if (cardDisplay.CardData.Character == target.data.characterType) {
                                    cardsToRemove.Add(cardDisplay);
                                }
                            }
                            foreach (CardDisplayController cardDisplay in cardsToRemove) {
                                GameManager.manager.Discard(cardDisplay.CardData);
                                GameManager.manager.RemoveCardFromHand(cardDisplay);
                            }
                            yield return CombatUIManager.Instance.DisplayMessage($"Discarding {target.data.name}'s cards from your hand!", 2f);
                        break;
                    case Enums.StatusEffects.Silence:
                        effect = new Silence(target);
                        break;
                }
            }

        }
        yield return null;
    }
}
