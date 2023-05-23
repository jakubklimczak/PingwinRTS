using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_logic : MonoBehaviour
{
    Inventory inv;
    public TextMeshProUGUI text_fish, text_ice, text_wood;

    // Start is called before the first frame update
    void Start()
    {
        inv = GameObject.Find("Inventory").GetComponent<Inventory>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        text_wood.text = "wood: "+inv.inventory[0].Amount.ToString();
        text_ice.text = "ice: "+inv.inventory[1].Amount.ToString();
        text_fish.text = "fish: "+inv.inventory[2].Amount.ToString();
    }
}
