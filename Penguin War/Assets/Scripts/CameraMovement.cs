using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    GameObject mainCamera, cameraObject;

    // Start is called before the first frame update
    void Start()
    {
        //vsync
        QualitySettings.vSyncCount = 1;

        cameraObject = GameObject.Find("CameraObject");
        mainCamera = cameraObject.transform.GetChild(0).gameObject;
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 childGlobalPosition = this.transform.TransformPoint(mainCamera.transform.localPosition);

        if (Input.GetMouseButton(2))
        {
            Vector3 tmp = cameraObject.transform.position;
            tmp.x = -Input.GetAxis("Mouse X");
            tmp.z = -Input.GetAxis("Mouse Y");
            tmp.y = 0;


            if (mainCamera.transform.position.x < 5 && tmp.x < 0 || mainCamera.transform.position.x > 195 && tmp.x > 0)
            {
                tmp.x = 0;
            }
            if (mainCamera.transform.position.z < -5 && tmp.z < 0 || mainCamera.transform.position.z > 180 && tmp.z > 0)
            {
                tmp.z = 0;
            }

            cameraObject.transform.Translate(tmp);
        }

        if (Input.mouseScrollDelta.y != 0)
            {
                Vector3 tmp2 = new Vector3(0, 0, 0);
                tmp2.z = Input.mouseScrollDelta.y*5;
                Quaternion originalRotation = Quaternion.Euler(35f, 0f, 0f);
                Vector3 translatedPosition = childGlobalPosition + originalRotation * new Vector3(0f, 0f, tmp2.z);

                if (mainCamera.transform.position.y > 8 && tmp2.z > 0)
                {
                    mainCamera.transform.Translate(tmp2);
                }
                else if(mainCamera.transform.position.y < 30 && tmp2.z < 0 && translatedPosition.z > -3)
                {
                    mainCamera.transform.Translate(tmp2);
                }
                
            }
    }
}
