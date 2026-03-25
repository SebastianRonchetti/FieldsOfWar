using System.Collections;
using UnityEngine;

public class CombatUnit : Unit
{
    [Header("Combat")]
    [SerializeField] LayerMask detectionLayer;
    [SerializeField] GameObject targetEnemy;
    [SerializeField] Unit targetEnemyUnit;
    [SerializeField] Building targetEnemyBuilding;
    [SerializeField] float visionAngle;
    /* 
    [SerializeField] protected AudioClip attackSound;
    [SerializeField] protected AudioSource audioSource; */
    [SerializeField] string opposingFaction;

    public override void Update()
    {
        // detect if enemy unit or building is nearby, if so set closest enemy unit to target
        if (targetEnemy == null)
        {
            targetEnemy = FindEnemyInVision();
        }
        //move towards enemy
        if (command != 0)
        {
            if (targetEnemy != null)
            {
                if (CalculateDistanceToTarget(targetEnemy.transform) < shortRange && canAttack)
                {
                    if (targetEnemy.TryGetComponent(out targetEnemyBuilding))
                    {
                        if (targetEnemyBuilding.TakeDamage(damage))
                        {
                            targetEnemy = null;
                        }
                    }
                    else if (targetEnemy.TryGetComponent(out targetEnemyUnit))
                    {
                        targetEnemyUnit.TakeDamage(damage);
                    }
                    //audioSource.PlayOneShot(attackSound);
                    canAttack = false;
                    StartCoroutine(reloadAttack());
                }
            }
            else if (command == 3)
            {
                command = previousCommand;
            }

            //move towards checkpoint
            base.Update();
        }
    }

    public override void Spawn(string _faction, bool _isOnTopTrack, GameObject spawner)
    {
        base.Spawn(_faction, _isOnTopTrack, spawner);
        canAttack = true;
        if (faction == "Player")
        {
            detectionLayer = LayerMask.GetMask("Enemy", "Unbreakable"); //enemy layer
            opposingFaction = "Enemy";
        }
        else
        {
            detectionLayer = LayerMask.GetMask("Player", "Unbreakable"); //player layer
            opposingFaction = "Player";
        }
    }

    private GameObject FindEnemyInVision()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, longRange, detectionLayer);
        foreach (var hit in hits)
        {
            if (hit.CompareTag(opposingFaction))
            {
                if (IsInVision(hit.gameObject))
                {
                    target = hit.gameObject.transform;
                    previousCommand = command;
                    command = 3;
                    return hit.gameObject;
                }
            }
        }
        return null;
    }

    private bool IsInVision(GameObject obj)
    {
        Vector3 dirToTarget = (obj.transform.position - transform.position).normalized;
        float angle = Vector3.Angle(transform.forward, dirToTarget);

        return angle < visionAngle * 0.5f;
    }

    IEnumerator reloadAttack()
    {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
    }

    protected override void setCommand(int _command, bool _isOnTop, string _faction)
    {
        if(command != 3) base.setCommand(_command, _isOnTop, _faction);
    }

    public bool isTargetEnemyOrNull()
    {
        if (!target) return false;
        return target.gameObject.TryGetComponent<Unit>(out _);
    }

    public void setTargetRallyPoint(Transform rallyPoint, int _command)
    {
        command = _command;
        target = rallyPoint;
        agent.HandleCommand(_command);
    }
}