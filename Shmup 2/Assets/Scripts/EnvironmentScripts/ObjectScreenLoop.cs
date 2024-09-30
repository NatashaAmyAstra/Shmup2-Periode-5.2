using UnityEngine;

public class ObjectScreenLoop : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    private float _loopWidth;

    private void Awake() {
        // object must loop once it's off screen by at least its width
        float worldScreenWidth = Camera.main.ViewportToWorldPoint(Vector3.right).x * 2;
        float spriteWidth = _spriteRenderer.bounds.size.x;
        _loopWidth = worldScreenWidth + spriteWidth;
    }

    private void Update() {
        // calculate new position
        Vector3 newPos = transform.position;
        newPos.x = LoopedPos(newPos.x);
        transform.position = newPos;
    }

    private float LoopedPos(float x) {
        // loop position using modulo
        x += _loopWidth / 2;
        x = Mod(x, _loopWidth);
        x -= _loopWidth / 2;
        return x;
    }

    private float Mod(float x, float m) {
        // custom modulo because C# % operator is remainder, not proper modulo
        return (x%m + m)%m;
    }
}
