using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LakeConnectorXD : MonoBehaviour
{
    GameObject finalLake;
    MeshFilter[] meshFilters = new MeshFilter[5];

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(gameObject.name);
        Debug.Log(gameObject.transform.Find("watah1").gameObject.name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
