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
        for (int i = 0; i < gridSize; ++i)
        {
            for (int j = 0; j < gridSize; ++j)
            {
                GameObject newTile = Instantiate(tile);
                float adjustment = j % 2 == 0 ? 0.0f : sqrt3 * size * 0.5f;
                newTile.transform.position = new Vector3(i * sqrt3 * size + adjustment, 0, j * 0.75f);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
