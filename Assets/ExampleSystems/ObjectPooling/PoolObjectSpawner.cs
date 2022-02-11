using UnityEngine;
using UnityEngine.Pool;

namespace TemplateProject
{
    public class PoolObjectSpawner : MonoBehaviour
    {
        [SerializeField]
        private PoolObject _pfPoolObject;
        [SerializeField]
        private int _spawnAmount = 5;
        [SerializeField]
        private float repeatRate = 1f;

        private ObjectPool<PoolObject> _poolObjectPool;

        public bool _usePool;
        void Start()
        {
            _poolObjectPool = new ObjectPool<PoolObject>(() =>
            {
                return Instantiate(_pfPoolObject, transform);
            }, poolObject =>
            {
                poolObject.gameObject.SetActive(true); // on pool.Get()
        }, poolObject =>
        {
            poolObject.gameObject.SetActive(false); // on pool.Get()
        }, poolObject =>
        {
            Destroy(poolObject.gameObject); // if no.objects exceeds default capacity
        }, false, // collection check
               100,    // default capacity(allocates enough memory for this amount, similar to array declaration)
               250);   // max capacity of objects (limits the previous value)

            InvokeRepeating(nameof(Spawn), 0.2f, repeatRate);
        }
        private void Spawn()
        {
            for (int i = 0; i < _spawnAmount; i++)
            {
                var poolObject = _usePool ? _poolObjectPool.Get() : Instantiate(_pfPoolObject);
                Vector2 randomVector2 = Random.insideUnitCircle;
                poolObject.transform.position = transform.position + new Vector3(randomVector2.x, randomVector2.y) * 2;
                poolObject.Init(KillPoolObject);
            }
        }
        private void KillPoolObject(PoolObject poolObject)
        {

            if (_usePool)
            {
                _poolObjectPool.Release(poolObject);
            }
            else
            {
                Destroy(poolObject.gameObject);
            }
        }
    }
}
