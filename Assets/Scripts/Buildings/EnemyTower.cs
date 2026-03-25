using UnityEngine;

public class EnemyTower : Tower
{
    LayerMask allyLayer;
    [SerializeField] GameObject[] alliesInGarrison;
    public int garrison = 0;
    public int maxGarrison = 5;
    [SerializeField] Transform rallyPoint;

    protected override void Awake()
    {
        base.Awake();
        allyLayer = LayerMask.GetMask("Enemy");
        alliesInGarrison = new GameObject[maxGarrison];
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        if (!checkGarrisonFull())
        {
            FindAlliesInRange();
            orderStop();
        }
        else
        {
            orderAdvance();
        }
    }

    bool checkGarrisonFull()
    {
        for (int i = 0; i < alliesInGarrison.Length; i++)
        {
            if (!alliesInGarrison[i]) return false;
        }
        return true;
    }

    void orderStop()
    {
        for(int i = 0; i < alliesInGarrison.Length; i++)
        {
            if(alliesInGarrison[i])
            {
                if (!alliesInGarrison[i].GetComponent<CombatUnit>().isTargetEnemyOrNull())
                {
                    alliesInGarrison[i].GetComponent<CombatUnit>().setTargetRallyPoint(rallyPoint, 3);
                }
            }
        }
    }

    void orderAdvance()
    {
        for(int i = 0; i < alliesInGarrison.Length; i++)
        {
            if(alliesInGarrison[i])
            {
                if (!alliesInGarrison[i].GetComponent<CombatUnit>().isTargetEnemyOrNull())
                {
                    alliesInGarrison[i].GetComponent<CombatUnit>().setTargetRallyPoint(null, 2);
                }
            }
        }
    }

    private void FindAlliesInRange()
    {
        bool targetInArray = false;
        //check if object is already on the array
        //Check if array full with existing objects
        Collider[] hits = Physics.OverlapSphere(transform.position, range, allyLayer);

        foreach (var hit in hits)
        {
            if (!hit.TryGetComponent<CombatUnit>(out _)) return;

            for (int i = 0; i < alliesInGarrison.Length; i++)
            {
                if (hit.gameObject == alliesInGarrison[i])
                {
                    targetInArray = true;
                    break;
                }
                targetInArray = false;
            }

            if (!targetInArray)
            {
                for (int i = 0; i < alliesInGarrison.Length; i++)
                {
                    if (!alliesInGarrison[i])
                    {
                        alliesInGarrison[i] = hit.gameObject;
                        break;
                    }
                }
            }
        }
    }

    public void AllyDiesOrLeaves()
    {
        garrison--;
        AskForReinforcements();
    }

    private void AskForReinforcements()
    {
        EnemyManager.Instance.RequestReinforcements(this);
    }
}
