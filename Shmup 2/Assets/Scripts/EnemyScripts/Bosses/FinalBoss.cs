using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FinalBoss : Enemy
{
    [Serializable]
    public class Weapon {
        public Transform ProjectileOrigin;
        public WeaponSO WeaponSO;
    }

    #region MembersAndProperties
    [Header("Weapons")]
    [SerializeField] private Weapon _leftWingCanon;
    [SerializeField] private Weapon _rightWingCanon;
    [SerializeField] private Weapon _cockpitTurret;
    public Weapon LeftWingCanon { get { return _leftWingCanon; } }
    public Weapon RightWingCanon { get { return _rightWingCanon; } }
    public Weapon CockpitTurret { get { return _cockpitTurret; } }

    [Header("Central stats")]
    [SerializeField] private float _fireDelaySeconds;
    [SerializeField] private float _phaseTravelTimeSeconds;
    [SerializeField] private float _speed;
    private bool _isInvulnerable = true; // boss is invulnerable during entry sequence

    public float FireDelaySeconds { get { return _fireDelaySeconds; } }
    public float PhaseTravelTimeSeconds { get { return _phaseTravelTimeSeconds; } }
    public float Speed { get { return _speed; } }


    [Header("State starting positions")]
    [SerializeField] private Vector2 _restPosition;
    [SerializeField] private Vector2 _staticPhaseStartPosition;
    [SerializeField] private Vector2 _spiralPhaseStartPosition;
    [SerializeField] private Vector2 _sidestepPhaseStartPosition;

    // boss phases
    private BossPhase _activePhase;
    private BossPhase[] _phases; // initialized in awake

    [Header("Static phase")]
    [SerializeField] private float _staticPhaseDurationSeconds;
    public float StaticPhaseDurationSeconds { get { return _staticPhaseDurationSeconds; } }

    [Header("Spiral phase")]
    [SerializeField] private float _spiralMaxRadius;
    [SerializeField] private float _numberOfRotations;
    [SerializeField] private float _cycleDurationSeconds;
    public float SpiralMaxRadius { get { return _spiralMaxRadius; } }
    public float NumberOfRotations { get { return _numberOfRotations; } }
    public float CycleDurationSeconds { get { return _cycleDurationSeconds; } }

    //[Header("Sidestep phase")]
    #endregion

    #region DerivedMethods
    protected override void Setup() {
        // initialize boss phases
        _phases = new BossPhase[] {
            new BossStaticPhase(_staticPhaseStartPosition),
            new BossSpiralPhase(_spiralPhaseStartPosition),
            new BossSidestepPhase(_sidestepPhaseStartPosition)
        };

        // choose a random starting phase
        _activePhase = GetNewPhase();
    }

    protected override void StartSequence() {
        // once the boss starts attacking, disable invulnerability and enter the first chosen state
        _isInvulnerable = false;
        StartCoroutine(EnterNewPhase(_activePhase, _phaseTravelTimeSeconds / 2));
    }

    public override void Damage(int damage) {
        // while invulnerable, ignore damage calls
        if(_isInvulnerable)
            return;

        base.Damage(damage);
        // call event for updating boss health bar
    }
    #endregion

    #region StateMethods
    public BossPhase GetNewPhase(BossPhase excludePhase = null) {
        // copy list of phases and remove exlcuded phase
        List<BossPhase> filteredList = _phases.ToList();
        filteredList.Remove(excludePhase);

        // return a random phase from the filtered list
        return filteredList[UnityEngine.Random.Range(0, filteredList.Count)];
    }

    public void ChangePhase(BossPhase newPhase) {
        StartCoroutine(TransitionToPhase(newPhase));
    }

    private IEnumerator TransitionToPhase(BossPhase newPhase) {
        float waitTime = (_phaseTravelTimeSeconds - _restPeriodSeconds) / 2;
        waitTime = Mathf.Clamp(waitTime, 0, _phaseTravelTimeSeconds);

        // move to rest position
        yield return MoveOverTime(_restPosition, waitTime);

        // briefly wait before moving to the new phase's start position
        yield return new WaitForSeconds(_restPeriodSeconds);

        yield return EnterNewPhase(newPhase, waitTime);
    }

    private IEnumerator EnterNewPhase(BossPhase newPhase, float travelDuration) {
        // then move to the start position of the new phase
        yield return MoveOverTime(newPhase.StartPosition, travelDuration);

        // briefly pause before starting new phase
        yield return new WaitForSeconds(_restPeriodSeconds);

        // set a new phase as active and run it's enter method
        _activePhase = newPhase;
        _activePhase.EnterPhase(this);
    }

    public void FireWeapon(Weapon weapon, shootDirection direction) {
        Shoot(direction, weapon.WeaponSO, weapon.ProjectileOrigin);
    }

    public Coroutine StartStateCoroutine(BossPhase.StateCoroutine coroutine) {
        return StartCoroutine(coroutine(this));
    }

    public void EndCoroutine(Coroutine coroutine) {
        StopCoroutine(coroutine);
    }

    public void EndCoroutines(List<Coroutine> coroutines) {
        foreach(Coroutine coroutine in coroutines)
        {
            StopCoroutine(coroutine);
        }
    }
    #endregion
}
