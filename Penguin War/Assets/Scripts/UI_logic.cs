using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Net.NetworkInformation;

public class UI_logic : MonoBehaviour
{
    Inventory inv;
    public TextMeshProUGUI text_fish, text_ice, text_wood;

    public Slider hp_value_slider;

    public Camera mainCamera;

    public bool IsPaused = false;

    public GameObject PauseUI, GameplayUI;
    public CursorMovementTracker CursorTracker;

    private int current_health = 1, current_max_health = 1;
    private string current_owner = "";
    private int current_attack = 0;
    private PenguinLogic current_penguin = null;
    private HouseInfo current_house = null;
    private NestSpawner current_nest = null;

    public TextMeshProUGUI CurrentMaxHealth, CurrentValueHealth,AttackValue,Owner;
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
    private void Update()
    {
        updateUnitPanel();
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
            case "SelectUnitButton":
                CursorTracker.whatToBuild = 0;
            break;

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
    private void updateUnitPanel()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out RaycastHit pinguRayHit, float.MaxValue, 1 << 8))//8 bo 8 layer to pingwiny
        {
            current_penguin = pinguRayHit.collider.gameObject.GetComponent<PenguinLogic>();
            current_health = current_penguin.health;
            current_max_health = current_penguin.maxHealth;
            current_attack = current_penguin.damage;
            current_owner = current_penguin.isBot ? "Enemy" : "Ally";
            setCurrentHealth();
            return;
        }

        if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out RaycastHit hatkaRayHit, float.MaxValue, 1 << 7))//8 bo 8 layer to pingwiny
        {
            current_house = hatkaRayHit.collider.gameObject.GetComponent<HouseInfo>();
            current_health = current_house.health;
            current_max_health = current_house.maxHealth;
            current_attack = 0;
            current_owner = current_house.isBot ? "Enemy" : "Ally";
            setCurrentHealth();
            return;
        }

        if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out RaycastHit nestRayHit, float.MaxValue, 1 << 9))//8 bo 8 layer to pingwiny
        {
            if (nestRayHit.collider.gameObject.GetComponent<NestSpawner>() != null)
            {
                current_nest = nestRayHit.collider.gameObject.GetComponent<NestSpawner>();
                current_health = current_nest.health;
                current_max_health = current_nest.maxHealth;
                current_attack = 0;
                current_owner = current_nest.isBot ? "Enemy" : "Ally";
                setCurrentHealth();
                return;
            }

            if (nestRayHit.collider.gameObject.GetComponent<HouseInfo>() != null)//trochê to skoplikowane wiêc ³atwiej ¿eby by³ kod zdublowany
            {
                current_house = nestRayHit.collider.gameObject.GetComponent<HouseInfo>();
                current_health = current_house.health;
                current_max_health = current_house.maxHealth;
                current_attack = 0;
                current_owner = current_house.isBot ? "Enemy" : "Ally";
                setCurrentHealth();
                return;
            }
        }
        if(current_house != null)
        {
            current_health = current_house.health;
            current_max_health = current_house.maxHealth;
        }
        if (current_nest != null)
        {
            current_health = current_nest.health;
            current_max_health = current_nest.maxHealth;
        }
        if (current_penguin != null)
        {
            current_health = current_penguin.health;
            current_max_health = current_penguin.maxHealth;
        }
        setCurrentHealth();
        setCurrentAttack();
        setCurrentOwner();

    }

    private void setCurrentHealth() 
    {
        float health_percent = (float)current_health / (float)current_max_health;
        hp_value_slider.value = health_percent;
        CurrentMaxHealth.text = current_max_health.ToString();
        CurrentValueHealth.text = current_health.ToString();
    }

    private void setCurrentAttack()
    {
        AttackValue.text = current_attack.ToString();
    }

    private void setCurrentOwner()
    {
        Owner.text = current_owner;
    }
}
