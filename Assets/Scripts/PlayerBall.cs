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
    [SerializeField] private Vector2 mousePos;
 
    private void Update()
    {
        GetInput();
        // StopBall();
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
                startPosition = transform.position;
                startPosition.y = transform.position.y;
                Debug.Log(startPosition);
            }
        }
        if (isDragging && Input.GetMouseButton(0))
        {
            mousePos = new Vector2(Input.mousePosition.x, Camera.main.pixelHeight - Input.mousePosition.y);
            var currentPosition = Camera.main.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, Camera.main.nearClipPlane));
            currentPosition.y = transform.position.y;
            Debug.Log(currentPosition);
            var dragDirection = (startPosition - currentPosition).normalized;
            currentDragDirection = Vector3.Lerp(currentDragDirection, dragDirection, directionDamping * Time.deltaTime);
            var ballRotation = new Vector3(currentDragDirection.x, 0, currentDragDirection.y);
            power = Mathf.Clamp(startPosition.y - Input.mousePosition.y, 0, 200);
            var newRotation = Quaternion.FromToRotation(Vector3.forward, currentDragDirection);
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

    private void StopBall()
    {
        if (rb.velocity.magnitude < 0.5f)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}
