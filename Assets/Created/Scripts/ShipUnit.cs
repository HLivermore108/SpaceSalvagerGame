using System.Collections;
using UnityEngine;

public class ShipUnit : MonoBehaviour
{
    [Header("Ship Info")]
    public string shipName;
    public int ownerId;

    [Header("Stats")]
    public int speed = 2;

    [Header("Position")]
    public HexTile currentTile;

    [Header("Height Above Tile")]
    public float hoverHeight = 0.5f;

    private void Start()
    {
        if (currentTile != null)
            SetTile(currentTile);
    }

    private void OnMouseDown()
    {
        if (HexBoardManager.Instance != null)
            HexBoardManager.Instance.OnShipClicked(this);
    }

    public void SetTile(HexTile tile)
    {
        if (currentTile != null)
        {
            currentTile.isOccupied = false;
            currentTile.occupyingShip = null;
        }

        currentTile = tile;

        if (currentTile != null)
        {
            currentTile.isOccupied = true;
            currentTile.occupyingShip = this;
            transform.position = GetWorldPositionForTile(currentTile);
        }
    }

    public IEnumerator MoveSmoothToTile(HexTile tile, float moveSpeed = 6f)
    {
        if (currentTile != null)
        {
            currentTile.isOccupied = false;
            currentTile.occupyingShip = null;
        }

        currentTile = tile;
        currentTile.isOccupied = true;
        currentTile.occupyingShip = this;

        Vector3 targetPos = GetWorldPositionForTile(tile);

        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }

        transform.position = targetPos;
    }

    private Vector3 GetWorldPositionForTile(HexTile tile)
    {
        return tile.transform.position + Vector3.up * hoverHeight;
    }
}