using UnityEngine;
using System.Collections;

public class DestroyOnContact : MonoBehaviour {

    Vector3 desplazamiento;
    public BeatCounter qb;
    Level level;

    void Start(){
        level = GameObject.Find("Level").GetComponent<Level>();
        desplazamiento = new Vector3(0, 0.05F, 0);
    }

    public void desplazar(){
        level.waterHeight += desplazamiento.y;
        transform.Translate(desplazamiento, Camera.main.transform);
    }

	void OnTriggerEnter2D(Collider2D otro){
        qb.observers.Remove(otro.gameObject);
        level.resetCombo();
        level.updateScore(0);
        Destroy(otro.gameObject);
        desplazar();
    }
}
