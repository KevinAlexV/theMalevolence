using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Card)), CanEditMultipleObjects]
public class CardEditor : Editor {
    Card card;
    SerializedObject GetTarget;
    CardEffectsMaker effectMaker;
    static bool displayBools;

    #region CARD DATA
    SerializedProperty CardName;
    SerializedProperty CardDescription;
    SerializedProperty CardCorruptionPassDescription;
    SerializedProperty CardCorruptionFailDescription;
    SerializedProperty CardFlavor;
    SerializedProperty CardCharacter;
    SerializedProperty CardExile;
    SerializedProperty CardBossCard;
    SerializedProperty CardBossCorTargets;
    SerializedProperty CardFront;
    SerializedProperty CardBack;
    SerializedProperty CardAnim;
    SerializedProperty CardIcons;
    SerializedProperty CardEffects;
    SerializedProperty CardCorPass;
    SerializedProperty CardCorFail;
    SerializedProperty CardEffectBuffer;
    #endregion

    private void OnEnable () {
        card = (Card)target;
        GetTarget = new SerializedObject(card);

        #region CARD DATA
        CardName = GetTarget.FindProperty("cardName");
        CardDescription = GetTarget.FindProperty("cardDescription");
        CardCorruptionPassDescription = GetTarget.FindProperty("cardCorruptionPassDescription");
        CardCorruptionFailDescription = GetTarget.FindProperty("cardCorruptionFailDescription");
        CardFlavor = GetTarget.FindProperty("cardFlavor");
        CardCharacter = GetTarget.FindProperty("cardCharacter");
        CardExile = GetTarget.FindProperty("exiled");
        CardBossCard = GetTarget.FindProperty("bossCard");
        CardBossCorTargets = GetTarget.FindProperty("bossCorTargets");
        CardFront = GetTarget.FindProperty("cardFront");
        CardBack = GetTarget.FindProperty("cardBack");
        CardAnim = GetTarget.FindProperty("cardAnimation");
        CardIcons = GetTarget.FindProperty("cardIcons");
        CardEffects = GetTarget.FindProperty("cardEffects");
        CardCorPass = GetTarget.FindProperty("cardCorPass");
        CardCorFail = GetTarget.FindProperty("cardCorFail");
        #endregion
    }

