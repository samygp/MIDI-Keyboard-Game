using UnityEngine;
using System.Collections;

/// <summary>
/// This class should be attached to the audio source for which synchronization should occur, and is 
/// responsible for synching up the beginning of the audio clip with all active beat counters and pattern counters.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class BeatSynchronizer : MonoBehaviour {

	public float bpm = 120f;		// Tempo in beats per minute of the audio clip.
	public float startDelay = 1f;	// Number of seconds to delay the start of audio playback.
	public delegate void AudioStartAction(double syncTime);
    public AudioSource bgMusic;
    public static event AudioStartAction OnAudioStart;
	
	
	void Start (){
		double initTime = AudioSettings.dspTime;
        //Debug.Log("SE supone que empieza: " + (initTime + startDelay));
		GetComponent<AudioSource>().PlayScheduled(initTime + startDelay);
        if (OnAudioStart != null) {
			OnAudioStart(initTime + startDelay);
		}
	}

    /// <summary>
    /// Carga la pista de fondo sobre la cual se jugará.
    /// </summary>
    /// <param name="song">Nombre del archivo con la pista de audio mp3.</param>
    public bool loadMusic(string song){
        try{
            TagLib.File tagFile = TagLib.File.Create(Application.dataPath + "/Resources/" + song + ".mp3");
            bpm = tagFile.Tag.BeatsPerMinute;
            bgMusic.clip = (AudioClip)Resources.Load(song);
            bgMusic.loop = true;
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
            return false;
        }
        return true;
    }
}
