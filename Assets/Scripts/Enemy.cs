using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("References")]
    public Rigidbody[] rigidbodies;
    
    private Animator _animator;
    private CapsuleCollider _collider;

    private void Awake()
    {
        TryGetComponent(out _animator);
        TryGetComponent(out _collider);
    }

    public void Destroy(float time)
    {
        StartCoroutine(WaitAndDestroy(time));
    }

    private IEnumerator WaitAndDestroy(float time)
    {
        yield return new WaitForSeconds(0.2f);
        
        _animator.enabled = false;
        _collider.enabled = false;
        
        for (int i = 0; i < rigidbodies.Length; i++)
            rigidbodies[i].isKinematic = false;

        Destroy(gameObject, time);
        Destroy(this);
    }
}