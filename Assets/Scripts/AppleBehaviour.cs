using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class AppleGame : MonoBehaviour
{
    public GameObject apple;
    public TextMeshProUGUI scoreText;
    public GameObject restartButton; // Reference to the Restart Button

    private int score = 0;
    private bool isGolden = false;
    private bool playerHoveredDuringGolden = false;

    private Coroutine scoreCoroutine;
    private float baitDurationMax = 0.5f;
    private Coroutine goldenTimerCoroutine;

    void Start()
    {
        UpdateScoreText();
        StartCoroutine(ColorChangeRoutine());       // Begin color change loop
        scoreCoroutine = StartCoroutine(ScoreIncreaseRoutine()); // Start score increment coroutine
        restartButton.SetActive(false);  // Hide the restart button at the start
    }

    void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        bool isMouseOverApple = apple.GetComponent<Collider2D>().bounds.Contains(mousePosition);

        if (isMouseOverApple && !isGolden)
        {
            Debug.Log("Game Over - Mouse touched apple while it was red.");
            GameOver();
        }
        else if (isMouseOverApple && isGolden && Input.GetMouseButtonDown(0))
        {
            score += 50;
            UpdateScoreText();
            playerHoveredDuringGolden = true;
            Debug.Log("Player clicked on golden apple. Score: " + score);
        }
    }

    private IEnumerator ScoreIncreaseRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            bool isMouseOverApple = apple.GetComponent<Collider2D>().bounds.Contains(mousePosition);

            if (!isGolden && !isMouseOverApple)
            {
                score += 1;
                UpdateScoreText();
                Debug.Log("Score increased by 1. Current score: " + score);
            }
        }
    }

    private IEnumerator ColorChangeRoutine()
    {
        while (true)
        {
            float nextGoldenChange = Random.Range(2f, 6f);
            Debug.Log("Next golden apple in: " + nextGoldenChange + " seconds");
            yield return new WaitForSeconds(nextGoldenChange);

            bool isQuickFlash = Random.value > 0.5f;

            if (isQuickFlash)
            {
                StartCoroutine(QuickFlash());
            }
            else
            {
                isGolden = true;
                apple.GetComponent<SpriteRenderer>().color = Color.yellow;
                
                playerHoveredDuringGolden = false;
                Debug.Log("Apple turned golden (bait mechanic). Waiting for player to hover and click...");
                goldenTimerCoroutine = StartCoroutine(GoldenBaitDuration());
            }
        }
    }

    private IEnumerator GoldenBaitDuration()
    {
        while (isGolden)
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            
            if (apple.GetComponent<Collider2D>().bounds.Contains(mousePosition) && !playerHoveredDuringGolden)
            {
                float duration = Random.Range(0.3f, baitDurationMax);
                Debug.Log("Player hovered over golden apple. Reverting to red in " + duration + " seconds if not clicked.");
                yield return new WaitForSeconds(duration);

                if (!playerHoveredDuringGolden)
                {
                    Debug.Log("Player failed to click golden apple in time. Game Over.");
                    GameOver();
                }
                else
                {
                    isGolden = false;
                    apple.GetComponent<SpriteRenderer>().color = Color.red;
                    Debug.Log("Apple reverted to red after successful click.");
                }
            }
            else
            {
                yield return null;
            }
        }
    }

    private IEnumerator QuickFlash()
    {
        isGolden = true;
        apple.GetComponent<SpriteRenderer>().color = Color.yellow;

        float flashDuration = Random.Range(0.2f, 1f);
        Debug.Log("Apple is golden (quick flash) for " + flashDuration + " seconds.");
        yield return new WaitForSeconds(flashDuration);

        isGolden = false;
        apple.GetComponent<SpriteRenderer>().color = Color.red;
        Debug.Log("Apple reverted to red after quick flash.");
    }

    public void GameOver()
    {
        if (goldenTimerCoroutine != null)
        {
            StopCoroutine(goldenTimerCoroutine);
        }

        isGolden = false;
        apple.GetComponent<SpriteRenderer>().color = Color.red;
        scoreText.text = "Game Over! Final Score: " + score;
        Debug.Log("Game Over! Final Score: " + score);

        StopAllCoroutines();

        restartButton.SetActive(true); // Show the restart button
    }

    private void UpdateScoreText()
    {
        scoreText.text = "Score: " + score;
    }

    // Method to restart the game
    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Reload the current scene
    }
}
