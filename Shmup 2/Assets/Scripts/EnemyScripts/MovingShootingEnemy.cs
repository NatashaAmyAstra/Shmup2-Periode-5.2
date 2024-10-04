using System.Collections;
using UnityEngine;

public class MovingShootingEnemy : MovingEnemy
{
    [Header("Shooting")]
    [SerializeField] private WeaponSO _weapon;
    [SerializeField] private Transform _projectileOrigin;
    [SerializeField] private float _fireDelaySeconds;
    [SerializeField] private float _targetHeightOffset;

    protected override void StartSequence() {
        base.StartSequence();
        StartCoroutine(Attack());
    }

    private IEnumerator Attack() {
        // enemy will continue shooting until it is destroyed
        while(transform.position.y > Player.Instance.transform.position.y + _targetHeightOffset)
        {
            // fire projectile at the player
            Shoot();

            yield return new WaitForSeconds(_fireDelaySeconds);
        }
    }

    /// <summary>
    /// fire projectilePrefab either straight down or towards the player<br></br>
    /// shoots from projectileOrigin
    /// </summary>
    private void Shoot() {
        // set target for projectile either to the player or straight downwards
        Vector3 targetPosition = Player.Instance.transform.position;
        
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
