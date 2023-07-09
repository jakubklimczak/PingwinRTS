using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using static Inventory;
using static NestSpawner;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UIElements;
using System.Linq;

public class GridLogic : MonoBehaviour
{
    const int worldSize = 200;
    public GameObject[] housesPrefabs = new GameObject[15];
    public GameObject[] pinguinsPrefabs = new GameObject[5];
    [SerializeField]

    public PinguData SavedPinguins = new PinguData();
    public HouseData Savedhouses = new HouseData();
    

    string penguinFilePath = "Assets/Save/penguinData.pingu";
    string houseFilePath = "Assets/Save/housesData.pingu";
    string statsFilePath = "Assets/Save/statsData.pingu";

    //This is the grid for 'placed' objects
    //public int[,] grid = new int[worldSize, worldSize];
    /*
        -1 is beyond the island
        0 is a empty space
        1 is a igloo etc
    */


    //This is for the tiles of the map
    public int[,] map = new int[worldSize, worldSize];
    /*
        0 means a empty space
        1 is a normal tile
        2 is a border etc
   */

// Start is called before the first frame update
void Start()
    {
        string mode = PlayerPrefs.HasKey("GameMode") ? PlayerPrefs.GetString("GameMode") : "newGame";

        if(mode=="newGame")
        {
            if (File.Exists(penguinFilePath))
            {
                File.Delete(penguinFilePath);
            }

            if (File.Exists(houseFilePath))
            {
                File.Delete(houseFilePath);
            }

            if (File.Exists(statsFilePath))
            {
                File.Delete(statsFilePath);
            }

            GenerateCsv("Assets/Save/map.csv");
            PrepareGame();
        }else if(mode=="loadGame")
        {
           PrepareGame();
        }
        //IEnumerator PinguDataCor = UpdateAllPenguinePos(.2f);
        //StartCoroutine(PinguDataCor);
        //UpdateAllPenguinePos(.2f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            Debug.Log("Saving");
            SaveWorld("Assets/Save/map.csv");
        }

