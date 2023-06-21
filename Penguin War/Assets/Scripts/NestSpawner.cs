using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;


public class NestSpawner : MonoBehaviour
{
    public GameObject pinguPrefab;
    GameObject zoo;
    public int health = 1000;
    public int timeToSpawn = 300;
    public int delay = 1;
    Vector3 nestPos;

    public bool isBot = false;

    public int timerXD = 1000;

    string statsFile = "Assets/Save/statsData.pingu";


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
            System.Random rnd = new System.Random();
            Vector3 randPosAroundNest = new Vector3(nestPos.x + rnd.Next(1, 2) * (rnd.Next(0,2) == 0 ? 1 : -1),
            nestPos.y, nestPos.z + rnd.Next(1, 2) * (rnd.Next(0,2) == 0 ? 1 : -1));

            GameObject tmpPingu = Instantiate(pinguPrefab, randPosAroundNest, new Quaternion(), zoo.transform);

            tmpPingu.GetComponent<PenguinLogic>().isBot = isBot;
            tmpPingu.GetComponent<PenguinLogic>().type = 0;

            timeToSpawn = 300;
        }
        timeToSpawn-=delay;
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
}
