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
            Shoot(shootDirection.player, _weapon, _projectileOrigin);

            // wait for fire delay
            yield return new WaitForSeconds(_fireDelaySeconds);
        }
    }
}
