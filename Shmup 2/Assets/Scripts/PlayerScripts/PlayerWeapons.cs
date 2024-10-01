using UnityEngine;

public class PlayerWeapons : MonoBehaviour
{
    [SerializeField] private float _shootDelaySeconds = 0.5f;
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Transform _projectileOrigin;
    private float _timeSinceLastShot = 0;

    private void Awake() {
        // subscribe to input event
        InputManager.OnFireSpecial += FireSpecial;
    }

    private void Update() {
        // count every frame how long it has been since the last shot was fired
        _timeSinceLastShot += Time.deltaTime;

        // if the player is holding the shoot input, fire as soon as the delay is over
        if(InputManager.IsFiring && _timeSinceLastShot >= _shootDelaySeconds)
        {
            FirePrimary();
            _timeSinceLastShot = 0;
        }
    }

    private void FirePrimary() {
        // spawn a bullet at projectile origin
        Instantiate(_projectilePrefab, _projectileOrigin.position, Quaternion.identity);
    }

    private void FireSpecial() {
    }
}
