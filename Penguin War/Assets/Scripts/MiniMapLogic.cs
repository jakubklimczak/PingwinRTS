using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MiniMapLogic : MonoBehaviour
{
    public Texture ally_units_texture;
    public Texture ally_buildings_texture;
    public GridLogic grid_logic;

    int[,] map_data;

    public RectTransform canvasTransform;
    Vector3[] ParentPosition = new Vector3[4];
    // Start is called before the first frame update
    void Start()
    {
        map_data = grid_logic.map;
        GetCorrnersOfPanel();
    }
    private void OnGUI()
    {
        for (int i = 0; i < map_data.GetLength(0); i++)
            for(int j = 0; j < map_data.GetLength(1); j++)
            {
                if(map_data[j, i] == 13)
                {

                }
                    //Debug.Log(map_data[j,i]);
            }
            
        GUI.DrawTexture(new Rect(ParentPosition[1].x, /*ParentPosition[1].y*/500, 100, 100), ally_units_texture, ScaleMode.StretchToFill, true, 10.0F);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void DrawCellOfMap(int posX,int posY)
    {

    }

    private void GetCorrnersOfPanel()
    {
        canvasTransform.GetWorldCorners(ParentPosition);
        float screen_height = Screen.currentResolution.height;
        Debug.Log(ParentPosition[1].y);
        //ParentPosition[1].y = screen_height - ParentPosition[1].y - 100;
        //Debug.Log(ParentPosition[1].y);
    }
}
