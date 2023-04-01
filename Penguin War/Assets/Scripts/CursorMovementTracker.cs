using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CursorMovementTracker : MonoBehaviour
{
    public Camera mainCamera;
    public LayerMask layerMask;
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
        if(Physics.Raycast(ray, out RaycastHit raycastHit,float.MaxValue , layerMask))
        {
            Vector3 tmp = new Vector3(((float)Math.Round(raycastHit.point.x)), 0.2f, ((float)Math.Round(raycastHit.point.z)));
            transform.position = tmp;
            
            if(Input.GetMouseButtonDown(0) && (int)tmp.x >= 0 && (int)tmp.z >= 0 && (int)tmp.x < 100 && (int)tmp.x < 100)
            {
                if (gridScript.grid[(int)tmp.x, (int)tmp.z] == 0)
                {
                    GameObject parent = GameObject.Find("Infrastructure");
                    Instantiate(Igloo, tmp, new Quaternion(), parent.transform);
                    gridScript.grid[(int)tmp.x, (int)tmp.z] = 1;
                    Debug.Log("boop");
                }
            }
        }

        
    }
}
