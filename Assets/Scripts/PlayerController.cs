using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    public Transform points;
    [Header("Pool settings")]
    public Bullet bulletPrefab;
    public bool autoExpand;
    public int count = 10;
    public float speed = 10;
    [Header("Raycast Settings")]
    public float maxLength = 10;
    public LayerMask enemyLayer;

    private NavMeshAgent _agent;
    private Animator _animator;

    private Pool<Bullet> _pool;

    private Transform[] _points;
    
    private int _currentIndex;
    private int _nextIndex;
    
    private bool _moved;
    private bool _waitForShoot;
    
    private static readonly int Run = Animator.StringToHash("Run");

    private void Awake()
    {
        TryGetComponent(out _agent);
        TryGetComponent(out _animator);
        
        _points = new Transform[points.childCount];
        for (int i = 0; i < _points.Length; i++)
            _points[i] = points.GetChild(i);

        _pool = new Pool<Bullet>(bulletPrefab, transform, count) { AutoExpand = autoExpand };
    }

    private void Update()
    {
        _waitForShoot = _points[_currentIndex].childCount > 0;
        
        bool end = _points[_currentIndex].GetSiblingIndex() >= points.childCount - 1
                   && _points[_currentIndex].childCount == 0;
            
        if (end)
        {
            int id = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(id);
        }
        
        if (_waitForShoot)
        {
            if (!Input.GetMouseButtonDown(0))
                return;
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, out RaycastHit hit, maxLength, enemyLayer))
                return;

            if (!_pool.HasFreeElement(out Bullet b))
                return;
            
            Vector3 pos = transform.position + Vector3.up * 1.5f;
            b.transform.position = pos;
            b.Direction = (hit.point - pos).normalized;
            b.Speed = speed;
            
            return;
        }
        
        if (_moved)
            return;
        
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(nameof(Move));
        }
    }

    private IEnumerator Move()
    {
        _moved = true;
        
        GetCurrentIndex();
        
        while (_moved)
        {
            Vector3 pos = _points[_nextIndex].position;
            
            _agent.SetDestination(pos);

            float distance = (pos - transform.position).sqrMagnitude;
            
            _animator.SetBool(Run, true);

            if (distance < 0.2f * 0.2f)
            {
                _moved = false;
                _animator.SetBool(Run, false);
                GetCurrentIndex();
            }

            yield return null;
        }
    }

    private void GetCurrentIndex()
    {
        float minDistance = Mathf.Infinity;
        int index = 0;
        
        for (int i = 0; i < _points.Length; i++)
        {
            float distance = (_points[i].position - transform.position).sqrMagnitude;
            if (distance >= minDistance)
                continue;

            minDistance = distance;
            index = i;
        }

        _currentIndex = index;
        _nextIndex = Mathf.Clamp(index + 1, 0, _points.Length - 1);
    }
}