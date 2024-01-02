using Photon.Pun;
using UnityEngine;

public class PlayerBall : MonoBehaviour
{

    [SerializeField] private float power;
    [SerializeField] private Player player;
    [SerializeField] private bool isDragging;
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private GameObject direction;
    [SerializeField] private Rigidbody rb;
    [SerializeField] private DirectionModifier directionModifier;
    [SerializeField] private float directionDamping = 0.8f;
    [SerializeField] private Vector3 lastSavedPosition;
    [SerializeField] private LayerMask groundLayer;
    private Plane inputPlane;
    private Vector3 currentDragDirection;
    private Vector3 mousePos;
    private PhotonView _photonView;
    private bool isMoving;

    private void Start()
    {
        _photonView = transform.parent.GetComponent<PhotonView>();
    }

    private void Update()
    {
        if (!isMoving)
        {
            GetInput();
        }
        CheckVelocity();
    }

    public float GetPower()
    {
        return power;
    }

    private void GetInput()
    {
        if (_photonView.IsMine && Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit) && hit.transform.CompareTag("Player"))
            {
                directionModifier.enabled = true;
                isDragging = true;
                startPosition = transform.position;
            }
        }
        if (_photonView.IsMine && isDragging && Input.GetMouseButton(0))
        {
            // Plane plane = new Plane(Vector3.up, transform.position);
            inputPlane.SetNormalAndPosition(Vector3.up, transform.position);
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (inputPlane.Raycast(ray, out var hit))
            {
                mousePos = ray.GetPoint(hit);
                currentDragDirection = (startPosition - mousePos).normalized;
            }
            // if (Physics.Raycast(ray, out var hit, groundLayer))
            // {
            //     mousePos = hit.point;
            //     mousePos.y = transform.position.y;
            // }
            // var dragDirection = (startPosition - mousePos).normalized;
            // currentDragDirection = Vector3.Lerp(currentDragDirection, dragDirection, 2 * Time.deltaTime);
            var ballRotation = new Vector3(currentDragDirection.x, 0, currentDragDirection.z);
            power = Mathf.Clamp(Vector3.Distance(startPosition, mousePos), 0, 5);
            var newRotation = Quaternion.LookRotation(ballRotation, Vector3.up);
            transform.localRotation = newRotation;
        }
        if (_photonView.IsMine && isDragging && Input.GetMouseButtonUp(0))
        {
            lastSavedPosition = startPosition;
            directionModifier.enabled = false;
            if (power > 1)
            {
                rb.AddForce(direction.transform.forward * (power * 3), ForceMode.Impulse);
                player.IncreaseStroke();
            }
            isDragging = false;
        }
    }

    public Vector3 GetLastSavedPosition()
    {
        return lastSavedPosition;
    }

    public void SetLastSavedPosition(Vector3 newHolePosition)
    {
        lastSavedPosition = newHolePosition;
    }

    private void CheckVelocity()
    {
        if (rb.velocity.magnitude <= 0)
        {
            isMoving = false;
        }
        else
        {
            isMoving = true;
        }
    }
}
