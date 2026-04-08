using UnityEngine;

public class ShipSpawner : MonoBehaviour
{
    [System.Serializable]
    public class ShipPrefabEntry
    {
        public ShipType shipType;
        public GameObject prefab;
    }

    [Header("Ship Prefabs")]
    public ShipPrefabEntry[] shipPrefabs;

    private void Start()
    {
        SpawnSelectedShips();
    }

    private void SpawnSelectedShips()
    {
        if (HexBoardManager.Instance == null)
        {
            Debug.LogError("HexBoardManager not found.");
            return;
        }

        if (GameSetupManager.Instance == null)
        {
            Debug.LogError("GameSetupManager not found.");
            return;
        }

        int playerCount = GameSetupManager.Instance.playerCount;

        for (int i = 0; i < playerCount; i++)
        {
            if (i >= GameSetupManager.Instance.selectedShips.Count)
            {
                Debug.LogWarning($"No ship selected for player {i + 1}");
                continue;
            }

            ShipType selectedType = GameSetupManager.Instance.selectedShips[i];
            GameObject prefab = GetPrefabForType(selectedType);

            if (prefab == null)
            {
                Debug.LogError($"No prefab assigned for ship type {selectedType}");
                continue;
            }

            HexTile spawnTile = GetSpawnTileForPlayer(i);

            if (spawnTile == null)
            {
                Debug.LogError($"No spawn tile found for player {i + 1}");
                continue;
            }

            GameObject shipObj = Instantiate(prefab);
            ShipUnit shipUnit = shipObj.GetComponent<ShipUnit>();

            if (shipUnit == null)
            {
                Debug.LogError($"Prefab {prefab.name} is missing ShipUnit.");
                continue;
            }

            shipUnit.ownerId = i;
            shipUnit.SetTile(spawnTile);
        }
    }

    private GameObject GetPrefabForType(ShipType type)
    {
        foreach (var entry in shipPrefabs)
        {
            if (entry.shipType == type)
                return entry.prefab;
        }

        return null;
    }

    private HexTile GetSpawnTileForPlayer(int playerIndex)
    {
        switch (playerIndex)
        {
            case 0:
                return HexBoardManager.Instance.GetTileAt(0, 1);
            case 1:
                return HexBoardManager.Instance.GetTileAt(HexBoardManager.Instance.width - 2, 1);
            case 2:
                return HexBoardManager.Instance.GetTileAt(0, HexBoardManager.Instance.height - 2);
            case 3:
                return HexBoardManager.Instance.GetTileAt(HexBoardManager.Instance.width - 2, HexBoardManager.Instance.height - 2);
            default:
                return null;
        }
    }
}