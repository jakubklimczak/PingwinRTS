using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpponentLogic : MonoBehaviour
{
    public bool isGameFinished = false;
    NestSpawner playerNest, enemyNest;

    bool hasFishingPinguin = false;
    bool hasIcingPinguin = false;
    bool hasScrappingPinguin = false;

    public Dictionary<string, int> enemyInv = new Dictionary<string, int>();
    public Dictionary<string, Dictionary<string, int>> costs;
    public GameObject[] houses;
    
    public int[,] map;

    System.Random random = new System.Random();
    

    void Start()
    {
        playerNest = GameObject.Find("koszary").GetComponent<NestSpawner>();
        enemyNest = GameObject.Find("koszary").GetComponent<NestSpawner>();

        costs = GameObject.Find("CursorObject").GetComponent<CursorMovementTracker>().costs;
        
        StartCoroutine(LateStart());

        map = GameObject.Find("Grid").GetComponent<GridLogic>().map;

        enemyInv.Add("wood", 100);
        enemyInv.Add("ice", 100);
        enemyInv.Add("fish", 1000);
        enemyInv.Add("scraps", 200);
        enemyInv.Add("ingots", 1000);
    }

    void spawnHouse(string houseName, int houseId){
        bool canPlace = true;

        //checks costs
        foreach (KeyValuePair<string, Dictionary<string, int>> houseCost in costs)
        {
            if (houseCost.Key == houseName)
            {
                foreach (KeyValuePair<string, int> resource in houseCost.Value)
                {
                    if (resource.Value + enemyInv[resource.Key] < 0)
                    {
                        canPlace = false;
                    }
                }
            }
        }

        if(canPlace){
            bool foundPlace = false;
            int x = 0, z = 0;
            while(!foundPlace){
                x = random.Next(140, 200);
                z = random.Next(140, 200);

                if(houseId == 4){
                    x = random.Next(120, 150);
                    z = random.Next(120, 150);
                }

                if(map[z,x] == 0)
                    foundPlace = true;

                if(houseId==4 && !(map[z + 1, x] == 3  || map[z - 1, x] == 3  || map[z, x + 1] == 3  || map[z, x - 1] == 3)){
                    foundPlace = false;
                }
            }

            Vector3 whereToPlace = new Vector3(x, 0.1f, z);
            Quaternion rotation = new Quaternion();


            if(houseId == 4){
                if(map[z + 1, x] == 3){//up
                    rotation = Quaternion.Euler(0, 90, 0);
                }else if(map[z - 1, x] == 3){//down
                    rotation = Quaternion.Euler(0, -90, 0);
                }else if(map[z, x + 1] == 3){//prawo
                    rotation = Quaternion.Euler(0, -180, 0);
                }else if(map[z, x - 1] == 3){//lewo
                    rotation = Quaternion.Euler(0, 0, 0);
                }
            }

            GameObject parent = GameObject.Find("Infrastructure");
            GameObject child = Instantiate(houses[houseId], whereToPlace, rotation, parent.transform);
            HouseInfo childHouseInfo = child.GetComponent<HouseInfo>();
            childHouseInfo.type = houses[houseId].name;
            childHouseInfo.isBot = true;

            map[z, x] = houseId + 1;


            //removing from inventory
            Dictionary<string, int> houseCost = costs[houseName];

            foreach (KeyValuePair<string, int> resource in houseCost)
            {
                enemyInv[resource.Key] += resource.Value;
            }
        }
    }

    IEnumerator ChooseAction()
    {
        while(!isGameFinished){

        int decisionNumber = random.Next(0, 6);

        switch(decisionNumber){
            case 0://placing houses
                int houseNumber = random.Next(0, 6);
                switch(houseNumber){
                    case 0://igloo
                        spawnHouse("igloo", 1);
                    break;

                    case 1://molo
                        spawnHouse("molo", 4);
                    break;

                    case 2://huta
                        spawnHouse("huta_igloo", 8);
                    break;

                    case 3://upgr
                        spawnHouse("upgr", 9);
                    break;
                }

            break;

            case 1:

                

            break;

            case 2:

                

            break;

            case 3:

                

            break;

            case 4:

                

            break;

            case 5:

                

            break;
        }



        /*

        if () //enemy has enough resources (we will pick a value) && random number in good range
        {
            //build a random thing they can afford (upgrades, buildings etc) - priority - igloo
            //buildings can be created near the opponents base (2 tiles from their last building AND away from the nest) OR in case of the bridge - nearest water tile
        }

        if() { //idle worker penguin
            // random - send for a resource bot has the least, make a warrior (smaller chance)
        }

        if() { //idle warrior
            //move near other warrior
        }

        if() { //enemy near && more than 1 warrior
            //send warriors near the enemy penguins
        }

        if() { //a lot of warriors in bots possesion
            //send warriors near the players base
        }

        if() { //bot has more than X of a resource (too much)
            //change one worker from that resource to one they have the least of
        }

        */

        //Debug.Log("XD");

        if(enemyNest.health <= 0 || playerNest.health <= 0)
        {
            isGameFinished = true;
        }

        yield return new WaitForSeconds(1f);
        }
    }

    IEnumerator LateStart()
    {
        yield return new WaitForSeconds(5f);// może nie działać bo chyba powinien czekać aż skończy spawnować bo mu dodaje do spawnowania XDD

        houses = GameObject.Find("Grid").GetComponent<GridLogic>().housesPrefabs;
        spawnHouse("igloo", 1);
        spawnHouse("molo", 4);
        spawnHouse("molo", 4);
        spawnHouse("molo", 4);
        spawnHouse("molo", 4);
        spawnHouse("molo", 4);
        spawnHouse("molo", 4);
        spawnHouse("molo", 4);
        spawnHouse("molo", 4);
        spawnHouse("molo", 4);
        spawnHouse("molo", 4);
        spawnHouse("molo", 4);

        StartCoroutine(ChooseAction());
    }
}
