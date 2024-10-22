using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Enemy : MonoBehaviour, IDamagable
{
    // strings for movement queues
    public enum moveDirection {
        vertical,
        horizontal
    }

    // strings for shoot sequences
    public enum shootDirection {
        player,
        straight
    }

    // event called on enemy death
    public event EnemyDeathDelegate OnEnemyDeath;
    public delegate void EnemyDeathDelegate(Enemy enemy);

    [Header("Base enemy values")]
    [SerializeField] private int _maxHealth;
    private int _health;

    [SerializeField] private float _entranceTimeSeconds;
    [SerializeField] protected float _restPeriodSeconds;

    private void Awake() {
        // set health and generate enemy path
        _health = _maxHealth;
        Setup();


        // DEBUG
        Activate(new Vector3(0, 4, 0));
    }

    /// <summary>
    /// Requires a vector3 start position to lerp the enemy to it's initial position<br></br>
    /// after reaching its beginning position, will start moving over its generated path
    /// </summary>
    public virtual void Activate(Vector3 startPosition) {
        // enter screen
        StartCoroutine(Enter(startPosition));
    }

    private IEnumerator Enter(Vector3 destination) {
        yield return MoveOverTime(destination, _entranceTimeSeconds);

        // wait a number of seconds before starting the rest of the enemy's behaviour pattern
        yield return new WaitForSeconds(_restPeriodSeconds);

        // initiate sequenced logic
        StartSequence();
    }

    #region DerivedMethods
    public virtual void Damage(int damage) {
        _health -= damage;

        // call enemy death event so waves can track current enemies
        if(_health <= 0)
        {
            OnEnemyDeath?.Invoke(this);
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Generate the enemy's random sequence such as move pattern or firing routine
    /// </summary>
    protected abstract void Setup();

    /// <summary>
    /// Use this method to start the coroutine(s) for the enemy's logic
    /// </summary>
    protected abstract void StartSequence();
    #endregion

    #region Shooting
    /// <summary>
    /// fire projectilePrefab either straight down or towards the player<br></br>
    /// shoots from projectileOrigin
    /// </summary>
    protected void Shoot(shootDirection direction, WeaponSO weapon, Transform projectileOrigin) {
        // set target for projectile either to the player or straight downwards
        Vector3 targetPosition;
        if(direction == shootDirection.player)
            targetPosition = Player.Instance.transform.position;
        else
            targetPosition = projectileOrigin.position + Vector3.down;

        // fire number of projectiles set in weapon
        for(int i = 0; i < weapon.ProjectileCount; i++)
        {
            // calculate angle offset of projectile to account for multiple projectiles
            float projectileAngleOffset = weapon.MultishotAngle * (i - (weapon.ProjectileCount - 1) * 0.5f);

            // calculate angle to shoot projectile in
            Quaternion shootRotation = Quaternion.LookRotation(Vector3.forward, targetPosition - projectileOrigin.position);
            shootRotation.eulerAngles += Vector3.forward * projectileAngleOffset;

            // spawn projectile
            Instantiate(weapon.ProjectilePrefab, projectileOrigin.position, shootRotation);
        }
    }
    #endregion

    #region Moving
    /// <summary>
    /// move to given destination at a set speed
    /// </summary>
    protected IEnumerator MoveAtSpeed(Vector3 destination, float speed) {
        // calculate how much time it will take to travel given distance at given speed
        Vector3 startPosition = transform.position;
        float travelTimeSeconds = Vector3.Distance(startPosition, destination) / speed;

        // pass calculated time on to MoveOverTime
        yield return MoveOverTime(destination, travelTimeSeconds);
    }

    /// <summary>
    /// move to given destination over a given time
    /// </summary>
    protected IEnumerator MoveOverTime(Vector3 destination, float travelTimeSeconds) {
        Vector3 startPosition = transform.position;
        float t = 0;
        do
        {
            t += 1 / travelTimeSeconds * Time.deltaTime;

            // set position
            transform.position = Vector3.Lerp(startPosition, destination, t);

            yield return new WaitForEndOfFrame();
        } while(t <= 1);
    }

    protected IEnumerator PerformMovePattern(Queue<moveDirection> pathingQueue, float speed, float verticalMoveDistance) {
        // move until pathing queue is empty
        while(pathingQueue.Count > 0)
        {
            Vector3 destination = GetDestination(pathingQueue.Dequeue(), verticalMoveDistance);
            yield return MoveAtSpeed(destination, speed);
            yield return new WaitForSeconds(_restPeriodSeconds);
        }

        OnMoveSequenceEnd();
    }

    private Vector3 GetDestination(moveDirection instruction, float verticalMoveDistance) {
        // if we move horizontal, move to the same position on the opposite side of the screen
        if(instruction == moveDirection.horizontal)
        {
            float x = -transform.position.x;
            return new Vector3(x, transform.position.y, transform.position.z);
        }

        // otherwise move downwards
        return transform.position + Vector3.down * verticalMoveDistance;
    }

    protected virtual void OnMoveSequenceEnd() {
        // destroy enemy after reaching destination
        Destroy(gameObject);
    }
    #endregion
}
