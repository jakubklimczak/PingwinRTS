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
        text_wood.text = inv.inventory["wood"].ToString();
        text_ice.text = inv.inventory["ice"].ToString();
        text_fish.text = inv.inventory["fish"].ToString();
    }
}
