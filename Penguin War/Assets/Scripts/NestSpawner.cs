using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NestSpawner : MonoBehaviour
{
    public GameObject pinguPrefab;
    GameObject zoo;
    public int timeToSpawn = 300;
    public int delay = 1;
    Vector3 nestPos;


    void Start()
    {
        nestPos = new Vector3(this.transform.position.x, 0.1f, this.transform.position.z);
        zoo = GameObject.Find("Zoo");
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

            tmpPingu.GetComponent<PenguinLogic>().isBot = false;
            tmpPingu.GetComponent<PenguinLogic>().type = 0;

            timeToSpawn = 300;
        }
        timeToSpawn-=delay;
    }
}
