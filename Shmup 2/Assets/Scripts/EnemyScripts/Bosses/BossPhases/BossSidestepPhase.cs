using UnityEngine;
using System.Collections;

public class BossSidestepPhase : BossPhase
{
    public BossSidestepPhase(Vector3 startPosition) : base(startPosition) { }

    public override void EnterPhase(FinalBoss boss) {
        ExitPhase(boss);
    }

    protected override IEnumerator ShootPattern(FinalBoss boss) {
        yield return new WaitForEndOfFrame();

    }
}
