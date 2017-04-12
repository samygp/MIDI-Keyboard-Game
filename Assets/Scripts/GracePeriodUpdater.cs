using UnityEngine;
using SynchronizerData;

public class GracePeriodUpdater : MonoBehaviour {

    private BeatObserver beatObserver;
    private int graceCounter;
    Level lvl;
    // Use this for initialization
    void Start () {
        graceCounter = 0;
        try
        {
            lvl = GameObject.Find("Level").GetComponent<Level>();
            GameObject.Find("gracePeriodCounter").GetComponent<BeatCounter>().addObserver(gameObject); //Add to BeatCounter's observer
        }
        catch (System.NullReferenceException ex)
        {
            Debug.Log("Error: " + ex);
        }

        beatObserver = GetComponent<BeatObserver>();
    }
	
	// Update is called once per frame
	void Update () {
        if ((beatObserver.beatMask & BeatType.DownBeat) == BeatType.DownBeat)
        {
            //Debug.Log("Grace counter " + graceCounter);
            //Camera.main.transform.FindChild("D#5").GetComponent<AudioSource>().Play();
            if (graceCounter == 4)
                graceCounter = 0;
            //if (graceCounter == 1 || graceCounter == 2)
            if (graceCounter == 0 || graceCounter == 3)
                lvl.ingraceperiod = true;
            else
                lvl.ingraceperiod = false;
            graceCounter++;
        }
    }
}
