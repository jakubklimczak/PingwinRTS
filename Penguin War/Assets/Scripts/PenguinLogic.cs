using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;
using UnityEngine.UIElements;


public class PenguinLogic : MonoBehaviour
{
    // Start is called before the first frame update

    public int type;
    public int health = 20;
    public int maxHealth = 20;
    public int damage = 2;
    public Vector3 destination;
    public bool isBot = false;
    public bool isWarrior = false;
    public bool shouldAttack = false;
    public bool isAttacking = false;
    public int animationTimer = 100;

    List<string> directionList = new List<string>(); 

    public GameObject houseToAttack = null;
    public GameObject warriorPrefab;
    public GameObject lapka, lapka2, lapka3, lapka4, mieczor;
    Inventory inv;

    PathfindingXD pathfinding;

    SoundEffectsPlayer sounds;

    int ingotsNeededForWarrior = 10;
    int fishNeededForWarrior = 20;

    int ofs = 1;

    /*
     *  wywołujemy algorytm z PathfindingXD i wpisujemy do jakiejś tam listy
     *  mamy korutyne -> ma delay taki, że nie doleci do tego "następnego" pola
     *  w korutynie bierze pierwszą rzecz z listy, usuwa i ustawia destination tak żeby tam poleciał
     * */

    public IEnumerator setNextDestination()
    {
        if(directionList.Count > 0)
        {
            string currentDirection = directionList[0];
            switch (currentDirection)
            {
                case "Left":
                    destination = destination + new Vector3(-1, 0, 0 );
                    break;
                case "Right":
                    destination = destination + new Vector3(1, 0, 0);
                    break;
                case "Up":
                    destination = destination + new Vector3(0, 0, -1);
                    break;
                case "Down":
                    destination = destination + new Vector3(0, 0, 1);
                    break;
                default:
                    Debug.Log("uh oh, stinky - coś nie tak z pathfindingiem");
                    break;
            }
            directionList.RemoveAt(0);
        }
        yield return null;
    }

    public void penguinWannaMove(Vector3 origin, Vector3 destination)
    {
        int originX = Mathf.RoundToInt(origin.x);
        int originY = Mathf.RoundToInt(origin.y);
        int destinationX = Mathf.RoundToInt(destination.x);
        int destinationY = Mathf.RoundToInt(destination.y);
        directionList = pathfinding.FindPath(originX, originY, destinationX, destinationY);
    }

