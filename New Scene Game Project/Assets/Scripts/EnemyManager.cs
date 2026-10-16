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
    private float centerEdgeDistance;

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
                if (enemies.Count <= 3)
                {
                    enemyIndicatorsActive = true;
                    if (enemies.Count < 3 && i < enemies.Count)
                    {
                        Destroy(enemyIndicators[i]);
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
