using UnityEngine;

public class SyringePickup : MonoBehaviour, IInteractable
{
    [SerializeField] private string _promptMessage = "Sedative Syringe";
    [SerializeField] private MusicManager musicManager;
    [SerializeField] private AudioClip pickupSFX;
    [SerializeField] private GameObject tooltipCanvas;

    private bool _hasBeenPickedUp;
    public string PromptMessage => _promptMessage;

    private void Awake()
    {
        if (tooltipCanvas != null)
            tooltipCanvas.SetActive(false);
    }

    public bool CanInteract()
    {
        return !_hasBeenPickedUp;
    }

    public bool Interact(Interactor interactor)
    {
        if (_hasBeenPickedUp)
            return false;

        PlayerInventory inventory =
            interactor.GetComponent<PlayerInventory>();

        if (inventory == null)
        {
            Debug.LogWarning("PlayerInventory not found!");
            return false;
        }

        if (GameManager.Instance == null || tooltipCanvas == null)
        {
            Debug.LogError(
                "GameManager or Tooltip Canvas is missing!", this);
            return false;
        }

        if (!inventory.AddSyringe())
        {
            Debug.Log("Already carrying a syringe.");
            return false;
        }

        _hasBeenPickedUp = true;

        Debug.Log("Syringe pickup successful!");

        if (pickupSFX != null && musicManager != null)
        {
            musicManager.PlaySFX(pickupSFX);
        }

        GameManager.Instance.ShowInteractionTooltip(
            tooltipCanvas,
            gameObject
        );

        return true;
    }
}