using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.GraphicsBuffer;

public class PenguinLogic : MonoBehaviour
{
    // Start is called before the first frame update

    public string type;
    public Vector3 destination;


    void Start()
    {
        destination = this.gameObject.transform.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 pos = this.gameObject.transform.position;
        float step = 0.1f;

        if (pos.x != destination.x || pos.y!=destination.y || pos.z != destination.z)
        {
            Vector3 move = new Vector3(0, 0, 0);

            Vector3 dir = pos - destination;

            move.x = pos.x < destination.x ? step : -step;
            move.z = pos.z < destination.z ? step : -step;

            move.x = pos.x - step < destination.x && pos.x + step > destination.x ? 0 : move.x;
            move.z = pos.z - step < destination.z && pos.z + step > destination.z ? 0 : move.z;

            this.transform.Translate(move);
        }
    }
}
