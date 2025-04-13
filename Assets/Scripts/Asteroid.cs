using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [SerializeField]
    private float _rotateSpeed = 20.0f;
    [SerializeField]
    private GameObject _explosionPrefab;
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
            Destroy(collision.gameObject);
            _spawnManager.StartSpawning();
            Destroy(this.gameObject, 0.25f);
        }   
    }
}
