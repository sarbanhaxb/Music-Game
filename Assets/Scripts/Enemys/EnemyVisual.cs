using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyVisual : MonoBehaviour
{
    private Animator animator;
    private EnemyAI EnemyAI;


    [Header("Animators")]
    [SerializeField] private RuntimeAnimatorController defaultAnimator;
    [SerializeField] private RuntimeAnimatorController aggressiveAnimator;

    public bool IsAggressive = false;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        defaultAnimator = animator.runtimeAnimatorController;
        EnemyAI = transform.parent.GetComponent<EnemyAI>();
        EnemyAlertSystem.Instance.OnPlayerAttacked += BecomeAggressive;
    }

    
    //Триггер на агрессию
    public void BecomeAggressive()
    {
        switch (animator.GetBool("IsAggressive"))
        {
            case true:
                break;
            case false:
                animator.runtimeAnimatorController = aggressiveAnimator;
                animator.SetBool("IsAggressive", true);
                break;
        }
    }

    private void OnDestroy()
    {
        if(EnemyAlertSystem.Instance != null)
        {
            EnemyAlertSystem.Instance.OnPlayerAttacked -= BecomeAggressive;
        }
    }

    //Выключает поврежедение
    public void DamageOff()
    {
        animator.SetBool("IsDamage", false);
    }


    private void Update()
    {
        animator.SetBool("IsIdle", EnemyAI.isIdle);
        if (IsAggressive ) BecomeAggressive();
    }
}
