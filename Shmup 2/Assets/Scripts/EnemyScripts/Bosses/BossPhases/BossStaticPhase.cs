using UnityEngine;
using System.Collections;

public class BossStaticPhase : BossPhase
{
    public BossStaticPhase(Vector3 startPosition) : base(startPosition) { }

    public override void EnterPhase(FinalBoss boss) {
        StartCoroutine(boss, ShootPattern);
    }

    protected override IEnumerator ShootPattern(FinalBoss boss) {
        for(float i = 0; i < boss.StaticPhaseDurationSeconds; i += boss.FireDelaySeconds)
        {
            // wing canons target player
            boss.FireWeapon(boss.LeftWingCanon, Enemy.shootDirection.player);
            boss.FireWeapon(boss.RightWingCanon, Enemy.shootDirection.player);

            // turret shoots straight ahead
            boss.FireWeapon(boss.CockpitTurret, Enemy.shootDirection.straight);
            
            yield return new WaitForSeconds(boss.FireDelaySeconds);
        }

        ExitPhase(boss);
    }
}
