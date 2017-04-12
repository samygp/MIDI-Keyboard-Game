using UnityEngine;
using System.Collections.Generic;
using SynchronizerData;

public class CloudBehavior : MonoBehaviour {

    Vector3 localPosition;
    int counter, counterinterval;
    int maxChildren;
    public int health;
    private List<string> notes;
    public Vector4 cloudColor;
    Vector4 colorIncrement, currentColor;
    public bool active = false;
    public int type, ptchReg, lifePoints;
    static Camera cam = Camera.main;
    Level lvlObject;
    private Animator anim;
    private BeatObserver beatObserver;

    public string giveMeTheNote(){
        List<string> availableNotes= new List<string>(notes);
        string temp = "";
        for (int i = 0; i < transform.childCount; i++)
        {
            var child = transform.GetChild(i).gameObject;
            if (child != null) {
                string nota = child.GetComponent<DropletActions>().nota;
                int j;
                for (j = 0; j < availableNotes.Count && availableNotes[j] + ptchReg != nota; j++) ;
                if(j < availableNotes.Count)
                    availableNotes.RemoveAt(j);
                
            }
        }
        foreach (string n in availableNotes)
            temp += n + ',';
        //Debug.Log("Available Notes after: " + temp);
        return availableNotes[Random.Range(0, availableNotes.Count)];
    }

    void assignColor(){
        switch (ptchReg){
            case 1:
                cloudColor = new Vector4((float)0.823, 0,0,1);
                break;
            case 2:
                cloudColor = new Vector4((float)0.823, (float)0.1176, (float)0.431,1);
                break;
            case 3:
                cloudColor = new Vector4((float)0.823, (float)0.1176, (float)0.823, 1);
                break;
            case 4:
                cloudColor = new Vector4((float)0.431, (float)0.1176, (float)0.823, 1);
                break;
            case 5:
                cloudColor = new Vector4(0, 0, (float)0.823, 1);
                break;
            default:
                cloudColor = new Vector4();
                break;
        }
        currentColor = new Vector4(cloudColor.x, cloudColor.y, cloudColor.z, 1);
        colorIncrement = new Vector4();
        colorIncrement.w = 0;
        colorIncrement.x = (1 - currentColor.x) / lifePoints;
        colorIncrement.y = (1 - currentColor.y) / lifePoints;
        colorIncrement.z = (1 - currentColor.z) / lifePoints;
        gameObject.GetComponent<SpriteRenderer>().color = currentColor;
    }

    void die(){
        lvlObject.cloudNumber--;
        active = false;
        Destroy(gameObject);
    }

    public void takeDamage(int damage){
        Debug.Log("Health: " + health);
        health -= damage;
        currentColor += colorIncrement;
        gameObject.GetComponent<SpriteRenderer>().color = currentColor;
        if (health < 1)
            die();
    }

    Vector3 randPosition(GameObject gota){
        float widthGota = gota.GetComponent<Renderer>().bounds.size.x / 2;
        float widthNube = gameObject.GetComponent<Renderer>().bounds.size.x / 2;
        float rangeA = localPosition.x - widthNube + widthGota;
        float rangeB = localPosition.x + widthNube - widthGota;
        return new Vector3(Random.Range(rangeA,rangeB), localPosition.y, localPosition.z);
    }

    void spawnDroplet(){
        GameObject instancia = (GameObject)Instantiate(Resources.Load("Prefabs/gota"), transform.localPosition, Quaternion.identity);
        instancia.transform.localPosition = randPosition(instancia);
        instancia.transform.SetParent(transform);
    }

    void setPosition(int id, int numOfClouds){
        float camHalfHeight = Camera.main.orthographicSize;
        float camHalfWidth = Camera.main.aspect * camHalfHeight;
        float distance = (camHalfWidth * 2) / numOfClouds;
        float leftOffs = distance / 2;
        float xPos = -camHalfWidth + leftOffs + distance * id;
        float yPos = camHalfHeight - GetComponent<Renderer>().bounds.size.y / 2;
        transform.position = new Vector3(xPos,yPos,0);
    }
    
	// Use this for initialization
	public void initialize (int mc, int ptch, int lp, int id, int numOfClouds) {
        lvlObject = transform.parent.GetComponent<Level>();
        tag = "Cloud";
        //type = t;
        ptchReg = ptch;
        health = this.lifePoints = lp;
        notes = transform.parent.GetComponent<Level>().getNotes();
        assignColor();
        counter = 0;
        maxChildren = mc;
        setPosition(id, numOfClouds);
        localPosition = transform.position;
	}

    public void start(){
        active = true;
        //anim = GetComponent<Animator>();
        //beatObserver = GetComponent<BeatObserver>();
        //GameObject.Find("OnBeat").GetComponent<BeatCounter>().addObserver(gameObject);
    }

    void Update(){
        if (active){
            if (transform.childCount < maxChildren){
                /*if (counter < counterinterval)
                    counter++;
                else{*/
                    spawnDroplet();
                    counter = 0;
                //}
            }
        }
    }
	
}
