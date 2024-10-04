using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBase : MonoBehaviour, IDamagable
{
    // strings for movement queues
    protected enum moveType {
        down,
        horizontal
    }

    // strings for shoot sequences
    protected enum shootType {
        player,
        straight
    }

    // event called on enemy death
    public event EnemyDeathDelegate OnEnemyDeath;
    public delegate void EnemyDeathDelegate(EnemyBase enemy);

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

    private IEnumerator Enter(Vector3 destination) {
        // move enemy straight down from the top of the screen
        Vector3 startPos = destination;
        startPos.y = transform.position.y;

        float t = 0;
        while(t <= 1)
        {
            t += 1 / _entranceTimeSeconds * Time.deltaTime;

            // move to destination
            transform.position = Vector3.Lerp(startPos, destination, t);
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(_restPeriodSeconds);

        // initiate sequenced logic
        StartSequence();
    }

    /// <summary>
    /// Use this method to start the coroutine(s) for the enemy's logic
    /// </summary>
    protected abstract void StartSequence();
}
