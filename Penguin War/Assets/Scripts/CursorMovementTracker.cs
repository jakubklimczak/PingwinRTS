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

    bool selectingArea = false;
    bool areaSelected = false;
    Vector3 firstAreaPoint,lastAreaPoint;

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

        costs.Add("molo", new Dictionary<string, int>() {
            {"wood", -20}
        });
    }

    // Update is called once per frame
    void Update()
    {
        //wybieranie domków
        Ray houseRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(houseRay, out RaycastHit houseRaycastHit, float.MaxValue, houseLayerMask))
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log(houseRaycastHit.collider.GetComponentInParent<HouseInfo>().type);
            }
            return;
        }

        //wybieranie pingwina
        Ray penguinRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(houseRay, out RaycastHit penguinRaycastHit, float.MaxValue, penguinLayerMask))
        {
            if (Input.GetMouseButtonDown(0))
            {
                penguinSelected = true;
                selectedPenguin = penguinRaycastHit.collider.GetComponentInParent<PenguinLogic>();
            }
            return;
        }

        //wybieranie terenu
        Ray areaRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(areaRay, out RaycastHit areaRaycastHit, float.MaxValue, groundLayerMask) && whatToBuild == 0)
        {
            if(!selectingArea && Input.GetMouseButtonDown(0))
            {
                firstAreaPoint = areaRaycastHit.point;
                selectingArea = true;
            }else if(selectingArea && Input.GetMouseButtonUp(0))
            {
                selectingArea = false;
                lastAreaPoint = areaRaycastHit.point;
                areaSelected = true;
            }
        }

        //stawianie domków
        Ray groundRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(groundRay, out RaycastHit groundRaycastHit, float.MaxValue, groundLayerMask))
        {
            //ustawainie dest pingu
            if(penguinSelected && selectedPenguin!=null && Input.GetMouseButtonDown(1) && !areaSelected)
            {
                Vector3 tmp2 = new Vector3(((float)Math.Round(groundRaycastHit.point.x)), (float)Math.Round(groundRaycastHit.point.y), ((float)Math.Round(groundRaycastHit.point.z)));
                selectedPenguin.destination = tmp2;
                selectedPenguin = null;
                return;
            }

            //ustawianie objectu pod cursorem
            Vector3 tmp = new Vector3(((float)Math.Round(groundRaycastHit.point.x)), 0.2f, ((float)Math.Round(groundRaycastHit.point.z)));
            transform.position = tmp;

            //stawianie domków
            if (Input.GetMouseButtonDown(0) && (int)tmp.x >= 0 && (int)tmp.z >= 0 && (int)tmp.x < worldSize && (int)tmp.x < worldSize && !areaSelected && whatToBuild != 0)
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

                //4 to molo
                if (gridScript.map[(int)tmp.x, (int)tmp.z] == 0 && canPlace && whatToBuild != 4)
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
                }else if(gridScript.map[(int)tmp.x, (int)tmp.z] == 0 && canPlace && whatToBuild == 4)
                {
                    if(gridScript.map[(int)tmp.x, (int)tmp.z - 1] == 3)
                    {
                        GameObject parent = GameObject.Find("Infrastructure");
                        GameObject child = Instantiate(houses[whatToBuild], tmp, Quaternion.Euler(0, -90, 0), parent.transform);
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
                    }else if(gridScript.map[(int)tmp.x, (int)tmp.z + 1] == 3)
                    {
                        GameObject parent = GameObject.Find("Infrastructure");
                        GameObject child = Instantiate(houses[whatToBuild], tmp, Quaternion.Euler(0, 90, 0), parent.transform);
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
                    }else if(gridScript.map[(int)tmp.x + 1, (int)tmp.z] == 3)
                    {
                        GameObject parent = GameObject.Find("Infrastructure");
                        GameObject child = Instantiate(houses[whatToBuild], tmp, Quaternion.Euler(0, -180, 0), parent.transform);
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
                    }else if(gridScript.map[(int)tmp.x - 1, (int)tmp.z] == 3)
                    {
                        GameObject parent = GameObject.Find("Infrastructure");
                        GameObject child = Instantiate(houses[whatToBuild], tmp, Quaternion.Euler(0, 180, 0), parent.transform);
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

                /*if (map[i, j] == 5)
                {
                    if(map[i + 1, j] == 3)
                    {
                        //string tmpPath = @"..\\Penguin War\\Assets\\Images\\Tiles\\tile0.png";
                        Vector3 tmpPos = new(Mathf.Floor(i), 0.1f, Mathf.Floor(j));
                        GameObject tmpObj3 = Instantiate(houses[map[i, j] -1], tmpPos, Quaternion.LookRotation(Vector3.forward,Vector3.up));//change this later to be able to spawn more things
                        tmpObj3.transform.SetParent(parent.transform);
                        HouseInfo tmpHouseInfo3 = tmpObj3.GetComponent<HouseInfo>();
                        tmpHouseInfo3.type = houses[map[i, j] - 1].name;
                        tmpHouseInfo3.counter = i;
                    }
                }*/
            }

            if(areaSelected == true && Input.GetMouseButtonDown(1))
            {
                GameObject[] pinguins = GameObject.FindGameObjectsWithTag("pingu");
                Vector3 tmp2 = new Vector3(((float)Math.Round(groundRaycastHit.point.x)), (float)Math.Round(groundRaycastHit.point.y), ((float)Math.Round(groundRaycastHit.point.z)));

                foreach(GameObject p in pinguins)
                {
                    bool isBetweenX = (p.transform.position.x >= Mathf.Min(firstAreaPoint.x, lastAreaPoint.x)) && (p.transform.position.x <= Mathf.Max(firstAreaPoint.x, lastAreaPoint.x));
                    bool isBetweenZ = (p.transform.position.z >= Mathf.Min(firstAreaPoint.z, lastAreaPoint.z)) && (p.transform.position.z <= Mathf.Max(firstAreaPoint.z, lastAreaPoint.z));

                    if(isBetweenX && isBetweenZ)
                    {
                        System.Random random = new System.Random();
                        double rand1 = random.NextDouble() * (1.5 - 0.8) + 0.8;
                        double rand2 = random.NextDouble() * (1.5 - 0.8) + 0.8;
                        p.GetComponent<PenguinLogic>().destination = new Vector3((float)(tmp2.x + rand1), tmp2.y, (float)(tmp.z + rand2));
                    }
                }
                areaSelected = false;
            }
        }
    }
}
