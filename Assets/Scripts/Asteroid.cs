using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private float _rotateSpeed = 20.0f;
    [SerializeField]
    private GameObject _explosionPrefab;
    [SerializeField]
    private AudioClip _explosionAudioClip;
    private SpawnManager _spawnManager;

    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.FindObjectOfType<SpawnManager>();
        if (_spawnManager == null)
            Debug.Log("No reference to the Spawn Manager");
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.back * _rotateSpeed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Laser")
        {
            GameObject explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            Destroy(explosion.gameObject, 2.5f);
            if (collision.GetComponent<CircleCollider2D>() == null) // if it isn't a bomb, destroy the laser.
                Destroy(collision.gameObject);
            PlayExplosionSound();
            _spawnManager.StartSpawning();
            Destroy(this.gameObject, 0.25f);
        }   
    }

    private void PlayExplosionSound()
    {
        AudioSource.PlayClipAtPoint(_explosionAudioClip, this.gameObject.transform.position, 0.5f);
    }
}
