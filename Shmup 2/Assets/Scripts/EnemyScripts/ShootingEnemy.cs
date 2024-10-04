using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingEnemy : EnemyBase
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
    private List<shootType> _attackPattern = new List<shootType>();

    protected override void GenerateRandom() {
        // randomly choose a set number of attack types to create a shooting pattern
        for(int i = 0; i < _attackPatternLength; i++)
        {
            _attackPattern.Add(Random.value < 0.5f ? shootType.player : shootType.straight);
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
                Shoot(_attackPattern[i]);

                yield return new WaitForSeconds(_fireDelaySeconds);
            }

            // if burst fire is enabled, pause shortly between shooting routines
            if(_burstFire == true)
            {
                yield return new WaitForSeconds(_burstDelay);
            }
        }
    }

    /// <summary>
    /// fire projectilePrefab either straight down or towards the player<br></br>
    /// shoots from projectileOrigin
    /// </summary>
    private void Shoot(shootType target) {
        // set target for projectile either to the player or straight downwards
        Vector3 targetPosition;
        if(target == shootType.player)
            targetPosition = Player.Instance.transform.position;
        else
            targetPosition = transform.position + Vector3.down;
        
        // fire number of projectiles set in weapon
        for(int i = 0; i < _weapon.ProjectileCount; i++)
        {
            // calculate angle offset of projectile to account for multiple projectiles
            float projectileAngleOffset = _weapon.MultishotAngle * (i - (_weapon.ProjectileCount - 1) * 0.5f);
            
            // calculate angle to shoot projectile in
            Quaternion shootRotation = Quaternion.LookRotation(Vector3.forward, targetPosition - transform.position);
            shootRotation.eulerAngles += Vector3.forward * projectileAngleOffset;

            // spawn projectile
            Instantiate(_weapon.ProjectilePrefab, _projectileOrigin.position, shootRotation);
        }
    }
}
