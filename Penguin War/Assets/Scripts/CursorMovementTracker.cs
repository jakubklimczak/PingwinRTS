using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CursorMovementTracker : MonoBehaviour
{
    public Camera mainCamera;
    public LayerMask groundLayerMask;
    public LayerMask houseLayerMask;
    public GameObject[] houses;
    public GameObject invObj;
    Inventory inv;
    GridLogic gridScript;
    int worldSize = 200;
    public int whatToBuild = 1;

    //on the index 0 is the cost of a house with index 0 in houses[]
    public Inventory.Item[][] costs = new Inventory.Item[2][];

    // Start is called before the first frame update
    void Start()
    {
        gridScript = GameObject.Find("Grid").GetComponent<GridLogic>();
        houses = gridScript.houses;
        inv = invObj.GetComponent<Inventory>();


        //for some reason you can't initialize it while you create it
        //here the old_igloo which is houses[0] costs 10 ice for example
        costs[0] = new Inventory.Item[1] {
            new Inventory.Item { Type = "ice", Amount = -20 },
        };

        //here the igloo which is houses[1] costs 20 ice and 10 wood for example
        costs[1] = new Inventory.Item[2] {
            new Inventory.Item { Type = "ice", Amount = -20 },
            new Inventory.Item { Type = "wood", Amount = -10 }
        };
    }

    // Update is called once per frame
    void Update()
    {
        Ray groundRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(groundRay, out RaycastHit groundRaycastHit, float.MaxValue, groundLayerMask))
        {
            Vector3 tmp = new Vector3(((float)Math.Round(groundRaycastHit.point.x)), 0.2f, ((float)Math.Round(groundRaycastHit.point.z)));
            transform.position = tmp;

            if (Input.GetMouseButtonDown(0) && (int)tmp.x >= 0 && (int)tmp.z >= 0 && (int)tmp.x < worldSize && (int)tmp.x < worldSize)
            {
                bool canPlace = true;

                for (int i = 0; i < costs[whatToBuild].Length; i++)
                {
                    for(int j=0;j<inv.inventory.Length;j++)
                    {
                        if (inv.inventory[j].Type == costs[whatToBuild][i].Type)
                        {
                            //Debug.Log("inv: " + inv.inventory[j].Amount +" and " + costs[whatToBuild][i].Amount + " daje " + (inv.inventory[j].Amount + costs[whatToBuild][i].Amount));
                            //Debug.Log(costs[whatToBuild][i].Amount);
                            if (inv.inventory[j].Amount + costs[whatToBuild][i].Amount < 0)
                            {
                                canPlace = false;
                                Debug.Log("Not enough");
                            }
                        }
                    }
                }

                if (gridScript.map[(int)tmp.x, (int)tmp.z] == 0 && canPlace)
                {
                    GameObject parent = GameObject.Find("Infrastructure");
                    GameObject child = Instantiate(houses[whatToBuild], tmp, new Quaternion(), parent.transform);
                    HouseInfo childHouseInfo = child.GetComponent<HouseInfo>();
                    childHouseInfo.type = houses[whatToBuild].name;
                    childHouseInfo.counter = 2;//nwm co to xd
                    gridScript.map[(int)tmp.x, (int)tmp.z] = 1;

                    for(int i = 0; i < costs[whatToBuild].Length;i++)
                    {
                        //Debug.Log(costs[whatToBuild][i].Type);
                        inv.ChangeAmmount(costs[whatToBuild][i].Type, costs[whatToBuild][i].Amount);
                    }

                    //to tak musi byæ bo to inv to component który ma zmienn¹ inventory...
                    //Debug.Log(inv.inventory[0].Amount);
                    //Debug.Log(inv.inventory[1].Amount);
                    //Debug.Log(inv.inventory[2].Amount);
                }
            }
        }


        Ray houseRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(houseRay, out RaycastHit houseRaycastHit, float.MaxValue, houseLayerMask))
        {
            if(Input.GetMouseButtonDown(0))
            {
                Debug.Log(houseRaycastHit.collider.GetComponentInParent<HouseInfo>().type);
            }
        }
    }
}
