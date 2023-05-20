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
    GridLogic gridScript;
    int worldSize = 200;

    // Start is called before the first frame update
    void Start()
    {
        gridScript = GameObject.Find("Grid").GetComponent<GridLogic>();
        houses = gridScript.houses;
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
                if (gridScript.map[(int)tmp.x, (int)tmp.z] == 0)
                {
                    GameObject parent = GameObject.Find("Infrastructure");
                    GameObject child = Instantiate(houses[1], tmp, new Quaternion(), parent.transform);
                    HouseInfo childHouseInfo = child.GetComponent<HouseInfo>();
                    childHouseInfo.type = "igloo";
                    childHouseInfo.counter = 2;
                    gridScript.map[(int)tmp.x, (int)tmp.z] = 1;
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
