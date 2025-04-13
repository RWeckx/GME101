using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed = 3.0f;
    private float _lowerBounds = -7.3f;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        CalculateMovement();
    }

    public void CalculateMovement()
    {
        transform.Translate(Vector3.down * _moveSpeed * Time.deltaTime);
        if (transform.position.y <= _lowerBounds)
            Destroy(this.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            Player player = collision.GetComponent<Player>();
            if (player != null)
                player.SetTripleShotActive();
            Destroy(this.gameObject);
        }       
    }
}
