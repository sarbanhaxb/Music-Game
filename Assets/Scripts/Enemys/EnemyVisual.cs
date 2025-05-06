using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyVisual : MonoBehaviour
{
    private Animator animator;
    private EnemyAI EnemyAI;
    private SpriteRenderer spriteRenderer;

    [Header("Animators")]
    [SerializeField] private RuntimeAnimatorController defaultAnimator;
    [SerializeField] private RuntimeAnimatorController aggressiveAnimator;

    [Header("Death parameter")]
    [SerializeField] private float jumpHeight = 0.5f;
    [SerializeField] private float jumpDuration = 0.4f;
    [SerializeField] private float destroyDelay = 2f;
    [SerializeField] private float blinkInterval = 0.1f;

    public bool IsAggressive = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        defaultAnimator = animator.runtimeAnimatorController;
        EnemyAI = transform.parent.GetComponent<EnemyAI>();
        EnemyAlertSystem.Instance.OnPlayerAttacked += BecomeAggressive;
    }

    private void IsDamageOff() => animator.SetBool("IsDamage", false);

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
        if (EnemyAlertSystem.Instance != null)
        {
            EnemyAlertSystem.Instance.OnPlayerAttacked -= BecomeAggressive;
        }
    }

    private void Update()
    {
        animator.SetBool("IsIdle", EnemyAI.isIdle);
        if (IsAggressive) BecomeAggressive();
    }


    public IEnumerator DeathEffects(Vector3 originalPosition)
    {
        float timer = 0f;
        while (timer < jumpDuration)
        {
            timer += Time.deltaTime;
            float progress = Mathf.Sin((timer / jumpDuration) * Mathf.PI);
            transform.position = originalPosition + Vector3.up * progress * jumpHeight;
            yield return null;
        }
        transform.position = originalPosition;
        StartCoroutine(BlinkAndDestroy());
    }

    IEnumerator BlinkAndDestroy()
    {
        float timer = 0f;
        bool isVisible = true;

        while (timer < destroyDelay)
        {
            spriteRenderer.enabled = isVisible;
            isVisible = !isVisible;
            timer += blinkInterval;
            yield return new WaitForSeconds(blinkInterval);
        }

        Destroy(gameObject);
    }
}
