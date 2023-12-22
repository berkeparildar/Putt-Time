using UnityEngine;

public class DirectionModifier : MonoBehaviour
{

    [SerializeField] private GameObject rotatingArm;
    [SerializeField] private GameObject directionObject;
    [SerializeField] private Vector3 currentDirection;
    [SerializeField] private int rotationMagnitude;
    [SerializeField] private int rotationSpeed;
    [SerializeField] private float currentPower;
    [SerializeField] private PlayerBall playerBall;
    [SerializeField] private bool rotatingLeft;

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
        MoveDirection();
    }

    private void SetValuesAccordingToPower()
    {
    }

    private void MoveDirection()
    {
        float rotationAngle = Mathf.Sin(Time.time * rotationSpeed) * 45f;
        rotatingArm.transform.localRotation = Quaternion.Euler(0f, rotationAngle, 0);
    }
}
