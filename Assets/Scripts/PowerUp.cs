using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed = 3.0f;
    [SerializeField] // 0 = Triple Shot , 1 = Speed , 2 = Shields, 3 = Ammo
    private int _powerupID;
    [SerializeField]
    private AudioClip _powerUpAudioClip;
    private float _lowerBounds = -7.3f;

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
            {
                switch (_powerupID)
                {
                    case 0:
                        player.SetTripleShotActive();
                        break;
                    case 1:
                        player.SetSpeedBoostActive();
                        break;
                    case 2:
                        player.SetShieldActive();
                        break;
                    case 3:
                        player.IncreaseAmmo();
                        break;
                    case 4:
                        player.AddPlayerLife();
                        break;
                    case 5:
                        player.SetBombActive();
                        break;
                    case 6:
                        player.SetSlowActive();
                        break;
                } 
            }
            if (_powerUpAudioClip != null)
                AudioSource.PlayClipAtPoint(_powerUpAudioClip, transform.position, 0.5f);

            Destroy(this.gameObject);
        }       
    }
}
