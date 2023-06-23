using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    // Start is called before the first frame update

    public class Item
    {
           public string Type { get; set; }
           public int Amount { get; set; }
    }

    public Dictionary<string, int> inventory = new Dictionary<string, int>();

    void Start()
    {
        inventory.Add("wood", 100);
        inventory.Add("ice", 100);
        inventory.Add("fish", 1000);
        inventory.Add("scraps", 100);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
