using System.Collections;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _enemyContainer;
    [SerializeField]
    private float _baseSpawnTime = 5.0f;
    [SerializeField]
    private float _spawnTimeModifier = 1.0f;
    private Coroutine _spawnEnemyCoroutine;
    private bool _stopSpawning = false;

    private int _aliveEnemies;
    private int _killedEnemies; //updated on the DestroySelf function in Enemy script
    
    // Start is called before the first frame update
    void Start()
    {
        _spawnEnemyCoroutine = StartCoroutine(SpawnEnemyRoutine(_baseSpawnTime));
    }

    private IEnumerator SpawnEnemyRoutine(float spawnTime)
    {
        while (_stopSpawning == false)
        {
            //Update the spawn timer, it gets faster the more enemies you kill
            _spawnTimeModifier = 1 + _killedEnemies * 0.01f;
            float currentSpawnTime = _baseSpawnTime / _spawnTimeModifier;
            Mathf.Clamp(currentSpawnTime, 1.5f, 6f);

            if(_aliveEnemies < 12)
            {
                SpawnEnemyAtLocation();
            }
            yield return new WaitForSeconds(currentSpawnTime);
        }
    }

    void SpawnEnemyAtLocation()
    {
        float randomX = Random.Range(-9.3f, 9.3f);
        Vector3 spawnLocation = new Vector3(randomX, 9, 0);
        GameObject spawnedEnemy = Instantiate(_enemyPrefab, spawnLocation, Quaternion.identity);
        spawnedEnemy.transform.parent = _enemyContainer.transform;
        _aliveEnemies++;
        print("Enemies alive: " + _aliveEnemies);
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
