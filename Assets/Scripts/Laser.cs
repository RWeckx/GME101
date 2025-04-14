using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField]
    private float _speed = 3.0f;
    private float _direction = 1.0f;
    private bool _isEnemyLaser;
    private GameObject _enemyOwner;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        MoveLaser();
        if (transform.position.y >= 8 || transform.position.y < -5.0f)
        {
            if (transform.parent != null)
                Destroy(transform.parent.gameObject);
            Destroy(gameObject);
        }
    }

    void MoveLaser()
    {
        transform.Translate(Vector3.up * _direction * _speed * Time.deltaTime);
    }

    public void SetAsEnemyLaser(GameObject enemyOwner)
    {
        _direction *= -1.0f;
        _isEnemyLaser = true;
        _enemyOwner = enemyOwner;
    }

    public bool GetIsEnemyLaser()
    {
        return _isEnemyLaser;
    }
}
