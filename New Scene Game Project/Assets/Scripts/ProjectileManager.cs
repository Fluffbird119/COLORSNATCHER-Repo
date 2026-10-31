using UnityEngine;
using UnityEngine.Pool;

public class ProjectileManager : MonoBehaviour
{
    [SerializeField] private Camera mainCamera;
    public ObjectPool<GameObject> ProjectilePool;

    [SerializeField] private GameObject bulletPrefab;

    public static ProjectileManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // setting up

        ProjectilePool = new ObjectPool<GameObject>(
            createFunc: MakeProjectile,
            actionOnRelease: OnRelease,
            actionOnGet: OnGet,
            actionOnDestroy: OnDestroyItem,
            defaultCapacity: 20, // allocates memory for the first 20 objects
            collectionCheck: true
        );
    }

    private GameObject MakeProjectile()
    {
        GameObject newBullet = Instantiate(bulletPrefab, Vector3.zero, Quaternion.identity);
        newBullet.SetActive(false);
        return newBullet;
    }
    
    private void OnRelease(GameObject pooledObject)
    {
        pooledObject.SetActive(false);

        Rigidbody2D bulletRigidbody2D = pooledObject.GetComponent<Rigidbody2D>();
        bulletRigidbody2D.linearVelocity = Vector3.zero;
        bulletRigidbody2D.angularVelocity = 0f;
    }

    private void OnGet(GameObject pooledObject)
    {
        pooledObject.SetActive(false);
    }

    private void OnDestroyItem(GameObject pooledObject)
    {
        Destroy(pooledObject);
    }

    public void ShootProjectileTowardsMouse(ProjectileScriptableObject projectileScriptableObject, Transform firePoint)
    {
        Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, Mathf.Abs(Input.mousePosition.z)));

        Vector2 shootDirection = (mouseWorldPos - firePoint.position).normalized;

        ShootProjectile(projectileScriptableObject, firePoint, shootDirection);
    }

    public void ShootProjectileTowardsPoint(ProjectileScriptableObject projectileScriptableObject, Transform firePoint, Transform targetPoint)
    {
        Vector2 shootDirection = (targetPoint.position - firePoint.position).normalized;

        ShootProjectile(projectileScriptableObject, firePoint, shootDirection);
    }

    private void ShootProjectile(ProjectileScriptableObject projectileScriptableObject, Transform firePoint, Vector2 direction)
    {
        GameObject bullet = ProjectilePool.Get();
        Bullet bulletScript = bullet.GetComponent<Bullet>();
        bulletScript.PopulateBulletValues(projectileScriptableObject, firePoint, direction.normalized);
    }
}
