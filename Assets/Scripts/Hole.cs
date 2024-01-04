using UnityEngine;

public class Hole
{
    private readonly Vector3 _spawnPosition;
    private readonly int _par;

    public Hole(Vector3 spawnPos, int par)
    {
        _spawnPosition = spawnPos;
        _par = par;
    }

    public Vector3 GetSpawnPosition()
    {
        return _spawnPosition;
    }

    public int GetParCount()
    {
        return _par;
    }
}
