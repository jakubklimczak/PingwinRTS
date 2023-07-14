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
        inventory.Add("wood", 30);
        inventory.Add("ice", 0);
        inventory.Add("fish", 0);
        inventory.Add("scraps", 0);
        inventory.Add("ingots", 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool checkUpgradePos()
    {
        if (inventory["fish"]>20 && inventory["ingots"] > 20)
        {
            inventory["fish"] -= 20;
            inventory["ingots"] -= 20;
            return true;
        }
        return false;
    }

}
