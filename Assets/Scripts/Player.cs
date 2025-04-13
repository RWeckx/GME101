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
    private GameObject _shieldVisuals;
    [SerializeField]
    private float _baseFireRate = 0.5f;
    [SerializeField]
    private int _lives = 3;
    [SerializeField]
    private float _tripleShotPowerDownTime = 2.0f;
    [SerializeField]
    private float _moveSpeedPowerDownTime = 2.0f;
    [Tooltip("A float representing a percentage. Eg.: 0.2 means 20%")]
    [SerializeField]
    private float _moveSpeedBoostMultiplier = 0.3f;
    [SerializeField]
    private float _shieldPowerDownTime = 20.0f;

    private float _canFire = 0f;
    private SpawnManager _spawnManager;
    private bool _isTripleShotActive = false;
    private bool _isSpeedBoostActive = false;
    private bool _isShieldActive = false;

    private float _moveSpeed;
    private float _moveSpeedMultiplier = 0.0f; // A float representing a percentage. Eg.: 0.2 means 20%
    private float _fireRateMultiplier = 0.0f; // A float representing a percentage. Eg.: 0.2 means 20%

    private UIManager _uiManager;
    private int _score;

    private GameManager _gameManager;

    //Variables to define the playable space. The player cannot move outside of these bounds
    private float _upperBounds = 0;
    private float _lowerBounds = -3.9f;
    private float _rightBounds = 11.5f;
    private float _leftBounds = -11.5f;


    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        _shieldVisuals.SetActive(false);
        _uiManager = GameObject.FindObjectOfType<Canvas>().GetComponent<UIManager>();
        _gameManager = GameObject.FindObjectOfType<GameManager>();

        if (_spawnManager == null)
            Debug.Log("The Spawn Manager is null");
        if (_uiManager == null)
            Debug.Log("The UI Manager is null");
        if (_gameManager == null)
            Debug.Log("The Game Manager is null");
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
        _moveSpeed = _baseMoveSpeed * (1 + _moveSpeedMultiplier);

        transform.Translate(direction * _moveSpeed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, Mathf.Clamp(transform.position.y, _lowerBounds,_upperBounds), transform.position.z);

        // If the player tries to move to either side of the play space, they wrap around to the other side. Meaning if they go all the way left, they end up showing on the right side of the space and vice versa.
        if (transform.position.x >= _rightBounds)
        {
            transform.position = new Vector3(_leftBounds, transform.position.y, transform.position.z);
        }
        else if (transform.position.x <= _leftBounds)
        {
            transform.position = new Vector3(_rightBounds, transform.position.y, transform.position.z);
        }
    }

    void FireLaser()
    {
        float _fireRate = _baseFireRate * (1 + _fireRateMultiplier);
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
        if (_isShieldActive) // if the shield is active, don't do anything in this method except setting shield back to false
        {
            _isShieldActive = false;
            _shieldVisuals.SetActive(false);
            return;
        }

        _lives--;
        _uiManager.UpdateLivesDisplay(_lives);

        if (_lives <= 0)
        {
            HandlePlayerDeath();
        }
    }

    public void SetTripleShotActive()
    {
        _isTripleShotActive = true;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    public void SetSpeedBoostActive()
    {
        _isSpeedBoostActive = true;
        _moveSpeedMultiplier += _moveSpeedBoostMultiplier;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    public void SetShieldActive()
    {
        _isShieldActive = true;
        _shieldVisuals.SetActive(true);
        StartCoroutine(ShieldPowerDownRoutine());
    }

    public void AddScore(int amount)
    {
        _score += amount;
        _uiManager.UpdateScoreDisplay(_score);
    }

    public void HandlePlayerDeath()
    {
        _spawnManager.OnPlayerDeath();
        _uiManager.ShowGameOverText();
        _gameManager.GameOver();
        Destroy(this.gameObject);
    }

    private IEnumerator TripleShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(_tripleShotPowerDownTime);
        _isTripleShotActive = false;
    }
    
    private IEnumerator SpeedBoostPowerDownRoutine()
    {
        yield return new WaitForSeconds(_moveSpeedPowerDownTime);
        _moveSpeedMultiplier -= _moveSpeedBoostMultiplier;
        _isSpeedBoostActive = false;
    }

    private IEnumerator ShieldPowerDownRoutine()
    {
        yield return new WaitForSeconds(_shieldPowerDownTime);
        _isShieldActive = false;
        _shieldVisuals.SetActive(false);
    }
}
