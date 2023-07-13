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

    int maxIce = 200;
    int maxFish = 300;
    int maxWood = 200;
    int maxScraps = 50;

    public Dictionary<string, int> enemyInv = new Dictionary<string, int>();
    public Dictionary<string, Dictionary<string, int>> costs;
    public GameObject[] houses;
    
    public int[,] map;

    System.Random random = new System.Random();
    int numOfPingus;

    int resourceTimer = 10;
    

    void Start()
    {
        playerNest = GameObject.Find("koszary").GetComponent<NestSpawner>();
        enemyNest = GameObject.Find("koszary").GetComponent<NestSpawner>();
        numOfPingus = enemyNest.howManyPingus;

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


    void findPenguinWork(bool worker)
    {
        if(worker){
            GameObject[] houses = GameObject.FindGameObjectsWithTag("houses");

            int resourceDecision = random.Next(0, 3);
            List<GameObject> resourceStations = new List<GameObject>();
            List<GameObject> pingusWithoutWork = new List<GameObject>();
            List<Vector3> workStationsPoints = new List<Vector3>();

            GameObject[] pingus = GameObject.FindGameObjectsWithTag("pingu");

            for(int i = 0; i < pingus.Length; i++){
                Vector3 dest = pingus[i].GetComponent<PenguinLogic>().destination;
                float distance = Vector3.Distance(dest, enemyNest.transform.position);
                if(distance > 10 && pingus[i].GetComponent<PenguinLogic>().isBot){
                    pingusWithoutWork.Add(pingus[i]);
                }
            }

            switch(resourceDecision){
                case 0://fishing
                    for(int i = 0; i < houses.Length; i++){
                        if(houses[i].name == "molo(Clone)" && houses[i].GetComponent<HouseInfo>().isBot){
                            resourceStations.Add(houses[i]);
                        }
                    }
                break;
                    
                case 1://icing
                    for(int i=199;i>100 ;i--){
                        for(int j=199;j>100 ;j--){
                            if(map[i,j] == 5)
                                workStationsPoints.Add(new Vector3(j, 0.1f, i));
                        }
                    }
                break;
                    
                case 2://scrapping
                    for(int i=199;i>100 ;i--){
                        for(int j=199;j>100 ;j--){
                            if(map[i,j] == 4)
                                workStationsPoints.Add(new Vector3(j, 0.1f, i));
                        }
                    }
                break;
            }

            if(resourceDecision == 0){
                int r = random.Next(0, resourceStations.Count);
                int p = random.Next(0, pingusWithoutWork.Count);

                if(resourceStations.Count == 0 || pingusWithoutWork.Count == 0)
                {
                    return;
                }


                hasFishingPinguin = true;

                pingusWithoutWork[p].GetComponent<PenguinLogic>().destination = resourceStations[r].transform.position;
                pingusWithoutWork[p].GetComponent<PenguinLogic>().shouldAttack = true;
                pingusWithoutWork[p].GetComponent<PenguinLogic>().houseToAttack = resourceStations[r];
            }else if (resourceDecision == 1)
            {
                int r = random.Next(0, workStationsPoints.Count);
                int p = random.Next(0, pingusWithoutWork.Count);

                if(resourceStations.Count == 0 || pingusWithoutWork.Count == 0)
                {
                    return;
                }

                hasIcingPinguin = true;

                pingusWithoutWork[p].GetComponent<PenguinLogic>().destination = workStationsPoints[r];
                pingusWithoutWork[p].GetComponent<PenguinLogic>().shouldAttack = true;

            }else if(resourceDecision == 2){
                int r = random.Next(0, workStationsPoints.Count);
                int p = random.Next(0, pingusWithoutWork.Count);

                if(resourceStations.Count == 0 || pingusWithoutWork.Count == 0)
                {
                    return;
                }

                hasScrappingPinguin = true;

                pingusWithoutWork[p].GetComponent<PenguinLogic>().destination = workStationsPoints[r];
                pingusWithoutWork[p].GetComponent<PenguinLogic>().shouldAttack = true;
            }

        }else if(!worker && enemyInv["ingots"] > 5){
            GameObject[] houses = GameObject.FindGameObjectsWithTag("houses");
            List<GameObject> resourceStations = new List<GameObject>();
            List<GameObject> pingusWithoutWork = new List<GameObject>();
            GameObject[] pingus = GameObject.FindGameObjectsWithTag("pingu");

            for(int i = 0; i < pingus.Length; i++){
                Vector3 dest = pingus[i].GetComponent<PenguinLogic>().destination;
                float distance = Vector3.Distance(dest, enemyNest.transform.position);
                if(distance > 10 && pingus[i].GetComponent<PenguinLogic>().isBot){
                    pingusWithoutWork.Add(pingus[i]);
                }
            }

            for(int i = 0; i < houses.Length; i++){
                if(houses[i].name == "upgr(Clone)" && houses[i].GetComponent<HouseInfo>().isBot){
                    resourceStations.Add(houses[i]);
                }
            }

            int r = random.Next(0, resourceStations.Count);
            int p = random.Next(0, pingusWithoutWork.Count);

            if(resourceStations.Count == 0 || pingusWithoutWork.Count == 0)
            {
                return;
            }

            hasFishingPinguin = true;

            pingusWithoutWork[p].GetComponent<PenguinLogic>().destination = resourceStations[r].transform.position;
            pingusWithoutWork[p].GetComponent<PenguinLogic>().shouldAttack = true;
            pingusWithoutWork[p].GetComponent<PenguinLogic>().houseToAttack = resourceStations[r];
        }
    }
    
    void moveWarrior(){
        List<GameObject> pingusWarriors = new List<GameObject>();

        GameObject[] pingus = GameObject.FindGameObjectsWithTag("pingu");

        for(int i = 0; i < pingus.Length; i++){
            if(pingus[i].GetComponent<PenguinLogic>().isBot && pingus[i].GetComponent<PenguinLogic>().isWarrior){
                pingusWarriors.Add(pingus[i]);
            }
        }

        int first = random.Next(0, pingusWarriors.Count);
        int second = random.Next(0, pingusWarriors.Count);

        if(first != second){
            Vector3 dest = pingusWarriors[first].transform.position;
            Vector3 finalDest = new Vector3(dest.x +random.Next(0,4), 0.1f, dest.z +random.Next(0,4));
            pingusWarriors[first].GetComponent<PenguinLogic>().destination = finalDest;
        }
    }

    void sendWarriorsOver(){
        List<GameObject> pingusWarriors = new List<GameObject>();

        GameObject[] pingus = GameObject.FindGameObjectsWithTag("pingu");

        for(int i = 0; i < pingus.Length; i++){
            if(pingus[i].GetComponent<PenguinLogic>().isBot && pingus[i].GetComponent<PenguinLogic>().isWarrior){
                pingusWarriors.Add(pingus[i]);
            }
        }

        if(pingusWarriors.Count > 20){
            for(int i = 0; i < pingusWarriors.Count; i++){
                pingusWarriors[i].GetComponent<PenguinLogic>().destination = new Vector3(30, 30); // nwm walić
            }
        }
    }

    void driveBy(){
        List<GameObject> pingusWarriors = new List<GameObject>();
        List<GameObject> playersPenguins = new List<GameObject>();

        GameObject[] pingus = GameObject.FindGameObjectsWithTag("pingu");

        for(int i = 0; i < pingus.Length; i++){
            if(pingus[i].GetComponent<PenguinLogic>().isBot && pingus[i].GetComponent<PenguinLogic>().isWarrior){
                pingusWarriors.Add(pingus[i]);
            }else if(!pingus[i].GetComponent<PenguinLogic>().isBot){
                playersPenguins.Add(pingus[i]);
            }
        }

        for(int i = 0; i < playersPenguins.Count; i++){
            if(playersPenguins[i].transform.position.x > 150 && playersPenguins[i].transform.position.z > 150){
                int whichOnes = random.Next(0, pingusWarriors.Count - 2);

                if(pingusWarriors.Count > 10){
                    pingusWarriors[whichOnes].GetComponent<PenguinLogic>().destination = playersPenguins[i].transform.position;
                    pingusWarriors[whichOnes].GetComponent<PenguinLogic>().shouldAttack = true;
                    pingusWarriors[whichOnes].GetComponent<PenguinLogic>().houseToAttack = playersPenguins[i];

                    pingusWarriors[whichOnes + 1].GetComponent<PenguinLogic>().destination = playersPenguins[i].transform.position;
                    pingusWarriors[whichOnes + 1].GetComponent<PenguinLogic>().shouldAttack = true;
                    pingusWarriors[whichOnes + 1].GetComponent<PenguinLogic>().houseToAttack = playersPenguins[i];
                }
            }
        }
    }
    
    IEnumerator ChooseAction()
    {
        while(!isGameFinished){

        int decisionNumber = random.Next(0, 6);


        if(resourceTimer > 0){
            resourceTimer--;
        }else if(resourceTimer == 0){
            resourceTimer = 10;
            int resourceId = random.Next(0,4);
            if(resourceId == 0 && hasFishingPinguin){
                enemyInv["fish"]+=1;
                enemyInv["wood"]+=1;
            }else if(resourceId == 1 && hasIcingPinguin){
                enemyInv["ice"]+=1;
            }else if(resourceId == 2 && hasScrappingPinguin){
                enemyInv["scraps"]+=1;
            }else if(resourceId == 3 && hasScrappingPinguin && hasIcingPinguin && hasFishingPinguin){
                if(enemyInv["scraps"]>10){
                    enemyInv["scraps"]-=5;
                    enemyInv["wood"]-=5;
                    enemyInv["ingots"]+=1;
                }
            }
        }

        switch(decisionNumber){
            case 0://placing houses
                int houseNumber = random.Next(0, 5);
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
                int penguinType = random.Next(0, 2);
                switch(penguinType){
                    case 0://go arbeit
                    case 1:
                        findPenguinWork(true);
                    break;

                    case 2://make warrior
                        findPenguinWork(false);
                    break;
                }
            break;
                
            case 2:
                moveWarrior();
            break;

            case 3:
                sendWarriorsOver();
            break;

            case 4:
                driveBy();
            break;
        }

        if(enemyNest.health <= 0 || playerNest.health <= 0)
        {
            isGameFinished = true;
        }

        yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator LateStart()
    {
        yield return new WaitForSeconds(5f);// może nie działać bo chyba powinien czekać aż skończy spawnować bo mu dodaje do spawnowania XDD

        houses = GameObject.Find("Grid").GetComponent<GridLogic>().housesPrefabs;
        spawnHouse("igloo", 1);

        StartCoroutine(ChooseAction());
    }
}
