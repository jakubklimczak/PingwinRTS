using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Net.NetworkInformation;

public class UI_logic : MonoBehaviour
{
    Inventory inv;
    public TextMeshProUGUI text_fish, text_ice, text_wood,text_scrap,text_iron;

    public GameObject unit_panel;
    public GameObject upgrade_btn;
    public Image unit_icon;
    public Sprite[] icon_sprites_array;

    public Slider hp_value_slider;

    public Camera mainCamera;

    public bool IsPaused = false;

    public GameObject PauseUI, GameplayUI;
    public CursorMovementTracker CursorTracker;

    private int current_health = 1, current_max_health = 1;
    private string current_owner = "";
    private int current_attack = 0;

    private HouseInfo current_house;
    private NestSpawner current_nest;

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
        text_scrap.text = inv.inventory["scraps"].ToString();
        text_iron.text = inv.inventory["ingots"].ToString();
    }

    private void CheckForPause() 
    {
        if (Input.GetKeyUp("escape"))
        {
            IsPaused = true;
            Time.timeScale = 0.0f;
            PauseUI.SetActive(true);
            GameplayUI.SetActive(false);
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

            case "HutaButton":
                CursorTracker.whatToBuild = 8;
            break;

            case "IceWallButton":
                CursorTracker.whatToBuild = 10;
            break;

            case "UpgradeButton":
                CursorTracker.whatToBuild = 9;
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
        changeToggle.colors = cb;
    }
    private void updateUnitPanel()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out RaycastHit hatkaRayHit, float.MaxValue, 1 << 7))//8 bo 8 layer to pingwiny
        {
            current_house = hatkaRayHit.collider.gameObject.GetComponent<HouseInfo>();
            current_nest = null;
            current_health = current_house.health;
            current_max_health = current_house.maxHealth;
            current_attack = 0;
            current_owner = current_house.isBot ? "Enemy" : "Ally";
            unit_panel.SetActive(true);
            setCurrentUnitIcon(current_house.name);


        }

        if (Input.GetMouseButtonDown(0) && Physics.Raycast(ray, out RaycastHit nestRayHit, float.MaxValue, 1 << 9))//8 bo 8 layer to pingwiny
        {
            if (nestRayHit.collider.gameObject.GetComponent<NestSpawner>() != null)
            {
                current_nest = nestRayHit.collider.gameObject.GetComponent<NestSpawner>();
                current_house = null;
                current_health = current_nest.health;
                current_max_health = current_nest.maxHealth;
                current_attack = 0;
                current_owner = current_nest.isBot ? "Enemy" : "Ally";
            }

            if (nestRayHit.collider.gameObject.GetComponent<HouseInfo>() != null)//troch� to skoplikowane wi�c �atwiej �eby by� kod zdublowany
            {
                current_house = nestRayHit.collider.gameObject.GetComponent<HouseInfo>();
                current_nest = null;
                current_health = current_house.health;
                current_max_health = current_house.maxHealth;
                current_attack = 0;
                current_owner = current_house.isBot ? "Enemy" : "Ally";
            }
            unit_panel.SetActive(true);

            if(current_house!=null)
                setCurrentUnitIcon(current_house.name);

            if (current_nest != null)
                setCurrentUnitIcon(current_nest.name);
        }


        PenguinLogic temp_pingu_logic = CursorTracker.GetCurrentlySelectedPenguine();
        if (temp_pingu_logic != null)
        {
            current_health = temp_pingu_logic.health;
            current_max_health = temp_pingu_logic.maxHealth;
            current_attack = temp_pingu_logic.damage;
            current_owner = temp_pingu_logic.isBot ? "Enemy" : "Ally";
            if (temp_pingu_logic.isWarrior)
            {
                setCurrentUnitIcon("single_warrior");
            }
            else
            {
                setCurrentUnitIcon("single_basic");
            }

            current_nest = null;
            current_house = null;
            unit_panel.SetActive(true);
        }

        //moim zdaniem takie rozbijanie na list i pojednyczego to dalej średni pomysł
        List<PenguinLogic> temp_pingu_list = CursorTracker.GetCurrentlySelectedPenguineList();
        if (temp_pingu_list.Count > 0)
        {
            setAveragesPinguStatsFromList(temp_pingu_list);
            if(temp_pingu_list.Count > 1)
            {
                setCurrentUnitIcon("multiple");
            }
            else
            {
                setCurrentUnitIcon("single_basic");
            }

            current_nest = null;
            current_house = null;
            unit_panel.SetActive(true);
        }

        if (temp_pingu_logic == null && temp_pingu_list.Count <= 0 && current_house == null && current_nest == null)
        {
            unit_panel.SetActive(false);
        }

        if(current_house != null)
        {
            if (current_house.name == "upgr(Clone)")
            {
                upgrade_btn.SetActive(true);
            }
        }

        if(current_house == null || current_house.name != "upgr(Clone)")
        {
            upgrade_btn.SetActive(false);
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

    private void setAveragesPinguStatsFromList(List<PenguinLogic> pingu_list) 
    {
        int avg_max_health = 0;
        int avg_health = 0;
        int avg_attack = 0;

        foreach (PenguinLogic p in pingu_list)
        {
            avg_max_health += p.maxHealth;
            avg_health += p.health;
            avg_attack += p.damage;
            current_owner = p.isBot ? "Enemy" : "Ally";
        }
        int list_count = pingu_list.Count;

        current_max_health = avg_max_health / list_count;
        current_health = avg_health/list_count;
        current_attack = avg_attack/list_count;

    }

    private void setCurrentUnitIcon(string unit_type)
    {
        switch (unit_type)
        {
            case "single_basic":
                unit_icon.sprite = icon_sprites_array[0];
                break;
            case "single_warrior":
                unit_icon.sprite = icon_sprites_array[1];
                break;
            case "multiple":
                unit_icon.sprite = icon_sprites_array[2];
                break;
            case "Igloo(Clone)":
                unit_icon.sprite = icon_sprites_array[3];
                break;
            case "huta_igloo(Clone)":
                unit_icon.sprite = icon_sprites_array[4];
                break;
            case "ice-wall(Clone)":
                unit_icon.sprite = icon_sprites_array[5];
                break;
            case "upgr(Clone)":
                unit_icon.sprite = icon_sprites_array[6];
                break;
            case "koszary":
            case "koszaryEnemy":
                unit_icon.sprite = icon_sprites_array[7];
                break;
            default:
            case "none":
                Debug.Log(unit_type);
                break;
        }
    }

    public void upgradeUnits()
    {
        float value = 0.1f;
        if (inv.checkUpgradePos())
        {
            GameObject.Find("koszary").GetComponent<NestSpawner>().upgradeDefaultHP(value);
        }
            

    }
}
