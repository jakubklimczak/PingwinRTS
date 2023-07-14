using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.VisualScripting;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class CursorMovementTracker : MonoBehaviour
{
    public Camera mainCamera;
    public LayerMask groundLayerMask;
    public LayerMask houseLayerMask;
    public LayerMask nestLayerMask;
    public LayerMask penguinLayerMask;
    public LayerMask UILayerMask;

    GraphicRaycaster m_Raycaster;
    PointerEventData m_PointerEventData;
    EventSystem m_EventSystem;
    
    public GameObject[] houses;
    public GameObject invObj;
    public GameObject mark;
    Inventory inv;
    GridLogic gridScript;
    int worldSize = 200;
    public int whatToBuild = 1;

    bool penguinSelected = false;
    PenguinLogic selectedPenguin = null;
    List<GameObject> selectedPenguins = new List<GameObject>();

    bool selectingArea = false;
    bool areaSelected = false;
    Vector3 firstAreaPoint,lastAreaPoint;

    public Dictionary<string, Dictionary<string, int>> costs = new Dictionary<string, Dictionary<string, int>>();

    SoundEffectsPlayer sounds;

    // Start is called before the first frame update
    void Start()
    {
        gridScript = GameObject.Find("Grid").GetComponent<GridLogic>();
        houses = gridScript.housesPrefabs;
        inv = invObj.GetComponent<Inventory>();

        //Fetch the Raycaster from the GameObject (the Canvas)
        m_Raycaster = GameObject.Find("UI").GetComponent<GraphicRaycaster>();
        //Fetch the Event System from the Scene
        m_EventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();

        //Debug.Log(m_Raycaster);
        //Debug.Log(m_EventSystem);

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

        costs.Add("ice-wall", new Dictionary<string, int>() {
            {"ice", -20}
        });

        costs.Add("huta_igloo", new Dictionary<string, int>() {
            {"ice", -80},
            {"wood", -50}
        });

        costs.Add("upgr", new Dictionary<string, int>() {
            {"ice", -80},
            {"scraps", -50},
            {"wood", -60},
            {"ingots", -5}
        });

        sounds = GameObject.Find("CameraObject").GetComponent<SoundEffectsPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        //jeśli się nie rusza - tu kombinować
        //Vector3 finalDestination = selectedPenguin.transform.position;
        //checking if the user clicked on UI
        m_PointerEventData = new PointerEventData(m_EventSystem);
        m_PointerEventData.position = Input.mousePosition;
        List<RaycastResult> results = new List<RaycastResult>();
        m_Raycaster.Raycast(m_PointerEventData, results);


        //podświetlenie pingwinów
        if(selectedPenguin!=null && selectedPenguin.transform.childCount == 2)
        {
            Vector3 pos = selectedPenguin.transform.position;
            pos.y = 0.2f;
            Instantiate(mark, pos, new Quaternion(),selectedPenguin.transform);
        }

        if(selectedPenguins.Count > 0 && selectedPenguins[0].transform.childCount == 2)
        {
            for(int i=0;i<selectedPenguins.Count; i++)
            {
                Vector3 pos = selectedPenguins[i].transform.position;
                pos.y = 0.2f;
                Instantiate(mark, pos, new Quaternion(), selectedPenguins[i].transform);
            }
        }
   
        if(results.Count > 0 && Input.GetMouseButtonDown(0))
        {
            return;
        }

        //wybieranie domków
        Ray houseRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(houseRay, out RaycastHit houseRaycastHit, float.MaxValue, houseLayerMask))
        {
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log(houseRaycastHit.collider.GetComponentInParent<HouseInfo>().type);
                if(selectedPenguin.transform.childCount > 2)
                            Destroy(selectedPenguin.transform.GetChild(2).gameObject);
                selectedPenguin = null;

                foreach(GameObject s in selectedPenguins)
                {
                    if(s.transform.childCount > 2)
                    {
                        Destroy(s.transform.GetChild(2).gameObject);
                    }
                }
                selectedPenguins.Clear();
                areaSelected = false;
                penguinSelected = false;
            }

            if(penguinSelected && selectedPenguin!=null && Input.GetMouseButtonDown(1) && !areaSelected)
            {
                if(selectedPenguin.isWarrior || !selectedPenguin.isWarrior)
                {
                    if(!selectedPenguin.isAttacking)
                    {
                        Vector3 tmp2 = new Vector3(((float)Math.Round(houseRaycastHit.point.x)), (float)Math.Round(houseRaycastHit.point.y), ((float)Math.Round(houseRaycastHit.point.z)));
                        //selectedPenguin.destination = tmp2;
                        selectedPenguin.penguinWannaMove(selectedPenguin.transform.position, tmp2);
                        selectedPenguin.houseToAttack = houseRaycastHit.collider.gameObject;
                        if (!houseRaycastHit.collider.gameObject.GetComponent<HouseInfo>().isBot)
                        {
                            bool should_exit = false;
                            for(int i = -1; i <= 1; i++)
                            {
                                for(int j = -1; j <= 1; j ++)
                                {
                                    
                                    if (i!=0 || j!=0) 
                                    {
                                        Vector3 correct_dest_proposition = new Vector3(tmp2.x + i, tmp2.y, tmp2.z + j);
                                        if (gridScript.IsTraversable(correct_dest_proposition)) 
                                        {
                                            //selectedPenguin.destination = correct_dest_proposition;
                                            //finalDestination = correct_dest_proposition;
                                            selectedPenguin.penguinWannaMove(selectedPenguin.transform.position, correct_dest_proposition);

                                            should_exit = true;
                                            break;
                                        }
                                    }
                                        
                                }
                                if (should_exit)
                                    break;
                            }
                            if(!should_exit)
                            {
                                selectedPenguin.destination = selectedPenguin.transform.position; //nie ma miejsca w destynacji, error.mp3
                                Debug.Log("error.mp3");
                                sounds.ply_cant();
                            }

                        }
                        if (houseRaycastHit.collider.gameObject.GetComponent<HouseInfo>().isBot)
                        {
                            selectedPenguin.shouldAttack = true;
                        }
                        if(!houseRaycastHit.collider.gameObject.GetComponent<HouseInfo>().isBot && 
                            houseRaycastHit.collider.gameObject.name == "molo(Clone)")
                        {
                            selectedPenguin.shouldAttack = true;
                        }
                        if(!houseRaycastHit.collider.gameObject.GetComponent<HouseInfo>().isBot && 
                            houseRaycastHit.collider.gameObject.name == "huta_igloo(Clone)")
                        {
                            selectedPenguin.shouldAttack = true;
                        }
                        if(selectedPenguin.transform.childCount > 2)
                            Destroy(selectedPenguin.transform.GetChild(2).gameObject);
                        selectedPenguin = null;
                        return;
                    }else
                    {
                        Vector3 tmp2 = new Vector3(((float)Math.Round(houseRaycastHit.point.x)), (float)Math.Round(houseRaycastHit.point.y), ((float)Math.Round(houseRaycastHit.point.z)));
                        //selectedPenguin.destination = tmp2;
                        selectedPenguin.penguinWannaMove(selectedPenguin.transform.position, tmp2);
                        selectedPenguin.houseToAttack = houseRaycastHit.collider.gameObject;
                        if(houseRaycastHit.collider.gameObject.GetComponent<HouseInfo>().isBot)
                        {
                            selectedPenguin.shouldAttack = true;
                        }
                        if(!houseRaycastHit.collider.gameObject.GetComponent<HouseInfo>().isBot && 
                            houseRaycastHit.collider.gameObject.name == "molo(Clone)")
                        {
                            selectedPenguin.shouldAttack = true;
                        }
                        if(!houseRaycastHit.collider.gameObject.GetComponent<HouseInfo>().isBot && 
                            houseRaycastHit.collider.gameObject.name == "huta(Clone)")
                        {
                            selectedPenguin.shouldAttack = true;
                        }
                        if(selectedPenguin.transform.childCount > 2)
                            Destroy(selectedPenguin.transform.GetChild(2).gameObject);
                        selectedPenguin = null;
                        return;
                    }
                    
                }else{
                    Vector3 tmp2 = new Vector3(((float)Math.Round(houseRaycastHit.point.x)), (float)Math.Round(houseRaycastHit.point.y), ((float)Math.Round(houseRaycastHit.point.z)));
                    //selectedPenguin.destination = tmp2;
                    selectedPenguin.penguinWannaMove(selectedPenguin.transform.position, tmp2);
                    if (houseRaycastHit.collider.gameObject.GetComponent<HouseInfo>().isBot)
                    {
                        selectedPenguin.shouldAttack = false;
                        selectedPenguin.houseToAttack = null;
                    }
                    if(selectedPenguin.transform.childCount > 2)
                            Destroy(selectedPenguin.transform.GetChild(2).gameObject);
                    selectedPenguin = null;
                    return;
                }
            }
            

            return;
        }

        //nest raycast
        Ray nestRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(nestRay, out RaycastHit nestRaycastHit, float.MaxValue, nestLayerMask))
        {
            if (Input.GetMouseButtonDown(0))
            {
                //if(nestRaycastHit.collider.GetComponentInParent<NestSpawner>()!=null)
                //    Debug.Log(nestRaycastHit.collider.GetComponentInParent<NestSpawner>().health);
            }

            if(penguinSelected && selectedPenguin!=null && Input.GetMouseButtonDown(1) && !areaSelected)
            {
                if(selectedPenguin.isWarrior || !selectedPenguin.isWarrior)
                {
                    if(!selectedPenguin.isAttacking)
                    {
                        Vector3 tmp2 = new Vector3(((float)Math.Round(nestRaycastHit.point.x)), (float)Math.Round(nestRaycastHit.point.y), ((float)Math.Round(nestRaycastHit.point.z)));
                        //selectedPenguin.destination = tmp2;
                        selectedPenguin.penguinWannaMove(selectedPenguin.transform.position, tmp2);
                        selectedPenguin.houseToAttack = nestRaycastHit.collider.gameObject;
                        if(nestRaycastHit.collider.gameObject.name == "upgr(Clone)" || 
                            nestRaycastHit.collider.gameObject.GetComponent<ResourceLogic>()!=null || 
                            nestRaycastHit.collider.gameObject.GetComponent<NestSpawner>().isBot)
                        {
                            selectedPenguin.shouldAttack = true;
                        }
                        if(selectedPenguin.transform.childCount > 2)
                            Destroy(selectedPenguin.transform.GetChild(2).gameObject);
                        selectedPenguin = null;
                        return;
                    }else
                    {
                        Vector3 tmp2 = new Vector3(((float)Math.Round(nestRaycastHit.point.x)), (float)Math.Round(nestRaycastHit.point.y), ((float)Math.Round(nestRaycastHit.point.z)));
                        //selectedPenguin.destination = tmp2;
                        selectedPenguin.penguinWannaMove(selectedPenguin.transform.position, tmp2);
                        selectedPenguin.houseToAttack = nestRaycastHit.collider.gameObject;
                        if(nestRaycastHit.collider.gameObject.name == "upgr(Clone)" || 
                            nestRaycastHit.collider.gameObject.GetComponent<ResourceLogic>()!=null || 
                            nestRaycastHit.collider.gameObject.GetComponent<NestSpawner>().isBot)
                        {
                            selectedPenguin.shouldAttack = true;
                        }
                        if(selectedPenguin.transform.childCount > 2)
                            Destroy(selectedPenguin.transform.GetChild(2).gameObject);
                        selectedPenguin = null;
                        return;
                    }
                    
                }else{
                    Vector3 tmp2 = new Vector3(((float)Math.Round(nestRaycastHit.point.x)), (float)Math.Round(nestRaycastHit.point.y), ((float)Math.Round(nestRaycastHit.point.z)));
                    //selectedPenguin.destination = tmp2;
                    selectedPenguin.penguinWannaMove(selectedPenguin.transform.position, tmp2);
                    if (nestRaycastHit.collider.gameObject.name == "upgr(Clone)" || 
                        nestRaycastHit.collider.gameObject.GetComponent<ResourceLogic>()!=null ||
                        nestRaycastHit.collider.gameObject.GetComponent<NestSpawner>().isBot)
                    {
                        selectedPenguin.shouldAttack = false;
                        selectedPenguin.houseToAttack = null;
                    }
                    if(selectedPenguin.transform.childCount > 2)
                            Destroy(selectedPenguin.transform.GetChild(2).gameObject);
                    selectedPenguin = null;
                    return;
                }
            }
            

            return;
        }

        //wybieranie pingwina
        Ray penguinRay = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(penguinRay, out RaycastHit penguinRaycastHit, float.MaxValue, penguinLayerMask))
        {
            //atakowanie innego pingwina
            if (selectedPenguin != null && selectedPenguin.GetComponent<PenguinLogic>().isWarrior && Input.GetMouseButtonDown(1) && penguinRaycastHit.collider.GetComponentInParent<PenguinLogic>().isBot)
            {
                selectedPenguin.GetComponent<PenguinLogic>().shouldAttack = true;
                selectedPenguin.GetComponent<PenguinLogic>().penguinWannaMove(selectedPenguin.GetComponent<PenguinLogic>().transform.position, penguinRaycastHit.collider.gameObject.transform.position);
                //selectedPenguin.GetComponent<PenguinLogic>().destination = penguinRaycastHit.collider.gameObject.transform.position;
                selectedPenguin.GetComponent<PenguinLogic>().houseToAttack = penguinRaycastHit.collider.gameObject;
                if(selectedPenguin.transform.childCount > 2)
                            Destroy(selectedPenguin.transform.GetChild(2).gameObject);
                selectedPenguin = null;
                return;
            }

            if (Input.GetMouseButtonDown(0) && !penguinRaycastHit.collider.GetComponentInParent<PenguinLogic>().isBot)
            {
                if(selectedPenguin != null)
                    if(selectedPenguin.transform.childCount > 2)
                            Destroy(selectedPenguin.transform.GetChild(2).gameObject);

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

                GameObject[] pinguins = GameObject.FindGameObjectsWithTag("pingu");

                foreach(GameObject p in pinguins)
                {
                    bool isBetweenX = (p.transform.position.x >= Mathf.Min(firstAreaPoint.x, lastAreaPoint.x)) && (p.transform.position.x <= Mathf.Max(firstAreaPoint.x, lastAreaPoint.x));
                    bool isBetweenZ = (p.transform.position.z >= Mathf.Min(firstAreaPoint.z, lastAreaPoint.z)) && (p.transform.position.z <= Mathf.Max(firstAreaPoint.z, lastAreaPoint.z));

                    if(isBetweenX && isBetweenZ && !p.GetComponent<PenguinLogic>().isBot)
                    {
                        selectedPenguins.Add(p);
                    }
                }
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
                if (gridScript.IsTraversable(tmp2))
                {
                    //selectedPenguin.destination = tmp2;
                    selectedPenguin.penguinWannaMove(selectedPenguin.transform.position, tmp2);
                    //gridScript.UpdatePosition(selectedPenguin.transform.position, tmp2, 13); //13 bo pingiwn
                    if (selectedPenguin.isAttacking)
                    {
                        selectedPenguin.isAttacking = false;
                        selectedPenguin.shouldAttack = false;
                        selectedPenguin.houseToAttack = null;
                    }
                    if (selectedPenguin.transform.childCount > 2)
                        Destroy(selectedPenguin.transform.GetChild(2).gameObject);
                    selectedPenguin = null;
                    return;
                }
                else
                {
                    //tutaj feedback jak nie wolno się przesunąć
                    sounds.ply_cant();
                }
            }

            //ustawianie objectu pod cursorem
            Vector3 tmp = new Vector3(((float)Math.Round(groundRaycastHit.point.x)), 0.01f, ((float)Math.Round(groundRaycastHit.point.z)));
            transform.position = tmp;

            //stawianie domków
            if (Input.GetMouseButtonDown(0) && (int)tmp.x >= 0 && (int)tmp.z >= 0 && (int)tmp.x < worldSize && (int)tmp.x < worldSize && !areaSelected && whatToBuild != 0)
            {
                bool canPlace = true;

                //Debug.Log(gridScript.map[(int)tmp.x, (int)tmp.z]);

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
                                sounds.ply_cant();
                            }
                        }
                    }
                }

                //4 to molo
                if (gridScript.map[(int)tmp.x, (int)tmp.z] == 0 && canPlace && whatToBuild != 4)
                {
                    GameObject parent = GameObject.Find("Infrastructure");
                    GameObject child = Instantiate(houses[whatToBuild], tmp, new Quaternion(), parent.transform);
                    sounds.ply_build();
                    HouseInfo childHouseInfo = child.GetComponent<HouseInfo>();
                    childHouseInfo.type = houses[whatToBuild].name;
                    //childHouseInfo.counter = 2;//nwm co to xd
                    //gridScript.map[(int)tmp.x, (int)tmp.z] = whatToBuild +1;
                    gridScript.map[(int)tmp.z, (int)tmp.x] = whatToBuild +1;


                    //removing from inventory
                    Dictionary<string, int> houseCost = costs[houses[whatToBuild].name.ToLower()];

                    foreach (KeyValuePair<string, int> resource in houseCost)
                    {
                        inv.inventory[resource.Key] += resource.Value;
                    }
                }else if(gridScript.map[(int)tmp.z, (int)tmp.x] == 0 && canPlace && whatToBuild == 4)
                {
                    if(gridScript.map[(int)tmp.z - 1, (int)tmp.x] == 3)//dol działa
                    {
                        GameObject parent = GameObject.Find("Infrastructure");
                        GameObject child = Instantiate(houses[whatToBuild], tmp, Quaternion.Euler(0, -90, 0), parent.transform);
                        sounds.ply_build();
                        HouseInfo childHouseInfo = child.GetComponent<HouseInfo>();
                        childHouseInfo.type = houses[whatToBuild].name;
                        //childHouseInfo.counter = 2;//nwm co to xd
                        gridScript.map[(int)tmp.z, (int)tmp.x] = whatToBuild +1;


                        //removing from inventory
                        Dictionary<string, int> houseCost = costs[houses[whatToBuild].name.ToLower()];

                        foreach (KeyValuePair<string, int> resource in houseCost)
                        {
                            inv.inventory[resource.Key] += resource.Value;
                        }
                    }else if(gridScript.map[(int)tmp.z + 1, (int)tmp.x] == 3)//up działa
                    {
                        GameObject parent = GameObject.Find("Infrastructure");
                        GameObject child = Instantiate(houses[whatToBuild], tmp, Quaternion.Euler(0, 90, 0), parent.transform);
                        sounds.ply_build();
                        HouseInfo childHouseInfo = child.GetComponent<HouseInfo>();
                        childHouseInfo.type = houses[whatToBuild].name;
                        //childHouseInfo.counter = 2;//nwm co to xd
                        gridScript.map[(int)tmp.z, (int)tmp.x] = whatToBuild +1;


                        //removing from inventory
                        Dictionary<string, int> houseCost = costs[houses[whatToBuild].name.ToLower()];

                        foreach (KeyValuePair<string, int> resource in houseCost)
                        {
                            inv.inventory[resource.Key] += resource.Value;
                        }
                    }else if(gridScript.map[(int)tmp.z, (int)tmp.x + 1] == 3)//Prawo działa
                    {
                        GameObject parent = GameObject.Find("Infrastructure");
                        GameObject child = Instantiate(houses[whatToBuild], tmp, Quaternion.Euler(0, -180, 0), parent.transform);
                        sounds.ply_build();
                        HouseInfo childHouseInfo = child.GetComponent<HouseInfo>();
                        childHouseInfo.type = houses[whatToBuild].name;
                        //childHouseInfo.counter = 2;//nwm co to xd
                        gridScript.map[(int)tmp.z, (int)tmp.x] = whatToBuild +1;


                        //removing from inventory
                        Dictionary<string, int> houseCost = costs[houses[whatToBuild].name.ToLower()];

                        foreach (KeyValuePair<string, int> resource in houseCost)
                        {
                            inv.inventory[resource.Key] += resource.Value;
                        }
                    }else if(gridScript.map[(int)tmp.z, (int)tmp.x - 1] == 3)//lewo nie działa
                    {
                        GameObject parent = GameObject.Find("Infrastructure");
                        GameObject child = Instantiate(houses[whatToBuild], tmp, Quaternion.Euler(0, 0, 0), parent.transform);
                        sounds.ply_build();
                        HouseInfo childHouseInfo = child.GetComponent<HouseInfo>();
                        childHouseInfo.type = houses[whatToBuild].name;
                        //childHouseInfo.counter = 2;//nwm co to xd
                        gridScript.map[(int)tmp.z, (int)tmp.x] = whatToBuild +1;


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

                    if(isBetweenX && isBetweenZ && !p.GetComponent<PenguinLogic>().isBot)
                    {
                        System.Random random = new System.Random();
                        double rand1 = random.NextDouble() * (1.5 - 0.8) + 0.8;
                        double rand2 = random.NextDouble() * (1.5 - 0.8) + 0.8;

                        if(p.transform.childCount > 2)
                        {
                            Destroy(p.transform.GetChild(2).gameObject);
                        }
                        selectedPenguins.Clear();

                        p.GetComponent<PenguinLogic>().penguinWannaMove(p.GetComponent<PenguinLogic>().transform.position, new Vector3((float)(tmp2.x + rand1), tmp2.y, (float)(tmp.z + rand2)));
                        //p.GetComponent<PenguinLogic>().destination = new Vector3((float)(tmp2.x + rand1), tmp2.y, (float)(tmp.z + rand2));
                    }
                }

                areaSelected = false;
            }
        }

        if(Input.GetMouseButtonDown(0))
        {
            if(areaSelected == true)
            {
                foreach(GameObject s in selectedPenguins)
                {
                    if(s.transform.childCount > 2)
                    {
                        Destroy(s.transform.GetChild(2).gameObject);
                    }
                }
                selectedPenguins.Clear();
                areaSelected = false;
            }

            if(selectedPenguin!=null)
            {
                if(selectedPenguin.transform.childCount > 2)
                    Destroy(selectedPenguin.transform.GetChild(2).gameObject);
                selectedPenguin = null;
            }
        }
    }

    public PenguinLogic GetCurrentlySelectedPenguine()
    {
        return this.selectedPenguin;
    }

    //casting gameobject list into PenguineList
    public List<PenguinLogic> GetCurrentlySelectedPenguineList()
    {
        List<PenguinLogic> result = new List<PenguinLogic>();
        foreach (GameObject p in selectedPenguins)
        {
            result.Add(p.GetComponent<PenguinLogic>());
        }


        return result;
    }
    
}
