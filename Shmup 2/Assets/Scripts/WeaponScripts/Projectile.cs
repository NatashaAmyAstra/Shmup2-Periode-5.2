using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _borderDestroyMargin = 1;
    [SerializeField] private int _damage = 1;

    private float _screenBorderY;
    private float _screenBorderX;

    private void Awake() {
        // calculate the height of the screen to know when to delete the projectile
        _screenBorderY = Camera.main.orthographicSize;
        _screenBorderX = _screenBorderY * Camera.main.aspect;
    }

    private void Update() {
        // move projectile forward
        transform.position += transform.up * _speed * Time.deltaTime;

        // if the projectile moves off screen, delete it
        if(ProjectileOffScreen() == true)
        {
            Destroy(gameObject);
        }
    }

    private bool ProjectileOffScreen() {
        // since camera is always stationary at the world origin, 
        float absX = Mathf.Abs(transform.position.x);
        float absY = Mathf.Abs(transform.position.y);
        return absX > _screenBorderX + _borderDestroyMargin || absY > _screenBorderY + _borderDestroyMargin;
    }

    private void OnTriggerEnter(Collider other) {
        // if projectile collides with a damagable object, deal damage to it and remove projectile
        if(other.TryGetComponent(out IDamagable damagable))
        {
            damagable.Damage(_damage);
            Destroy(gameObject);
        }
    }
}
