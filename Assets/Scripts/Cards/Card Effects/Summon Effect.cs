using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/** <summary>Special card effect: Summons a friendly unit to the caster's side.</summary> */
[System.Serializable]
public class SummonEffect : CardEffect {
    [SerializeField] private Enums.Character Boss;

    public override IEnumerator ApplyEffect () {
        PuzzleboxCharacter puzzlebox;
        HeadmasterCharacter headmaster;
        switch (Boss) {
            case Enums.Character.PuzzleBox:
                puzzlebox = (PuzzleboxCharacter)GameManager.manager.foes[0];
                yield return puzzlebox.SpawnShard();
                break;
            case Enums.Character.Headmaster:
                headmaster = (HeadmasterCharacter)GameManager.manager.foes[0];
                if (Random.Range(1, 2) == 1) {
                    for (int i = 0; i < 4; i++) {
                        yield return headmaster.SpawnStudent();
                    }
                } else {
                    for (int i = 0; i < 2; i++) {
                        yield return headmaster.SpawnFaculty();
                    }
                }
                break;
            case Enums.Character.Student:
                headmaster = (HeadmasterCharacter)GameManager.manager.foes[0];
                yield return headmaster.SpawnStudent();
                break;
            case Enums.Character.Faculty:
                headmaster = (HeadmasterCharacter)GameManager.manager.foes[0];
                yield return headmaster.SpawnFaculty();
                break;
        }
    }
}
