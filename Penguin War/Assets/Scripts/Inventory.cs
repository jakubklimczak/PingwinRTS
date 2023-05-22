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

    public Item[] inventory = new Item[3];//change this if you need items in inv

    void Start()
    {
        inventory[0] = new Item { Type = "wood", Amount = 50 };
        inventory[1] = new Item { Type = "ice", Amount = 50 };
        inventory[2] = new Item { Type = "fish", Amount = 1000 };
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ChangeAmmount(string Ty, int Amm)
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            if (inventory[i].Type==Ty)
            {
                inventory[i].Amount += Amm;
            }
        }
    }
}
