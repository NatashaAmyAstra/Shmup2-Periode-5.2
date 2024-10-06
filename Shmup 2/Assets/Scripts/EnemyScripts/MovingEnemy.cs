using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingEnemy : Enemy
{
    [Header("Movement")]
    [SerializeField] private float _speed;
    [SerializeField] private float _verticalMoveDistance;
    private Queue<moveDirection> _pathingQueue = new Queue<moveDirection>();

    protected override void GenerateRandom() {
        // create a queue for enemy's travel
        float verticalDistanceTravelled = 0;

        // track total distance traveled vertically. Path must end below screen
        while(verticalDistanceTravelled < Camera.main.orthographicSize * 2)
        {
            if(UnityEngine.Random.value < 0.5f)
            {
                _pathingQueue.Enqueue(moveDirection.vertical);
                verticalDistanceTravelled += _verticalMoveDistance;
            }
            else
            {
                _pathingQueue.Enqueue(moveDirection.horizontal);
            }
        }
    }

    protected override void StartSequence() {
        StartCoroutine(PerformMovePattern(_pathingQueue, _speed, _verticalMoveDistance));
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.TryGetComponent(out IDamagable damagable))
        {
            damagable.Damage();
        }
    }
}
