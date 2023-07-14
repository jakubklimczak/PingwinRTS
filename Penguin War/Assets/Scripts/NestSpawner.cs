using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class NestSpawner : MonoBehaviour
{
    public GameObject pinguPrefab;
    public GridLogic gridLogic;
    public int health = 1000;
    public int maxHealth = 1000;
    public int timeToSpawn = 3000;
    public int delay = 1;
    Vector3 nestPos;

    public bool isBot = false;

    public int timerXD = 1000;

    string statsFile = "Assets/Save/statsData.pingu";

    public int howManyIglos = 1;
    public int howManyPingus = 1;

    GameObject Inf;
    GameObject TM;
    GameObject Zoo;

    SoundEffectsPlayer sounds;

    bool finished = false;
    bool canPlymp = true;

    float expected_hp_mul = 1.0f;
    public GameObject endGamePanel;
    int timer = 300;
    bool finalCountDown = false;



    void Start()
    {
        nestPos = new Vector3(this.transform.position.x, 0.1f, this.transform.position.z);
        if(isBot == false)
        {
            loadStats();
        }

        Inf = GameObject.Find("Infrastructure");
        TM = GameObject.Find("TilesMap");
        Zoo = GameObject.Find("Zoo");
        sounds = GameObject.Find("CameraObject").GetComponent<SoundEffectsPlayer>();
        //endGamePanel = GameObject.Find("EndGamePanel");
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if(finalCountDown){
            timer--;
            if(timer<=0){
                timer = 300;
                finalCountDown = false;
                UnityEngine.SceneManagement.SceneManager.LoadScene("Menu");
            }
        }

        if(timeToSpawn <= 0)
        {
            StartCoroutine(CountIgloos());//and pingus

            if(howManyPingus < howManyIglos * 4)
            {
                System.Random rnd = new System.Random();
                Vector3 randPosAroundNest;
                int timeout_counter = 10;

                // DateTime currentTime = DateTime.Now;
                // string filename = currentTime.ToString().Replace(":","_");
                // gridLogic.SaveArrayToFile(gridLogic.map, filename);

                while (!gridLogic.IsTraversable(randPosAroundNest = new Vector3(Mathf.RoundToInt(nestPos.x) + rnd.Next(1, 2) * (rnd.Next(0, 2) == 0 ? 1 : -1),
                        Mathf.RoundToInt(nestPos.y), Mathf.RoundToInt(nestPos.z) + rnd.Next(1, 2) * (rnd.Next(0, 2) == 0 ? 1 : -1)))
                    ) 
                {
                    
                    timeout_counter--;
                    if (timeout_counter <= 0)
                    {
                        //zabi�em bo mnie wkurwia�o ;) Kuba
                        //Debug.Log("thank you, gud buy");
                        if(canPlymp == true)
                            sounds.ply_cant();
                        canPlymp = false;
                        return;
                    }
                        
                }
                 

                GameObject tmpPingu = Instantiate(pinguPrefab, randPosAroundNest, new Quaternion(), Zoo.transform);
                canPlymp = true;
                sounds.ply_spawned();


                tmpPingu.GetComponent<PenguinLogic>().isBot = isBot;
                tmpPingu.GetComponent<PenguinLogic>().type = 0;
                tmpPingu.GetComponent<PenguinLogic>().health = (int)(tmpPingu.GetComponent<PenguinLogic>().health * expected_hp_mul);
                tmpPingu.GetComponent<PenguinLogic>().maxHealth = (int)(tmpPingu.GetComponent<PenguinLogic>().maxHealth * expected_hp_mul);
                gridLogic.SetObjectAtPosition(randPosAroundNest, 13);
            }

            if(howManyPingus > howManyIglos * 4)
            {
                GameObject[] tmpGO = GameObject.FindGameObjectsWithTag("pingu");

                for(int i = 0; i < tmpGO.Length; i++)
                {
                    if(tmpGO[i].GetComponent<PenguinLogic>().isBot == isBot)
                    {
                        Destroy(tmpGO[i]);
                        break;
                    }
                }
            }

            timeToSpawn = 300;
        }
        timeToSpawn-=delay;


        if(health <=0 && !finished)
        {
            finished = true;
            if(isBot == true)
            {
                Debug.Log("You won!");
                sounds.ply_won();
                endGamePanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "You Won!";
                endGamePanel.SetActive(true);
                finalCountDown = true;
            }else
            {
                Debug.Log("You lost!");
                sounds.ply_lost();
                endGamePanel.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "You Lost!";
                endGamePanel.SetActive(true);
                finalCountDown = true;
            }
        }
    }

    void loadStats()
    {
        if (File.Exists(statsFile))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fileStream = File.Open(statsFile, FileMode.Open);
            statsData loadedData = (statsData)formatter.Deserialize(fileStream);
            fileStream.Close();

            NestSpawner koszary = GameObject.Find("koszary").GetComponent<NestSpawner>();
            NestSpawner koszaryEnemy = GameObject.Find("koszaryEnemy").GetComponent<NestSpawner>();

            koszary.delay=loadedData.info[0].delay;//0 is players
            koszary.health=loadedData.info[0].health;
            koszary.timeToSpawn=loadedData.info[0].timeToSpawn;

            koszaryEnemy.delay=loadedData.info[1].delay;//1 is enemys
            koszaryEnemy.health=loadedData.info[1].health;
            koszaryEnemy.timeToSpawn=loadedData.info[1].timeToSpawn;
        }
    }

    public void upgradeDefaultHP(float how_much)
    {
        this.expected_hp_mul += how_much;
    }
    [Serializable]
    public class statsInfoStruct
    {
        [SerializeField]
        public int health;
        [SerializeField]
        public int timeToSpawn;
        [SerializeField]
        public int delay;
        [SerializeField]
        public bool isBot;
    }

    [Serializable]
    public class statsData
    {
        [SerializeField]
        public List<statsInfoStruct> info = new List<statsInfoStruct>();
    }


    IEnumerator CountIgloos()
    {
        List<GameObject> tmpIgloos = new List<GameObject>();
        int tmpICount = 0;
        int tmpPCount = 0;

        //get igloos
        for(int i = 0; i < Inf.transform.childCount; i++)
        {
            if(Inf.transform.GetChild(i).name == "Igloo(Clone)" && Inf.transform.GetChild(i).GetComponent<HouseInfo>().isBot == isBot)
            {
                tmpICount++;
                yield return 0.1f;
            }
        }

        for(int i = 0; i < TM.transform.childCount; i++)
        {
            if(TM.transform.GetChild(i).name == "Igloo(Clone)" && TM.transform.GetChild(i).GetComponent<HouseInfo>().isBot == isBot)
            {
                tmpICount++;
                yield return 0.1f;
            }
        }

        //get pingus
        for(int i = 0; i < Zoo.transform.childCount; i++)
        {
            if(Zoo.transform.GetChild(i).GetComponent<PenguinLogic>().isBot == isBot)
            {
                tmpPCount++;
                yield return 0.1f;
            }
        }

        howManyIglos = tmpICount == 0 ? 1 : tmpICount;
        howManyPingus = tmpPCount == 0 ? 1 : tmpPCount;
    }
}
