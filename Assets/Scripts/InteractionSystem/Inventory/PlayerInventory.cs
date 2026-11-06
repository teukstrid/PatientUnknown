
using UnityEngine;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    [SerializeField] private Image syringeIcon;

    public bool HasSyringe { get; private set; }

    private void Start()
    {
        UpdateUI();
    }

    public bool AddSyringe()
    {
        if (HasSyringe)
        {
            return false;
        }

        HasSyringe = true;
        UpdateUI();

        return true;
    }

    public bool UseSyringe()
    {
        if (!HasSyringe)
        {
            return false;
        }

        HasSyringe = false;
        UpdateUI();

        return true;
    }

    private void UpdateUI()
    {
        syringeIcon.gameObject.SetActive(HasSyringe);
    }
}
