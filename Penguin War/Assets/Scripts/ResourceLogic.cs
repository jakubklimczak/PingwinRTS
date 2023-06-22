using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceLogic : MonoBehaviour
{

    public string type;

    // Start is called before the first frame update
    void Start()
    {
        if(this.gameObject.transform.parent.gameObject.name=="Boats")
        {
            type="scraps";
        }else if(this.gameObject.transform.parent.gameObject.name=="Glaciers")
        {
            type="ice";
        }
        if(this.gameObject.name=="molo(Clone)")
        {
            type="molo";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
