using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEntity : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private float jumpHeight = 0.5f;
    [SerializeField] private float jumpDuration = 0.4f;
    [SerializeField] private float destroyDelay = 2f;
    [SerializeField] private float blinkInterval = 0.1f;

    [SerializeField] private bool isDead = false;

    [Header("HP")]
    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private Vector3 originalPosition;


    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
        currentHealth = maxHealth;
    }


    //Получение урона
    public void TakeDamage(int damage)
    {
        if (!EnemyAI.aggressiveStatus)
        {
            GetComponent<EnemyAI>().SetAggressiveStatus();
        }

        if (!animator.GetBool("IsAggressive"))
        {
            EnemyAlertSystem.Instance.TriggerAggression();
        }
        currentHealth -= damage;
        DetectDeath();
    }


    //Проверка смерти
    private void DetectDeath()
    {
        if (currentHealth <= 0)
        {
            originalPosition = transform.position;
            DieAnimation();
        }
        else
        {
            animator.SetBool("IsDamage", true);
        }
    }

    //Анимации смерти
    private void DieAnimation()
    {
        if (isDead) return;
        isDead = true;

        GetComponent<CapsuleCollider2D>().enabled = false;
        if (TryGetComponent(out EnemyAI ai)) ai.SetDeath();
        if (TryGetComponent(out Rigidbody2D rb)) rb.simulated = false;
        animator.SetTrigger("IsDead");

        StartCoroutine(DeathEffects());
    }

    IEnumerator DeathEffects()
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
