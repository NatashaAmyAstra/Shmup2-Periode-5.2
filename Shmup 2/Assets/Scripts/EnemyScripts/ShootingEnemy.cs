using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingEnemy : Enemy
{
    [Header("Shooting")]
    [SerializeField] private WeaponSO _weapon;
    [SerializeField] private Transform _projectileOrigin;

    [Header("Burst fire")]
    [SerializeField] private bool _burstFire;
    [SerializeField] private float _burstDelay;

    [Header("Projectile logic")]
    [SerializeField] private float _fireDelaySeconds;
    [SerializeField] private int _attackPatternLength;
    private List<shootDirection> _attackPattern = new List<shootDirection>();

    protected override void GenerateRandom() {
        // randomly choose a set number of attack types to create a shooting pattern
        for(int i = 0; i < _attackPatternLength; i++)
        {
            _attackPattern.Add(Random.value < 0.5f ? shootDirection.player : shootDirection.straight);
        }
    }

    protected override void StartSequence() {
        StartCoroutine(Attack());
    }

    private IEnumerator Attack() {
        // enemy will continue shooting until it is destroyed
        while(true)
        {
            for(int i = 0; i < _attackPatternLength; i++)
            {
                // fire the projectile using the EnemyBase shoot method
                Shoot(_attackPattern[i], _weapon, _projectileOrigin);

                yield return new WaitForSeconds(_fireDelaySeconds);
            }

            // if burst fire is enabled, pause shortly between shooting routines
            if(_burstFire == true)
            {
                yield return new WaitForSeconds(_burstDelay);
            }
        }
    }
}
