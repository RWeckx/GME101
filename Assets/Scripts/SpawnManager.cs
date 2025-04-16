using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject[] _powerups;
    [SerializeField]
    private int _amountEnemiesToSpawn = 7;
    [SerializeField]
    private float _baseSpawnTime = 4.0f;
    [Tooltip("X is min power up spawn time; Y is max spawn time")]
    [SerializeField]
    private Vector2 _minMaxPowerUpSpawnTime = new Vector2(6.0f, 10.0f);
    private float _powerUpSpawnTime = 5.0f;
    private Coroutine _spawnEnemyCoroutine;
    private Coroutine _spawnPowerupCoroutine;
    private bool _stopSpawning = false;

    private int _aliveEnemies;
    private int _killedEnemies; //updated on the DestroySelf function in Enemy script
    private bool _readyForWave;
    [SerializeField]
    private int _enemiesSpawnedThisWave;
    private float _timeToNextWave = 5.0f;


    public void StartSpawning()
    {
        _spawnEnemyCoroutine = StartCoroutine(SpawnEnemyRoutine(_amountEnemiesToSpawn));
        _spawnPowerupCoroutine = StartCoroutine(SpawnPowerupRoutine());
        StartCoroutine(WaveCountdownRoutine(_timeToNextWave));
    }

    private IEnumerator SpawnEnemyRoutine(int amountToSpawn)
    {
        while (_stopSpawning == false)
        {
            float currentSpawnTime = _baseSpawnTime;
            currentSpawnTime = Mathf.Clamp(currentSpawnTime, 1.5f, 6f);

            if(_enemiesSpawnedThisWave < amountToSpawn && _readyForWave == true)
            {
                SpawnEnemyAtLocation();
                _enemiesSpawnedThisWave++;
            }
            else if (_enemiesSpawnedThisWave == amountToSpawn)
            {
                _readyForWave = false;
                _amountEnemiesToSpawn += 3;
                _baseSpawnTime -= 0.5f;
                _baseSpawnTime = Mathf.Clamp(_baseSpawnTime, 1.5f, 6.0f);
                _enemiesSpawnedThisWave = 0;
                StartCoroutine(WaveCountdownRoutine(_timeToNextWave));
            }
                yield return new WaitForSeconds(currentSpawnTime);
        }
    }

    private IEnumerator SpawnPowerupRoutine()
    {
        while (_stopSpawning == false)
        {
            _powerUpSpawnTime = Random.Range(_minMaxPowerUpSpawnTime.x, _minMaxPowerUpSpawnTime.y);
            yield return new WaitForSeconds(_powerUpSpawnTime);
            float randomX = Random.Range(-9.3f, 9.3f);
            Vector3 spawnLocation = new Vector3(randomX, 9, 0);
            int randomPowerUp = Random.Range(0, _powerups.Length);
            Instantiate(_powerups[randomPowerUp], spawnLocation, Quaternion.identity);
        }
    }

    //Once the WaveCountdown is over, more enemies can be spawned. This is to give a tiny breather inbetween waves.
    private IEnumerator WaveCountdownRoutine(float timeToNextWave)
    {
            yield return new WaitForSeconds(timeToNextWave);
            _readyForWave = true;
    }

    void SpawnEnemyAtLocation()
    {
        float randomX = Random.Range(-9.3f, 9.3f);
        Vector3 spawnLocation = new Vector3(randomX, 9, 0);
        GameObject spawnedEnemy = Instantiate(_enemyPrefab, spawnLocation, Quaternion.identity);
        spawnedEnemy.transform.parent = _enemyContainer.transform;
        _aliveEnemies++;
    }

    public void OnPlayerDeath()
    {
        _stopSpawning = true;
    }

    public void OnEnemyDeath()
    {
        _aliveEnemies--;
        _killedEnemies++;
    }
}
