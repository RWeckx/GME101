using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEditor.UIElements;

public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text _scoreText;
    [SerializeField]
    private Image _livesImg;
    [SerializeField]
    private Sprite[] _livesSprites;
    [SerializeField]
    private TMP_Text _gameOverText;
    [SerializeField]
    private TMP_Text _restartText;
    [SerializeField]
    private Image _thrusterProgressBar;
    [SerializeField]
    private TMP_Text _ammoCountText;

    private bool _isRechargingThruster;
    
    // Start is called before the first frame update
    void Start()
    {
        UpdateLivesDisplay(3);
        _scoreText.text = "Score: " + 0;
        _ammoCountText.text = "Ammo: " + 15 + "/" + 15;
        _gameOverText.gameObject.SetActive(false);
        _restartText.gameObject.SetActive(false);
        _thrusterProgressBar.color = new Color(1, 1, 1, 0); // hide Thruster progress bar until it's used
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void UpdateScoreDisplay(int score)
    {
        _scoreText.text = "Score: " + score;
    }

    public void UpdateLivesDisplay(int currentLives)
    {
        _livesImg.sprite = _livesSprites[currentLives];
    }

    public void UpdateAmmoText(int currentAmmo, int maxAmmo)
    {
        _ammoCountText.text = "Ammo: " + currentAmmo + "/" + maxAmmo;
        if (currentAmmo == 0)
        {
            StartCoroutine(FlickerAmmoTextRoutine());
        }
    }
    
    public void ShowGameOverText()
    {
        _gameOverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        StartCoroutine(GameOverFlickerRoutine());
    }

    public void RechargeThrusterBar(float thrusterRechargeTime)
    {
        _isRechargingThruster = true;
        StartCoroutine(RechargeThrusterRoutine(thrusterRechargeTime));
    }

    public void DrainThrusterBar(float thrusterDrainTime)
    {
        _isRechargingThruster = false;
        StartCoroutine(DrainThrusterRoutine(thrusterDrainTime));
    }
    
    public float GetThrusterBarAmount()
    {
        return _thrusterProgressBar.fillAmount;
    }
    
    IEnumerator GameOverFlickerRoutine()
    {
        while (true)
        {
            _gameOverText.gameObject.SetActive(true);
            yield return new WaitForSeconds(0.5f);
            _gameOverText.gameObject.SetActive(false);
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator DrainThrusterRoutine(float thrusterDrainTime)
    {
        //show the bar in case it was hidden because it was full, and stop the hide coroutine
        _thrusterProgressBar.color = new Color(1, 1, 1, 1);
        StopCoroutine(HideThrusterBarRoutine());

        while (_isRechargingThruster == false)
        {
            _thrusterProgressBar.fillAmount -= 1.0f / thrusterDrainTime * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator RechargeThrusterRoutine(float thrusterFillTime)
    {
        yield return new WaitForSeconds(0.5f);
        while (_isRechargingThruster == true && _thrusterProgressBar.fillAmount < 1.0f)
        {
            _thrusterProgressBar.fillAmount += 1.0f / thrusterFillTime * Time.deltaTime;

            if (_thrusterProgressBar.fillAmount == 1.0f)
            {
                _isRechargingThruster = false;
                StartCoroutine(HideThrusterBarRoutine());
            }

            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator HideThrusterBarRoutine()
    {
        while (_thrusterProgressBar.color.a > 0.0f && _isRechargingThruster == false)
        {
            float speed = 1.0f;
            _thrusterProgressBar.color = new Color(1, 1, 1, _thrusterProgressBar.color.a - speed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
    }

    IEnumerator FlickerAmmoTextRoutine()
    {
        _ammoCountText.color = Color.red;
        yield return new WaitForSeconds(0.25f);
        _ammoCountText.color = Color.white;
    }
}
