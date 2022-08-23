using System;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Stats stats = new Stats();
    public event Action OnDead;
    private void InitStatsEnemy ()
    {
        stats.life = 50;
        stats.damage = 5;
    }

    private void Start ()
    {
        InitStatsEnemy();
    }

    private void Update ()
    {

    }

    private void Dead ()
    {
        OnDead?.Invoke();
    }
}