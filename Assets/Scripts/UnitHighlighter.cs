using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class UnitHighlighter : MonoBehaviour
{
    // Cache the property ID instead of using strings in Update/Events
    private static readonly int OutlineAlphaID = Shader.PropertyToID("_OutlineAlpha");

    [SerializeField, ColorUsage(true, true)]
    private Color highlightColor = Color.white;

    private SpriteRenderer spriteRenderer;
    private MaterialPropertyBlock propertyBlock;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        propertyBlock = new MaterialPropertyBlock();
    }

    private void OnMouseEnter()
    {
        SetHighlight(1f); // Show outline
    }

    private void OnMouseExit()
    {
        SetHighlight(0f); // Hide outline
    }

    private void SetHighlight(float alpha)
    {
        // Get current block to preserve other properties
        spriteRenderer.GetPropertyBlock(propertyBlock);

        // Update only the alpha/visibility property
        propertyBlock.SetFloat(OutlineAlphaID, alpha);

        // Apply back to the renderer safely without duplicating the material
        spriteRenderer.SetPropertyBlock(propertyBlock);
    }
}
