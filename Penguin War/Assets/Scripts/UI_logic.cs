using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_logic : MonoBehaviour
{
    Inventory inv;
    public TextMeshProUGUI text_fish, text_ice, text_wood;

    public bool IsPaused = false;

    public GameObject PauseUI;
    public GameObject GameplayUI;
    public CursorMovementTracker CursorTracker; 

    // Start is called before the first frame update
    void Start()
    {
        inv = GameObject.Find("Inventory").GetComponent<Inventory>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        CheckForPause();
        UpdateResources();

    }

    private void UpdateResources()
    {
        text_wood.text = inv.inventory["wood"].ToString();
        text_ice.text = inv.inventory["ice"].ToString();
        text_fish.text = inv.inventory["fish"].ToString();
    }

    private void CheckForPause() 
    {
        if (Input.GetKeyUp("escape"))
        {
            IsPaused = true;
            Time.timeScale = 0.0f;
            PauseUI.SetActive(true);
            GameplayUI.SetActive(false);
            Debug.Log("enabling pause");
        }
    }

    public void SetCursorMode(int new_mode)
    {
        CursorTracker.whatToBuild = new_mode;
    }

    public void SetSelectedButton() 
    {
        switch (CursorTracker.whatToBuild)
        {
            case 0:

            break;

            case 1:
                GameObject temp = GameplayUI.transform.Find("BuildingListPanel/IglooButton").gameObject;
                Debug.Log(temp.name);
                temp.GetComponent<Toggle>().isOn = true;
                Debug.Log("Fixing igloo");
             break;

            case 4:

            break;
        }
    }

}
