using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class AttackZoneScript : MonoBehaviour
{
    public BoxCollider2D attackZone;
    public static AttackZoneScript Instance { get; private set; }

    private void Awake()
    {
        attackZone = GetComponent<BoxCollider2D>();
        Instance = this;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<EnemyAI>(out var enemy))
        {
            enemy.TakeDamage(50);
        }
    }
}