using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class PlayerScript : MonoBehaviour
{

    public static PlayerScript Instance {  get; private set; }

    [SerializeField]
    private float movingSpeed = 5f;
    private float minMovingSpeed = 0.1f;
    private bool isRunning = false;

    private Rigidbody2D rb;

    private Vector2 inputVector;

    private void Awake()
    {
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        GameInput.Instance.OnPlayerAttack += Player_OnPlayerAttack;
    }

    private void Player_OnPlayerAttack(object sender, System.EventArgs e)
    {
        PlayerVisual.instance.OnAttack();

    }

    private void Update()
    {
        if (!PlayerVisual.instance.animator.GetBool("IsAttack"))
        {
            inputVector = GameInput.Instance.GetMovementVector();
        }
    }

    private void FixedUpdate()
    {
        if (!PlayerVisual.instance.animator.GetBool("IsAttack"))
            HandleMovement();
    }

    private void HandleMovement()
    {
        rb.MovePosition(rb.position + inputVector * (movingSpeed * Time.fixedDeltaTime));

        if (Mathf.Abs(inputVector.x) > minMovingSpeed || Mathf.Abs(inputVector.y) > minMovingSpeed)
        {
            isRunning = true;
        }
        else
        {
            isRunning = false;
        }
    }

    public bool IsRunning() => isRunning;
    public Vector3 GetPlayerPosition() => Camera.main.WorldToScreenPoint(transform.position);

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        EnemyEntity enemy = collision.GetComponent<CapsuleCollider2D>().GetComponent<EnemyEntity>();
        if (enemy != null) enemy.TakeDamage(50);
    }
}
