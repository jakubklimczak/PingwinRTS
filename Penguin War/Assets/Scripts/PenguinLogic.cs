using UnityEngine;


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
    public int animationTimer = 0;
    

    public GameObject houseToAttack = null;
    public GameObject warriorPrefab;
    Inventory inv;

    int ingotsNeededForWarrior = 10;
    int fishNeededForWarrior = 20;

    int ofs = 1;


    void Start()
    {
        destination = this.gameObject.transform.position;
        inv = GameObject.Find("Inventory").GetComponent<Inventory>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 pos = this.gameObject.transform.position;
        float step = 0.1f;

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

    void Update() {
        if(isAttacking)
        {
            if(animationTimer > 0)
            {
                //tukej animejszons

                animationTimer--;
            }else
            {
                animationTimer = 100;//nwm czy tyle wystarczy na animacje
            }
        }else
        {
            animationTimer = 0;
        }
    }
}
