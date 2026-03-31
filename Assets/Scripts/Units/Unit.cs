using System;
using UnityEngine;

public abstract class Unit : MonoBehaviour, IUnitBase
{
    [Header("Stats")]
    
    public float speed, shortRange, longRange, distanceToTarget, attackCooldown;
    public int damage, health, maxHealth, command, previousCommand;
    public bool canAttack, isOnTopTrack, moving;
    public string faction;
    [SerializeField] GameObject playerGraphics, enemyGraphics;
    protected NavigationAgent agent;
    HealthBar healthBar;
    [Header("Targeting")]
    [SerializeField]protected Transform target, previousTarget;
    Rigidbody rb;
    public Action<animationActions> triggerAnimationAction;
    public Action<Unit> onDeath;

    void Awake()
    {
        CommunicationEvents.setUnitOrders += setCommand;
        healthBar = GetComponentInChildren<HealthBar>();
        agent = GetComponent<NavigationAgent>();
        healthBar.setMaxValue(maxHealth);
        command = 1; //Hold position
        TryGetComponent(out rb);
    }

    public virtual void Update()
    {
        if (target && target != previousTarget)
        {
            previousTarget = target;
            MoveTowardsTarget(target);
        }

        if (health <= 0)
        {
            Die();
        }
    }

    public virtual void Move(int move) //Retreat (0), Hold(1), Advance(2)
    {
        command = move;
    }

    public virtual void MoveTowardsTarget(Transform targetPosition)
    {
        command = 3;
        target = targetPosition;
    }

    public void Attack(Unit target)
    {
        target.TakeDamage(damage);
    }

    public void TakeDamage(int dmg)
    {
        health -= dmg;
        if (health <= 0)
        {
            Die();
        }
        healthBar.reduceHP(dmg);
    }

    public Transform getTarget()
    {
        return target;
    }

    public virtual void Spawn(string _faction, bool _isOnTopTrack, GameObject Spawner)
    {
        gameObject.tag = _faction;
        faction = _faction;
        isOnTopTrack = _isOnTopTrack;

        if (_faction == "Player")
        {
            transform.Find("player-graphics").gameObject.SetActive(true);
            transform.Find("enemy-graphics").gameObject.SetActive(false);
        }
        else
        {
            transform.Find("enemy-graphics").gameObject.SetActive(true);
            transform.Find("player-graphics").gameObject.SetActive(false);
        }
        GetComponent<UnitAnimationManager>().setAnimations(_faction);
        health = maxHealth;
        gameObject.layer = LayerMask.NameToLayer(_faction);
        Spawner.GetComponent<BaseController>().addUnitToUnitList(faction, this, _isOnTopTrack);
    }

    public void Die()
    {
        CommunicationEvents.RemoveUnitFromLists(this);
        onDeath?.Invoke(this);
        Destroy(gameObject);
    }

    protected virtual void setCommand(int _command, bool _isOnTop, string _faction)
    {
        if (_faction != faction) return;
        if (_isOnTop != isOnTopTrack) return;
        command = _command;
    }

    public float CalculateDistanceToTarget(Transform target)
    {
        return Vector3.Distance(transform.position, target.position);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, shortRange);
        Gizmos.DrawWireSphere(transform.position, longRange);
    }
}