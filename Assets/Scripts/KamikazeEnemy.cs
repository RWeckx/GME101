using System.Collections;
using UnityEngine;

public class KamikazeEnemy : Enemy
{
    [SerializeField]
    private float _speedBoostWhenInRange = 1.3f;
    [SerializeField]
    private float _distanceToRam;
    [SerializeField]
    private float _rotationOffset = -90f;

    private bool _inRangeOfPlayer = false;

    float r;

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
        
        // if in range of player, we set a bool to allow kamikaze to ram the player. If the player is position higher on Y, then the kamikaze stops following
        if (CheckDistanceToPlayer() <= _distanceToRam && transform.position.y > _player.transform.position.y)
        {
            _inRangeOfPlayer = true;
        }
        else
        {
            _inRangeOfPlayer = false;
        }
    }


    // Kamikaze only moves along Y as a counterplay to its aggressive nature (otherwise it might be too hard for players not to get hit)
    protected override void CalculateMovement()
    {
        if (_inRangeOfPlayer == true && _isDead == false)
        {   
            transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, _moveSpeed * _speedBoostWhenInRange * Time.deltaTime);
            FaceTowardsPlayer();
        }
        else
        {
            transform.Translate(Vector3.down * _moveSpeed * Time.deltaTime);
            transform.rotation = Quaternion.Euler(0f, 0f, 0f); // reset rotation to default;
        }
            
        if (transform.position.y <= _lowerBounds)
        {
            float randomX = Random.Range(_leftBounds, _rightBounds);
            transform.position = new Vector3(randomX, _upperBounds, 0);
            _movementState = 0;
            if (_isDead)
                Destroy(this.gameObject);
        }
    }

    float CheckDistanceToPlayer()
    {
        if (_player != null)
        {
            float distance = Vector3.Distance(_player.transform.position, transform.position);
            return distance;
        }
        else return (_distanceToRam + 5.0f);  // return a value outside the ram distance
    }

    private void FaceTowardsPlayer()
    {
        Vector3 diff = _player.transform.position - transform.position;
        diff.Normalize();
        float rot_z = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rot_z - _rotationOffset);
    }

}
