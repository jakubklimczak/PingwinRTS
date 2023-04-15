using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;


public class GridLogic : MonoBehaviour
{
    const int worldSize = 100;
    public GameObject tile0;


    //This is the grid for 'placed' objects
    public int[,] grid = new int[worldSize, worldSize];
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

        //GenerateCsv("potato.csv");

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

        StartCoroutine(SpawnTiles());


        for (int i=0;i<worldSize;i++)
        {
            for(int j=0;j< worldSize; j++)
            {
                if (map[i,j]!=0)
                {
                    grid[i, j] = 0;
                }else
                {
                    grid[i, j] = -1;
                }    
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static Texture2D LoadPNG(string filePath)
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

    public IEnumerator SpawnTiles()
    {
        GameObject parent = GameObject.Find("TilesMap");
        for(int i = 0; i<map.GetLength(0);i++)
        {
            for (int j = 0; j < map.GetLength(1); j++)
            {
                if (map[i, j] != 0)
                {
                    string tmpPath = @"..\\Penguin War\\Assets\\Images\\Tiles\\tile0.png";
                    Vector3 tmpPos = new(Mathf.Floor(i), 0.1f, Mathf.Floor(j));
                    GameObject tmpObj = Instantiate(tile0, tmpPos, new Quaternion());
                    tmpObj.transform.SetParent(parent.transform);
                    tmpObj.GetComponent<Renderer>().material.mainTexture = LoadPNG(tmpPath);
                }
            }
            yield return new WaitForSeconds(0);
        }
        
    }

    public static void GenerateCsv(string filename)
    {
        // Create a 100x100 array of zeros
        int[,] data = new int[100, 100];

        // Set the values within the circle to 1
        for (int i = 0; i < 100; i++)
        {
            for (int j = 0; j < 100; j++)
            {
                double distance = Math.Sqrt(Math.Pow(i - 50, 2) + Math.Pow(j - 50, 2));
                if (distance < 25)
                {
                    data[i, j] = 1;
                }
            }
        }

        // Write the data to a CSV file
        using (var writer = new StreamWriter(filename))
        {
            for (int i = 0; i < 100; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    writer.Write(data[i, j]);
                    if (j < 99)
                    {
                        writer.Write(",");
                    }
                }
                writer.WriteLine();
            }
        }
    }
}
