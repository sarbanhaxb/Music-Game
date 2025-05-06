using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class PlayerScript : MonoBehaviour
{
    public static PlayerScript Instance {  get; private set; }

    private static readonly int IsDamageHash = Animator.StringToHash("IsDamage");
    private static readonly int IsAttackHash = Animator.StringToHash("IsAttack");
    private static readonly int Death = Animator.StringToHash("Death");

    [Header("HP")]
    [SerializeField] private int maxHealth = 1000;
    [SerializeField] private int currentHealth;


    [SerializeField] private float movingSpeed = 5f;
    private float minMovingSpeed = 0.1f;
    private bool isRunning = false;

    private Rigidbody2D rb;

    private Vector2 inputVector;

    private void Awake()
    {
        currentHealth = maxHealth;
        Instance = this;
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        GameInput.Instance.OnPlayerAttack += Player_OnPlayerAttack;
    }

    private void Player_OnPlayerAttack(object sender, System.EventArgs e)
    {
        PlayerVisual.Instance.OnAttack();
    }

    private void Update()
    {
        if (!PlayerVisual.Instance.animator.GetBool(IsAttackHash) && !PlayerVisual.Instance.animator.GetBool(IsDamageHash))
        {
            inputVector = GameInput.Instance.GetMovementVector();
        }
    }

    private void FixedUpdate()
    {
        if (!PlayerVisual.Instance.animator.GetBool(IsAttackHash))
            HandleMovement();
    }

    private void TakeDamage(int damage)
    {
        if (currentHealth - damage <= 0)
        {
            PlayerVisual.Instance.animator.SetTrigger(Death);
        }
        else
        {
            currentHealth -= damage;
            PlayerVisual.Instance.animator.SetBool(IsDamageHash, true);
        }
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
        Debug.Log(collision.gameObject.tag);
        Debug.Log(collision.CompareTag("Player"));
        
        if (collision.TryGetComponent<EnemyAI>(out var enemy))
        {
            TakeDamage(50);
        }
    }
}
