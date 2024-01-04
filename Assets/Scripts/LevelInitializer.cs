using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelInitializer : MonoBehaviour
{
    public List<Hole> LevelHoles;
    private void Awake()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "Game":
            var holeOne = new Hole(new Vector3(0, 13.62f, 0), 3);
            var holeTwo = new Hole(new Vector3(0, 13.62f, 0), 3);
            LevelHoles = new List<Hole>() {holeOne, holeTwo};
            break;
        }
    }
}
