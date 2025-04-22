using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    [SerializeField]
    private GameObject _laserCannon1;
    [SerializeField]
    private GameObject _laserCannon2;
    [SerializeField]
    private GameObject _laserCannon3;
    [SerializeField]
    private GameObject _laserCannon4;
    [SerializeField]
    private GameObject _laserCannon5;
    [SerializeField]
    private GameObject[] _laserCannons;

    private bool _moveToCenter;
    private bool _canStartMoveRoutine;
    private bool _canStartFiring;
    private bool _startFiringBarrage;
    private IEnumerator _fireBarrageRoutine;

    
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
        _movementCoroutine = MovementCoroutine();
        _fireBarrageRoutine = FireBarrageRoutine();
        _rightBounds = 5.44f;
        _leftBounds = -5.44f;
    }

    // Update is called once per frame
    void Update()
    {
        // Move down to center and start moving if we reach that point
        if (transform.position.y > 2.6f)
        {
            _movementState = 0;
        }
        else if (transform.position.y <= 2.6f && _canStartMoveRoutine == false)
        {
            _movementState = 3;
            _canStartFiring = true;
            _canStartMoveRoutine = true;
            StartCoroutine(_movementCoroutine);
            StartCoroutine(_fireBarrageRoutine);
        }

        CalculateMovement();

        if (Time.time > _canFire && transform.position.y < 6.0f && _canStartFiring == true && _isDead == false)
        {
            if (_startFiringBarrage == false)
                FireLaser();
            else
                FireBarrage();
        }
    }

    protected override void CalculateMovement()
    {
        switch (_movementState)
        {
            case 0:
                transform.Translate(Vector3.down * _moveSpeed * Time.deltaTime);
                break;
            case 1:
                transform.Translate(Vector3.right * _moveSpeed * Time.deltaTime);
                break;
            case 2:
                transform.Translate(Vector3.left * _moveSpeed * Time.deltaTime);
                break;
            case 3:
                break;
        }

        // Stop moving if you hit the right or left bounds
        if (transform.position.x >= _rightBounds || transform.position.x <= _leftBounds)
        {
            _movementState = 3;
        }

        
        // Move to center from either left or right side of the screen
        if (transform.position.x != 0 && _moveToCenter == true)
        {
            _movementState = 3;
            transform.position = Vector3.MoveTowards(transform.position, new Vector3(0, transform.position.y, transform.position.z), _moveSpeed * Time.deltaTime);
        }

    }

    protected override void FireLaser()
    {
        _fireRateInSec = Random.Range(_minMaxFireRate.x, _minMaxFireRate.y);
        _canFire = Time.time + _fireRateInSec;
        GameObject instantiatedLaser;
        Vector3 spawnPosLaser;
        int spawnPosIndex = Random.Range(0, 5);

        switch (spawnPosIndex)
        {
            case 0:
                spawnPosLaser = _laserCannon1.transform.position;
                break;
            case 1:
                spawnPosLaser = _laserCannon2.transform.position;
                break;
            case 2:
                spawnPosLaser = _laserCannon3.transform.position;
                break;
            case 3:
                spawnPosLaser = _laserCannon4.transform.position;
                break;
            case 4:
                spawnPosLaser = _laserCannon5.transform.position;
                break;
            default:
                spawnPosLaser = _laserCannon1.transform.position;
                break;
        }

        instantiatedLaser = Instantiate(_projectilePrefab, spawnPosLaser, Quaternion.identity);
        Laser[] lasers = instantiatedLaser.GetComponentsInChildren<Laser>();

        for (int i = 0; i < lasers.Length; i++)
        {
            lasers[i].SetAsEnemyLaser(-1.0f);
        }
    }

    private void FireBarrage()
    {
        _fireRateInSec = Random.Range(_minMaxFireRate.x, _minMaxFireRate.y);
        _canFire = Time.time + _fireRateInSec;
        GameObject instantiatedLaser;

        foreach (GameObject cannon in _laserCannons)
        {
            instantiatedLaser = Instantiate(_projectilePrefab, cannon.transform.position, Quaternion.identity);
            Laser[] lasers = instantiatedLaser.GetComponentsInChildren<Laser>();

            for (int i = 0; i < lasers.Length; i++)
            {
                lasers[i].SetAsEnemyLaser(-1.0f);
            }
        }
    }
    
    private IEnumerator MovementCoroutine()
    {
        while(_isDead == false)
        {
            yield return new WaitForSeconds(8.0f);
            _moveToCenter = false;
            _movementState = 1; // Move right
            yield return new WaitForSeconds(8.0f);
            _movementState = 2; // Move left
            yield return new WaitForSeconds(10.0f);
            _moveToCenter = true;
            _movementState = 1;
        }
    }

    private IEnumerator FireBarrageRoutine()
    {
        yield return new WaitForSeconds(16f);
        while (_isDead == false)
        {
            _startFiringBarrage = false;
            float waitForSeconds = Random.Range(10f, 20f);
            yield return new WaitForSeconds(waitForSeconds);
            _startFiringBarrage = true;
            yield return new WaitForSeconds(3.0f);
        }

    }
}
