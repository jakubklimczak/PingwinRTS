using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;

public class CursorMovementTracker : MonoBehaviour
{
    public Camera mainCamera;
    public LayerMask groundLayerMask;
    public LayerMask houseLayerMask;
    public LayerMask penguinLayerMask;
    public GameObject[] houses;
    public GameObject invObj;
    Inventory inv;
    GridLogic gridScript;
    int worldSize = 200;
    public int whatToBuild = 1;

    bool penguinSelected = false;
    PenguinLogic selectedPenguin = null;

    public Dictionary<string, Dictionary<string, int>> costs = new Dictionary<string, Dictionary<string, int>>();

    // Start is called before the first frame update
    void Start()
    {
        gridScript = GameObject.Find("Grid").GetComponent<GridLogic>();
        houses = gridScript.houses;
        inv = invObj.GetComponent<Inventory>();

        //costs initialization

        //names from prefabs
        costs.Add("old_igloo", new Dictionary<string, int>() {
            {"ice", -20}
        });

        costs.Add("igloo", new Dictionary<string, int>() {
            {"ice", -20},
            {"wood", -10}
        });
    }

    // Update is called once per frame
    void Update()
    {
        Ray houseRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(houseRay, out RaycastHit houseRaycastHit, float.MaxValue, houseLayerMask))
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log(houseRaycastHit.collider.GetComponentInParent<HouseInfo>().type);
            }
            return;
        }



        Ray penguinRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(houseRay, out RaycastHit penguinRaycastHit, float.MaxValue, penguinLayerMask))
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log(penguinRaycastHit.collider.GetComponentInParent<PenguinLogic>().destination);
                penguinSelected = true;
                selectedPenguin = penguinRaycastHit.collider.GetComponentInParent<PenguinLogic>();
            }
            return;
        }


        Ray groundRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(groundRay, out RaycastHit groundRaycastHit, float.MaxValue, groundLayerMask))
        {
            if(penguinSelected && selectedPenguin!=null && Input.GetMouseButtonDown(0))
            {
                Vector3 tmp2 = new Vector3(((float)Math.Round(groundRaycastHit.point.x)), (float)Math.Round(groundRaycastHit.point.y), ((float)Math.Round(groundRaycastHit.point.z)));
                selectedPenguin.destination = tmp2;
                selectedPenguin = null;
                return;
            }

            Vector3 tmp = new Vector3(((float)Math.Round(groundRaycastHit.point.x)), 0.2f, ((float)Math.Round(groundRaycastHit.point.z)));
            transform.position = tmp;

            if (Input.GetMouseButtonDown(0) && (int)tmp.x >= 0 && (int)tmp.z >= 0 && (int)tmp.x < worldSize && (int)tmp.x < worldSize)
            {
                bool canPlace = true;

                //check if has enough resources
                foreach (KeyValuePair<string, Dictionary<String, int>> houseCost in costs)
                {
                    if (houseCost.Key == houses[whatToBuild].name.ToLower())
                    {
                        foreach (KeyValuePair<string, int> resource in houseCost.Value)
                        {
                            if (resource.Value + inv.inventory[resource.Key] < 0)
                            {
                                canPlace = false;
                                Debug.Log("Not enough " + resource.Key);
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
                    gridScript.map[(int)tmp.x, (int)tmp.z] = whatToBuild;


                    //removing from inventory
                    Dictionary<string, int> houseCost = costs[houses[whatToBuild].name.ToLower()];

                    foreach (KeyValuePair<string, int> resource in houseCost)
                    {
                        inv.inventory[resource.Key] += resource.Value;
                    }
                }
            }
        }
    }
}
