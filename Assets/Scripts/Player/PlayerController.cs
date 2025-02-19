using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PlayerDataBank PDB = new();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PDB.level = 1;
        PDB.Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
