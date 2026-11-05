using UnityEngine;

public class ItemHighlight : MonoBehaviour
{
    private static readonly int BaseColorId = Shader.PropertyToID("_BaseColor");
    private static readonly int ColorId = Shader.PropertyToID("_Color");

    private Renderer _itemRenderer;
    private MaterialPropertyBlock _propertyBlock;
    private int _colorPropertyId;

    [SerializeField] private Color highlightColor = Color.yellow;

    private void Awake()
    {
        _itemRenderer = GetComponent<Renderer>();
        if (_itemRenderer == null)
        {
            _itemRenderer = GetComponentInChildren<Renderer>();
        }

        if (_itemRenderer == null)
        {
            Debug.LogError("ItemHighlight needs a Renderer on the same object or a child object.", this);
            enabled = false;
            return;
        }

        Material material = _itemRenderer.sharedMaterial;
        if (material != null && material.HasProperty(BaseColorId))
        {
            _colorPropertyId = BaseColorId;
        }
        else if (material != null && material.HasProperty(ColorId))
        {
            _colorPropertyId = ColorId;
        }
        else
        {
            Debug.LogError("The item's material has no supported color property to highlight.", this);
            enabled = false;
            return;
        }

        _propertyBlock = new MaterialPropertyBlock();
    }

    public void SetHighlighted(bool highlighted)
    {
        if (_itemRenderer == null || !enabled)
        {
            return;
        }

        if (!highlighted)
        {
            _itemRenderer.SetPropertyBlock(null);
            return;
        }

        _itemRenderer.GetPropertyBlock(_propertyBlock);
        _propertyBlock.SetColor(_colorPropertyId, highlightColor);
        _itemRenderer.SetPropertyBlock(_propertyBlock);
    }
}