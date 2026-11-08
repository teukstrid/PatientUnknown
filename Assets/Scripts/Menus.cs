using UnityEngine;

public class Menus : MonoBehaviour
{
    [SerializeField] private GameObject _startPanel;
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private GameObject _winPanel;
    [SerializeField] private GameObject _HUDPanel;

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += ShowScreen;

        if (GameManager.Instance != null)
        {
            ShowScreen(GameManager.Instance.CurrentState);
        }
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= ShowScreen;
    }

    private void ShowScreen(GameState state)
    {
        Debug.Log($"Game state changed to: {state}");
        SetPanelActive(_startPanel, state == GameState.Menu);
        SetPanelActive(_pausePanel, state == GameState.Paused);
        SetPanelActive(_winPanel, state == GameState.Won);
        SetPanelActive(_HUDPanel, state == GameState.Playing);
        SetMenuOpen(state != GameState.Playing);
    }

    private void SetPanelActive(GameObject panel, bool active)
    {
        if (panel != null)
        {
            panel.SetActive(active);
        }
    }

    private void SetMenuOpen(bool isOpen)
    {
        Cursor.visible = isOpen;
        Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
    }
}
