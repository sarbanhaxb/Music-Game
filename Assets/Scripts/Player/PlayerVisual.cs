using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class PlayerVisual : MonoBehaviour
{
    public Animator animator;
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private BoxCollider2D attackZone;

    public static PlayerVisual instance;

    private void Awake()
    {
        instance = this;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        animator.SetBool("IsRunning", PlayerScript.Instance.IsRunning());
        if (!animator.GetBool("IsAttack"))
        {
            AdjustPlayerFacingDirection();
        }
    }


    //Определяет в каком направлении движется ГГ
    private void AdjustPlayerFacingDirection()
    {
        Vector3 mousePos = GameInput.Instance.GetMousePosition();
        Vector3 playerPos = PlayerScript.Instance.GetPlayerPosition();

        if (mousePos.x < playerPos.x)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }

    //включает состояние атаки
    public void OnAttack()
    {
        if (!animator.GetBool("IsAttack"))
        {
            if (GameInput.Instance.GetMousePosition().x < PlayerScript.Instance.GetPlayerPosition().x)
            {
                attackZone.transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                attackZone.transform.localScale = new Vector3(1, 1, 1);
            }
            animator.SetBool("IsAttack", true);
        }
    }


    //включает коллайдер зоны атаки
    private void OnAttackStart()
    {
        attackZone.enabled = true;
    }

    //выключает состояния атаки и анимацию.
    private void OnAttackEnd()
    {
        animator.SetBool("IsAttack", false);
        attackZone.enabled = false;
    }

}
