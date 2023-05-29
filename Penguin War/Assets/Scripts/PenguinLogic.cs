using UnityEngine;


public class PenguinLogic : MonoBehaviour
{
    // Start is called before the first frame update

    public string type;
    public int health = 20;
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

            move.x = pos.x < destination.x ? step : -step;
            move.z = pos.z < destination.z ? step : -step;

            move.x = pos.x - step < destination.x && pos.x + step > destination.x ? 0 : move.x;
            move.z = pos.z - step < destination.z && pos.z + step > destination.z ? 0 : move.z;

            this.transform.Translate(move);

            this.transform.GetChild(0).transform.LookAt(new Vector3(destination.x, 2, destination.z));
            this.transform.GetChild(1).transform.LookAt(new Vector3(destination.x, 2, destination.z));
        }
    }
}
