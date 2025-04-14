using System.Collections;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed = 4.0f;
    [SerializeField]
    private AudioClip _explosionAudioClip;
    private int _pointsToGive = 10;

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
            Destroy(other.gameObject);
            if (_player != null)
                _player.GetComponent<Player>().AddScore(_pointsToGive);
            HandleEnemyDeath();
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
