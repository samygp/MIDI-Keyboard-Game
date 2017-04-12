using UnityEngine;
using System.Text;
using System.IO;
using System.Collections.Generic;
using SynchronizerData;
using System;
using System.Collections;

public class Level : MonoBehaviour {
    private int dropletCounter, graceCounter=-1;
    public bool active, isLoading;
    private bool synchLoaded=false, errors=false;
    BeatSynchronizer sync;
    BeatObserver beatObserver;
    float bpm;
    public bool ingraceperiod;
    public int combo, score, songLength, cloudNumber;
    public float comboUpdater, waterHeight;
    public string level, song, bgspritename, key;
    GameObject scoreboard, synchronizer;
    BeatCounter qb, gracePeriod; //qb = QuarterBeat
    List<string[]> clouds;
    /// <summary>
    /// Lee el archivo de texto plano, obtiene la tonalidad, el nombre de la pista,
    /// el nombre del background sprite, los metadatos de las nubes.
    /// </summary>
    List<string[]> readFile(){
        try {//Lire le fichier pour retourner les valeurs basiques et créer les nuages
            StreamReader reader = new StreamReader(level, Encoding.Default);
            key = reader.ReadLine().Trim(); //Tonalidad
            song = "Sounds/Music/" + reader.ReadLine().Trim(); //Pista
            bgspritename = "Sprites/Fondo/"+reader.ReadLine().Trim(); //Sprite de fondo
            clouds = new List<string[]>();
            string temp = (string)reader.ReadLine().Trim();
            int i = 0;
            while(temp != "___") {
                clouds.Add(temp.Split(';'));
                temp = (string)reader.ReadLine().Trim();
                i++;
            }
            cloudNumber = i;
            return clouds;
        }
        catch (System.Exception e) {
            Debug.LogError("Error al leer el archivo"+e.Message);
            errors = true;
        }
        return new List<string[]>();
    }

    /// <summary>
    /// Regresa una lista con las notas correspondientes a la tonalidad.
    /// </summary>
    public List<string> getNotes() {
        List<string> x;
        switch (key) {
            case "C":
                x = new List<string> { "C","D","E","F","G","A","B" };
                break;
            case "Cblues":
                x = new List<string> { "C", "D#", "F", "F#", "G", "A#" };
                break;
            case "C#":
                x = new List<string> { "C#", "D#", "F", "F#", "G#", "A#", "C" };
                break;
            default:
                return new List<string>();
        }
        return x;
    }

    /// <summary>
    /// Crea un objeto tipo nube.
    /// </summary>
    void spawnCloud(int id){
        GameObject instancia = (GameObject)Instantiate(Resources.Load("Prefabs/SpawnerCloud"));
        instancia.transform.SetParent(transform);
        int pitchReg = System.Convert.ToInt16(clouds[id][0]);
        int maxChildren = System.Convert.ToInt16(clouds[id][1]);
        int lifePoints = System.Convert.ToInt16(clouds[id][2]);
        instancia.GetComponent<CloudBehavior>().initialize(maxChildren, pitchReg, lifePoints, id, cloudNumber);
    }

    /// <summary>
    /// Carga un nuevo objeto tipo BackGSprite, con la imagen que se obtuvo de la lectura del archivo.
    /// </summary>
    void loadBackground(){
        GameObject fondo = new GameObject("BackGSprite");
        SpriteRenderer spr = fondo.AddComponent<SpriteRenderer>();
        spr.sortingLayerName = "Background";
        spr.sprite = Resources.Load<Sprite>(bgspritename);
        fondo.transform.SetParent(transform);
        fondo.transform.localPosition = new Vector3(0, (float)2.3);
        fondo.transform.localScale = new Vector3((float)0.5, (float)0.5);
    }

    bool loadLevel(){
        if (readFile().Count < 1)
            return false;
        loadBackground();
        for (int i=0; i<clouds.Count; i++)//Carga las nubes según la descripción de cada una en el archivo
            spawnCloud(i);

        return true;
    }
    // Use this for initialization