    void Start()
    {
        destination = this.gameObject.transform.position;
        inv = GameObject.Find("Inventory").GetComponent<Inventory>();
        pathfinding = GameObject.Find("EventSystem").GetComponent<PathfindingXD>();

        animationTimer = 100;
        //Debug.Log(gameObject.transform.Find("Penguin/pelvis").GetChild(2).gameObject.name);
        lapka = gameObject.transform.Find("Penguin/pelvis").GetChild(2).GetChild(1).gameObject;
        lapka2 = gameObject.transform.Find("Penguin/pelvis").GetChild(2).GetChild(1).GetChild(0).gameObject;
        lapka3 = gameObject.transform.Find("Penguin/pelvis/spine/shoulder.R/shoulder.L.001/shoulder.L.002").gameObject;
        lapka4 = gameObject.transform.Find("Penguin/pelvis/spine/shoulder.R/shoulder.L.001/shoulder.L.002/shoulder.L.003").gameObject;
        if (this.isWarrior)
        {
            mieczor = gameObject.transform.Find("Penguin/pelvis").GetChild(2).GetChild(1).GetChild(0).GetChild(0).GetChild(0).GetChild(1).gameObject;
            mieczor.transform.Rotate(0, 0, -40);
        }


        sounds = GameObject.Find("CameraObject").GetComponent<SoundEffectsPlayer>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        StartCoroutine(setNextDestination());
        //bede umieral
        if (health<=0)
        {
            sounds.ply_died();
            Destroy(gameObject);
        }

        Vector3 pos = this.gameObject.transform.position;
        float step = 0.1f;

        if(houseToAttack !=null && houseToAttack.tag == "pingu")//goń pingwina
        {
            destination = houseToAttack.GetComponent<PenguinLogic>().destination;
        }

        if (pos.x != destination.x || pos.y!=destination.y || pos.z != destination.z)
        {
            Vector3 move = new Vector3(0, 0, 0);

            move.x = pos.x < destination.x ? step : -step;
            move.z = pos.z < destination.z ? step : -step;

            move.x = pos.x - step < destination.x && pos.x + step > destination.x ? 0 : move.x;
            move.z = pos.z - step < destination.z && pos.z + step > destination.z ? 0 : move.z;

            this.transform.Translate(move);

            this.transform.GetChild(0).transform.LookAt(new Vector3(destination.x, 2, destination.z));
            this.transform.GetChild(1).transform.LookAt(new Vector3(destination.x, 2, destination.z));
        }

        if (pos.x < destination.x + ofs && pos.x > destination.x - ofs &&
            pos.z < destination.z + ofs && pos.z > destination.z - ofs && shouldAttack)
        {
            isAttacking = true;
        }

        if(isAttacking && houseToAttack != null && animationTimer == 0)
        {
            if(houseToAttack.tag=="houses" && isWarrior)
            {
                houseToAttack.GetComponent<HouseInfo>().health -= damage;
                if(houseToAttack==null || houseToAttack.GetComponent<HouseInfo>()==null || houseToAttack.GetComponent<HouseInfo>().health <= 0)
                {
                    houseToAttack = null;
                    shouldAttack = false;
                    isAttacking = false;
                }
            }else if(houseToAttack.tag=="houses" && !isWarrior && houseToAttack.name=="huta_igloo(Clone)")
            {
                if(inv.inventory["scraps"] > 5 && inv.inventory["wood"] > 2 && !isBot)
                {
                    inv.inventory["scraps"] -= 5;
                    inv.inventory["wood"] -= 2;
                    inv.inventory["ingots"] += 1;
                }
            }else if(houseToAttack.tag=="houses" && !isWarrior)
            {//change to warrior
                if(inv.inventory["ingots"] > ingotsNeededForWarrior && inv.inventory["fish"] > fishNeededForWarrior && !isBot)
                {
                    inv.inventory["ingots"] -= ingotsNeededForWarrior;
                    inv.inventory["fish"] -= fishNeededForWarrior;

                    System.Random rnd1 = new System.Random();
                    Vector3 p1 = houseToAttack.gameObject.transform.position;
                    destination = new Vector3(rnd1.Next((int)p1.x + 1, (int)p1.x + 3), 0.1f, rnd1.Next((int)p1.z + 1, (int)p1.z + 3));
                    //isWarrior = true;

                    GameObject tmpP = Instantiate(warriorPrefab, gameObject.transform.position, gameObject.transform.rotation, gameObject.transform.parent);
                    tmpP.GetComponent<PenguinLogic>().isBot=isBot;
                    tmpP.GetComponent<PenguinLogic>().damage=damage;
                    tmpP.GetComponent<PenguinLogic>().destination=destination;
                    tmpP.GetComponent<PenguinLogic>().health=health;
                    tmpP.GetComponent<PenguinLogic>().houseToAttack=null;
                    tmpP.GetComponent<PenguinLogic>().isAttacking=false;
                    tmpP.GetComponent<PenguinLogic>().shouldAttack=false;
                    tmpP.GetComponent<PenguinLogic>().isWarrior=true;
                    tmpP.GetComponent<PenguinLogic>().type=type;
                    Destroy(gameObject);
                }
                System.Random rnd = new System.Random();
                Vector3 p = houseToAttack.gameObject.transform.position;
                destination = new Vector3(rnd.Next((int)p.x + 1, (int)p.x + 3), 0.1f, rnd.Next((int)p.z + 1, (int)p.z + 3));
                //houseToAttack.GetComponent<HouseInfo>().health -= damage;
                isAttacking=false;
                shouldAttack=false;
                houseToAttack=null;
            }else if(houseToAttack.tag=="EnemyNest" && isWarrior)
            {
                houseToAttack.GetComponent<NestSpawner>().health -= damage;
            }else if(houseToAttack.tag=="pingu")
            {

                houseToAttack.GetComponent<PenguinLogic>().health -=damage;

                if(houseToAttack.GetComponent<PenguinLogic>() == null || houseToAttack.GetComponent<PenguinLogic>().health <= 0)//jak atakowany zdechnie
                {
                    houseToAttack = null;
                    shouldAttack = false;
                    isAttacking = false;
                }

            }else if((houseToAttack.tag=="resource" && !isWarrior) || (!isWarrior && houseToAttack.GetComponent<ResourceLogic>().type=="molo"))
            {
                if(houseToAttack.GetComponent<ResourceLogic>().type!="molo" && !isBot)
                {
                    inv.inventory[houseToAttack.GetComponent<ResourceLogic>().type] += damage;
                }else if(houseToAttack.GetComponent<ResourceLogic>().type=="molo" && !isBot)
                {
                    System.Random rnd = new System.Random();

                    if(rnd.Next(0,2) == 1)
                    {
                        inv.inventory["fish"] += 1;
                    }else
                    {
                        inv.inventory["wood"] += damage;
                    }
                }
            }
            
        }
    }

