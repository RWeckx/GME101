using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed = 4.0f;
    [SerializeField]
    private GameObject _laserPrefab;
    [SerializeField]
    private AudioClip _explosionAudioClip;
    private int _pointsToGive = 10;
    [SerializeField]
    private float _fireRateInSec = 3.0f;
    [Tooltip("X & Y represent the min & max time in seconds it takes enemies to fire a laser")]
    [SerializeField]
    private Vector2 _minMaxFireRate = new Vector2(3.0f, 6.0f);
    private float _canFire = 0.0f;
    [SerializeField]
    private Vector3 _laserOffset = new Vector3(0, -1.2f, 0);

    //Define bounds of playable space for enemies
    private float _upperBounds = 9.0f;
    private float _lowerBounds = -5.4f;
    private float _rightBounds = 9.3f;
    private float _leftBounds = -9.3f;

    private bool _isDead;

    private SpawnManager _spawnManager;
    private Player _player;
    private Animator _animator;

    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.FindObjectOfType<SpawnManager>();
        _player = GameObject.FindObjectOfType<Player>();
        _animator = GetComponent<Animator>();
        if (_player == null)
            Debug.Log("No reference to Player");
        if (_animator == null)
            Debug.Log("No reference to Animator");
        _pointsToGive = Random.Range(5, 15);

    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
        if (Time.time > _canFire && transform.position.y < 6.0f && _isDead == false)
            FireLaser();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            DealDamage(other);
            other.GetComponent<Player>().AddScore(_pointsToGive);
            HandleEnemyDeath();
        }
        if (other.tag == "Laser")
        {
            Laser laser = other.gameObject.GetComponent<Laser>();
            if (laser != null && laser.GetIsEnemyLaser() == false)
            {
                if (_player != null)
                    _player.GetComponent<Player>().AddScore(_pointsToGive);
                Destroy(other.gameObject);
                HandleEnemyDeath();
            }
        }
    }

    private void CalculateMovement()
    {
        transform.Translate(Vector3.down * _moveSpeed * Time.deltaTime);
        if (transform.position.y <= _lowerBounds)
        {
            float randomX = Random.Range(_leftBounds, _rightBounds);
            transform.position = new Vector3(randomX, _upperBounds, 0);
            if (_isDead)
                Destroy(this.gameObject);
        }
    }

    private void FireLaser()
    {
        _fireRateInSec = Random.Range(_minMaxFireRate.x, _minMaxFireRate.y);
        _canFire = Time.time + _fireRateInSec;

        GameObject instantiatedLaser = Instantiate(_laserPrefab, transform.position, Quaternion.identity);

        Laser[] lasers = instantiatedLaser.GetComponentsInChildren<Laser>();
        for (int i = 0; i < lasers.Length; i++)
        {
            lasers[i].SetAsEnemyLaser();
        }
    }

    void DealDamage(Collider2D other)
    {
        Player player = other.transform.GetComponent<Player>();
        if (player != null)
        {
            player.TakeDamage();
        }
    }

    void HandleEnemyDeath()
    {
        if(_spawnManager != null)
        {
            _spawnManager.OnEnemyDeath();
        }
        _isDead = true;
        _animator.SetTrigger("OnEnemyDeath");
        GetComponent<Collider2D>().enabled = false;
        AudioSource.PlayClipAtPoint(_explosionAudioClip, transform.position, 0.5f);
    }
}