    /// <summary>
    /// Obtiene las notas correspondientes a la escala y carga los archivos mp3 del registro del teclado correspondiente a la nube.
    /// </summary>
    /// <param name="register">El registro del teclado al que corresponden las notas.</param>
    bool loadNotes(int register){
        try{
            List<string> notes = getNotes();
            foreach(string note in notes){
                GameObject noteObj = (GameObject)Instantiate(Resources.Load("Prefabs/NotePlayer"));
                noteObj.transform.SetParent(Camera.main.transform);
                noteObj.name = note + register;
                AudioSource noteSource = noteObj.GetComponent<AudioSource>();
                noteSource.clip = (AudioClip)Resources.Load("Sounds/Notes/" + noteObj.name);
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log(ex.Message);
            return false;
        }
        return true;
    }

    /// <summary>
    /// Prueba a que ya exista un sincronizador en la escena, si no existe esperará 1 segundo
    /// </summary>
    IEnumerator LoadSynchronizer(){
        Debug.Log("Cargando Synch");
        try{
            sync = GameObject.Find("synchronizer").GetComponent<BeatSynchronizer>();
            sync.bgMusic = GameObject.Find("synchronizer").GetComponent<AudioSource>();
            synchLoaded = true;
        }catch (NullReferenceException ex) {
            Debug.Log("No ha cargado el sincronizador"+ex);
            synchLoaded = false;
        }
        if (synchLoaded){
            LoadMusic();
            yield return null;
        }
        yield return new WaitForSeconds(1f);
    }

    /// <summary>
    /// Carga los BeatCounters del nivel
    /// </summary>
    private void LoadCounters(){
        GameObject qbcounter = (GameObject)Instantiate(Resources.Load("Prefabs/BeatCounter"));
        qbcounter.name = "quarterBeat";
        qb = qbcounter.GetComponent<BeatCounter>();
        BeatSynchronizer.OnAudioStart += qb.StartBeatCheck;
        GameObject gpcounter = (GameObject)Instantiate(Resources.Load("Prefabs/BeatCounter"));
        gpcounter.name = "gracePeriodCounter";
        gracePeriod = gpcounter.GetComponent<BeatCounter>();
        gracePeriod.beatValue = BeatValue.SixteenthBeat;
        gracePeriod.beatType = BeatType.DownBeat;
        BeatSynchronizer.OnAudioStart += gracePeriod.StartBeatCheck;
        Instantiate(Resources.Load("Prefabs/GracePeriodUpdater"));
        //Esto guarda la referencia en el objeto del nivel del agua, para que en contacto sepa de dónde
        //eliminar las gotas de la lista para evitar referencias nulas.
        GameObject.Find("WaterLevel").GetComponent<DestroyOnContact>().qb = qb.GetComponent<BeatCounter>();
    }

    /// <summary>
    /// Carga la música de fondo y notas carga los BeatCounters si carga exitosamente los sonidos.
    /// </summary>
    private void LoadMusic(){
        if (synchLoaded){
            Debug.Log("Cargando musica");
            if (sync.loadMusic(song))  {//loadMusic carga la pista a memoria
                GameObject[] nuages = GameObject.FindGameObjectsWithTag("Cloud");//Encuentra todas las nubes
                bool goodNotes = true;
                for (int i = 2; i < 6 && goodNotes; i++){
                    goodNotes = loadNotes(i);//Carga las notas de los 4 registros de notas  a memoria devuelve true si no hubo errores
                }
                if (goodNotes){
                    LoadCounters();
                    foreach (GameObject cloud in nuages)
                        cloud.GetComponent<CloudBehavior>().start();
                    isLoading = false;
                    active = true;
                }
            }
            else{
                errors = true;
            }
        }
        else{
            errors = true;
        }
    }

    void Start () {
        isLoading = true;
        gameObject.name = "Level";
        active = false;
        score = 0;
        combo = 1;
        createScoreBoard(0);
        waterHeight = GameObject.Find("WaterLevel").transform.position.y;
        level = Application.dataPath + "/Resources/Levels/test.txt";//Ruta del nivel que se intenta cargar
        if (loadLevel()){//Lee la información del archivo, las nubes que tendrá, la escala, nombre de la pista, de la imagen de fondo y así
            synchronizer = (GameObject)Instantiate(Resources.Load("Prefabs/Synchronizer"));
            synchronizer.name = "synchronizer";
            StartCoroutine("LoadSynchronizer");
        }
    }
    
    /// <summary>
    /// Inicializa los valores del scoreboard
    /// </summary>
    void createScoreBoard(int initScore){
        Vector3 screenPosition = new Vector3(10, Camera.main.pixelHeight-45, 20);
        scoreboard = GameObject.Find("ScoreText");
        TextMesh texto = scoreboard.GetComponent<TextMesh>();
        texto.fontSize = 60;
        texto.characterSize = (float)0.1;
        texto.lineSpacing = 0;
        texto.color = Color.white;
        texto.text = "Score: " + initScore + " Combo: " + combo;
        scoreboard.transform.SetParent(transform);
        scoreboard.GetComponent<Renderer>().sortingLayerName = "Foreground";
        scoreboard.GetComponent<Renderer>().sortingOrder = 99;
        scoreboard.transform.position = Camera.main.ScreenToWorldPoint(screenPosition);
    }

    /// <summary>
    /// Actualiza el valor del multiplicador del combo
    /// </summary>
    public int updateCombo() {

        if (ingraceperiod)
            combo += 1;
        return combo;
    }

    public void resetCombo()
    {
        combo = 1;
    }

    /// <summary>
    /// Actualiza el marcador de puntuación
    /// </summary>
    public void updateScore(int addValue) {
        score += addValue;
        scoreboard.GetComponent<TextMesh>().text = "Score: " + score + " Combo: "+combo;
    }

    /// <summary>
    /// Las gotas tienen un ID secuencial, este método lo aumenta.
    /// </summary>
    public string newDropletID(){
        return dropletCounter++.ToString();
    }

    /// <summary>
    /// Destruye las gotas e inactiva las nubes.
    /// </summary>
    private void destroyEverything(){
        GameObject[] nubes = GameObject.FindGameObjectsWithTag("Cloud");
        for (int i = 0; i < nubes.Length; i++)
            nubes[i].GetComponent<CloudBehavior>().active = false;

        GameObject[] gotas = GameObject.FindGameObjectsWithTag("Droplet");
        for (int i = 0; i < gotas.Length; i++){
            qb.observers.Remove(gotas[i]);
            Destroy(gotas[i]);
        }
    }

    void Update(){
        if (active){
            if (waterHeight > -6.5) {
                Debug.Log("FIN");//Application.Quit();
                destroyEverything();
                active = false;
                scoreboard.GetComponent<TextMesh>().text = "Final Score: " + score + " Perdiste";
                Debug.Break();
            }
            else if (cloudNumber < 1){
                Debug.Log("FIN");//Application.Quit();
                scoreboard.GetComponent<TextMesh>().text = "Final Score: " + score + " Ganaste";
                Debug.Break();
            }
        }
    }
    
}
