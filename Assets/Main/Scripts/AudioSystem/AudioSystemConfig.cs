using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AudioSystemConfig", menuName = "AudioSystem/AudioSystemConfig")]
public class AudioSystemConfig : ScriptableObject
{
    public string AUDIO_MIXER_PATH = "Audio/Main";
    public string MUSIC_PATH = "Audio/Musics/";
    public string SFX_PATH = "Audio/SFXs/";
    public string AUDIO_MIXER_MUSIC = "Music";
    public string AUDIO_MIXER_SFX = "Sfx";

    public string MUSIC_GAMEOBJECT_NAME = "Music - [{0}]";  // {0} = Music channel
    public string SFX_GAMEOBJECT_NAME = "SFX - [{0}]";      // {0} = SFX name
    
    public AnimationCurve audioFalloffCurve;
}
