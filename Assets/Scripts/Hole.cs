using UnityEngine;

public class Hole
{
    private Vector3 spawnPosition;
    private int par;

    public Hole(Vector3 spawnPos, int par)
    {
        spawnPosition = spawnPos;
        this.par = par;
    }

    public Vector3 GetSpawnPosition()
    {
        return spawnPosition;
    }

    public int GetParCount()
    {
        return par;
    }
}