    void Update()
    {
        if (isAttacking || true)
        {
            if (animationTimer > 0)
            {
                //tukej animejszons
                if (isWarrior == true && isAttacking == true)
                {
                    if (animationTimer < 25)
                    {
                        //lapka.transform.Translate(0.0020579f, 0, -0.0000558976f);
                        //lapka2.transform.Translate(-0.0005582816f, 0, 0.00013782336f);
                        //lapka3.transform.Translate(-0.01413232f, 0, 0.00018630092f);
                        //lapka4.transform.Translate(-0.0006052968f, 0, -0.00037247104f);
                        lapka.transform.Rotate(0.001f, 1, 0.01f);
                        lapka2.transform.Rotate(0.001f, 1, 0.01f);
                        lapka3.transform.Rotate(0.001f, 1, 0.01f);
                        lapka4.transform.Rotate(0.001f, 1, 0.01f);
                        mieczor.transform.Rotate(0.0001f, 0.0001f, 0.01f);
                        lapka.transform.Translate(-0.00002f, 0, -0.00001f);
                        lapka2.transform.Translate(-0.00002f, 0, -0.00001f);
                        lapka3.transform.Translate(-0.00002f, 0, -0.00001f);
                        mieczor.transform.Translate(-0.00002f, -0.00001f, -0.00001f);
                    }
                    else if (animationTimer >= 25 && animationTimer < 75)
                    {
                        //lapka.transform.Translate(-0.0020579f, 0, 0.0000558976f);
                        //lapka2.transform.Translate(0.0005582816f, 0, -0.00013782336f);
                        //lapka3.transform.Translate(0.01413232f, 0, -0.00018630092f);
                        //lapka4.transform.Translate(0.0006052968f, 0, 0.00037247104f);
                        lapka.transform.Rotate(-0.001f, -1, -0.01f);
                        lapka2.transform.Rotate(-0.001f, -1, -0.01f);
                        lapka3.transform.Rotate(-0.001f, -1, -0.01f);
                        lapka4.transform.Rotate(-0.001f, -1, -0.01f);
                        mieczor.transform.Rotate(-0.0001f, -0.0001f, -0.01f);
                        lapka.transform.Translate(0.00002f, 0, 0.00001f);
                        lapka2.transform.Translate(0.00002f, 0, 0.00001f);
                        lapka3.transform.Translate(0.00002f, 0, 0.00001f);
                        mieczor.transform.Translate(0.00002f, 0.00001f, 0.00001f);
                    }
                    else
                    {
                        //lapka.transform.Translate(0.0020579f, 0, -0.0000558976f);
                        //lapka2.transform.Translate(-0.0005582816f, 0, 0.00013782336f);
                        //lapka3.transform.Translate(-0.01413232f, 0, 0.00018630092f);
                        //lapka4.transform.Translate(-0.0006052968f, 0, -0.00037247104f);
                        lapka.transform.Rotate(0.001f, 1, 0.01f);
                        lapka2.transform.Rotate(0.001f, 1, 0.01f);
                        lapka3.transform.Rotate(0.001f, 1, 0.01f);
                        lapka4.transform.Rotate(0.001f, 1, 0.01f);
                        mieczor.transform.Rotate(0.0001f, 0.0001f, 0.01f);
                        lapka.transform.Translate(-0.00002f, 0, -0.00001f);
                        lapka2.transform.Translate(-0.00002f, 0, -0.00001f);
                        lapka3.transform.Translate(-0.00002f, 0, -0.00001f);
                        mieczor.transform.Translate(-0.00002f, -0.00001f, -0.00001f);
                    }
                }
                animationTimer--;
            }
            else
            {
                animationTimer = 100;//nwm czy tyle wystarczy na animacje
            }
        }
        else
        {
            animationTimer = 0;
        }
    }
}
