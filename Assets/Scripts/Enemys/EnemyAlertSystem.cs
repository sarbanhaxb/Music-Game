using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAlertSystem : MonoBehaviour
{
    public static EnemyAlertSystem Instance { get; private set; }

    public event Action OnPlayerAttacked;

    private void Awake()
    {
        if (Instance == null) Instance = this;
    }

    public void TriggerAggression()
    {
        OnPlayerAttacked?.Invoke();
    }
}
