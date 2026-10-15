using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance { get; private set; }
    private List<GameObject> enemies = new List<GameObject>();

    private bool enemyIndicatorsActive = false;

    [SerializeField] private Transform playerTransform;
    [SerializeField] private GameObject enemyIndicatorPrefab;
    private List<GameObject> enemyIndicators = new List<GameObject>();

    private Vector3 playerColliderOffset = new Vector3(0.06916094f, -1.707474f, 0);

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        EnemyScript.EnemyDeath += RemoveEnemy;
    }

    void Update()
    {
        if (enemyIndicatorsActive)
        {
            for (int i = 0; i < enemies.Count; i++)
            {
                if (enemyIndicators.Count == i)
                {
                    enemyIndicators.Add(Instantiate(enemyIndicatorPrefab, new Vector3(0, 0, 0), Quaternion.Euler(0, 0, 0)));
                }

                Vector3 direction = (enemies[i].transform.position - playerTransform.position).normalized;
                float angleDegrees = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;

                enemyIndicators[i].transform.rotation = Quaternion.Euler(0, 0, angleDegrees);
                enemyIndicators[i].transform.position = (direction * 5) + playerTransform.position + playerColliderOffset;
            }
        }
    }

    public void AddEnemy(GameObject enemy)
    {
        enemies.Add(enemy);
        if (enemies.Count >= 4)
        {
            enemyIndicatorsActive = false;
        }
    }

    public void DeleteEnemies()
    {
        for (int i = enemies.Count - 1; i >= 0; --i)
        {
            Destroy(enemies[i]);
            enemies.RemoveAt(i);
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
                if (enemies.Count <= 3)
                {
                    enemyIndicatorsActive = true;
                    if (enemies.Count < 3)
                    {
                        enemyIndicators.RemoveAt(i);
                    }
                }
                return;
            }
        }
    }

    public void RemoveNulls()
    {
        enemies.RemoveAll(enemy => !enemy);
    }
}
