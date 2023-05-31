using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;
using static Inventory;
using static UnityEditor.Progress;
using System.Runtime.Serialization.Formatters.Binary;

public class GridLogic : MonoBehaviour
{
    const int worldSize = 200;
    public GameObject[] housesPrefabs = new GameObject[10];
    public GameObject[] pinguinsPrefabs = new GameObject[5];
    [SerializeField]

    public PinguData SavedPinguins = new PinguData();
    public HouseData Savedhouses = new HouseData();

    string penguinFilePath = "Assets/Save/penguinData.pingu";
    string houseFilePath = "Assets/Save/housesData.pingu";

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
            GenerateCsv("Assets/Save/map.csv");

        }else if(mode=="loadGame")
        {
            //reads the save "map.csv"
            using (var reader = new StreamReader(@"..\\Penguin War\\Assets\\Save\\map.csv"))
            {
                int iter = 0;
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    for(int i=0; i < values.Length - 1; i++)
                    {
                        map[i, iter] = int.Parse(values[i]);
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
                }
            }

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            Debug.Log("Saving");
            SaveWorld("Assets/Save/map.csv");
        }
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

    public IEnumerator SpawnHouses()//change to spawn houses and other stuff          and later mobs
    {
        GameObject parent = GameObject.Find("TilesMap");
        for(int i = 0; i < map.GetLength(0); i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if (map[i, j] != 0 && map[i, j] != 3 && map[i, j] != 4)
                {
                    //string tmpPath = @"..\\Penguin War\\Assets\\Images\\Tiles\\tile0.png";
                    Vector3 tmpPos = new(Mathf.Floor(i), 0.1f, Mathf.Floor(j));
                    GameObject tmpObj = Instantiate(housesPrefabs[map[i, j] -1], tmpPos, new Quaternion());//change this later to be able to spawn more things
                    tmpObj.transform.SetParent(parent.transform);
                    HouseInfo tmpHouseInfo = tmpObj.GetComponent<HouseInfo>();
                    tmpHouseInfo.type = housesPrefabs[map[i, j] - 1].name;
                    //tmpObj.GetComponent<Renderer>().material.mainTexture = LoadPNG(tmpPath);
                }
            }
            yield return new WaitForSeconds(0);
        }

        if (File.Exists(houseFilePath))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream fileStream = File.Open(houseFilePath, FileMode.Open);
            HouseData loadedData = (HouseData)formatter.Deserialize(fileStream);
            fileStream.Close();
            
            GameObject[] spawnedHouses = GameObject.FindGameObjectsWithTag("houses");

            foreach(HouseInfoStruct h in loadedData.info)
            {
                Savedhouses.info.Add(h);
                Vector3 tmpPos = new Vector3(h.position[0], h.position[1], h.position[2]);
                Quaternion tmpRotation = new Quaternion(h.rotation[0], h.rotation[1], h.rotation[2], h.rotation[3]);

                foreach(GameObject GO in spawnedHouses)
                {
                    if(GO.transform.position.x == h.position[0] && 
                     GO.transform.position.y == h.position[1] &&
                     GO.transform.position.z == h.position[2])
                    {
                        GO.transform.rotation = new Quaternion(h.rotation[0], h.rotation[1], h.rotation[2], h.rotation[3]);
                        GO.GetComponent<HouseInfo>().health = h.health;
                        GO.GetComponent<HouseInfo>().resources = h.resources;
                        GO.GetComponent<HouseInfo>().hasPinguinAssigned = h.hasPinguinAssigned;
                        GO.GetComponent<HouseInfo>().hasPinguinAssigned = h.hasPinguinAssigned;
                    }
                }
            }
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
                    writer.Write(map[j, i]);
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
            SavedPinguins.info.Add(tmp);
        }

        BinaryFormatter formatterForPenguins = new BinaryFormatter();

        FileStream fileStreamforPenguins = File.Create(penguinFilePath);
        formatterForPenguins.Serialize(fileStreamforPenguins, SavedPinguins);
        fileStreamforPenguins.Close();

        // //save houses data
        GameObject[] housesObjs = GameObject.FindGameObjectsWithTag("houses");

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
            tmp.hasPinguinAssigned = h.GetComponent<HouseInfo>().hasPinguinAssigned;
            tmp.resources = h.GetComponent<HouseInfo>().resources;

            Savedhouses.info.Add(tmp);
        }

        BinaryFormatter formatterForHouses = new BinaryFormatter();

        FileStream fileStreamForHouses = File.Create(houseFilePath);
        formatterForHouses.Serialize(fileStreamForHouses, Savedhouses);
        fileStreamForHouses.Close();
    }
    public void GenerateCsv(string filename)
    {
        // Create a 100x100 array of zeros
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
                if (i==j)
                    data[i, j] = 1;
                else if (distance1 < 30)
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
        public bool isBot;
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
        public bool hasPinguinAssigned;
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
