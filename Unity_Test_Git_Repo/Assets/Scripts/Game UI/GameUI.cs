using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using  UnityEngine.SceneManagement;

public class GameUI : MonoBehaviour
{
    public Image fadePlane;
    public GameObject gameOverUI;

    public RectTransform newWaveBanner;
    public Text newWaveTitle;
    public Text nextWaveEnemyCount;

    [Header("Health bar")]
    public RectTransform healthBar;

    private Spawner spawner;
    private Player player;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
        player.OnDeath += OnGameOver;
    }

    private void Awake()
    {
        spawner = FindObjectOfType<Spawner>();
        spawner.OnNewWave += OnNewWave;
    }
    private void Update()
    {
        float healthPercent = 0;
        if (player != null)
        {
            healthPercent = player._health / player._startingHealth;
        }
        healthBar.localScale = new Vector3(healthPercent, 1, 1);
    }

    private void OnNewWave(int waveNumber)
    {
        newWaveTitle.text = $"Wave {waveNumber}";
        nextWaveEnemyCount.text = $"Enemies: {spawner.waves[waveNumber - 1].enemyCount + 1}";

        StartCoroutine(AnimateNewWaveBanner());
    }

    IEnumerator AnimateNewWaveBanner()
    {
        float speed = 2.5f;
        float delayTime = 2f; // cat timp sta bannerul pe ecran
        float animationPercent = 0;
        int direction = 1;
        float endDelayTime = Time.time + 1/speed + delayTime;

        while (animationPercent >= 0)
        {
            animationPercent += Time.deltaTime * speed * direction;

            if (animationPercent >= 1)
            {
                animationPercent = 1;

                if (Time.time > endDelayTime)
                {
                    direction = -1;
                }
            }

            newWaveBanner.anchoredPosition = Vector2.up * Mathf.Lerp(400, 218, animationPercent);
            yield return null;
        }
    }

    void OnGameOver()
    {
        StartCoroutine(Fade(Color.clear, Color.black, 1));
        Cursor.visible = true;
        gameOverUI.SetActive(true);
    }

    IEnumerator Fade(Color from, Color to, float time)
    {
        float speed = 1 / time;
        float percent = 0;

        while (percent < 1)
        {
            percent += Time.deltaTime * speed;
            fadePlane.color = Color.Lerp(from, to, percent);
            yield return null;
        }
    }

    // UI Input

    public void StartNewGame()
    { 
        SceneManager.LoadScene("Dev");
    }
}
