using UnityEngine;

public class PlayerBall : MonoBehaviour
{
    [SerializeField] private float power;
    [SerializeField] private bool isDragging;
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private GameObject direction;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private DirectionModifier directionModifier;
    [SerializeField] private float directionDamping = 0.8f;
    [SerializeField] private Vector3 currentDragDirection;

    private void Update()
    {
        GetInput();
    }

    public float GetPower()
    {
        return power;
    }

    private void GetInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit) && hit.transform.CompareTag("Player"))
            {
                directionModifier.enabled = true;
                isDragging = true;
                startPosition = Input.mousePosition;
            }
        }
        if (isDragging && Input.GetMouseButton(0))
        {
            power = Mathf.Clamp(startPosition.y - Input.mousePosition.y, 0, 200);
            var dragDirection = (startPosition - Input.mousePosition).normalized;
            currentDragDirection = Vector3.Lerp(currentDragDirection, dragDirection, directionDamping * Time.deltaTime);
            var ballRotation = new Vector3(currentDragDirection.x, 0, currentDragDirection.y);
            power = Mathf.Clamp(startPosition.y - Input.mousePosition.y, 0, 200);
            var newRotation = Quaternion.LookRotation(ballRotation, Vector3.up);
            transform.localRotation = newRotation; 
        }
        if (isDragging && Input.GetMouseButtonUp(0))
        {
            directionModifier.enabled = false;
            Debug.Log(power / 10);
            rb.AddForce(direction.transform.forward * (power / 10), ForceMode.Impulse);
            isDragging = false;
        }
    }
}
