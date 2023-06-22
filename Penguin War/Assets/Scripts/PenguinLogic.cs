using UnityEngine;


public class PenguinLogic : MonoBehaviour
{
    // Start is called before the first frame update

    public int type;
    public int health = 20;
    public int damage = 2;
    public Vector3 destination;
    public bool isBot = false;
    public bool isWarrior = false;
    public bool shouldAttack = false;
    public bool isAttacking = false;
    public int animationTimer = 0;

    public GameObject houseToAttack = null;
    Inventory inv;

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
            }else if(houseToAttack.tag=="EnemyNest" && isWarrior)
            {
                houseToAttack.GetComponent<NestSpawner>().health -= damage;
            }else if((houseToAttack.tag=="resource" && !isWarrior) || (!isWarrior && houseToAttack.GetComponent<ResourceLogic>().type=="molo"))
            {
                if(houseToAttack.GetComponent<ResourceLogic>().type!="molo")
                {
                    inv.inventory[houseToAttack.GetComponent<ResourceLogic>().type] += damage;
                }else if(houseToAttack.GetComponent<ResourceLogic>().type=="molo")
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
