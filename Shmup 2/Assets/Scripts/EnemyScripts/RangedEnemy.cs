using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangedEnemy : MonoBehaviour, IDamagable
{
    private const string _shootStraight = "straight";
    private const string _shootPlayer = "player";

    // [SerializeField] private PowerUp _droppedPowerup
    [SerializeField] private int _maxHealth;
    private int _health;

    [SerializeField] private float _entranceTimeSeconds;

    [Header("Shooting")]
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Transform _projectileOrigin;

    [SerializeField] private bool _burstFire;
    [SerializeField] private float _burstDelay;
    [SerializeField] private float _fireDelaySeconds;
    [SerializeField] private int _attackPatternLength;
    private List<string> _attackPattern = new List<string>();

    private void Awake() {
        // set health and generate enemy path
        _health = _maxHealth;
        GenerateAttackPattern();

        Activate(new Vector3(2, 4, 0));
    }

    /// <summary>
    /// Requires a vector3 start position to lerp the enemy to it's initial position<br></br>
    /// after reaching its beginning position, will start attacking
    /// </summary>
    public void Activate(Vector3 startPosition) {
        // enter screen
        StartCoroutine(EnterScreen(startPosition));
    }

    public void Damage(int damage) {
        _health -= damage;
        if(_health <= 0)
        {
            Destroy(gameObject);
        }
    }

    private void GenerateAttackPattern() {
        for(int i = 0; i < _attackPatternLength; i++)
        {
            _attackPattern.Add(Random.value < 0.5f ? _shootPlayer : _shootStraight);
        }
    }
    
    private IEnumerator EnterScreen(Vector3 destination) {
        Vector3 startPos = transform.position;
        float t = 0;
        while(t <= 1)
        {
            t += 1 / _entranceTimeSeconds * Time.deltaTime;
            transform.position = Vector3.Lerp(startPos, destination, t);
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(_fireDelaySeconds);

        // start movement
        StartCoroutine(Attack());
    }

    private IEnumerator Attack() {
        // keep shooting untill dead
        while(true)
        {
            for(int i = 0; i < _attackPatternLength; i++){
                if(_attackPattern[i] == _shootPlayer)
                {
                    Shoot(Player.Instance.transform.position);
                }
                else
                {
                    Shoot(transform.position + Vector3.down);
                }

                yield return new WaitForSeconds(_fireDelaySeconds);
            }
            if(_burstFire == true)
            {
                yield return new WaitForSeconds(_burstDelay);
            }
        }
    }

    private void Shoot(Vector3 shootPosition) {
        Quaternion shootRotation = Quaternion.LookRotation(Vector3.forward, shootPosition - transform.position);
        Instantiate(_projectilePrefab, _projectileOrigin.position, shootRotation);
    }
}
