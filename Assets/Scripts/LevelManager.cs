using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private static LevelManager _instance;
    public static LevelManager Instance {
        get{
            return _instance;
        }
    }

    /* LEVEL NAME DEFINITIONS */

    public static string BossDriver = "BossOne";
    public static string BossHeadmaser = "BossHeadmaster";
    public static string BossPuzzleBox = "BossPuzzleBox";
    public static string BossEntity = "BossEntity";

    public List<string> levels;
    private int levelIndex = 0;
    public delegate void OnLoad();

    // Start is called before the first frame update
    void Start()
    {
        if(_instance != null){
            Destroy(gameObject);
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void ToNextLevel(){
        //temp, we should create a victory scene
        if(levelIndex == levels.Count){
            ToMainMenu();
        }
        var nextLevel = levels[levelIndex];
        if(nextLevel.Split('_')[0].Equals("Cutscene")){
            ToCutscene(levels[levelIndex]);
        } else if(nextLevel.Split('_')[0].Equals("Drafting")){
            ToDeckBuilder();
        } else {
            StartCoroutine(LoadScene(levels[levelIndex]));
        }
        
        levelIndex++;
    }

    public void ToDeckBuilder(){
        StartCoroutine(LoadScene("DeckBuilder"));
    }

    public void ToCutscene(string levelName){
        IEnumerator sceneLoader = LoadScene("Cutscene", ()=>{
            CutsceneManager cs = GameObject.Find("Cutscene Manager").GetComponent<CutsceneManager>();
            cs.StartScene(levelName);
        });
        StartCoroutine(sceneLoader);
    }

    public void ToMainMenu(){
        levelIndex = 0;
        SceneManager.LoadScene("Main Menu");
        GameObject b = GameObject.Find("StartGameBtn");
        b.GetComponent<UnityEngine.UI.Button>().onClick.RemoveAllListeners();
        b.GetComponent<UnityEngine.UI.Button>().onClick.AddListener(()=>{
            ToNextLevel();
        });
    }

    public IEnumerator LoadScene(string sceneName, OnLoad onLoadHandler = null){
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        if(onLoadHandler != null){
            onLoadHandler();
        }
    }
}
