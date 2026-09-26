using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject enemyPrefab;
    public GameObject smallEnemyPrefab;
    public GameObject bigEnemyPrefab;
    public GameObject speedEnemyPrefab;
    public GameObject shooterPrefab;
    public int[] waveCount = new int[] { 10, 20, 20, 10, 10 };
    public float minBounds = -100f;
    public float maxBounds = 100f;
    public int killsThisWave = 0;
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
        while(killsThisWave < waveCount[0])
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
        while (killsThisWave < waveCount[1])
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
        while (killsThisWave < waveCount[2])
        {
            yield return null;
        }
        StartCoroutine(DialogueSystem.instance.StartCutscene(4));
        while (DialogueSystem.instance.isCutscenePlaying)
        {
            yield return null;
        }
        StartCoroutine(WaveFour());
    }

    public IEnumerator WaveFour()
    {
        waveNumber = 4;
        /*StartCoroutine(DialogueSystem.instance.StartCutscene(5));
        while (DialogueSystem.instance.isCutscenePlaying)
        {
            yield return null;
        }*/
        killsThisWave = 0;
        CreateWave();
        while (killsThisWave < waveCount[3])
        {
            yield return null;
        }
        StartCoroutine(WaveFive());
    }

    public IEnumerator WaveFive()
    {
        waveNumber = 5;
        /*StartCoroutine(DialogueSystem.instance.StartCutscene(5));
        while (DialogueSystem.instance.isCutscenePlaying)
        {
            yield return null;
        }*/
        killsThisWave = 0;
        CreateWave();
        while (killsThisWave < waveCount[4])
        {
            yield return null;
        }
    }

    public void CreateWave()
    {
        if (waveNumber == 1)
        {
            SpawnEnemies(enemyPrefab, waveCount[0]);
        }
        else if (waveNumber == 2)
        {
            SpawnEnemies(smallEnemyPrefab, waveCount[1] / 2);
            SpawnEnemies(enemyPrefab, waveCount[1] / 2);
        }
        else if (waveNumber == 3)
        {
            SpawnEnemies(smallEnemyPrefab, waveCount[2] / 4);
            SpawnEnemies(enemyPrefab, waveCount[2] / 2);
            SpawnEnemies(bigEnemyPrefab, waveCount[2] / 4);
        }
        else if (waveNumber == 4)
        {
            SpawnEnemies(speedEnemyPrefab, waveCount[3]);
        }
        else if (waveNumber == 5)
        {
            SpawnEnemies(shooterPrefab, waveCount[4]);
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