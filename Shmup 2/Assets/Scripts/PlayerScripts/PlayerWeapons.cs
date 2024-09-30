using UnityEngine;

public class PlayerWeapons : MonoBehaviour
{
    [SerializeField] private float _shootDelaySeconds = 0.5f;
    private float _timeSinceLastShot = 0;

    private void Awake() {
        // subscribe to input event
        InputManager.OnFireSpecial += FireSpecial;
    }

    private void Update() {
        _timeSinceLastShot += Time.deltaTime;
        if(InputManager.IsFiring && _timeSinceLastShot >= _shootDelaySeconds)
        {
            FirePrimary();
        }
    }

    private void FirePrimary() {
    }

    private void FireSpecial() {
    }
}
