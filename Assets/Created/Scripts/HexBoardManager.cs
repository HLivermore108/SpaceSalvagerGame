using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HexBoardManager : MonoBehaviour
{
    public static HexBoardManager Instance;

    [Header("Grid Settings")]
    public GameObject hexTilePrefab;
    public int width = 14;
    public int height = 18;

    [Tooltip("Horizontal spacing between hex centers.")]
    public float hexWidth = 1.0f;

    [Tooltip("Forward spacing between rows.")]
    public float hexHeight = 0.866f;

    public Transform gridParent;

    [Header("Board Placement")]
    public Vector3 boardOrigin = Vector3.zero;

    [Header("UI")]
    public Button confirmMoveButton;
    public Button cancelMoveButton;

    [Header("Turn")]
    public int currentPlayerId = 0;

    [Header("Generated Tiles")]
    public List<HexTile> allTiles = new List<HexTile>();

    private readonly Dictionary<Vector2Int, HexTile> tileDictionary = new Dictionary<Vector2Int, HexTile>();

    private ShipUnit selectedShip;
    private HexTile selectedDestination;
    private readonly List<HexTile> highlightedTiles = new List<HexTile>();

    private void Awake()
    {
        Instance = this;
        GenerateGrid();
    }

    private void Start()
    {
        if (confirmMoveButton != null)
        {
            confirmMoveButton.onClick.AddListener(ConfirmMove);
            confirmMoveButton.interactable = false;
        }

        if (cancelMoveButton != null)
        {
            cancelMoveButton.onClick.AddListener(CancelSelection);
            cancelMoveButton.interactable = false;
        }
    }

    public void GenerateGrid()
    {
        allTiles.Clear();
        tileDictionary.Clear();

        for (int r = 0; r < height; r++)
        {
            for (int q = 0; q < width; q++)
            {
                float x = q * hexWidth + (r % 2 == 1 ? hexWidth * 0.5f : 0f);
                float z = r * hexHeight;

                Vector3 spawnPos = boardOrigin + new Vector3(x, 0f, z);

                GameObject tileObj = Instantiate(hexTilePrefab, spawnPos, Quaternion.identity, gridParent);
                tileObj.name = $"Hex ({q}, {r})";

                HexTile tile = tileObj.GetComponent<HexTile>();
                if (tile == null)
                {
                    Debug.LogError("Hex tile prefab is missing HexTile script.");
                    continue;
                }

                tile.q = q;
                tile.r = r;

                SetSpecialTileType(tile);

                allTiles.Add(tile);
                tileDictionary[new Vector2Int(q, r)] = tile;
            }
        }

        Debug.Log($"Generated {allTiles.Count} hex tiles.");
    }

    private void SetSpecialTileType(HexTile tile)
    {
        if (tile.q == 0 && tile.r == 1)
            tile.tileType = HexTile.TileType.PlayerStart1;
        else if (tile.q == width - 2 && tile.r == 1)
            tile.tileType = HexTile.TileType.PlayerStart2;
        else if (tile.q == 0 && tile.r == height - 2)
            tile.tileType = HexTile.TileType.PlayerStart3;
        else if (tile.q == width - 2 && tile.r == height - 2)
            tile.tileType = HexTile.TileType.PlayerStart4;
        else
            tile.tileType = HexTile.TileType.Normal;
    }

    public void OnShipClicked(ShipUnit ship)
    {
        if (ship == null) return;
        if (ship.ownerId != currentPlayerId) return;

        selectedShip = ship;
        selectedDestination = null;

        ClearHighlights();

        List<HexTile> tilesInRange = GetTilesInRange(ship.currentTile, ship.speed);
        highlightedTiles.AddRange(tilesInRange);

        foreach (HexTile tile in highlightedTiles)
        {
            if (!tile.isOccupied || tile == ship.currentTile)
                tile.SetHighlight();
            else
                tile.SetBlocked();
        }

        ship.currentTile.SetSelected();

        if (cancelMoveButton != null)
            cancelMoveButton.interactable = true;

        if (confirmMoveButton != null)
            confirmMoveButton.interactable = false;
    }

    public void OnTileClicked(HexTile tile)
    {
        if (selectedShip == null || tile == null) return;
        if (!highlightedTiles.Contains(tile)) return;
        if (tile.isOccupied && tile != selectedShip.currentTile) return;

        selectedDestination = tile;

        foreach (HexTile t in highlightedTiles)
        {
            if (!t.isOccupied || t == selectedShip.currentTile)
                t.SetHighlight();
            else
                t.SetBlocked();
        }

        selectedShip.currentTile.SetSelected();
        selectedDestination.SetSelected();

        if (confirmMoveButton != null)
            confirmMoveButton.interactable = selectedDestination != selectedShip.currentTile;
    }

    public void ConfirmMove()
    {
        if (selectedShip == null || selectedDestination == null) return;
        if (selectedDestination == selectedShip.currentTile) return;

        StartCoroutine(ConfirmMoveRoutine());
    }

    private IEnumerator ConfirmMoveRoutine()
    {
        yield return StartCoroutine(selectedShip.MoveSmoothToTile(selectedDestination));
        ClearSelection();
    }

    public void CancelSelection()
    {
        ClearSelection();
    }

    private void ClearSelection()
    {
        selectedShip = null;
        selectedDestination = null;

        ClearHighlights();

        if (confirmMoveButton != null)
            confirmMoveButton.interactable = false;

        if (cancelMoveButton != null)
            cancelMoveButton.interactable = false;
    }

    private void ClearHighlights()
    {
        foreach (HexTile tile in allTiles)
        {
            if (tile != null)
                tile.SetNormal();
        }

        highlightedTiles.Clear();
    }

    public List<HexTile> GetTilesInRange(HexTile center, int range)
    {
        List<HexTile> results = new List<HexTile>();

        foreach (HexTile tile in allTiles)
        {
            int dist = GetHexDistance(center, tile);
            if (dist <= range)
                results.Add(tile);
        }

        return results;
    }

    public int GetHexDistance(HexTile a, HexTile b)
    {
        Vector3Int ac = OffsetToCube(a.q, a.r);
        Vector3Int bc = OffsetToCube(b.q, b.r);

        return Mathf.Max(
            Mathf.Abs(ac.x - bc.x),
            Mathf.Abs(ac.y - bc.y),
            Mathf.Abs(ac.z - bc.z)
        );
    }

    private Vector3Int OffsetToCube(int q, int r)
    {
        int x = q - (r - (r & 1)) / 2;
        int z = r;
        int y = -x - z;
        return new Vector3Int(x, y, z);
    }

    public HexTile GetTileAt(int q, int r)
    {
        Vector2Int key = new Vector2Int(q, r);
        return tileDictionary.TryGetValue(key, out HexTile tile) ? tile : null;
    }
}