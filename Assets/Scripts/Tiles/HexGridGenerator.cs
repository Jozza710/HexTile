using System;
using System.Collections.Generic;
using UnityEngine;

public enum TileType
{
    Void,
    Island,
    Resource
}

[Serializable]
public class TileProperties
{
    public TileType type;
    [Range(0f, 1f)]
    public float frequency;
    public Color tileColour;
}

public class HexGridGenerator : MonoBehaviour
{
    [SerializeField] private int radius = 3;
    public GameObject tilePrefab;
    public int seed = 1;
    public float noiseScale = 0.1f;
    public Vector2Int startPosition;
    public bool connectIsland = true;

    public TileProperties[] tileProperties;

    public Color borderColour = Color.red;

    private readonly float sqrt3 = Mathf.Sqrt(3);

    // TODO Change so that this stores a script with reference to the tile information instead of the GameObject itself
    private Dictionary<Vector2Int, GameObject> tileDictionary = new();
    private Dictionary<Vector2Int, TileType> tileTypes = new();

    [ContextMenu("Generate")]
    void Start()
    {
        foreach (GameObject tile in tileDictionary.Values)
            Destroy(tile);

        tileDictionary.Clear();
        tileTypes.Clear();

        SpawnInRings(radius, startPosition.x, startPosition.y);
        if (connectIsland)
            ConnectIsland();
        MarkSurroundingArea();

        // Debug Code
        foreach (KeyValuePair<Vector2Int, TileType> tileType in tileTypes)
        {
            GameObject tile = tileDictionary[tileType.Key];
            tile.name += $" {tileType.Value}";
        }
    }

    public void SpawnInRings(int range, int startX, int startY)
    {
        float size = tilePrefab.GetComponent<Renderer>().bounds.size.z; // Point to point height
        float hexWidth = sqrt3 * size * 0.5f;
        float hexHeight = size * 0.75f;

        // Convert coordinates to world space
        float worldStartX = startX * hexWidth;
        float worldStartZ = startY * hexHeight;
        if (startY % 2 != 0) worldStartX += hexWidth * 0.5f; // Offset for odd rows

        Vector3 startPosition = new(worldStartX, 0, worldStartZ); // World position (Incase we need to use it from player idk)

        // Loop over rows (y-axis) relative to the center.
        for (int yPos = -range; yPos <= range; yPos++)
        {
            int yAbsHalf = Math.Abs(yPos) / 2;
            int xMod = yPos % 2 == 0 ? 0 : 1;
            int rangeMinusYAbsHalf = range - yAbsHalf;

            for (int xPos = -rangeMinusYAbsHalf; xPos <= (rangeMinusYAbsHalf - xMod); xPos++)
            {
                // Spawn  as a child of spawner to clean shit up
                GameObject newTile = Instantiate(tilePrefab, transform);

                float adjustment = yPos % 2 == 0 ? 0.0f : hexWidth * 0.5f;

                Vector3 localPosition = new(xPos * hexWidth + adjustment, 0, yPos * hexHeight);

                // Offset ffs unity
                newTile.transform.position = startPosition + localPosition;

                Vector2Int offsetCoordinates = new(xPos + startX, yPos + startY);
                newTile.name = $"Tile: {offsetCoordinates.x}, {offsetCoordinates.y}";

                TileType type = (offsetCoordinates == Vector2Int.zero)
                                ? TileType.Island
                                : DetermineTileType(offsetCoordinates);
                tileTypes[offsetCoordinates] = type;

                // Should cache renderer
                Color tileColour = GetColourForTileType(type);
                newTile.GetComponent<Renderer>().material.color = tileColour;

                tileDictionary[offsetCoordinates] = newTile;
            }
        }
    }

    private TileType DetermineTileType(Vector2Int coord)
    {
        float noiseX = (coord.x + seed) * noiseScale;
        float noiseY = (coord.y + seed) * noiseScale;
        float noiseValue = Mathf.PerlinNoise(noiseX, noiseY);

        float total = 0f;
        foreach (TileProperties tileProperty in tileProperties)
        {
            total += tileProperty.frequency;
            if (noiseValue < total)
                return tileProperty.type;
        }
        return tileProperties[^1].type;
    }

    private Color GetColourForTileType(TileType type)
    {
        foreach (TileProperties tileProperties in tileProperties)
        {
            if (tileProperties.type == type)
                return tileProperties.tileColour;
        }
        return Color.white;
    }

    private void ConnectIsland()
    {
        Vector2Int center = Vector2Int.zero;
        if (!tileTypes.ContainsKey(center))
            return;

        // Force the center tile to be island
        tileTypes[center] = TileType.Island;

        HashSet<Vector2Int> connected = new();
        Queue<Vector2Int> queue = new();
        queue.Enqueue(center);
        connected.Add(center);

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();
            foreach (Vector2Int neighbor in GetNeighborCoordinates(current))
            {
                if (tileTypes.ContainsKey(neighbor) && tileTypes[neighbor] == TileType.Island && !connected.Contains(neighbor))
                {
                    connected.Add(neighbor);
                    queue.Enqueue(neighbor);
                }
            }
        }

        // Change any tile that isn't connected to type void
        // TEST to see if this creates a bug where the player could spawn on a void tile
        foreach (var kvp in new Dictionary<Vector2Int, TileType>(tileTypes))
        {
            if (kvp.Value == TileType.Island && !connected.Contains(kvp.Key))
            {
                tileTypes[kvp.Key] = TileType.Void;
                if (tileDictionary.ContainsKey(kvp.Key))
                {
                    tileDictionary[kvp.Key].GetComponent<Renderer>().material.color = GetColourForTileType(TileType.Void);
                }
            }
        }
    }

    private void MarkSurroundingArea()
    {
        foreach (var kvp in tileDictionary)
        {
            Vector2Int coordinate = kvp.Key;
            GameObject tile = kvp.Value;
            if (tileTypes[coordinate] == TileType.Void)
                continue;

            foreach (Vector2Int neighbor in GetNeighborCoordinates(coordinate))
            {
                if (tileTypes.ContainsKey(neighbor) && tileTypes[neighbor] == TileType.Void)
                {
                    tile.GetComponent<Renderer>().material.color = borderColour;
                    break;
                }
            }
        }
    }

    private Vector2Int[] GetNeighborCoordinates(Vector2Int coordinate)
    {
        Vector2Int[] evenOffsets = new Vector2Int[]
        {
            new(1, 0),
            new(0, -1),
            new(-1, -1),
            new(-1, 0),
            new(-1, 1),
            new(0, 1)
        };

        Vector2Int[] oddOffsets = new Vector2Int[]
        {
            new(1, 0),
            new(1, -1),
            new(0, -1),
            new(-1, 0),
            new(0, 1),
            new(1, 1)
        };

        bool startEven = startPosition.y % 2 == 0;
        bool coordinateEven = coordinate.y % 2 == 0;
        Vector2Int[] offsets = (startEven == coordinateEven) ? evenOffsets : oddOffsets;
        Vector2Int[] neighbors = new Vector2Int[6];
        for (int neighbourIndex = 0; neighbourIndex < 6; ++neighbourIndex)
        {
            neighbors[neighbourIndex] = coordinate + offsets[neighbourIndex];
        }
        return neighbors;
    }
}
