using UnityEngine;

public class SpawnCrab : MonoBehaviour
{
    public CrabGame crabGame;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        crabGame.SpawnNewCrab(transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
