using UnityEngine;
using SynchronizerData;

[RequireComponent(typeof(BeatObserver))]
public class DropletActions : MonoBehaviour {

    private Vector3 desplazamiento;
    private BeatObserver beatObserver;
    private GameObject objetoTexto;
    private int dropletType;
    private Animator anim;
    public string nota;
    private BeatCounter counter;
    Level lvlObject;

    private void createText(){
        objetoTexto = getNoteText();
        objetoTexto.transform.SetParent(transform);
        objetoTexto.GetComponent<Renderer>().sortingLayerID = this.GetComponent<Renderer>().sortingLayerID;
        objetoTexto.GetComponent<Renderer>().sortingOrder = this.GetComponent<Renderer>().sortingOrder + 1;
        objetoTexto.transform.localScale = new Vector3((float)0.1, (float)0.1, (float)0.1);
        objetoTexto.transform.localPosition = new Vector3((float)(-0.27 * objetoTexto.GetComponent<TextMesh>().text.Length),
                                                                    (float)-0.5,
                                                                    10);
    }

    void Start () {
        lvlObject = transform.parent.transform.parent.GetComponent<Level>();
        dropletType = Random.Range(1, 4); //Esto se usará en siguientes versiones para el tipo de gota
        tag ="Droplet";
        gameObject.name = lvlObject.newDropletID();
        GetComponent<SpriteRenderer>().color = transform.parent.GetComponent<CloudBehavior>().cloudColor; //Hacer la gota del color de la nube
        GetComponent<Rigidbody2D>().gravityScale = 0.02f;//Random.Range((float)0.05, (float)0.1); //Qué tan rápido puede caer la gota
        GetComponent<Renderer>().sortingOrder += 1; //Que esté una capa encima de la nube
        //desplazamiento = new Vector3(0, 0.00005f, 0);
        createText();
        //animacion
        try {
            GameObject.Find("quarterBeat").GetComponent<BeatCounter>().addObserver(gameObject); //Add to BeatCounter's observer
        }
        catch(System.NullReferenceException ex)
        {
            Debug.Log("Error: "+ex);
        }
        anim = GetComponent<Animator>();
        //anim.Stop();
        beatObserver = GetComponent<BeatObserver>();
        //beatObserver.;
    }

    void CheckNote(string note){
        if (note.ToUpper() == nota)
            destroyDroplet();
    }

    void BounceAnim(){
        //anim.Play("Bounce");
        
    }

    void destroyDroplet(){
        lvlObject.updateScore(10*(dropletType+lvlObject.updateCombo()));
        GetComponentInParent<CloudBehavior>().takeDamage(1);
        Camera.main.transform.FindChild(nota).GetComponent<AudioSource>().Play();
        Destroy(gameObject);
    }

    GameObject getNoteText()  {
        GameObject objetoTexto = new GameObject();
        TextMesh texto = objetoTexto.AddComponent<TextMesh>();
        texto.text = transform.parent.GetComponent<CloudBehavior>().giveMeTheNote();
        texto.fontSize = 80;
        texto.characterSize = 1;
        texto.lineSpacing = 0;
        texto.color = Color.white;
        nota = texto.text + transform.parent.GetComponent<CloudBehavior>().ptchReg;
        return objetoTexto;
    }

    // Update is called once per frame
    void Update () {

        if ((beatObserver.beatMask & BeatType.DownBeat) == BeatType.DownBeat)
        {
            anim.SetTrigger("BounceTrigger");
        }
        //transform.localRotation = Quaternion.identity;
        //BroadcastMessage("BounceAnim");
        //transform.position += desplazamiento;
    }
}
