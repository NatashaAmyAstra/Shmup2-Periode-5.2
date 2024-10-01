using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private float _borderDestroyMargin = 1;
    [SerializeField] private int _damage = 1;

    private float _screenBorder;

    private void Awake() {
        // calculate the height of the screen to know when to delete the projectile
        _screenBorder = Camera.main.ViewportToWorldPoint(Vector3.up).y;
        Debug.Log(_screenBorder);
    }

    private void Update() {
        // move projectile forward each frame, delete it if it passes the screen border
        transform.position += transform.up * _speed * Time.deltaTime;

        if(Mathf.Abs(transform.position.y) > _screenBorder + _borderDestroyMargin)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision) {
        // if projectile collides with a damagable object, deal damage to it and remove projectile
        if(collision.gameObject.TryGetComponent(out IDamagable damagable))
        {
            damagable.Damage(_damage);
            Destroy(gameObject);
        }
    }
}
