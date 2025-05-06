using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MusicGames.Utils;

[SelectionBase]
public class EnemyAI : MonoBehaviour
{
    private Animator animator;
    public bool isIdle = false;
    public static bool aggressiveStatus;
    public NavMeshAgent navMeshAgent;
    private EnemyVisual enemyVisual;

    private static readonly int IsAttackHash = Animator.StringToHash("IsAttack");
    private static readonly int IsDamageHash = Animator.StringToHash("IsDamage");
    private static readonly int IsDeadHash = Animator.StringToHash("IsDead");

    [Header("HP")]
    [SerializeField] private int maxHealth;
    [SerializeField] private int currentHealth;

    [Header("Roaming parameter")]
    [SerializeField] private float roamingDistanceMax = 7f;
    [SerializeField] private float roamingDistanceMin = 3f;
    [SerializeField] private float roamingTimerMax = 8f;
    [SerializeField] private float roamingTime;
    private Vector3 roamingPosition;
    private Vector3 startingPosition;
    private Vector3 pastPosition;

    [Header("Attack parameter")]
    [SerializeField] private float attackDistance = 1f;

    [Header("Chasing parameter")]
    [SerializeField] private float chasingDistance = 6f;

    [Header("States")]
    [SerializeField] private State startingState;
    [SerializeField] private State currentState;


    private void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        pastPosition = transform.position;
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        currentState = startingState;
        aggressiveStatus = false;
        currentHealth = maxHealth;
        enemyVisual = GetComponentInChildren<EnemyVisual>();
    }

    public enum State
    {
        Roaming,
        Attacking,
        Chasing,
        Death,
        Nothing
    }

    private void Update()
    {
        CheckIdle();
        StateHandler();
    }


    //Контроллер на текущий статус
    void StateHandler()
    {
        switch (currentState)
        {
            default:
                break;
            case State.Attacking:
                AttackAction();
                GetCurrentState();
                break;
            case State.Chasing:
                ChasingAction();
                GetCurrentState();
                break;
            case State.Death:
                DeathAction();
                break;

            case State.Roaming:
                RoamingAction();
                GetCurrentState();
                break;
            case State.Nothing:
                break;
        }
    }

    //Проверка корректности текущего состояния
    public void GetCurrentState()
    {
        if (aggressiveStatus && Vector3.Distance(transform.position, PlayerScript.Instance.transform.position) <= chasingDistance)
        {
            if (Vector3.Distance(transform.position, PlayerScript.Instance.transform.position) <= attackDistance)
            {
                currentState = State.Attacking;
            }
            else
            {
                if (animator.GetBool(IsDamageHash))
                {
                    navMeshAgent.isStopped = true;
                }
                else
                {
                    currentState = State.Chasing;
                    navMeshAgent.isStopped = false;
                }
            }
        }
        else
        {
            currentState = State.Roaming;
        }
    }

    //Получение урона
    public void TakeDamage(int damage)
    {
        if (!aggressiveStatus) aggressiveStatus = true;
        if (!animator.GetBool("IsAggressive")) EnemyAlertSystem.Instance.TriggerAggression();
        currentHealth -= damage;
        if (!DetectDeath()) animator.SetBool(IsDamageHash, true);
    }


    //Логика атаки (НЕ РЕАЛИЗОВАНО)
    private void AttackAction()
    {
        //animator.SetBool(IsAttackHash, true);
        //ChangeFacingDirection(transform.position, PlayerScript.Instance.transform.position);
    }

    //Логика смерти
    private bool DetectDeath()
    {
        if (currentHealth <= 0)
        {
            currentState = State.Death;
            return true;
        }
        return false;
    }
    private void DeathAction()
    {
        navMeshAgent.isStopped = true;
        GetComponent<Rigidbody2D>().simulated = false;
        GetComponent<CapsuleCollider2D>().enabled = false;
        animator.SetTrigger(IsDeadHash);
        StartCoroutine(enemyVisual.DeathEffects(transform.position));
        currentState = State.Nothing;
    }

    //Логика преследования
    private void ChasingAction()
    {
        navMeshAgent.ResetPath();
        navMeshAgent.SetDestination(PlayerScript.Instance.transform.position);
        ChangeFacingDirection(transform.position, PlayerScript.Instance.transform.position);
    }

    //Логика обычного передвижения
    private void RoamingAction()
    {
        roamingTime -= Time.deltaTime;
        if (roamingTime < 0)
        {
            Roaming();
            roamingTime = Random.Range(5f, roamingTimerMax);
        }
    }
    private void Roaming()
    {
        startingPosition = transform.position;
        roamingPosition = GetRoamingPosition();
        ChangeFacingDirection(startingPosition, roamingPosition);
        navMeshAgent.SetDestination(roamingPosition);
    }
    //Проверка стоит ли на месте или движется
    private void CheckIdle()
    {
        if (pastPosition == transform.position) isIdle = true;
        else isIdle = false;
        pastPosition = transform.position;
    }
    //Выдает рандомно позицию для брожения
    private Vector3 GetRoamingPosition() => startingPosition + Utils.GetRandomDir() * Random.Range(roamingDistanceMin, roamingDistanceMax);
    //Разворот анимации, при необходимости
    private void ChangeFacingDirection(Vector3 sourcePosition, Vector3 targetPosition)
    {
        if (sourcePosition.x > targetPosition.x) transform.rotation = Quaternion.Euler(0, -180, 0);
        else transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}
