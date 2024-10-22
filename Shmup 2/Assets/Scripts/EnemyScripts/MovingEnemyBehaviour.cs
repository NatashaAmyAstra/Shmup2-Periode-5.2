using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class MovingEnemyBehaviour : MonoBehaviour
{
    private enum MoveType
    {
        horizontal,
        diagonal
    }

    private MoveType _moveType;

    [SerializeField] private float _entryPosY = 4;
    [SerializeField] private float _speed;
    [SerializeField, Range(1, 10)] private int _verticalStepCount = 1;
    private float _verticalStepDistance;

    private void Start() {
        // choose a random move type
        _moveType = (MoveType)(UnityEngine.Random.value * Enum.GetValues(typeof(MoveType)).Length);

        _verticalStepDistance = (Player.Instance.transform.position.y - _entryPosY) / _verticalStepCount;

        // start enemy behaviour
        StartCoroutine(EnemyMovementBehaviour());
    }

    private IEnumerator EnemyMovementBehaviour() {
        // move down to the given entry height
        Vector3 entryPoint = transform.position;
        entryPoint.y = _entryPosY;
        yield return MoveToPosition(entryPoint);

        while(true)
        {
            Vector3 targetPosition = transform.position;
            switch(_moveType)
            {
                case MoveType.diagonal:
                    // move down
                    targetPosition.y += _verticalStepDistance;
                    
                    // always move horizontally
                    goto case MoveType.horizontal;
                case MoveType.horizontal:
                    // move to the opposite side of the screen
                    targetPosition.x *= -1;
                    break;
            }

            // move to new position
            yield return MoveToPosition(targetPosition);
        }
    }

    private IEnumerator MoveToPosition(Vector3 targetPosition) {
        // set start position
        Vector3 startPosition = transform.position;

        // get distance so GetLerpTime can calculate time
        float distance = Vector3.Distance(startPosition, targetPosition);

        float t = 0;
        do
        {
            // time = distance / velocity
            // 1 / time to translate deltaTime to a 0 - 1 scale on the calculated time
            t += 1 / (distance / _speed) * Time.deltaTime;

            // move towards target position
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);

            yield return new WaitForEndOfFrame();
        } while(t < 1);
    }
}
