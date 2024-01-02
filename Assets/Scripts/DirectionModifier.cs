using UnityEngine;

public class DirectionModifier : MonoBehaviour
{

    [SerializeField] private GameObject rotatingArm;
    [SerializeField] private GameObject directionObject;
    [SerializeField] private Vector3 currentDirection;
    [SerializeField] private float rotationMagnitude;
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float currentPower;
    [SerializeField] private PlayerBall playerBall;
    [SerializeField] private float previousRotationAngle;
    [SerializeField] private float currentMagnitude;
    [SerializeField] private float rotationAngle;
    [SerializeField] private bool rotatingRight;
    [SerializeField] private Vector3 rotationVector;
    [SerializeField] private GameObject[] directionArrows;

    private void Start()
    {
       directionArrows = new GameObject[rotatingArm.transform.childCount];
       for (int i = 0; i < directionArrows.Length; i++)
       {
           directionArrows[i] = rotatingArm.transform.GetChild(i).gameObject;
       }
    }

    private void OnEnable()
    {
        rotatingArm.SetActive(true);
    }

    private void OnDisable()
    {
        rotatingArm.SetActive(false);
    }

    private void Update()
    {
        currentPower = playerBall.GetPower();
        SetValuesAccordingToPower();
        RotateDirectionIndicator();
        UpdatePowerVisualizer();
    }

    private void SetValuesAccordingToPower()
    {
        rotationMagnitude = currentPower * 4;
    }

    private void UpdatePowerVisualizer()
    {
        for (int i = 1; i <= directionArrows.Length; i++)
        {
            if (currentPower >= i)
            {
                directionArrows[i - 1].SetActive(true);
            }
            else
            {
                directionArrows[i - 1].SetActive(false);
            }
        }
    }

    private void RotateDirectionIndicator()
    {
        currentMagnitude = Mathf.Lerp(currentMagnitude, rotationMagnitude, 0.1f);
        float angle = Mathf.Sin(Time.time * rotationSpeed) * currentMagnitude;
        rotationVector = new Vector3(0, angle, 0);
        rotatingArm.transform.localRotation = Quaternion.Euler(rotationVector);
    }
}
