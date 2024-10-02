using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour, IDamagable
{
    public Action OnPlayerDamage;

    [SerializeField] private int _maxHealth = 3;
    public int MaxHealth { get { return _maxHealth; } }

    private int _health;
    public int Health { get { return _health; } }

    private void Awake() {
        _health = _maxHealth;
    }

    public void Damage(int damage = 1) {
        _health -= damage;
        OnPlayerDamage?.Invoke();
    }
}
