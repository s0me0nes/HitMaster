using UnityEngine;

public class Bullet : MonoBehaviour
{ 
    public Vector3 Direction { get; set; }
    public float Speed { get; set; } = 1;

    private Rigidbody _rigidbody;

    private void Awake() => TryGetComponent(out _rigidbody);

    private void OnEnable()
    {
        _rigidbody.velocity = Vector3.zero;
        Invoke(nameof(Disable), 5);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!other.transform.TryGetComponent(out Enemy enemy))
            return;
        
        enemy.Destroy(3);
        gameObject.SetActive(false);
    }

    private void Update()
    {
        transform.position += Direction * Time.deltaTime * Speed;
    }

    private void Disable()
    {
        gameObject.SetActive(false);
    }
}