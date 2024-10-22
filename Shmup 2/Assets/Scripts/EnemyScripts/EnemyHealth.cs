using System;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class EnemyHealth : MonoBehaviour, IDamagable
{
    public Action<EnemyHealth> OnEnemyDeath;

    [SerializeField] private int _maxHealth = 3;
    public int MaxHealth { get { return _maxHealth; } }

    private int _health;
    public int Health { get { return _health; } }

    private void Awake() {
        _health = _maxHealth;
    }

    public void Damage(int damage = 1) {
        _health -= damage;

        if(_health <= 0)
        {
            EnemyDeath();
        }
    }

    private void EnemyDeath() {
        OnEnemyDeath?.Invoke(this);
        Destroy(gameObject);
    }
}
