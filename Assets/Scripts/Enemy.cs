using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed = 4.0f;

    //Define bounds of playable space for enemies
    private float _upperBounds = 9.0f;
    private float _lowerBounds = -5.4f;
    private float _rightBounds = 9.3f;
    private float _leftBounds = -9.3f;

    private SpawnManager _spawnManager;

    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.FindObjectOfType<SpawnManager>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.down * _moveSpeed * Time.deltaTime);
        if(transform.position.y <= _lowerBounds)
        {
            float randomX = Random.Range(_leftBounds, _rightBounds);
            transform.position = new Vector3(randomX, _upperBounds, 0); 
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            DealDamage(other);
            DestroySelf();
        }
        if (other.tag == "Laser")
        {
            Destroy(other.gameObject);
            DestroySelf();
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

    void DestroySelf()
    {
        if(_spawnManager != null)
        {
            _spawnManager.OnEnemyDeath();
        }
        Destroy(this.gameObject);
    }
}
