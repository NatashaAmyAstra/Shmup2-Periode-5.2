using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private int _damage = 1;

    private void Update() {
        transform.position += transform.up * _speed * Time.deltaTime;
    }

    private void OnCollisionEnter(Collision collision) {
        if(collision.gameObject.TryGetComponent(out IDamagable damagable))
        {
            damagable.Damage(_damage);
        }
    }
}
