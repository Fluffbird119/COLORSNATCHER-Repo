using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }
    private List<GameObject> enemies = new List<GameObject>();

    private bool enemyIndicatorsActive = false;

    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject enemyIndicatorPrefab;
    [SerializeField] private GameObject pickupPrefab;
    private List<GameObject> enemyIndicators = new List<GameObject>();

    private Vector3 playerColliderOffset = new Vector3(0.06916094f, -1.707474f, 0);
    private float centerEdgeDistance;
    private float timeSinceKill = 0f;
    private int enemiesRapidKilled = 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        EnemyScript.EnemyDeath += RemoveEnemy;
        centerEdgeDistance = CameraSmoothFollowPlayer.centerEdgeDistance;
        CameraSmoothFollowPlayer.ScreenSizeChange += UpdateCamSize;
    }

    void Update()
    {
        timeSinceKill += Time.deltaTime;
        if (enemyIndicatorsActive)
        {
            for (int i = 0; i < enemies.Count; ++i)
            {
                if (enemyIndicators.Count == i)
                {
                    enemyIndicators.Add(Instantiate(enemyIndicatorPrefab, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0)));
                }

                Vector3 direction = enemies[i].transform.position - playerTransform.position;
                float angleDegrees = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                if (direction.magnitude < (centerEdgeDistance / 2f))
                {
                    enemyIndicators[i].GetComponent<SpriteRenderer>().enabled = false;
                }
                else
                {
                    enemyIndicators[i].GetComponent<SpriteRenderer>().enabled = true;
                }

                direction = direction.normalized;

                enemyIndicators[i].transform.rotation = Quaternion.Euler(0, 0, angleDegrees - 90f);
                enemyIndicators[i].transform.position = (direction * 8) + playerTransform.position + playerColliderOffset;
            }
        }
    }

    public void AddEnemy(GameObject enemy)
    {
        enemies.Add(enemy);
        if (enemies.Count >= 4)
        {
            enemyIndicatorsActive = false;
            for (int i = 0; i < enemyIndicators.Count; i++)
            {
                enemyIndicators[i].GetComponent<SpriteRenderer>().enabled = false;
            }
        }
    }

    public void DeleteEnemies()
    {
        for (int i = enemies.Count - 1; i >= 0; --i)
        {
            Destroy(enemies[i]);
            enemies.RemoveAt(i);
        }

        for (int i = enemyIndicators.Count - 1; i >= 0; --i)
        {
            Destroy (enemyIndicators[i]);
            enemyIndicators.RemoveAt(i);
        }
    }

    public int NumEnemies()
    {
        return enemies.Count;
    }

    public void RemoveEnemy(GameObject enemy)
    {
        for (int i = enemies.Count - 1; i >= 0; --i)
        {
            if (enemies[i] == enemy)
            {
                enemies.RemoveAt(i);
                if (timeSinceKill <= 1f)
                {
                    ++enemiesRapidKilled;
                }
                else
                {
                    enemiesRapidKilled = 1;
                }
                timeSinceKill = 0f;
                TrySpawnPickups(enemy.transform.position);
                if (enemies.Count <= 3)
                {
                    enemyIndicatorsActive = true;
                    if (enemies.Count < 3 && i < enemyIndicators.Count)
                    {
                        Destroy(enemyIndicators[i]);
                        enemyIndicators.RemoveAt(i);
                    }
                }
                return;
            }
        }
    }

    private void TrySpawnPickups(Vector3 position)
    {
        if (Random.Range(-5f, enemiesRapidKilled) > 0)
        {
            GameObject pickup = Instantiate(pickupPrefab, position, Quaternion.identity);
            ItemPickup pickupScript = pickup.GetComponent<ItemPickup>();
            pickupScript.CreateRagePickup(Mathf.Floor(Random.Range(1f, 1f + enemiesRapidKilled / 10f)));
        }
        if (Random.Range(-8f, enemiesRapidKilled) > 0)
        {
            GameObject pickup = Instantiate(pickupPrefab, position, Quaternion.identity);
            ItemPickup pickupScript = pickup.GetComponent<ItemPickup>();
            pickupScript.CreateHealthPickup(Mathf.Floor(Random.Range(1f, 1f + enemiesRapidKilled / 10f)));
        }
    }

    public void RemoveNulls()
    {
        enemies.RemoveAll(enemy => !enemy);
    }

    private void UpdateCamSize(float newSize)
    {
        centerEdgeDistance = newSize;
    }

    void OnDestroy()
    {
        EnemyScript.EnemyDeath -= RemoveEnemy;
        CameraSmoothFollowPlayer.ScreenSizeChange -= UpdateCamSize;
    }
}
