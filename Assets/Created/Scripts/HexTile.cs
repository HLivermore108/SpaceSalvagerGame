using UnityEngine;

public class HexTile : MonoBehaviour
{
    [Header("Hex Coordinates")]
    public int q;
    public int r;

    [Header("Occupancy")]
    public bool isOccupied;
    public ShipUnit occupyingShip;

    [Header("Visuals")]
    [SerializeField] private Renderer tileRenderer;

    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color highlightColor = Color.cyan;
    [SerializeField] private Color selectedColor = Color.green;
    [SerializeField] private Color blockedColor = Color.red;

    [Header("Special Tile Type")]
    public TileType tileType = TileType.Normal;

    public enum TileType
    {
        Normal,
        PlayerStart1,
        PlayerStart2,
        PlayerStart3,
        PlayerStart4,
        T1,
        T2,
        T3,
        Retaliator
    }

    private MaterialPropertyBlock propertyBlock;

    private void Awake()
    {
        if (tileRenderer == null)
            tileRenderer = GetComponentInChildren<Renderer>();

        propertyBlock = new MaterialPropertyBlock();
        SetNormal();
    }

    private void OnMouseDown()
    {
        if (HexBoardManager.Instance != null)
            HexBoardManager.Instance.OnTileClicked(this);
    }

    private void SetColor(Color color)
    {
        if (tileRenderer == null) return;

        tileRenderer.GetPropertyBlock(propertyBlock);
        propertyBlock.SetColor("_Color", color);
        propertyBlock.SetColor("_BaseColor", color);
        tileRenderer.SetPropertyBlock(propertyBlock);
    }

    public void SetNormal() => SetColor(normalColor);
    public void SetHighlight() => SetColor(highlightColor);
    public void SetSelected() => SetColor(selectedColor);
    public void SetBlocked() => SetColor(blockedColor);
}