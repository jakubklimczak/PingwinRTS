using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseInfo : MonoBehaviour
{

    public string type;
    public bool isBot = false;
    public int health = 50;
    public int maxHealth = 50;
    public int resources = 0;
    SoundEffectsPlayer sounds;


    // Start is called before the first frame update
    void Start()
    {
        sounds = GameObject.Find("CameraObject").GetComponent<SoundEffectsPlayer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(health <= 0)
        {
            sounds.ply_colapse();
            Destroy(gameObject);
        }
    }
}
