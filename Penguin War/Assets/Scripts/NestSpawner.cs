using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;


public class NestSpawner : MonoBehaviour
{
    public GameObject pinguPrefab;
    GameObject zoo;
    public int health = 1000;
    public int timeToSpawn = 3000;
    public int delay = 1;
    Vector3 nestPos;

    public bool isBot = false;

    public int timerXD = 1000;

    string statsFile = "Assets/Save/statsData.pingu";

    public int howManyIglos = 0;
    public int howManyPingus = 0;



    void Start()
    {
        nestPos = new Vector3(this.transform.position.x, 0.1f, this.transform.position.z);
        zoo = GameObject.Find("Zoo");
        if(isBot == false)
        {
            loadStats();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {

        if(timeToSpawn <= 0)
        {
            CountIgloos();//and pingus

            if(howManyPingus < howManyIglos * 4)
            {
                System.Random rnd = new System.Random();
                Vector3 randPosAroundNest = new Vector3(nestPos.x + rnd.Next(1, 2) * (rnd.Next(0,2) == 0 ? 1 : -1),
                nestPos.y, nestPos.z + rnd.Next(1, 2) * (rnd.Next(0,2) == 0 ? 1 : -1));

                GameObject tmpPingu = Instantiate(pinguPrefab, randPosAroundNest, new Quaternion(), zoo.transform);

                tmpPingu.GetComponent<PenguinLogic>().isBot = isBot;
                tmpPingu.GetComponent<PenguinLogic>().type = 0;
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


        if(health <=0)
        {
            if(isBot == true)
            {
                Debug.Log("You won!");
            }else
            {
                Debug.Log("You lost!");
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


    void CountIgloos()
    {
        List<GameObject> tmpIgloos = new List<GameObject>();
        int tmpICount = 0;
        int tmpPCount = 0;

        GameObject Inf = GameObject.Find("Infrastructure");
        GameObject TM = GameObject.Find("TilesMap");
        GameObject Zoo = GameObject.Find("Zoo");

        //get igloos
        for(int i = 0; i < Inf.transform.childCount; i++)
        {
            if(Inf.transform.GetChild(i).name == "Igloo(Clone)" && Inf.transform.GetChild(i).GetComponent<HouseInfo>().isBot == isBot)
            {
                tmpICount++;
            }
        }

        for(int i = 0; i < TM.transform.childCount; i++)
        {
            if(TM.transform.GetChild(i).name == "Igloo(Clone)" && TM.transform.GetChild(i).GetComponent<HouseInfo>().isBot == isBot)
            {
                tmpICount++;
            }
        }

        //get pingus
        for(int i = 0; i < Zoo.transform.childCount; i++)
        {
            if(Zoo.transform.GetChild(i).name == "BasicPenguin(Clone)" && Zoo.transform.GetChild(i).GetComponent<PenguinLogic>().isBot == isBot)
            {
                tmpPCount++;
            }
        }

        howManyIglos = tmpICount;
        howManyPingus = tmpPCount;

        //Debug.Log("B" + isBot + " : " + tmpCount);
    }
}
