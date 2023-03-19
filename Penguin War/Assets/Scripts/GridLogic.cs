using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridLogic : MonoBehaviour
{
    //public int[,] Grid = new int[100, 100];
    public int[,] grid = new int[100, 100];

    /*
        0 means a empty space
        1 is a igloo etc
    */


    // Start is called before the first frame update
    void Start()
    {
        for(int i=0;i<100;i++)
        {
            for(int j=0;j<100;j++)
            {
                grid[i,j]=0;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
