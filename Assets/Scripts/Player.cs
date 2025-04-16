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
    private GameObject _bombPrefab;
    [SerializeField]
    private GameObject _shieldVisuals;
    [SerializeField]
    private GameObject[] _damagedEngineVisuals;
    [SerializeField]
    private GameObject _laserShotSFX;
    [SerializeField]
    private AudioClip _explosionAudioClip;
    [SerializeField]
    private AudioClip _thrusterAudioClip;
    [SerializeField]
    private float _baseFireRate = 0.5f;
    [SerializeField]
    private int _lives = 3;
    [SerializeField]
    private int _maxLives = 3;
    [SerializeField]
    private float _tripleShotPowerDownTime = 2.0f;
    [SerializeField]
    private float _moveSpeedPowerDownTime = 2.0f;
    [SerializeField]
    private float _bombShotPowerDownTime = 2.5f;
    [Tooltip("A float representing a percentage. Eg.: 0.2 means 20%")]
    [SerializeField]
    private float _moveSpeedBoostMultiplier = 0.3f;
    [SerializeField]
    private float _shieldPowerDownTime = 20.0f;
    [SerializeField]
    private float _thrusterMoveSpeedMultiplier = 0.2f;
    [Tooltip("How long in seconds until the thruster is fully drained")]
    [SerializeField]
    private float _thrusterDrainSpeed = 1.0f;
    [Tooltip("How long in seconds until the thruster is fully recharged")]
    [SerializeField]
    private float _thrusterRechargeSpeed = 4.0f;
    [SerializeField]
    private int _maxShieldLives = 3;
    [SerializeField]
    private int _maxAmmoCount = 15;

    private float _canFire = 0f;
    private SpawnManager _spawnManager;
    private bool _isTripleShotActive = false;
    private bool _isBombShotActive = false;

    [SerializeField]
    private float _moveSpeed;
    private float _moveSpeedMultiplier = 0.0f; // A float representing a percentage. Eg.: 0.2 means 20%
    private float _fireRateMultiplier = 0.0f; // A float representing a percentage. Eg.: 0.2 means 20%

    private UIManager _uiManager;
    private int _score;

    private GameManager _gameManager;
    private AudioSource _audioSource;
    private CameraManager _cameraManager;

    private GameObject _sourceLaserDamage;

    // private thruster vars
    private bool _leftShiftHeld;
    private bool _thrusterRanOut;

    // private shield vars
    private int _currentShieldLives;

    private int _currentAmmoCount;

    //Variables to define the playable space. The player cannot move outside of these bounds
    private float _upperBounds = 0;
    private float _lowerBounds = -3.9f;
    private float _rightBounds = 11.5f;
    private float _leftBounds = -11.5f;


    // Start is called before the first frame update
    void Start()
    {
        transform.position = new Vector3(0, 0, 0);
        _currentAmmoCount = _maxAmmoCount;
        _spawnManager = GameObject.Find("SpawnManager").GetComponent<SpawnManager>();
        _shieldVisuals.SetActive(false);
        _uiManager = GameObject.FindObjectOfType<Canvas>().GetComponent<UIManager>();
        _gameManager = GameObject.FindObjectOfType<GameManager>();
        _cameraManager = GameObject.FindObjectOfType<CameraManager>();
        _audioSource = GetComponent<AudioSource>();

        if (_spawnManager == null)
            Debug.Log("The Spawn Manager is null");
        if (_uiManager == null)
            Debug.Log("The UI Manager is null");
        if (_gameManager == null)
            Debug.Log("The Game Manager is null");
        if (_audioSource == null)
            Debug.Log("The Audio Source is null");

        for (int i = 0; i < _damagedEngineVisuals.Length; i++)
        {
            if (_damagedEngineVisuals[i] != null)
                _damagedEngineVisuals[i].SetActive(false);
            else
                Debug.Log("Null reference for " + _damagedEngineVisuals[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();

        // If shift is held, speed up. If it is released, go back to normal speed.
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            EngageThrusters();
        }
        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            DisengageThrusters();
        }


        if (Input.GetKey(KeyCode.Space) && Time.time > _canFire)
        {
            FireLaser();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            Debug.Log("Movespeed is: " + _moveSpeedMultiplier);
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
            _laserShotSFX.GetComponent<AudioSource>().Play();
        }
        else if (_isBombShotActive)
        {
            Instantiate(_bombPrefab, transform.position + Vector3.up, Quaternion.identity);
            _laserShotSFX.GetComponent<AudioSource>().Play();
        }
        else if (_currentAmmoCount > 0)
        {
            Instantiate(_laserPrefab, transform.position + Vector3.up, Quaternion.identity);
            _currentAmmoCount--;
            _uiManager.UpdateAmmoText(_currentAmmoCount, _maxAmmoCount);
            _laserShotSFX.GetComponent<AudioSource>().Play();
        }
        else if (_currentAmmoCount == 0)
        {
            _uiManager.UpdateAmmoText(_currentAmmoCount, _maxAmmoCount);
        }
    }

    void EngageThrusters()
    {
        _leftShiftHeld = true;
        _moveSpeedMultiplier += _thrusterMoveSpeedMultiplier;
        _uiManager.DrainThrusterBar(_thrusterDrainSpeed);
        StartCoroutine(EngageThrusterRoutine());
        HandleThrusterAudio(_thrusterAudioClip);
    }
       
    void DisengageThrusters()
    {
        _leftShiftHeld = false;
        if (_thrusterRanOut == false)
            _moveSpeedMultiplier -= _thrusterMoveSpeedMultiplier;
        _thrusterRanOut = false;
        _uiManager.RechargeThrusterBar(_thrusterRechargeSpeed);
        HandleThrusterAudio(null);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Laser")
        {
            Laser laser = collision.gameObject.GetComponent<Laser>();
            if (laser != null && laser.GetIsEnemyLaser() == true)
            {
                GameObject sourceLaserDamage = laser.transform.parent.gameObject;
                if (sourceLaserDamage == _sourceLaserDamage) // compare this laser's parent to the laser's parent that damaged us last. If true, don't deal damage with this laser.
                {
                    Destroy(collision.gameObject);
                    return;
                }
                else
                {
                    _sourceLaserDamage = laser.transform.parent.gameObject;
                    TakeDamage();
                    Destroy(collision.gameObject);
                }
            }        
        }
    }
    public void TakeDamage()
    {
        // if the shield is active, decrement _shieldLives and make the shield smaller. If the last life is spent, deactivate the shield
        if (_currentShieldLives >= 1) 
        {
            _currentShieldLives--;
            SetShieldVisuals();
            _cameraManager.enabled = true;
            _cameraManager.StartCameraShake(0.02f);
            return;
        }

        _lives--;
        _uiManager.UpdateLivesDisplay(_lives);
        HandlePlayerLives();
        _cameraManager.enabled = true;
        _cameraManager.StartCameraShake(0.1f);
    }

    public void HandlePlayerLives()
    {
        switch (_lives)
        {
            case 0:
                HandlePlayerDeath();
                break;
            case 1:
                _damagedEngineVisuals[1].SetActive(true);
                break;
            case 2:
                _damagedEngineVisuals[0].SetActive(true);
                _damagedEngineVisuals[1].SetActive(false); // for future "gain life" powerup
                break;
            case 3:
                _damagedEngineVisuals[0].SetActive(false); // for future "gain life" powerup
                break;
        }
    }

    public void SetTripleShotActive()
    {
        _isTripleShotActive = true;
        _isBombShotActive = false;
        StartCoroutine(TripleShotPowerDownRoutine());
    }

    public void SetSpeedBoostActive()
    {
        _moveSpeedMultiplier += _moveSpeedBoostMultiplier;
        StartCoroutine(SpeedBoostPowerDownRoutine());
    }

    public void SetShieldActive()
    {
        _currentShieldLives = _maxShieldLives;
        SetShieldVisuals();
        StartCoroutine(ShieldPowerDownRoutine());
    }

    public void SetShieldVisuals()
    {
        switch(_currentShieldLives)
        {
            case 0:
                _shieldVisuals.SetActive(false);
                break;
            case 1:
                _shieldVisuals.transform.localScale = new Vector3(1.4f, 1.4f, 1);
                break;
            case 2:
                _shieldVisuals.transform.localScale = new Vector3(1.6f, 1.6f, 1);
                break;
            case 3:
                _shieldVisuals.SetActive(true);
                _shieldVisuals.transform.localScale = new Vector3(1.8f, 1.8f, 1);
                break;
        }
    }

    public void IncreaseAmmo()
    {
        _maxAmmoCount += 5;
        _currentAmmoCount = _maxAmmoCount;
        _uiManager.UpdateAmmoText(_currentAmmoCount, _maxAmmoCount);
    }

    public void AddPlayerLife()
    {
        if (_lives < _maxLives)
            _lives++;

        _uiManager.UpdateLivesDisplay(_lives);
        HandlePlayerLives();
    }

    public void SetBombActive()
    {
        _isBombShotActive = true;
        _isTripleShotActive = false;
        StartCoroutine(BombShotPowerDownRoutine());
    }

    public void HandleThrusterAudio(AudioClip clip)
    {
        if (clip != null)
        {
            _audioSource.clip = clip;
            _audioSource.loop = true;
            _audioSource.Play();
        }
        else if (_audioSource.isPlaying == true)
        {
            _audioSource.Stop();
            _audioSource.clip = null;
            _audioSource.loop = false;
        }
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
        AudioSource.PlayClipAtPoint(_explosionAudioClip, transform.position, 0.5f);
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
    }

    private IEnumerator ShieldPowerDownRoutine()
    {
        yield return new WaitForSeconds(_shieldPowerDownTime);
        _currentShieldLives = 0;
        SetShieldVisuals();
    }

    private IEnumerator BombShotPowerDownRoutine()
    {
        yield return new WaitForSeconds(_bombShotPowerDownTime);
        _isBombShotActive = false;
    }

    private IEnumerator EngageThrusterRoutine()
    {
        while (_leftShiftHeld == true)
        {
            if (_uiManager.GetThrusterBarAmount() <= 0)
            {
                _thrusterRanOut = true;
                _moveSpeedMultiplier -= _thrusterMoveSpeedMultiplier;
                _leftShiftHeld = false;
                HandleThrusterAudio(null);
            }

            yield return new WaitForEndOfFrame();
        }
    }
}
