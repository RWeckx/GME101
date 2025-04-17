using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyFighterPrefab;
    [SerializeField]
    private GameObject _enemyBomberPrefab;
    [SerializeField]
    private GameObject _enemyKamikazePrefab;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private GameObject[] _powerups;
    [SerializeField]
    private int _amountEnemiesToSpawn = 5;
    [Tooltip("A value between 0.0 and 1.0. It represents a percentage.")]
    [SerializeField]
    private float _ratioEnemyFightersToSpawn = 1.0f;
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
    private int _enemiesSpawnedThisWave;
    private float _timeToNextWave = 3.0f;
    [SerializeField]
    private int _currentWave;


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
                _ratioEnemyFightersToSpawn -= 0.1f;
                Mathf.Clamp(_ratioEnemyFightersToSpawn, 0.6f, 1.0f);
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

            int powerUpToSpawn;
            int rollDice = Random.Range(0, 101);

            // spawn TripleShot if roll between 0 & 15 (inclusive)
            if (rollDice >= 0 && rollDice <= 15)
                powerUpToSpawn = 0;

            // spawn Speed if roll between 15 & 25 (inclusive)
            else if (rollDice > 15 && rollDice <= 25)
                powerUpToSpawn = 1;

            // spawn Shield if roll between 25 & 40 (inclusive)
            else if (rollDice > 25 && rollDice <= 40)
                powerUpToSpawn = 2;

            // spawn Ammo if roll between 40 & 70 (inclusive)
            else if (rollDice > 40 && rollDice <= 70)
                powerUpToSpawn = 3;

            // spawn Health if roll between 70 & 80 (inclusive)
            else if (rollDice > 70 && rollDice <= 80)
                powerUpToSpawn = 4;

            // spawn Bomb if roll between 80 & 90 (inclusive)
            else if (rollDice > 80 && rollDice <= 90)
                powerUpToSpawn = 5;

            // spawn Slow Debuff if roll 91 or higher
            else
                powerUpToSpawn = 6;

                Instantiate(_powerups[powerUpToSpawn], spawnLocation, Quaternion.identity);
        }
    }

    //Once the WaveCountdown is over, more enemies can be spawned. This is to give a tiny breather inbetween waves.
    private IEnumerator WaveCountdownRoutine(float timeToNextWave)
    {
        yield return new WaitForSeconds(timeToNextWave);
        _readyForWave = true;
        _currentWave++;
    }

    void SpawnEnemyAtLocation()
    {
        float randomX = Random.Range(-9.3f, 9.3f);
        Vector3 spawnLocation = new Vector3(randomX, 9, 0);
        GameObject spawnedEnemy;

        // roll to determine if we should spawn a fighter or bomber
        float enemyToSpawn = Random.Range(0.0f, 1.0f);
        float spawnWithShield = Random.Range(0, 6); 

        // spawn a fighter if the value is smaller or equal to the fighter spawn ratio
        if (enemyToSpawn <= _ratioEnemyFightersToSpawn) 
        {
            spawnedEnemy = Instantiate(_enemyFighterPrefab, spawnLocation, Quaternion.identity);
            spawnedEnemy.transform.parent = _enemyContainer.transform;
            _aliveEnemies++;
        }
        else // roll to spawn a bomber or a kamikaze
        {
            int rollToSpawn = Random.Range(0, 2);
            if (rollToSpawn == 0)
            {
                spawnedEnemy = Instantiate(_enemyBomberPrefab, spawnLocation, Quaternion.identity);
                spawnedEnemy.transform.parent = _enemyContainer.transform;
                _aliveEnemies++;
            }
            else
            {
                spawnedEnemy = Instantiate(_enemyKamikazePrefab, spawnLocation, Quaternion.identity);
                spawnedEnemy.transform.parent = _enemyContainer.transform;
                _aliveEnemies++;
            }

        }

        // 20% chance to spawn an enemy with shield if the shield roll was 0 and if the current wave is 3 or higher (so we don't spawn in the first 2 waves to keep early game beginner-friendly)
        if(spawnWithShield == 0 && _currentWave > 2)
        {
            spawnedEnemy.GetComponent<Enemy>().SetShieldActiveOnEnemy();
        }
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