        UpdateAllPenguinePos(.2f);
    }

    public Texture2D LoadPNG(string filePath)//load a texture from png
    {
        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath))
        {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData);
        }
        else 
        {
            Debug.Log("Can't find the file!");
        }
        return tex;
    }


    public void PrepareGame()
    {
        //reads the save "map.csv"
        using (var reader = new StreamReader("Assets/Save/map.csv"))
        {
            int iter = 0;
            while (!reader.EndOfStream)
            {
                var line = reader.ReadLine();
                var values = line.Split(',');

                for(int i=0; i < values.Length - 1; i++)
                {
                    map[iter, i] = int.Parse(values[i]);
                }
                iter++;
            }
        }

        //spawns read data
        StartCoroutine(SpawnHouses());

        //read Pinguin Data
        if (File.Exists(penguinFilePath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fileStream = File.Open(penguinFilePath, FileMode.Open);
            PinguData loadedData = (PinguData)formatter.Deserialize(fileStream);
            fileStream.Close();
            GameObject zoo = GameObject.Find("Zoo");

            foreach(PinguInfoStruct p in loadedData.info)
            {
                SavedPinguins.info.Add(p);
                Vector3 tmpPos = new Vector3(p.position[0], p.position[1], p.position[2]);
                Vector3 tmpDest = new Vector3(p.destination[0], p.destination[1], p.destination[2]);
                Quaternion tmpRotation = new Quaternion(p.rotation[0], p.rotation[1], p.rotation[2], p.rotation[3]);
                GameObject tmpObj = Instantiate(pinguinsPrefabs[p.type], tmpPos, tmpRotation, zoo.transform);
                tmpObj.GetComponent<PenguinLogic>().destination = tmpDest;
                tmpObj.GetComponent<PenguinLogic>().isBot = p.isBot;
                tmpObj.GetComponent<PenguinLogic>().shouldAttack = p.shoudlAttack;
                tmpObj.GetComponent<PenguinLogic>().isWarrior = p.isWarrior;
                tmpObj.GetComponent<PenguinLogic>().damage = p.damage;

                SetObjectAtPosition(tmpPos, 13);
            }
        }
    }
    public IEnumerator SpawnHouses()//change to spawn houses and other stuff          and later mobs
    {
        GameObject parent = GameObject.Find("TilesMap");
        HouseData loadedData = new HouseData();

        if (File.Exists(houseFilePath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fileStream = File.Open(houseFilePath, FileMode.Open);
            loadedData = (HouseData)formatter.Deserialize(fileStream);

            foreach(HouseInfoStruct h in loadedData.info)
            {
                Savedhouses.info.Add(h);
            }

            fileStream.Close();
        }

        for(int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                int cur_mp_id = map[j, i];
                if (cur_mp_id != 0 && cur_mp_id != 3 && cur_mp_id != 4 && cur_mp_id != 6 && cur_mp_id != 7 && cur_mp_id != 13) //pingwin to 13
                {
                    //string tmpPath = @"..\\Penguin War\\Assets\\Images\\Tiles\\tile0.png";
                    Vector3 tmpPos = new(Mathf.Floor(i), 0.1f, Mathf.Floor(j));

                    bool foundHouse = false;
                    HouseInfoStruct tmpInfoStruct = new HouseInfoStruct();

                    Quaternion tmpLoadedRotation = new Quaternion();

                    foreach (HouseInfoStruct h in loadedData.info)
                    {
                        Vector3 tmpLoadedPos = new Vector3((int)h.position[0], 0.1f, (int)h.position[2]);
                        tmpLoadedRotation = new Quaternion(h.rotation[0], h.rotation[1], h.rotation[2], h.rotation[3]);

                        if(tmpPos.x == h.position[0] && tmpPos.z == h.position[2])
                        {
                            foundHouse = true;
                            tmpInfoStruct = h;
                            break;
                        }
                    }

                    GameObject tmpObj = Instantiate(housesPrefabs[map[j, i] -1], tmpPos, tmpLoadedRotation, parent.transform);//change this later to be able to spawn more things
                    //tmpObj.transform.SetParent(parent.transform);
                    HouseInfo tmpHouseInfo = tmpObj.GetComponent<HouseInfo>();

                    if(foundHouse)
                    {
                        tmpHouseInfo.health = tmpInfoStruct.health;
                        tmpHouseInfo.resources = tmpInfoStruct.resources;
                        tmpHouseInfo.isBot = tmpInfoStruct.isBot;
                    }

                    tmpHouseInfo.type = housesPrefabs[map[j, i] - 1].name;
                    //Debug.Log(tmpPos + " , " + tmpHouseInfo.type + " , " + tmpObj.transform.position);
                    //tmpObj.GetComponent<Renderer>().material.mainTexture = LoadPNG(tmpPath);
                }
            }
            yield return new WaitForSeconds(0);
        }
    }

    public void SaveWorld(string filename)
    {
        try//delete file so it doesn't store old values
        {
            if (File.Exists(penguinFilePath))
            {
                File.Delete(penguinFilePath);
            }

            if (File.Exists(houseFilePath))
            {
                File.Delete(houseFilePath);
            }

            if (File.Exists(statsFilePath))
            {
                File.Delete(statsFilePath);
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine("An error occurred: " + ex.Message);
        }

        // Write the data to a CSV file
        using (var writer = new StreamWriter(filename))
        {
            for (int i = 0; i < worldSize; i++)
            {
                for (int j = 0; j < worldSize; j++)
                {
                    writer.Write(map[i, j]);
                    if (j < worldSize - 1)
                    {
                        writer.Write(",");
                    }
                }
                writer.WriteLine();
            }
        }

        //save pinguins data
        GameObject[] pinguinsObjs = GameObject.FindGameObjectsWithTag("pingu");

        foreach(GameObject p in pinguinsObjs)
        {
            PinguInfoStruct tmp = new PinguInfoStruct();

            tmp.health = p.GetComponent<PenguinLogic>().health;
            tmp.position[0] = p.transform.position.x;
            tmp.position[1] = p.transform.position.y;
            tmp.position[2] = p.transform.position.z;

            tmp.rotation[0] = p.transform.rotation.x;
            tmp.rotation[1] = p.transform.rotation.y;
            tmp.rotation[2] = p.transform.rotation.z;
            tmp.rotation[3] = p.transform.rotation.w;

            tmp.destination[0] = p.GetComponent<PenguinLogic>().destination.x;
            tmp.destination[1] = p.GetComponent<PenguinLogic>().destination.y;
            tmp.destination[2] = p.GetComponent<PenguinLogic>().destination.z;

            tmp.type = p.GetComponent<PenguinLogic>().type;
            tmp.isBot = p.GetComponent<PenguinLogic>().isBot;
            tmp.isWarrior = p.GetComponent<PenguinLogic>().isWarrior;
            tmp.shoudlAttack = p.GetComponent<PenguinLogic>().shouldAttack;
            tmp.damage = p.GetComponent<PenguinLogic>().damage;
            
            SavedPinguins.info.Add(tmp);
        }

        BinaryFormatter formatterForPenguins = new BinaryFormatter();

        FileStream fileStreamforPenguins = File.Create(penguinFilePath);
        formatterForPenguins.Serialize(fileStreamforPenguins, SavedPinguins);
        fileStreamforPenguins.Close();

        // //save houses data
        GameObject[] housesObjs = GameObject.FindGameObjectsWithTag("houses");

        Savedhouses.info.Clear();

        foreach(GameObject h in housesObjs)
        {
            HouseInfoStruct tmp = new HouseInfoStruct();
            
            tmp.position[0] = h.transform.position.x;
            tmp.position[1] = h.transform.position.y;
            tmp.position[2] = h.transform.position.z;

            tmp.rotation[0] = h.transform.rotation.x;
            tmp.rotation[1] = h.transform.rotation.y;
            tmp.rotation[2] = h.transform.rotation.z;
            tmp.rotation[3] = h.transform.rotation.w;

            tmp.type = h.GetComponent<HouseInfo>().type;
            tmp.health = h.GetComponent<HouseInfo>().health;
            tmp.isBot = h.GetComponent<HouseInfo>().isBot;
            tmp.resources = h.GetComponent<HouseInfo>().resources;

            Savedhouses.info.Add(tmp);
        }

        BinaryFormatter formatterForHouses = new BinaryFormatter();

        FileStream fileStreamForHouses = File.Create(houseFilePath);
        formatterForHouses.Serialize(fileStreamForHouses, Savedhouses);
        fileStreamForHouses.Close();




        //save nest stats
        NestSpawner koszary = GameObject.Find("koszary").GetComponent<NestSpawner>();
        NestSpawner koszaryEnemy = GameObject.Find("koszaryEnemy").GetComponent<NestSpawner>();

        statsData savedStats = new statsData();

        statsInfoStruct koszaryStruct = new statsInfoStruct();
        statsInfoStruct koszaryEnemyStruct = new statsInfoStruct();

        koszaryStruct.isBot = false;
        koszaryStruct.health = koszary.health;
        koszaryStruct.delay = koszary.delay;
        koszaryStruct.timeToSpawn = koszary.timeToSpawn;
        savedStats.info.Add(koszaryStruct);

        koszaryEnemyStruct.isBot = true;
        koszaryEnemyStruct.health = koszaryEnemy.health;
        koszaryEnemyStruct.delay = koszaryEnemy.delay;
        koszaryEnemyStruct.timeToSpawn = koszaryEnemy.timeToSpawn;
        savedStats.info.Add(koszaryEnemyStruct);

        BinaryFormatter formatterForStats = new BinaryFormatter();

        FileStream fileStreamforStats = File.Create(statsFilePath);
        formatterForStats.Serialize(fileStreamforStats, savedStats);
        fileStreamforStats.Close();
    }

    public static void addSomething(ref int[,] array, int y, int x, int whatToPlaceThere, int howManyX, int howManyY)
    {
        for(int k = 0; k < howManyX ; k++)
        {
            for(int l = 0; l < howManyY ; l++)
            {
                array[x + k, y + l] = whatToPlaceThere;
            }
        }

    }
    public void GenerateCsv(string filename)
    {
        // Create a 200x200 array of zeros
        int[,] data = new int[worldSize, worldSize];

        // Set the values within the circle to 1
        for (int i = 0; i < worldSize; i++)
        {
            for (int j = 0; j < worldSize; j++)
            {
                /*double distance = Math.Sqrt(Math.Pow(i - 50, 2) + Math.Pow(j - 50, 2));
                if (distance < 25)
                {
                    data[i, j] = 1;
                }*/

                double distance1 = Math.Sqrt(Math.Pow(i - worldSize/2, 2) + Math.Pow(j - worldSize/2, 2));
                double distance2 = Math.Sqrt(Math.Pow(i - worldSize / 2 -5, 2) + Math.Pow(j - worldSize / 2 - 32, 2));
                double distance3 = Math.Sqrt(Math.Pow(i - worldSize / 2 +5, 2) + Math.Pow(j - worldSize / 2 + 32, 2));
                double distance4 = Math.Sqrt(Math.Pow(i - worldSize / 2 -2, 2) + Math.Pow(j - worldSize / 2 - 16, 2));
                double distance5 = Math.Sqrt(Math.Pow(i - worldSize / 2 +2, 2) + Math.Pow(j - worldSize / 2 + 16, 2));
                //lake
                // if (i==j)
                //     data[i, j] = 1;
                // else 
                if (distance1 < 30)
                {
                    data[i, j] = 3;
                }else if (distance2 < 26)
                {
                    data[i, j] = 3;
                }else if (distance3 < 26)
                {
                    data[i, j] = 3;
                }
                else if (distance4 < 26)
                {
                    data[i, j] = 3;
                }
                else if (distance5 < 26)
                {
                    data[i, j] = 3;
                }
                else//default
                    data[i, j] = 0;

            }
        }

        //nest
        data[10, 10] = 7;
        data[190, 190] = 7;

        //boat (x6 * z3) 
        int x = 30;
        int y = 150;

        for(int k = 0; k < 3 ; k++)
        {
            for(int l = 0; l < 6 ; l++)
            {
                data[x + k, y + l] = 4;
            }
        }

        x = 60;
        y = 20;
        for(int k = 0; k < 3 ; k++)
        {
            for(int l = 0; l < 6 ; l++)
            {
                data[x + k, y + l] = 4;
            }
        }

        x = 130;
        y = 40;
        for(int k = 0; k < 3 ; k++)
        {
            for(int l = 0; l < 6 ; l++)
            {
                data[x + k, y + l] = 4;
            }
        }

        x = 170;
        y = 130;
        for(int k = 0; k < 3 ; k++)
        {
            for(int l = 0; l < 6 ; l++)
            {
                data[x + k, y + l] = 4;
            }
        }

        //glaciers
       addSomething(ref data, 37, 11, 6, 3, 4);
       addSomething(ref data, 67, 31, 6, 3, 4);
       addSomething(ref data, 107, 15, 6, 3, 4);
       addSomething(ref data, 147, 6, 6, 3, 4);
       addSomething(ref data, 167, 45, 6, 3, 4);
       addSomething(ref data, 117, 62, 6, 3, 4);
       addSomething(ref data, 172, 102, 6, 3, 4);

       addSomething(ref data, 163, 189, 6, 3, 4);
       addSomething(ref data, 153, 149, 6, 3, 4);
       addSomething(ref data, 93, 185, 6, 3, 4);
       addSomething(ref data, 53, 194, 6, 3, 4);
       addSomething(ref data, 33, 155, 6, 3, 4);
       addSomething(ref data, 83, 138, 6, 3, 4);
       addSomething(ref data, 28, 98, 6, 3, 4);


        // Write the data to a CSV file
        using (var writer = new StreamWriter(filename))
        {
            for (int i = 0; i < worldSize; i++)
            {
                for (int j = 0; j < worldSize; j++)
                {
                    writer.Write(data[i, j]);
                    if (j < worldSize - 1)
                    {
                        writer.Write(",");
                    }
                }
                writer.WriteLine();
            }
        }
    }
    //=============================================GRID OPERATIONS===================================================================
    //can you move into given position
    public bool IsTraversable(Vector3 position) 
    {
        int chosen_object = map[Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.z)];
        //Debug.Log("at("+ position.x+";"+ position.z +") there is:"+ chosen_object);
        if (chosen_object == 0 || chosen_object == 4)
        {
            return true;
        }
        return false;
    }
    public int GetObjectAtPosition(Vector3 position) 
    {
        return map[(int)position.x, (int)position.z];
    }

    public void SetObjectAtPosition(Vector3 position, int object_type)
    {
        map[(int)position.x, (int)position.z] = object_type;
    }

    ////Brak sprawdzania!
    //public void UpdatePosition(Vector3 initial,Vector3 destination, int objectType) 
    //{
    //    //Debug.Log("destination: "+destination.x + " " + destination.z);
    //    //Debug.Log("source: " + initial.x + " " + initial.z);
    //    map[(int)destination.x, (int)destination.z] += objectType;
    //    map[(int)initial.x, (int)initial.z] -= objectType;
    //    if(map[(int)initial.x, (int)initial.z] < 0)
    //    {
    //        map[(int)initial.x, (int)initial.z] = 0;
    //    }
    //}
    //mapping penguins corutine
    //private IEnumerator UpdateAllPenguinePos(float period)
    void UpdateAllPenguinePos(float period)
    {
        //while (true)
        //{
            GameObject[] pinguinsObjs = GameObject.FindGameObjectsWithTag("pingu");
            int[,] new_map = new int [200,200];
            
            //Array.Copy(map, new_map, map.Length);

            for(int i = 0;i<200 ;i++)
            {
                for(int j = 0;j<200 ;j++)
                {
                    if(map[i,j]==13)
                    {
                        //Debug.Log("kurwa jest " + i + ", " + j);
                    }
                    new_map[i,j] = map[i,j];
                }
            }


            for(int i = 0; i < 200; i++)
            {
                for(int j = 0; j < 200; j++)
                {
                    if (map[j, i]==13)
                    {
                        //Debug.Log(map[j, i] + "at i:  " + i + " j: " + j);
                    }
                    new_map[j, i] = (new_map[j, i] == 13 || new_map[j, i] == 17) ? new_map[j, i] - 13 : new_map[j, i];
                    //Debug.Log(new_map[i, j]);
                    
                }
            }            


            // foreach (GameObject p in pinguinsObjs)
            // {
            //     if (IsTraversable(p.transform.position))
            //     {
            //         Debug.Log(p.transform.position);
            //         new_map[Mathf.RoundToInt(p.transform.position.x), Mathf.RoundToInt(p.transform.position.z)] += 13;
            //         Debug.Log(new_map[Mathf.RoundToInt(p.transform.position.x), Mathf.RoundToInt(p.transform.position.z)]);
            //     }
            // }


            for(int j = 0; j < pinguinsObjs.Length; j++)
            {
                if (new_map[Mathf.RoundToInt(pinguinsObjs[j].transform.position.x), Mathf.RoundToInt(pinguinsObjs[j].transform.position.z)] != 13)
                {
                    //Debug.Log(new_map[Mathf.RoundToInt(pinguinsObjs[j].transform.position.x), Mathf.RoundToInt(pinguinsObjs[j].transform.position.z)]);
                    new_map[Mathf.RoundToInt(pinguinsObjs[j].transform.position.x), Mathf.RoundToInt(pinguinsObjs[j].transform.position.z)] += 13;
                }
            }

            //Debug.Log(new_map[x,y]);
           
            //map = new_map;
            //Array.Copy(new_map, map, map.Length);
            
            for(int i = 0;i<200 ;i++)
            {
                for(int j = 0;j<200 ;j++)
                {
                    if(map[i,j]==13)
                    {
                        //Debug.Log("a tu już nie " + i + ", " + j);
                    }
                    map[i,j] = new_map[i,j];
                }
            }


            //Debug.Log("====");
            

            // DateTime currentTime = DateTime.Now;
            // string filename = currentTime.ToString().Replace(":","_");
            // SaveArrayToFile(new_map, filename);

            //yield return new WaitForSeconds(period);
        //}
    }

    //dkgkjldfklgdfjhgl
    public void SaveArrayToFile(int[,] array, string filename)
    {
        using (StreamWriter writer = new StreamWriter(filename))
        {
            int rows = array.GetLength(0);
            int columns = array.GetLength(1);

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < columns; j++)
                {
                    writer.Write(array[i, j]);

                    // Add a delimiter between elements if needed
                    if (j < columns - 1)
                    {
                        writer.Write(",");
                    }
                }

                writer.WriteLine(); // Move to the next line
            }
        }
    }
        //=====================

        [Serializable]
    public class PinguInfoStruct
    {
        [SerializeField]
        public int type;
        [SerializeField]
        public float[] position = new float[3];
        [SerializeField]
        public float[] rotation = new float[4];
        [SerializeField]
         public float[] destination = new float[3];
        [SerializeField]
        public int health;
        [SerializeField]
        public int damage;
        [SerializeField]
        public bool isBot;
        [SerializeField]
        public bool shoudlAttack;
        [SerializeField]
        public bool isWarrior;
    }

    [Serializable]
    public class PinguData
    {
        [SerializeField]
        public List<PinguInfoStruct> info = new List<PinguInfoStruct>();
    }


    [Serializable]
    public class HouseInfoStruct
    {
        [SerializeField]
        public string type;
        [SerializeField]
        public bool isBot;
        [SerializeField]
        public int health;
        [SerializeField]
        public int resources;
        [SerializeField]
        public float[] position = new float[3];
        [SerializeField]
        public float[] rotation = new float[4];
    }

    [Serializable]
    public class HouseData
    {
        [SerializeField]
        public List<HouseInfoStruct> info = new List<HouseInfoStruct>();
    }
}
