using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyBehaviourBase : MonoBehaviour, IDamagable
{
    private const string _moveVertical = "vertical";
    private const string _moveHorizontal = "horizontal";

    // [SerializeField] private PowerUp _droppedPowerup
    [SerializeField] private int _maxHealth;
    private int _health;

    [Header("Movement")]
    [SerializeField] private float _speed;
    [SerializeField] private float _entranceTimeSeconds;
    [SerializeField] private float _verticalMoveDistance;
    [SerializeField] private float _restPeriodSeconds;
    private Queue<string> _pathingQueue = new Queue<string>();

    private void Awake() {
        // set health and generate enemy path
        _health = _maxHealth;
        GeneratePath();

        Activate(new Vector3(2, 4, 0));
    }

    /// <summary>
    /// Requires a vector3 start position to lerp the enemy to it's initial position<br></br>
    /// after reaching its beginning position, will start moving over its generated path
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

    private void GeneratePath() {
        // create a queue for enemy's travel
        float verticalDistanceTravelled = 0;

        // track total distance traveled vertically. Path must end below screen
        while(verticalDistanceTravelled < Camera.main.orthographicSize * 2)
        {
            if(UnityEngine.Random.value < 0.5f)
            {
                _pathingQueue.Enqueue(_moveVertical);
                verticalDistanceTravelled += _verticalMoveDistance;
            }
            else
            {
                _pathingQueue.Enqueue(_moveHorizontal);
            }
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
        yield return new WaitForSeconds(_restPeriodSeconds);

        // start movement
        StartCoroutine(MoveEnemy());
    }

    private IEnumerator MoveEnemy() {
        // move until pathing queue is empty
        while(_pathingQueue.Count > 0)
        {
            // determine path
            Vector3 startPosition = transform.position;
            Vector3 destination = GetDestination(_pathingQueue.Dequeue());
            float travelTimeSeconds = Vector3.Distance(startPosition, destination) / _speed;

            float t = 0;
            while(t <= 1)
            {
                t += 1 / travelTimeSeconds * Time.deltaTime;

                transform.position = Vector3.Lerp(startPosition, destination, t);

                yield return new WaitForEndOfFrame();
            }
            yield return new WaitForSeconds(_restPeriodSeconds);
        }

        // destroy enemy if it has not reached the 
        Destroy(gameObject);
    }

    private Vector3 GetDestination(string instruction) {
        // if we move horizontal, move to the same position on the opposite side of the screen
        if(instruction == _moveHorizontal)
        {
            float x = -transform.position.x;
            return new Vector3(x, transform.position.y, transform.position.z);
        }

        // otherwise move downwards
        return transform.position + Vector3.down * _verticalMoveDistance;
    }
}
