using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _speed;

    private void Update() {
        MovePlayer();
    }

    private void MovePlayer() {
        // move in given direction at given speed
        transform.position += InputManager.InputDirection * _speed * Time.deltaTime;
    }
}
