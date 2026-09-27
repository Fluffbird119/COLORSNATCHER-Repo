using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public GameObject player;
    public GameObject enemyPrefab;
    public GameObject smallEnemyPrefab;
    public GameObject bigEnemyPrefab;
    public GameObject speedEnemyPrefab;
    public GameObject shooterPrefab;
    public GameObject sniperPrefab;
    public GameObject zombiePrefab;
    public GameObject trooperPrefab;
    public GameObject chunkyPrefab;
    private int[] waveCount = new int[] { 10, 20, 20, 10, 10, 20, 15, 15, 30, 10, 20, 30 };
    public float minSpawnBounds = 40f;
    public float maxSpawnBounds = 60f;
    public float arenaMinBoundsX = -70f;
    public float arenaMaxBoundsX = 70f;
    public float arenaMinBoundsY = -55f;
    public float arenaMaxBoundsY = 35f;
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
        BackgroundManager.Instance.ChangeBackground(0);
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
        while (killsThisWave < waveCount[0])
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

        PlayerPowerUps.Instance.beatenRed = true;
        BackgroundManager.Instance.ChangeBackground(1);

        StartCoroutine(WaveFour());
    }

    public IEnumerator WaveFour()
    {
        waveNumber = 4;
        StartCoroutine(DialogueSystem.instance.StartCutscene(5));
        while (DialogueSystem.instance.isCutscenePlaying)
        {
            yield return null;
        }
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
        StartCoroutine(DialogueSystem.instance.StartCutscene(6));
        while (DialogueSystem.instance.isCutscenePlaying)
        {
            yield return null;
        }
        killsThisWave = 0;
        CreateWave();
        while (killsThisWave < waveCount[4])
        {
            yield return null;
        }
        StartCoroutine(WaveSix());
    }

    public IEnumerator WaveSix()
    {
        waveNumber = 6;
        StartCoroutine(DialogueSystem.instance.StartCutscene(7));
        while (DialogueSystem.instance.isCutscenePlaying)
        {
            yield return null;
        }
        killsThisWave = 0;
        CreateWave();
        while (killsThisWave < waveCount[5])
        {
            yield return null;
        }
        StartCoroutine(DialogueSystem.instance.StartCutscene(8));
        while (DialogueSystem.instance.isCutscenePlaying)
        {
            yield return null;
        }

        PlayerPowerUps.Instance.beatenYellow = true;
        BackgroundManager.Instance.ChangeBackground(2);

        StartCoroutine(WaveSeven());
    }

    public IEnumerator WaveSeven()
    {
        waveNumber = 7;
        StartCoroutine(DialogueSystem.instance.StartCutscene(9));
        while (DialogueSystem.instance.isCutscenePlaying)
        {
            yield return null;
        }
        killsThisWave = 0;
        CreateWave();
        while (killsThisWave < waveCount[6])
        {
            yield return null;
        }
        StartCoroutine(WaveEight());
    }

    public IEnumerator WaveEight()
    {
        waveNumber = 8;
        StartCoroutine(DialogueSystem.instance.StartCutscene(10));
        while (DialogueSystem.instance.isCutscenePlaying)
        {
            yield return null;
        }
        killsThisWave = 0;
        CreateWave();
        while (killsThisWave < waveCount[7])
        {
            yield return null;
        }
        StartCoroutine(WaveNine());
    }
    public IEnumerator WaveNine()
    {
        waveNumber = 9;
        StartCoroutine(DialogueSystem.instance.StartCutscene(11));
        while (DialogueSystem.instance.isCutscenePlaying)
        {
            yield return null;
        }
        killsThisWave = 0;
        CreateWave();
        while (killsThisWave < waveCount[8])
        {
            yield return null;
        }
        StartCoroutine(DialogueSystem.instance.StartCutscene(12));
        while (DialogueSystem.instance.isCutscenePlaying)
        {
            yield return null;
        }
        PlayerPowerUps.Instance.beatenGreen = true;
        BackgroundManager.Instance.ChangeBackground(3);
        WaveTen();
    }

    public IEnumerator WaveTen()
    {
        waveNumber = 10;
        StartCoroutine(DialogueSystem.instance.StartCutscene(13));
        while (DialogueSystem.instance.isCutscenePlaying)
        {
            yield return null;
        }
        
        killsThisWave = 0;
        CreateWave();
        while (killsThisWave < waveCount[9])
        {
            yield return null;
        }
        StartCoroutine(WaveEleven());
    }

    public IEnumerator WaveEleven()
    {
        waveNumber = 11;
        StartCoroutine(DialogueSystem.instance.StartCutscene(14));
        while (DialogueSystem.instance.isCutscenePlaying)
        {
            yield return null;
        }
        killsThisWave = 0;
        CreateWave();
        while (killsThisWave < waveCount[10])
        {
            yield return null;
        }
        StartCoroutine(WaveTwelve());
    }

    public IEnumerator WaveTwelve()
    {
        waveNumber = 12;
        StartCoroutine(DialogueSystem.instance.StartCutscene(15));
        while (DialogueSystem.instance.isCutscenePlaying)
        {
            yield return null;
        }
        killsThisWave = 0;
        CreateWave();
        while (killsThisWave < waveCount[11])
        {
            yield return null;
        }
        StartCoroutine(DialogueSystem.instance.StartCutscene(16));
        while (DialogueSystem.instance.isCutscenePlaying)
        {
            yield return null;
        }
        //send to main menu here
    }

    public void CreateWave()
    {
        if (waveNumber == 1)
        {
            SpawnEnemies(enemyPrefab, 10);
        }
        else if (waveNumber == 2)
        {
            SpawnEnemies(smallEnemyPrefab, 10);
            SpawnEnemies(enemyPrefab, 10);
        }
        else if (waveNumber == 3)
        {
            SpawnEnemies(smallEnemyPrefab, 5);
            SpawnEnemies(enemyPrefab, 10);
            SpawnEnemies(bigEnemyPrefab, 5);
        }
        else if (waveNumber == 4)
        {
            SpawnEnemies(speedEnemyPrefab, 10);
        }
        else if (waveNumber == 5)
        {
            SpawnEnemies(shooterPrefab, 10);
        }
        else if (waveNumber == 6)
        {
            SpawnEnemies(shooterPrefab, 10);
            SpawnEnemies(speedEnemyPrefab, 10);
        }
        else if (waveNumber == 7)
        {
            SpawnEnemies(zombiePrefab, 15);
        }
        else if (waveNumber == 8)
        {
            SpawnEnemies(sniperPrefab, 7);
            SpawnEnemies(zombiePrefab, 8);
        }
        else if (waveNumber == 9)
        {
            SpawnEnemies(zombiePrefab, 20);
            SpawnEnemies(sniperPrefab, 10);
        }
        else if (waveNumber == 10)
        {
            SpawnEnemies(trooperPrefab, 10);
        }
        else if (waveNumber == 11)
        {
            SpawnEnemies(chunkyPrefab, 10);
            SpawnEnemies(trooperPrefab, 10);
        }
        else if (waveNumber == 12)
        {
            SpawnEnemies(chunkyPrefab, 15);
            SpawnEnemies(trooperPrefab, 15);
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
                float randomTheta = Random.Range(0f, 2f * Mathf.PI);
                float randomDistance = Random.Range(minSpawnBounds, maxSpawnBounds);
                spawnPosition = new Vector2(randomDistance * Mathf.Cos(randomTheta) + player.transform.position.x, randomDistance * Mathf.Sin(randomTheta) + player.transform.position.y);
                Collider2D hitCollider = Physics2D.OverlapCircle(spawnPosition, checkRadius);
                if (hitCollider == null && spawnPosition.x > arenaMinBoundsX && spawnPosition.y > arenaMinBoundsY && spawnPosition.x < arenaMaxBoundsX && spawnPosition.y < arenaMaxBoundsY)
                {
                    validPositionFound = true;
                }

                attempts++;
            }
            if (attempts == maxAttempts)
            {
                if (Vector2.Distance(player.transform.position, new Vector2(0, 0)) < minSpawnBounds)
                {
                    Instantiate(prefabToSpawn, new Vector2(30 * (int)Random.Range(-1, 2), 30 * (int)Random.Range(-1, 2)), Quaternion.identity);
                }
                else
                {
                    Instantiate(prefabToSpawn, new Vector2(0f, 0f), Quaternion.identity);
                }
            }
            else
            {
                Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
            }
        }
    }
}