using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Voicelines : MonoBehaviour
{
    [SerializeField]
    private List<AudioClip> lines;

    public AudioClip getAudioClip(string name)
    {

        try
        {
            //AudioClip returnValue = lines.FindLast(sound => sound.name == name);
            //return returnValue;
            return null;
        }
        catch { return null; }

    }

    public List<AudioClip> getAudioClips()
    {
        if(lines != null)
            return lines;

        return null;
    }
}
