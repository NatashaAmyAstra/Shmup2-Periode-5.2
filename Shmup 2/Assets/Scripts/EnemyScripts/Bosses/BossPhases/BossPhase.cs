using System;
using System.Collections;

[Serializable]
public abstract class BossPhase
{
    public abstract void EnterPhase(FinalBoss boss);

    protected abstract IEnumerator ShootPattern(FinalBoss boss);

    protected virtual void ExitPhase(FinalBoss boss) {
        boss.ChangePhase(boss.GetNewPhase(this));
    }
}
