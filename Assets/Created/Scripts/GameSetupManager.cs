using System.Collections.Generic;
using UnityEngine;

public class GameSetupManager : MonoBehaviour
{
    public static GameSetupManager Instance;

    [Header("Game Setup")]
    public int playerCount = 2;

    [Header("Chosen Ships")]
    public List<ShipType> selectedShips = new List<ShipType>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetPlayerCount(int count)
    {
        playerCount = Mathf.Clamp(count, 2, 4);
        selectedShips.Clear();
    }

    public void AddShipSelection(ShipType shipType)
    {
        if (selectedShips.Count >= playerCount)
            return;

        selectedShips.Add(shipType);
    }

    public bool AllPlayersSelected()
    {
        return selectedShips.Count >= playerCount;
    }
}