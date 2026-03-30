using UnityEditor.Rendering;
using UnityEngine;

public class UnitAnimationManager : MonoBehaviour
{
    [SerializeField] RuntimeAnimatorController playerController, enemyController;
    Animator animator;
    [SerializeField] Avatar playerAvatar, enemyAvatar;

    //should have used enums for the factions but I didnt know how when starting this project -__-
    public void setAnimations(string myFaction)
    {
        GetComponent<Unit>().triggerAnimationAction += triggerAnimation;
        animator = GetComponent<Animator>();
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
        
        triggerAnimation(animationActions.Idle);
    }

    void triggerAnimation(animationActions anim)
    {
        switch (anim)
        {
            case animationActions.Attack:
                if (animator.GetBool("Walk")) animator.SetBool("Walk", false);
                if(animator.GetBool("Idle")) animator.SetBool("Idle", false);

                animator.SetTrigger("Attack");
                break;

            case animationActions.Walk:
                if(animator.GetBool("Idle")) animator.SetBool("Idle", false);
                animator.SetBool("Walk", true);
                break;

            case animationActions.Idle:
            default:
                if (animator.GetBool("Walk")) animator.SetBool("Walk", false);
                animator.SetBool("Idle", true);
                break;
                
        }
    }
}
