using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingEnemy : EnemyBase
{
    [Header("Movement")]
    [SerializeField] private float _speed;
    [SerializeField] private float _verticalMoveDistance;
    private Queue<moveType> _pathingQueue = new Queue<moveType>();

    protected override void GenerateRandom() {
        // create a queue for enemy's travel
        float verticalDistanceTravelled = 0;

        // track total distance traveled vertically. Path must end below screen
        while(verticalDistanceTravelled < Camera.main.orthographicSize * 2)
        {
            if(UnityEngine.Random.value < 0.5f)
            {
                _pathingQueue.Enqueue(moveType.down);
                verticalDistanceTravelled += _verticalMoveDistance;
            }
            else
            {
                _pathingQueue.Enqueue(moveType.horizontal);
            }
        }
    }

    protected override void StartSequence() {
        StartCoroutine(MoveEnemyIndefinite(_pathingQueue, _speed, _verticalMoveDistance));
    }

    protected IEnumerator MoveEnemyIndefinite(Queue<moveType> pathingQueue, float speed, float verticalMoveDistance) {
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

        // destroy enemy once it has reached its destination
        Destroy(gameObject);
    }

    private Vector3 GetDestination(moveType instruction, float verticalMoveDistance) {
        // if we move horizontal, move to the same position on the opposite side of the screen
        if(instruction == moveType.horizontal)
        {
            float x = -transform.position.x;
            return new Vector3(x, transform.position.y, transform.position.z);
        }

        // otherwise move downwards
        return transform.position + Vector3.down * verticalMoveDistance;
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.TryGetComponent(out IDamagable damagable))
        {
            damagable.Damage();
        }
    }
}
