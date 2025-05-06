using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using MusicGames.Utils;

public class EnemyAI : MonoBehaviour
{
    [Header("Roaming parameter")]
    [SerializeField] private float roamingDistanceMax = 7f;
    [SerializeField] private float roamingDistanceMin = 3f;
    [SerializeField] private float roamingTimerMax = 8f;
    [SerializeField] private float roamingTime;
    private Vector3 roamingPosition;
    private Vector3 startingPosition;
    private Vector3 pastPosition;

    [Header("Chasing parameter")]
    [SerializeField] private float chasingDistance = 6f;
    //[SerializeField] private float chasingSpeedMultiply = 2f;

    [Header("States")]
    [SerializeField] private State startingState = State.Roaming;
    [SerializeField] private State currentState;
    public bool isIdle = false;
    public static bool aggressiveStatus;

    public NavMeshAgent navMeshAgent;


    private void Awake()
    {
        pastPosition = transform.position;
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = false;
        navMeshAgent.updateUpAxis = false;
        currentState = startingState;
        aggressiveStatus = false;
    }

    //Метод, меняющий состояние на преследование. Публичный.
    public void SetAggressiveStatus()
    {
        aggressiveStatus = true;
    }

    //Метод, меняющий состояние на смерть. Публичный.
    public void SetDeath()
    {
        currentState = State.Death;
    }
    public enum State
    {
        Roaming,
        Attacking,
        Chasing,
        Death
    }

    private void Update()
    {
        CheckIdle();
        StateHandler();
    }

    //Проверка стоит ли на месте или движется
    private void CheckIdle()
    {
        if (pastPosition == transform.position)
        {
            isIdle = true;
        }
        else
        {
            isIdle = false;
        }
        pastPosition = transform.position;
    }


    //Контроллер на текущий статус
    void StateHandler()
    {
        switch (currentState)
        {
            default:
            case State.Attacking:
                break;
            case State.Chasing:
                GetCurrentState();
                ChasingTarget();
                break;
            case State.Death:
                navMeshAgent.isStopped = true;
                break;

            case State.Roaming:
                GetCurrentState();
                roamingTime -= Time.deltaTime;
                if (roamingTime < 0)
                {
                    Roaming();
                    roamingTime = Random.Range(5f, roamingTimerMax);
                }
                break;
        }
    }



    public void GetCurrentState()
    {
        if (aggressiveStatus && Vector3.Distance(transform.position, PlayerScript.Instance.transform.position) <= chasingDistance)
        {
            currentState = State.Chasing;
        }
        else
        {
            currentState = State.Roaming;
        }
    }

    //Логика преследования
    private void ChasingTarget()
    {
        navMeshAgent.ResetPath();
        navMeshAgent.SetDestination(PlayerScript.Instance.transform.position);
        ChangeFacingDirection(transform.position, PlayerScript.Instance.transform.position);
    }

    //Логика обычного передвижения
    private void Roaming()
    {
        startingPosition = transform.position;
        roamingPosition = GetRoamingPosition();
        ChangeFacingDirection(startingPosition, roamingPosition);
        navMeshAgent.SetDestination(roamingPosition);
    }

    private Vector3 GetRoamingPosition() => startingPosition + Utils.GetRandomDir() * Random.Range(roamingDistanceMin, roamingDistanceMax);
    private void ChangeFacingDirection(Vector3 sourcePosition, Vector3 targetPosition)
    {
        if (sourcePosition.x > targetPosition.x)
        {
            transform.rotation = Quaternion.Euler(0, -180, 0);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}
