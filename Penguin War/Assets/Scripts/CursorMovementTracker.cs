using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CursorMovementTracker : MonoBehaviour
{
    public Camera mainCamera;
    public GameObject Igloo;
    GridLogic gridScript;

    // Start is called before the first frame update
    void Start()
    {
        gridScript = GameObject.Find("Grid").GetComponent<GridLogic>();
    }

    // Update is called once per frame
    void Update()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if(Physics.Raycast(ray, out RaycastHit raycastHit))
        {
            transform.position = raycastHit.point;
            if(Input.GetMouseButtonDown(0))
            {
                Vector3 tmp = new Vector3((float)System.Math.Floor(raycastHit.point.x), 0, (float)System.Math.Floor(raycastHit.point.z));
                GameObject parent = GameObject.Find("Infrastructure");
                Instantiate(Igloo, tmp, new Quaternion(), parent.transform);
                gridScript.grid[(int)tmp.x, (int)tmp.z] = 1;
            }
        }

        
    }
}
