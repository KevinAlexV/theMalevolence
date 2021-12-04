using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadmasterCharacter : EnemyCharacter {

    [SerializeField] private List<Transform> CultistSpawns;
    private EnemyCharacter[] Cultists = new EnemyCharacter[12];
    private bool turnEnd = false;

    public override IEnumerator GetTurn () {
        deck.Shuffle();
        if (turnEnd) {
            for (int i = 0; i < Cultists.Length; i++) {
                if (Cultists[i] != null) {
                    yield return Cultists[i].GetTurn();
                }
            }
        }
        else {
            if (Action != Enums.Action.Stunned && Health > 0) {
                if (deck.CardList.Count == 0) deck.Reshuffle();
                CardToPlay = deck.Draw();
                yield return CombatUIManager.Instance.RevealCard(CardToPlay); //Should extend this time when not testing
                Debug.Log($"{name} playing card {CardToPlay.Name}");
                CombatUIManager.Instance.DisplayMessage($"{name} plays {CardToPlay.Name}");
                yield return CardToPlay.Activate();
            }
        }
        turnEnd = !turnEnd;
    }

    public IEnumerator SpawnStudent () {
        
        for (int i = 0; i < Cultists.Length; i++) {
            if (Cultists[i] == null){
                AfflictedStudentCharacter newStudent = Instantiate(Resources.Load<AfflictedStudentCharacter>("prefabs/Afflicted Student"), CultistSpawns[i].transform);
                newStudent.transform.localPosition = Vector3.zero;
                yield return CombatUIManager.Instance.DisplayMessage("An Afflicted Student has come to the Headmaster's aid");
                Cultists[i] = newStudent;
                GameManager.manager.foes.Add(newStudent);
                break;
            }
        }
    }

    public IEnumerator SpawnFaculty() {
        for (int i = 0; i < Cultists.Length; i++) {
                if (Cultists[i] == null)
                {
                    ZealousFacultyCharacter newFaculty = Instantiate(Resources.Load<ZealousFacultyCharacter>("prefabs/Zealous Faculty"), CultistSpawns[i].transform);
                    newFaculty.transform.localPosition = Vector3.zero;
                    yield return CombatUIManager.Instance.DisplayMessage("A Zealous Faculty has come to the Headmaster's aid");
                    Cultists[i] = newFaculty;
                    GameManager.manager.foes.Add(newFaculty);
                    break;
                }
            }
    }
}
