using UnityEngine;
using System.Collections;

public class BossSidestepPhase : BossPhase
{
    public override void EnterPhase(FinalBoss boss) {

    }

    protected override IEnumerator ShootPattern(FinalBoss boss) {
        yield return new WaitForEndOfFrame();

    }
}
