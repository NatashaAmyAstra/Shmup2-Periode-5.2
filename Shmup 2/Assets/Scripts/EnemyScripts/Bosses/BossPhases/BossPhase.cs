using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public abstract class BossPhase
{
    // constructor
    public BossPhase(Vector3 startPosition) {
        StartPosition = startPosition;
    }
    
    public Vector3 StartPosition;

    public delegate IEnumerator StateCoroutine(FinalBoss boss);

    public abstract void EnterPhase(FinalBoss boss);

    protected abstract IEnumerator ShootPattern(FinalBoss boss);


    protected List<Coroutine> _activeCoroutines = new List<Coroutine>();

    protected void StartCoroutine(FinalBoss boss, StateCoroutine coroutine) {
        _activeCoroutines.Add(boss.StartStateCoroutine(coroutine));
    }

    protected virtual void ExitPhase(FinalBoss boss) {
        boss.EndCoroutines(_activeCoroutines);
        boss.ChangePhase(boss.GetNewPhase(this));
    }
}