    public override void OnInspectorGUI () {
        GetTarget.Update();
        
        using (new EditorGUILayout.VerticalScope("HelpBox")) {
            EditorGUILayout.LabelField("Card Information", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(CardName);
            EditorGUILayout.PropertyField(CardDescription);
            EditorGUILayout.PropertyField(CardCorruptionPassDescription);
            EditorGUILayout.PropertyField(CardCorruptionFailDescription);
            EditorGUILayout.PropertyField(CardFlavor);
            EditorGUILayout.PropertyField(CardCharacter);
            displayBools = EditorGUILayout.Foldout(displayBools, "Extra Card Options");
            if (displayBools) {
                EditorGUILayout.PropertyField(CardExile);
                EditorGUILayout.PropertyField(CardBossCard);
            }
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Card Art", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(CardFront);
            EditorGUILayout.PropertyField(CardBack);
            EditorGUILayout.PropertyField(CardIcons);
            EditorGUILayout.PropertyField(CardAnim);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Card Effects", EditorStyles.boldLabel);
            InsertCardEffectFields(CardEffects, 0);
            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Corruption Effects", EditorStyles.boldLabel);
            if (card.BossCard)
                EditorGUILayout.PropertyField(CardBossCorTargets);
            EditorGUILayout.LabelField("Pass");
            InsertCardEffectFields(CardCorPass, 1);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Fail");
            InsertCardEffectFields(CardCorFail, 2);
        }

        GetTarget.ApplyModifiedProperties();
    }

    private void InsertCardEffectFields(SerializedProperty effectsList, int listNo) {
        //Go through each effect in the list
        for (int i = 0; i < effectsList.arraySize; i++) {
            effectMaker = card.GetEffectMaker(i, listNo);
            //Add the effect dropdown select, and remove button
            GUILayout.BeginHorizontal();
            effectMaker.effectType = (Enums.CardEffects)EditorGUILayout.EnumPopup("Card Effect", effectMaker.effectType);
            if (i != 0) {
                if (GUILayout.Button("Up", EditorStyles.miniButtonLeft, GUILayout.Width(30f))) {
                    //SerializedProperty buffer = effectsList.GetArrayElementAtIndex(i - 1);
                    effectsList.MoveArrayElement(i, i - 1);
                    continue;
                }
            }
            if (i != effectsList.arraySize - 1) {
                if (GUILayout.Button("Down", EditorStyles.miniButtonLeft, GUILayout.Width(40f))) {
                    effectsList.MoveArrayElement(i, i + 1);
                    continue;
                }
            }
            if (GUILayout.Button("Remove", EditorStyles.miniButtonLeft, GUILayout.Width(60f))) {
                effectsList.DeleteArrayElementAtIndex(i);
                continue;
            }
            GUILayout.EndHorizontal();

            #region CARD DATA
            CardEffectBuffer = effectsList.GetArrayElementAtIndex(i);
            #endregion

            #region EFFECT TYPES
            SerializedProperty afflictEffect = CardEffectBuffer.FindPropertyRelative("afflictEffect");
            SerializedProperty attackEffect = CardEffectBuffer.FindPropertyRelative("attackEffect");
            SerializedProperty cleanseEffect = CardEffectBuffer.FindPropertyRelative("cleanseEffect");
            SerializedProperty drawEffect = CardEffectBuffer.FindPropertyRelative("drawEffect");
            SerializedProperty insertEffect = CardEffectBuffer.FindPropertyRelative("insertEffect");
            SerializedProperty modifyEffect = CardEffectBuffer.FindPropertyRelative("modifyEffect");
            SerializedProperty reshuffleEffect = CardEffectBuffer.FindPropertyRelative("reshuffleEffect");
            SerializedProperty solveEffect = CardEffectBuffer.FindPropertyRelative("solveEffect");
            SerializedProperty summonEffect = CardEffectBuffer.FindPropertyRelative("summonEffect");
            SerializedProperty vitalityEffect = CardEffectBuffer.FindPropertyRelative("vitalityEffect");
            SerializedProperty baseEffect = CardEffectBuffer.FindPropertyRelative("cardEffect");
            #endregion

            //Draw input fields based on chosen effect
            switch (effectMaker.effectType) {
                case Enums.CardEffects.Afflict:
                    EditorGUILayout.PropertyField(afflictEffect);
                    break;
                case Enums.CardEffects.Attack:
                    EditorGUILayout.PropertyField(attackEffect);
                    break;
                case Enums.CardEffects.Cleanse:
                    EditorGUILayout.PropertyField(cleanseEffect);
                    break;
                case Enums.CardEffects.Draw:
                    EditorGUILayout.PropertyField(drawEffect);
                    break;
                case Enums.CardEffects.Insert:
                    EditorGUILayout.PropertyField(insertEffect);
                    break;
                case Enums.CardEffects.Modify:
                    EditorGUILayout.PropertyField(modifyEffect);
                    break;
                case Enums.CardEffects.Reshuffle:
                    EditorGUILayout.PropertyField(reshuffleEffect);
                    break;
                case Enums.CardEffects.Solve:
                    EditorGUILayout.PropertyField(solveEffect);
                    break;
                case Enums.CardEffects.Summon:
                    EditorGUILayout.PropertyField(summonEffect);
                    break;
                case Enums.CardEffects.Vitality:
                    EditorGUILayout.PropertyField(vitalityEffect);
                    break;
                default:
                    EditorGUILayout.PropertyField(baseEffect);
                    break;
            }

            EditorGUILayout.Space();
        }

        if (GUILayout.Button("Add Effect")) {
            card.AddCardEffectMaker(listNo);
        }

        //if (effectsList.Count > 0) {
        //for (int i = 0; i < effectsList.Count; i++) {
        /**effectMaker = effectsList[i];

        GUILayout.BeginHorizontal();
        effectMaker.effectType = (Enums.CardEffects)EditorGUILayout.EnumPopup("Card Effect", effectMaker.effectType);
        if (GUILayout.Button("Remove", EditorStyles.miniButtonLeft, GUILayout.Width(60f))) {
            effectsList.RemoveAt(i);
        }
        GUILayout.EndHorizontal();*/

        /**switch (effectMaker.effectType) {
            case Enums.CardEffects.Attack:
                EditorGUILayout.PropertyField(AttackEffect);
                break;
            case Enums.CardEffects.Cleanse:
                EditorGUILayout.PropertyField(CleanseEffect);
                break;
            case Enums.CardEffects.Corrupt:
                EditorGUILayout.PropertyField(CorruptEffect);
                break;
            case Enums.CardEffects.Draw:
                EditorGUILayout.PropertyField(DrawEffect);
                break;
            case Enums.CardEffects.Haste:
                EditorGUILayout.PropertyField(HealEffect);
                break;
            case Enums.CardEffects.Heal:
                EditorGUILayout.PropertyField(HealEffect);
                break;
            case Enums.CardEffects.Modify:
                EditorGUILayout.PropertyField(ModifyEffect);
                break;
            case Enums.CardEffects.Protect:
                EditorGUILayout.PropertyField(ProtectEffect);
                break;
            case Enums.CardEffects.Remove:
                EditorGUILayout.PropertyField(RemoveEffect);
                break;
            case Enums.CardEffects.ReplaceValue:
                EditorGUILayout.PropertyField(ReplaceValueEffect);
                break;
            case Enums.CardEffects.Reshuffle:
                EditorGUILayout.PropertyField(ReshuffleEffect);
                break;
            default:
                EditorGUILayout.PropertyField(CardEffect);
                break;
        }*/
        //}
        //}
    }
}
