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
        GenerateRandom();


        // DEBUG
        Activate(new Vector3(-2, 4, 0));
    }

    /// <summary>
    /// Requires a vector3 start position to lerp the enemy to it's initial position<br></br>
    /// after reaching its beginning position, will start moving over its generated path
    /// </summary>
    public void Activate(Vector3 startPosition) {
        // enter screen
        StartCoroutine(Enter(startPosition));
    }

    private IEnumerator Enter(Vector3 destination) {
        // enemy moves from its spawn position to the destination
        Vector3 startPos = transform.position;

        float t = 0;
        do
        {
            t += 1 / _entranceTimeSeconds * Time.deltaTime;

            // move towards destination
            transform.position = Vector3.Lerp(startPos, destination, t);
            yield return new WaitForEndOfFrame();
        } while(t <= 1);

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
    protected abstract void GenerateRandom();

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
    public void Shoot(shootDirection direction, WeaponSO weapon, Transform projectileOrigin) {
        // set target for projectile either to the player or straight downwards
        Vector3 targetPosition;
        if(direction == shootDirection.player)
            targetPosition = Player.Instance.transform.position;
        else
            targetPosition = transform.position + Vector3.down;

        // fire number of projectiles set in weapon
        for(int i = 0; i < weapon.ProjectileCount; i++)
        {
            // calculate angle offset of projectile to account for multiple projectiles
            float projectileAngleOffset = weapon.MultishotAngle * (i - (weapon.ProjectileCount - 1) * 0.5f);

            // calculate angle to shoot projectile in
            Quaternion shootRotation = Quaternion.LookRotation(Vector3.forward, targetPosition - transform.position);
            shootRotation.eulerAngles += Vector3.forward * projectileAngleOffset;

            // spawn projectile
            Instantiate(weapon.ProjectilePrefab, projectileOrigin.position, shootRotation);
        }
    }
    #endregion

    #region Moving
    protected IEnumerator PerformMovePattern(Queue<moveDirection> pathingQueue, float speed, float verticalMoveDistance) {
        // move until pathing queue is empty
        while(pathingQueue.Count > 0)
        {
            // determine next step
            Vector3 startPosition = transform.position;
            Vector3 destination = GetDestination(pathingQueue.Dequeue(), verticalMoveDistance);

            // calculate how long it will take to move X distance
            float travelTimeSeconds = Vector3.Distance(startPosition, destination) / speed;

            // lerp from start position to destination
            float t = 0;
            while(t <= 1)
            {
                t += 1 / travelTimeSeconds * Time.deltaTime;

                // set position
                transform.position = Vector3.Lerp(startPosition, destination, t);

                yield return new WaitForEndOfFrame();
            }
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
