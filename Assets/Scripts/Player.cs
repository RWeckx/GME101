using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private float _baseMoveSpeed = 3.0f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private GameObject _tripleShotPrefab;
    [SerializeField]
    private float _baseFireRate = 0.5f;
    [SerializeField]
    private int _lives = 3;
    [SerializeField]
    private float _TripleShotPowerDownTime = 2.0f;
    private float _canFire = 0f;
    private SpawnManager _spawnManager;
    private bool _isTripleShotActive = false;
    
    
    
    private float _moveSpeedModifier = 1.0f;
    private float _fireRateModifier = 1.0f;


    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
        if(Input.GetKey(KeyCode.Space) && Time.time > _canFire)
        {
            FireLaser();
        }
    }

    void CalculateMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontalInput, verticalInput, 0);

        // Calculate the movement speed of the player by multiplying the base movespeed with the movespeed modifier
        float moveSpeed = _baseMoveSpeed * _moveSpeedModifier;

        //Variables to define the playable space. The player cannot move outside of these bounds
        float upperBounds = 0;
        float lowerBounds = -3.9f;
        float rightBounds = 11.5f;
        float leftBounds = -11.5f;

        transform.Translate(direction * moveSpeed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, lowerBounds,upperBounds), transform.position.z);

        // If the player tries to move to either side of the play space, they wrap around to the other side. Meaning if they go all the way left, they end up showing on the right side of the space and vice versa.
        if (transform.position.x >= rightBounds)
        {
            transform.position = new Vector3(leftBounds, transform.position.y, transform.position.z);
        }
        else if (transform.position.x <= leftBounds)
        {
            transform.position = new Vector3(rightBounds, transform.position.y, transform.position.z);
        }
    }

    void FireLaser()
    {
        float _fireRate = _baseFireRate * _fireRateModifier;
        _canFire = Time.time + _fireRate;
        if (_isTripleShotActive)
        {
            Instantiate(_tripleShotPrefab, transform.position, Quaternion.identity);
        }

        else
            Instantiate(_laserPrefab, transform.position + new Vector3(0, 1, 0), Quaternion.identity);
    }

    public void TakeDamage()
    {
        _lives--;
        if (_lives <= 0)
        {
            _spawnManager.OnPlayerDeath();
            Destroy(this.gameObject);
        }
    }

    public void SetTripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    private IEnumerator TripleShotPowerDownRoutine()
    {
        while (_isTripleShotActive == true)
        {
            yield return new WaitForSeconds(_TripleShotPowerDownTime);
            _isTripleShotActive = false;
        }
    }
}
