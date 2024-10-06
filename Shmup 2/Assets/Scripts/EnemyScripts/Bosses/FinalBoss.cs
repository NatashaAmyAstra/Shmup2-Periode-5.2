using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FinalBoss : Enemy, IBoss
{
    [Serializable]
    public class Weapon {
        public Transform ProjectileOrigin;
        public WeaponSO WeaponSO;
    }

    [Header("Weapons")]
    [SerializeField] private Weapon _leftWingCanon;
    [SerializeField] private Weapon _rightWingCanon;
    [SerializeField] private Weapon _cockpitTurret;

    public Weapon LeftWingCanon { get { return _leftWingCanon; } }
    public Weapon RightWingCanon { get { return _rightWingCanon; } }
    public Weapon CockpitTurret { get { return _cockpitTurret; } }

    [Header("Attack Stats")]
    [SerializeField] private float _fireDelaySeconds;

    [Header("Movement stats")]
    [SerializeField] private float _speed;

    // boss is invulnerable during entry sequence
    private bool _isInvulnerable = true;

    // boss phases
    private BossPhase _activePhase;
    private BossPhase[] _phases = new BossPhase[] {
        new BossSpiralPhase(),
        new BossStaticPhase(),
        new BossSidestepPhase()
    };

    protected override void GenerateRandom() {
        // choose a random starting phase
        _activePhase = GetNewPhase(null);
    }

    protected override void StartSequence() {
        // once the boss starts attacking, disable invulnerability and enter the first chosen state
        _isInvulnerable = false;
        _activePhase.EnterPhase(this);
    }

    public override void Damage(int damage) {
        // while invulnerable, ignore damage calls
        if(_isInvulnerable)
            return;

        base.Damage(damage);
        // call event for updating boss health bar
    }

    public BossPhase GetNewPhase(BossPhase excludePhase) {
        // copy list of phases and remove exlcuded phase
        List<BossPhase> filteredList = _phases.ToList();
        filteredList.Remove(excludePhase);

        // return a random phase from the filtered list
        return filteredList[UnityEngine.Random.Range(0, filteredList.Count)];
    }

    public void ChangePhase(BossPhase newPhase) {
        // set a new phase as active and run it's enter method
        _activePhase = newPhase;
        _activePhase.EnterPhase(this);
    }

    public void FireWeapon(shootDirection direction, Weapon weapon) {
        Shoot(direction, weapon.WeaponSO, weapon.ProjectileOrigin);
    }
}
