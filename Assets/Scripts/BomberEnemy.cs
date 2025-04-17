using System.Collections;
using UnityEngine;

public class BomberEnemy : Enemy
{   
    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.FindObjectOfType<SpawnManager>();
        _player = GameObject.FindObjectOfType<Player>();
        _animator = GetComponent<Animator>();
        if (_spawnManager == null)
            Debug.Log("No reference to Spawn Manager");
        if (_player == null)
            Debug.Log("No reference to Player");
        if (_animator == null)
            Debug.Log("No reference to Animator");
        _pointsToGive = Random.Range(15, 25);
        _movementState = 0;
        _movementCoroutine = ChangeMovementDirectionRoutine();

        StartCoroutine(_movementCoroutine);
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
        if (Time.time > _canFire && transform.position.y < 6.0f && _isDead == false)
            FireLaser();
    }

    protected override void FireLaser()
    {
        _fireRateInSec = Random.Range(_minMaxFireRate.x, _minMaxFireRate.y);
        _canFire = Time.time + _fireRateInSec;

        GameObject instantiatedBomb = Instantiate(_projectilePrefab, transform.position + _laserOffset, Quaternion.identity);
        instantiatedBomb.GetComponent<Bomb>().SetAsEnemyLaser();
    }
    
    protected override IEnumerator ChangeMovementDirectionRoutine()
    {
        while (_isDead == false)
        {
                _movementState = 0;
                yield return new WaitForSeconds(2.0f);
                _movementState = 1;
                yield return new WaitForSeconds(2.5f);
                _movementState = 2;
                yield return new WaitForSeconds(2.5f);
        }
    }
}
