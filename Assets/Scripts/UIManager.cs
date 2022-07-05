using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text _scoreText;
    [SerializeField] private Sprite[] _liveSprites;
    [SerializeField] private Image _livesImg;
    [SerializeField] private Text _gameOverText;
    [SerializeField] private Text _restartText;
    [SerializeField] private Sprite[] _shieldStrengthSprites;
    [SerializeField] private Image _shieldsImg;
    [SerializeField] private Sprite[] _thrusterSprites;
    [SerializeField] private Image _thrusterImg;
    [SerializeField] private Text _ammoText;
    private GameManager _gameManager;

    // Start is called before the first frame update
    void Start()
    {
        _ammoText.text = "Ammo: " + 25 + " / 75";
        _scoreText.text = "Score: " + 0;
        _gameOverText.gameObject.SetActive(false);
        _gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        if (_gameManager == null)
        {
            Debug.LogError("GameManager is NULL.");
        }
    }

    public void UpdateAmmo(int playerAmmo)
    {
        _ammoText.text = "Ammo: " + playerAmmo.ToString() + " / 75";
    }

    public void UpdateScore(int playerScore)
    {
        _scoreText.text = "Score: " + playerScore.ToString();
    }

    public void UpdateLives (int currentLives)
    {
        _livesImg.sprite = _liveSprites[currentLives];

        if (currentLives == 0)
        {
            GameOverSequence();
        }
    }

    public void UpdateThrusterLevel(float currentThrusterLevel)
    {
        _thrusterImg.sprite = _thrusterSprites[(int) currentThrusterLevel];
    }

    public void UpdateShieldStrength(int currentShieldStrength)
    {
        _shieldsImg.sprite = _shieldStrengthSprites[currentShieldStrength];
    }

    IEnumerator GameOverFlickerRoutine()
    {
        while (true)
        {
            _gameOverText.text = "GAME OVER!";
            yield return new WaitForSeconds(0.5f);
            _gameOverText.text = "";
            yield return new WaitForSeconds(0.5f);
        }
    }

    void GameOverSequence()
    {
        _gameManager.GameOver();
        _gameOverText.gameObject.SetActive(true);
        _restartText.gameObject.SetActive(true);
        StartCoroutine(GameOverFlickerRoutine());
    }
}
