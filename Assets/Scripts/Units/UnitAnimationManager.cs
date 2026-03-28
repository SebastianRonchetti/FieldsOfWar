using UnityEditor.Rendering;
using UnityEngine;

public class UnitAnimationManager : MonoBehaviour
{
    [SerializeField] RuntimeAnimatorController playerController, enemyController;
    Animator animator;
    [SerializeField] Avatar playerAvatar, enemyAvatar;
    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void Start()
    {
        GetComponent<Unit>().triggerAnimationAction += triggerAnimation;
        triggerAnimation(animationActions.Idle);
    }
    //should have used enums for the factions but I didnt know how when starting this project -__-
    public void setAnimations(string myFaction)
    {
        switch (myFaction)
        {
            case "Player":
            animator.avatar = playerAvatar;
            animator.runtimeAnimatorController = playerController;
                break;
            case "Enemy":
            animator.avatar = enemyAvatar;
            animator.runtimeAnimatorController = enemyController;
                break;
        }
    }

    void triggerAnimation(animationActions anim)
    {
        switch (anim)
        {
            case animationActions.Attack:
                if (animator.GetBool("Move")) animator.SetBool("Move", false);
                if(animator.GetBool("Idle")) animator.SetBool("Idle", false);

                animator.SetTrigger("Attack");
                break;

            case animationActions.Walk:
                if(animator.GetBool("Idle")) animator.SetBool("Idle", false);
                animator.SetBool("Move", true);
                break;

            case animationActions.Idle:
            default:
                if (animator.GetBool("Move")) animator.SetBool("Move", false);
                animator.SetBool("Idle", true);
                break;
                
        }
    }
}
