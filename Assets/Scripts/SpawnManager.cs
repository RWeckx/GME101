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
    [SerializeField]
    private float _spawnTimeModifier = 1.0f;
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
    private float _timeToNextWave = 3.0f;


    public void StartSpawning()
    {
        _spawnEnemyCoroutine = StartCoroutine(SpawnEnemyRoutine(_baseSpawnTime, _amountEnemiesToSpawn));
        _spawnPowerupCoroutine = StartCoroutine(SpawnPowerupRoutine());
        StartCoroutine(WaveCountdown(_timeToNextWave));
    }

    private IEnumerator SpawnEnemyRoutine(float spawnTime, int amountToSpawn)
    {
        while (_stopSpawning == false)
        {
            //Update the spawn timer, it gets faster the more enemies you kill
            _spawnTimeModifier = 1 + _killedEnemies * 0.07f;
            float currentSpawnTime = _baseSpawnTime / _spawnTimeModifier;
            currentSpawnTime = Mathf.Clamp(currentSpawnTime, 1.5f, 6f);

            if(_enemiesSpawnedThisWave < amountToSpawn && _readyForWave == true)
            {
                SpawnEnemyAtLocation();
                _enemiesSpawnedThisWave++;
            }
            else if (_enemiesSpawnedThisWave == amountToSpawn)
            {
                _readyForWave = false;
                _timeToNextWave += 3.0f;
                _amountEnemiesToSpawn += 3;
                _enemiesSpawnedThisWave = 0;
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

    private IEnumerator WaveCountdown(float timeToNextWave)
    {
        while (_stopSpawning == false)
        {
            yield return new WaitForSeconds(timeToNextWave);
            _readyForWave = true;
        }
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
