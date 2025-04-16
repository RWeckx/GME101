using System.Collections;
using UnityEngine;

public class Bomb : Laser
{
    [SerializeField]
    private float _explosionRadius = 25f;
    [SerializeField]
    private float _timeToExplosion = 1.5f;
    [SerializeField]
    private GameObject _explosionPrefab;
    [SerializeField]
    private AudioClip _explosionSound;
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ExplosionCountDownRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        MoveLaser();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Enemy")
        {
            HandleBombExplosion();
        }
    }

    private void HandleBombExplosion()
    {
        GetComponent<CircleCollider2D>().radius = _explosionRadius;
        GameObject explosion = Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        Destroy(explosion.gameObject, 2.5f);
        AudioSource.PlayClipAtPoint(_explosionSound, this.gameObject.transform.position, 0.5f);
        GetComponent<SpriteRenderer>().enabled = false;
        Destroy(this.gameObject, 0.5f);
    }

    IEnumerator ExplosionCountDownRoutine()
    {
        yield return new WaitForSeconds(_timeToExplosion);
        HandleBombExplosion();
    }
}
