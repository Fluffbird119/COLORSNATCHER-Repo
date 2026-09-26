using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject smallEnemyPrefab;
    public GameObject bigEnemyPrefab;
    public int waveOneCount = 10;
    public float minBounds = -100f;
    public float maxBounds = 100f;
    public int killsThisWave = 0;
    public int waveTwoCount = 20;
    public int waveThreeCount = 20;
    public int waveNumber = 0;

    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        StartCoroutine(WaveOne());
    }

    public IEnumerator WaveOne()
    {
        waveNumber = 1;
        StartCoroutine(DialogueSystem.instance.StartCutscene(1));
        while (DialogueSystem.instance.isCutscenePlaying)
        {
            yield return null;
        }
        killsThisWave = 0;
        CreateWave();
        while(killsThisWave < waveOneCount)
        {
            yield return null;
        }
        StartCoroutine(WaveTwo());
    }
    public IEnumerator WaveTwo()
    {
        waveNumber = 2;
        StartCoroutine(DialogueSystem.instance.StartCutscene(2));
        while (DialogueSystem.instance.isCutscenePlaying)
        {
            yield return null;
        }
        killsThisWave = 0;
        CreateWave();
        while (killsThisWave < waveTwoCount)
        {
            yield return null;
        }
        StartCoroutine(WaveThree());
    }

    public IEnumerator WaveThree()
    {
        waveNumber = 3;
        StartCoroutine(DialogueSystem.instance.StartCutscene(3));
        while (DialogueSystem.instance.isCutscenePlaying)
        {
            yield return null;
        }
        killsThisWave = 0;
        CreateWave();
        while (killsThisWave < waveThreeCount)
        {
            yield return null;
        }
        StartCoroutine(DialogueSystem.instance.StartCutscene(4));
    }

    public void CreateWave()
    {
        if (waveNumber == 1)
        {
            SpawnEnemies(enemyPrefab, waveOneCount);
        }
        else if (waveNumber == 2)
        {
            SpawnEnemies(smallEnemyPrefab, waveTwoCount / 2);
            SpawnEnemies(enemyPrefab, waveTwoCount / 2);
        }
        else if (waveNumber == 3)
        {
            SpawnEnemies(smallEnemyPrefab, waveTwoCount / 4);
            SpawnEnemies(enemyPrefab, waveTwoCount / 2);
            SpawnEnemies(bigEnemyPrefab, waveTwoCount / 4);
        }
    }

    private void SpawnEnemies(GameObject prefabToSpawn, int count)
    {
        float checkRadius = 1.5f;
        int maxAttempts = 30;
        for (int i = 0; i < count; i++)
        {
            Vector2 spawnPosition = Vector2.zero;
            bool validPositionFound = false;
            int attempts = 0;
            while (!validPositionFound && attempts < maxAttempts)
            {
                float randomX = Random.Range(minBounds, maxBounds);
                float randomY = Random.Range(minBounds, maxBounds);
                spawnPosition = new Vector2(randomX, randomY);
                Collider2D hitCollider = Physics2D.OverlapCircle(spawnPosition, checkRadius);
                if (hitCollider == null)
                {
                    validPositionFound = true;
                }

                attempts++;
            }
            Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        }
    }
}