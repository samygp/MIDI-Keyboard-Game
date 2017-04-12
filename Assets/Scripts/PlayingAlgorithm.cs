using UnityEngine;
using System.Collections;
using MidiJack;

public class PlayingAlgorithm : MonoBehaviour {

	public string pianoKey;
    private string [] notes;
    void Start(){
        string[] c = new string[12] 
        {"C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
        notes = new string[48];
        for(int i=2; i < 6; i++){
            for(int j=0; j<12; j++){
                notes[(i - 2) * 12 + j] = c[j]+i.ToString();
            }
        }

    }
	// Update is called once per frame
	void Update () {
        pianoKey = "*";
        int i = 36;
        for (; i < 84 && !MidiMaster.GetKeyDown(i); i++) ;
        if (i < 84)
        {
            pianoKey = notes[i - 36];
            BroadcastMessage("CheckNote", pianoKey);
        }
	}

    /*
    string getPianoKey(){
        pianoKey = "";
		if (Input.GetKeyDown (KeyCode.C) || MidiMaster.GetKeyDown (36) || MidiMaster.GetKeyDown (48) || 
			MidiMaster.GetKeyDown (60) || MidiMaster.GetKeyDown (72) || MidiMaster.GetKeyDown (84) || 
			MidiMaster.GetKeyDown (37) || MidiMaster.GetKeyDown (49) || MidiMaster.GetKeyDown (61) || 
			MidiMaster.GetKeyDown (73)) {
			return "C";
		} else if (Input.GetKeyDown (KeyCode.D) || MidiMaster.GetKeyDown (38) || MidiMaster.GetKeyDown (50) || 
			MidiMaster.GetKeyDown (62) || MidiMaster.GetKeyDown (74) || MidiMaster.GetKeyDown (39) || 
			MidiMaster.GetKeyDown (51) || MidiMaster.GetKeyDown (63) || MidiMaster.GetKeyDown (75)) {
			return 'D';

		} else if (Input.GetKeyDown (KeyCode.E) || MidiMaster.GetKeyDown (40) || MidiMaster.GetKeyDown (52) || 
			MidiMaster.GetKeyDown (64) || MidiMaster.GetKeyDown (76)) {
			return 'E';
		} else if (Input.GetKeyDown (KeyCode.F) || MidiMaster.GetKeyDown (41) || MidiMaster.GetKeyDown (53) || 
			MidiMaster.GetKeyDown (65) || MidiMaster.GetKeyDown (77) || MidiMaster.GetKeyDown (42) || 
			MidiMaster.GetKeyDown (54) || MidiMaster.GetKeyDown (66) || MidiMaster.GetKeyDown (78)) {
			return 'F';
		} else if (Input.GetKeyDown (KeyCode.G) || MidiMaster.GetKeyDown (43) || MidiMaster.GetKeyDown (55) || 
			MidiMaster.GetKeyDown (67) || MidiMaster.GetKeyDown (79) || MidiMaster.GetKeyDown (44) || 
			MidiMaster.GetKeyDown (56) || MidiMaster.GetKeyDown (68) || MidiMaster.GetKeyDown (80)) {
			return 'G';
		} else if (Input.GetKeyDown (KeyCode.A) || MidiMaster.GetKeyDown (45) || MidiMaster.GetKeyDown (57) || 
			MidiMaster.GetKeyDown (69) || MidiMaster.GetKeyDown (81) || MidiMaster.GetKeyDown (46) || 
			MidiMaster.GetKeyDown (58) || MidiMaster.GetKeyDown (70) || MidiMaster.GetKeyDown (82)) {
			return 'A';
		} else if (Input.GetKeyDown (KeyCode.B) || MidiMaster.GetKeyDown (47) || MidiMaster.GetKeyDown (59) || 
			MidiMaster.GetKeyDown (71) || MidiMaster.GetKeyDown (83)) {
			return 'B';
		} else {
			return '*';
		}
	}*/
}