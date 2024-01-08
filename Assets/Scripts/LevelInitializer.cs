using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelInitializer : MonoBehaviour
{
    public List<Hole> LevelHoles;
    [SerializeField] private Image topBarImage;
    [SerializeField] private Image scoreCardImage;
    [SerializeField] private Image scoreCardEnd;
    private void Awake()
    {
        switch (SceneManager.GetActiveScene().name)
        {
            case "Classicton":
                var holeOne = new Hole(new Vector3(15, 31.5f, -25), 3);
                var holeTwo = new Hole(new Vector3(65, 1.5f, 175), 4);
                var holeThree = new Hole(new Vector3(60, 31.5f, -25), 3);
                var holeFour = new Hole(new Vector3(-35, 31.5f, -25), 6);
                var holeFive = new Hole(new Vector3(115, 1.5f, 170), 3);
                var holeSix = new Hole(new Vector3(165, 1.5f, 175), 3);
                LevelHoles = new List<Hole>() { holeOne, holeTwo, holeThree, holeFour, holeFive, holeSix };
                break;
            case "Jumpington":
                SetLevelColor(new Color(0.937255f, 0.8941177f,0.2156863f, 1));
                var jumpOne = new Hole(new Vector3(0, 1.5f, 0), 2);
                var jumpTwo = new Hole(new Vector3(0, 1.5f, 125), 3);
                var jumpThree = new Hole(new Vector3(50, 1.5f, 75), 4);
                var jumpFour = new Hole(new Vector3(-50, 1.5f, 120), 2);
                var jumpFive = new Hole(new Vector3(-50, 1.5f, 75), 3);
                var jumpSix = new Hole(new Vector3(170, 1.5f, 0), 3);
                LevelHoles = new List<Hole>() { jumpOne, jumpTwo, jumpThree, jumpFour, jumpFive, jumpSix };
                break;
            case "Spinnington":
                SetLevelColor(new Color(0.5254902f, 0.7450981f, 0.2f, 1));
                var spinOne = new Hole(new Vector3(0,71.5f,0), 2);
                var spinTwo = new Hole(new Vector3(60,71.5f,0), 3);
                var spinThree = new Hole(new Vector3(50,71.5f,100), 4);
                var spinFour = new Hole(new Vector3(55,71.5f,176.5f), 2);
                var spinFive = new Hole(new Vector3(0,71.5f,282.5f), 3);
                var spinSix = new Hole(new Vector3(250,71.5f,5), 3);
                LevelHoles = new List<Hole>() { spinOne, spinTwo, spinThree, spinFour, spinFive, spinSix };
                break;
        }
    }

    private void SetLevelColor(Color color)
    {
        topBarImage.color = color;
        scoreCardImage.color = color;
        scoreCardEnd.color = color;
    }
}
