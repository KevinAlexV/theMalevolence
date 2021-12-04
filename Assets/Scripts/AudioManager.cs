using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager audioMgr;

    //BGM
    [SerializeField]
    private AudioClip sceneBGM, sceneIntro, victoryTrack, failureTrack;
    private AudioSource currentTrack;

    //SFX
    public GameObject SFXSource;
    private AudioSource SFXPlayer {
        get{
            if(SFXSource != null){
                return SFXSource.GetComponent<AudioSource>();
            } else {
                SFXSource = GameObject.FindGameObjectWithTag("MainCanvas");
                if(SFXSource != null){
                    return SFXSource.GetComponent<AudioSource>();
                } else {
                    return GetComponent<AudioSource>();
                }
            }
        }
    }
    public List<AudioClip> SoundEffects;
    

    public void Awake()
    {
        if (audioMgr != null && audioMgr != this)
        {
            Destroy(gameObject);
            return;
        }
        //Play 'awake' SoundEffects for the scene (one time) CHANGE THIS IF WE GO WITH A 'STARTUP' SONG
        currentTrack = GetComponent<AudioSource>();
        currentTrack.volume = 0.2f;
        currentTrack.clip = sceneBGM;
        currentTrack.loop = true;


        //Configure audioSources
        SFXPlayer.volume = 0.5f;
        currentTrack.playOnAwake = false;
        SFXPlayer.playOnAwake = false;


        audioMgr = this;

        currentTrack.Play();
        DontDestroyOnLoad(gameObject);
    }

    public void PlayCharacterSFX(GameObject SourceObject, string SFXName)
    {
        if(SourceObject != null)
        { 
            Transform[] ts = SourceObject.transform.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in ts)
            { 
                if (t.gameObject.name == SFXName)
                { 
                    PlayObjectSFX(t.gameObject);
                    break;
                }

            }
        }
    }

    public void PlayObjectSFX(GameObject SFXObject)
    {

        SFXObject.GetComponent<AudioSource>().Play();
        
    }

    public void PlayVoiceline(GameObject SFXSourceObject, string filename)
    {
        Debug.Log("Attempting to play" + filename);
        if (SFXSourceObject != null)
        {
            Transform[] ts = SFXSourceObject.transform.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in ts)
            {
                if (t.gameObject.name == "VoiceLines")
                {
                    GameObject voice = t.gameObject;
                    AudioSource voiceLine = voice.GetComponent<AudioSource>();
                    voiceLine.clip = voice.GetComponent<Voicelines>().getAudioClip(filename);

                    Debug.Log($"{t.gameObject.name} is the current audio source gameobject");

                    if(voiceLine.clip != null)
                        PlayObjectSFX(voice);

                    break;
                }

            }
        }

    }

    public void PlayUISFX(string SFX)
    {

        switch (SFX)
        {
            case "PaperInteraction":
                SFXPlayer.clip = SoundEffects.FindLast(sound => sound.name == ("CardInteraction"));
                break;
            case "PickupCard":
                SFXPlayer.clip = SoundEffects.FindLast(sound => sound.name == ("PickupCard" + Random.Range(1, 3)));
                break;
            case "PlaceCard":
                SFXPlayer.clip = SoundEffects.FindLast(sound => sound.name == ("PlaceCard" + Random.Range(1, 2)));
                break;
            case "Shuffle":
                SFXPlayer.clip = SoundEffects.FindLast(sound => sound.name == ("Shuffle" + Random.Range(1, 2)));
                break;
            case "CorruptionFail":
                SFXPlayer.clip = SoundEffects.FindLast(sound => sound.name == ("CorruptionFail"));
                break;
            case "CorruptionPass":
                SFXPlayer.clip = SoundEffects.FindLast(sound => sound.name == ("CorruptionPass"));
                break;
            case "CorruptionGain":
                SFXPlayer.clip = SoundEffects.FindLast(sound => sound.name == ("CorruptionGain"));
                break;
            case "CorruptionCleanse":
                SFXPlayer.clip = SoundEffects.FindLast(sound => sound.name == ("CorruptionCleanse"));
                break;
            case "Heal":
                SFXPlayer.clip = SoundEffects.FindLast(sound => sound.name == ("Heal"));
                break;
            default:
                Debug.Log($"<color=red>AudioManager:</color> Sound effect of name {SFX} is not listed!");
                break;
        }

        SFXPlayer.Play();

    }

    public void StopMusic() { currentTrack.Stop(); }

    public void PauseMusic() { currentTrack.Pause(); }

    public void PlayMusic() { currentTrack.Play(); }

    public void ChangeMusic(AudioClip newClip)
    {
        var oldTrack = currentTrack;
        currentTrack = gameObject.AddComponent<AudioSource>();
        currentTrack.volume = 0.0f;
        currentTrack.clip = newClip;
        currentTrack.loop = true;
        currentTrack.Play();
        StartCoroutine(CrossFade(oldTrack, 0f, 0.001f));
        StartCoroutine(CrossFade(currentTrack, 0.2f, 0.001f));
    }

    private IEnumerator CrossFade(AudioSource source, float volumeTarget, float rate){
        float progress = 0f;
        while(source.volume != volumeTarget){
            source.volume = Mathf.Lerp(source.volume, volumeTarget, progress);
            progress += rate;
            yield return new WaitForFixedUpdate();
        }
        if(source.volume == 0){
            Destroy(source);
        }
    }
}
