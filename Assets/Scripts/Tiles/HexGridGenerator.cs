using System;
using UnityEngine;

public class HexGridGenerator : MonoBehaviour
{
    [SerializeField] private int gridSize = 10;
    public GameObject tile;
    private float sqrt3 = Mathf.Sqrt(3);


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        float size = tile.GetComponent<Renderer>().bounds.size.z * 0.5f;
        Debug.Log(size);
        // Print(tile.transform.bounds.size.x);
        /*for (int i = 0; i < gridSize; ++i)
        {
            for (int j = 0; j < gridSize; ++j)
            {
                GameObject newTile = Instantiate(tile);
                float adjustment = j % 2 == 0 ? 0.0f : sqrt3 * size * 0.5f;
                newTile.transform.position = new Vector3(i * sqrt3 * size + adjustment, 0, j * 0.75f);
            }
        }*/
        Vector2Int startPos = new Vector2Int(0, 0);
        SpawnInRings(3, startPos.x, startPos.y);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnInRings(int range, int startX, int startY)
    {
        float size = tile.GetComponent<Renderer>().bounds.size.z; // Point to point height
        float hexWidth = sqrt3 * size * 0.5f;
        float hexHeight = size * 0.75f;

        // Convert coordinates to world space
        float worldStartX = startX * hexWidth;
        float worldStartZ = startY * hexHeight;
        if (startY % 2 != 0) worldStartX += hexWidth * 0.5f; // Offset for odd rows

        Vector3 startPosition = new Vector3(worldStartX, 0, worldStartZ); // World position (Incase we need to use it from player idk)

        for (int yPos = -range; yPos <= range; yPos++)
        {
            int xMod = yPos % 2 == 0 ? 0 : 1;
        
            for (int xPos = -(range - Math.Abs(yPos / 2)); xPos <= range - Math.Abs(yPos / 2) - xMod; xPos++)
            {
                // Spawn  as a child of spawner to clean shit up
                GameObject newTile = Instantiate(tile, transform);
            
                float adjustment = yPos % 2 == 0 ? 0.0f : hexWidth * 0.5f;
            
                Vector3 localPosition = new Vector3(xPos * hexWidth + adjustment, 0, yPos * hexHeight);
            
                // Offset ffs unity
                newTile.transform.position = startPosition + localPosition;
                
                //Need to fix this to show real coords. maybe. who knows
                newTile.gameObject.name = $"Tile: {xPos + startX}, {yPos + startY}";
            }
        }
    }

}
