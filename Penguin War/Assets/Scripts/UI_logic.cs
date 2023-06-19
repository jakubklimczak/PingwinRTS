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

    public void ToggleWhatToBuild(Toggle change)
    {
        if(change.isOn)
            setNewBuildMode(change.name);

        changecolor(change);
    }

    private void setNewBuildMode(string name)
    { 
        switch (name)
        {
            case "IglooButton":
                CursorTracker.whatToBuild = 1;
            break;

            case "MoloButton":
                CursorTracker.whatToBuild = 4;
            break;

            default:

            break;
        }
    }
    
//change color of toggle button because unity is stupid...
private void changecolor(Toggle changeToggle)
    {
        ColorBlock cb = changeToggle.colors;
        bool isOn = changeToggle.isOn;
        if (isOn)
        {
            cb.normalColor = Color.white;
            cb.highlightedColor = Color.white;
        }
        else
        {
            cb.normalColor = Color.gray;
            cb.highlightedColor = Color.gray;
        }
        Debug.Log("here");
        Debug.Log(cb.normalColor);
        changeToggle.colors = cb;
    }

}
