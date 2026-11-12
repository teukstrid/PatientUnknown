using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public enum GameState { Menu, Playing, Paused, Inspecting, Caught, Won }

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private GameState _state = GameState.Menu;
    private static bool _skipStartScreen;
    public static event System.Action<GameState> OnGameStateChanged;
    public GameState CurrentState => _state;
    public MusicManager musicManager;
    private bool _victoryFadeStarted;
    private GameObject _activeInteractionTooltip;
    private GameObject _objectToDestroyAfterTooltip;
    [SerializeField] private CanvasGroup fadeOverlay;
    [SerializeField, Min(0f)] private float fadeDuration = 1f;
    [SerializeField] private MonoBehaviour playerMovement;
    [SerializeField] private LightFlicker[] flickeringLights;
    
    [Header("Player Caught Animation")]
    [SerializeField] private Animator playerAnimator;
    [SerializeField] private float caughtAnimationDuration = 2f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogError("There should only be one GameManager in the scene.", this);
            enabled = false;
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if (_skipStartScreen)
        {
            _skipStartScreen = false;
            SetState(GameState.Playing);
        }
        else
        {
            SetState(GameState.Menu);
        }
    }

    
    private void Update()
    {
        if (Keyboard.current == null ||
            !Keyboard.current.escapeKey.wasPressedThisFrame)
            return;

        if (_state == GameState.Inspecting)
        {
            CloseInteractionTooltip();
        }
        else if (_state == GameState.Playing)
        {
            SetState(GameState.Paused);
        }
        else if (_state == GameState.Paused)
        {
            SetState(GameState.Playing);
        }
    }


    private void SetState(GameState newState)
    {
        _state = newState;

        bool inMenus =
            newState == GameState.Menu ||
            newState == GameState.Paused ||
            newState == GameState.Inspecting ||
            newState == GameState.Won;
        StarterAssets.StarterAssetsInputs playerInputs = playerMovement.GetComponent<StarterAssets.StarterAssetsInputs>();
        if (inMenus)
        {
            playerInputs.cursorInputForLook = false;
            Time.timeScale = 0f;
        }
        else
        {
            playerInputs.cursorInputForLook = true;
            Time.timeScale = 1f;
        }

        Debug.Log("Game State changed to: " + _state);
        OnGameStateChanged?.Invoke(_state);

        if (musicManager == null)
        {
            Debug.LogError("Music Manager is not assigned.", this);
            return;
        }

        switch (newState)
        {
            case GameState.Menu:
                musicManager.StopAmbience();
                musicManager.PlayMusic(musicManager.menuMusic);
                break;

            case GameState.Playing:
                musicManager.PlayOrResumeAmbience();
                musicManager.PlayMusic(musicManager.gameplayMusic);
                break;

            case GameState.Paused:
                musicManager.PauseAmbience();
                musicManager.PlayMusic(musicManager.pauseMusic);
                break;

            case GameState.Won:
                musicManager.StopAmbience();
                musicManager.PlayMusic(musicManager.victoryMusic);
                break;

            case GameState.Caught:
                musicManager.StopAmbience();
                musicManager.StopMusic();
                break;
        }   
    }

    public void StartGame()
    {
        SetState(GameState.Playing);
    }

    public void Resume()
    {
        SetState(GameState.Playing);
        CloseInteractionTooltip();
    }

    public void Pause()
    {
        if (_state == GameState.Playing)
        {
            SetState(GameState.Paused);
        }
    }

    public void PlayerCaught()
    {
        if (_state != GameState.Playing)
            return;

        // Lock player controls.
        playerMovement.enabled = false;

        // Change state and stop gameplay music.
        SetState(GameState.Caught);

        // Play the player's caught animation.
        if (playerAnimator != null)
        {
            playerAnimator.ResetTrigger("PlayerCaught");
            playerAnimator.SetTrigger("PlayerCaught");

            Debug.Log("Player caught animation triggered.", playerAnimator);
        }
        else
        {
            Debug.LogWarning("Player Animator is not assigned!", this);
        }

        // Wait for the animation, then reset.
        StartCoroutine(CaughtSequence());
    }


    public void ShowInteractionTooltip(
        GameObject tooltipCanvas,
        GameObject objectToDestroy)
    {
        if (_state != GameState.Playing)
            return;

        _activeInteractionTooltip = tooltipCanvas;
        _objectToDestroyAfterTooltip = objectToDestroy;

        _activeInteractionTooltip.SetActive(true);
        SetState(GameState.Inspecting);
    }

    private void CloseInteractionTooltip()
    {
        if (_activeInteractionTooltip != null)
            _activeInteractionTooltip.SetActive(false);

        if (_objectToDestroyAfterTooltip != null)
            Destroy(_objectToDestroyAfterTooltip);

        _activeInteractionTooltip = null;
        _objectToDestroyAfterTooltip = null;

        SetState(GameState.Playing);
    }

    private System.Collections.IEnumerator CaughtSequence()
    {
        yield return new WaitForSecondsRealtime(caughtAnimationDuration);

        ResetAfterCaught();
    }

    public void PlayerWon()
    {
        if (_state == GameState.Playing)
        {
            SetState(GameState.Won);
        }
    }

    public void FadeToVictory()
    {
        if (_state != GameState.Playing || _victoryFadeStarted)
            return;

        _victoryFadeStarted = true;
        StartCoroutine(FadeToVictorySequence());
    }

    private System.Collections.IEnumerator FadeToVictorySequence()
    {
        if (fadeOverlay == null)
        {
            Debug.LogError("Fade overlay is not assigned; showing victory without a fade.", this);
            PlayerWon();
            yield break;
        }

        Time.timeScale = 1f;
        fadeOverlay.gameObject.SetActive(true);
        fadeOverlay.blocksRaycasts = true;
        fadeOverlay.interactable = false;

        yield return FadeOverlayTo(1f);
        PlayerWon();
        yield return FadeOverlayTo(0f);

        fadeOverlay.blocksRaycasts = false;
        fadeOverlay.gameObject.SetActive(false);
    }

    private System.Collections.IEnumerator FadeOverlayTo(float targetAlpha)
    {
        float elapsed = 0f;
        float startAlpha = fadeOverlay.alpha;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            fadeOverlay.alpha = Mathf.Lerp(startAlpha, targetAlpha, Mathf.Clamp01(elapsed / fadeDuration));
            yield return null;
        }

        fadeOverlay.alpha = targetAlpha;
    }

    public void PlayAgain()
    {
        _skipStartScreen = true;
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ResetAfterCaught()
    {
        StartCoroutine(FadeAndReset());
    }

    private System.Collections.IEnumerator FadeAndReset()
    {
        // Make sure the game is running during the fade.
        Time.timeScale = 1f;

        if (fadeOverlay != null)
        {
            fadeOverlay.gameObject.SetActive(true);
            fadeOverlay.blocksRaycasts = true;
            fadeOverlay.interactable = true;

            float elapsed = 0f;
            float startAlpha = fadeOverlay.alpha;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.unscaledDeltaTime;

                fadeOverlay.alpha = Mathf.Lerp(
                    startAlpha,
                    1f,
                    Mathf.Clamp01(elapsed / fadeDuration)
                );

                yield return null;
            }

            fadeOverlay.alpha = 1f;
        }

        PlayAgain();
    }

    public void QuitGame()
    {
        Debug.Log("Quitting the game.");
        Application.Quit();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
