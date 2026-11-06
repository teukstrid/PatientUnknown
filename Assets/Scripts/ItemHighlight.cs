using UnityEngine;

public class ItemHighlight : MonoBehaviour
{
    private Renderer itemRenderer;
    private Color originalColor;
    [SerializeField] private Color highlightColor = Color.yellow;
    
    void Awake()
    {
        itemRenderer = GetComponent<Renderer>();

        if (itemRenderer != null)
        {
            originalColor = itemRenderer.material.color;
        }
    }

    public void SetHighlighted(bool highlighted)
    {
        if (itemRenderer == null)
            return;

        itemRenderer.material.color =
        highlighted ? highlightColor : originalColor;
    }
}