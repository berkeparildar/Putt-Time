using System;
using Photon.Pun;
using Photon.Realtime;
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
    [SerializeField] private Vector3 lastSavedPosition;
    [SerializeField] private LayerMask playerLayer;

    public static Action UpdateStrokeUI;
    private Plane inputPlane;
    private Vector3 currentDragDirection;
    private Vector3 mousePos;
    private PhotonView _photonView;
    [SerializeField] private bool isMoving;

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
            playerLayer = LayerMask.GetMask("Player");
            float maxRaycastDistance = 100f;
            if (Physics.Raycast(ray, out var hit, maxRaycastDistance, playerLayer) && hit.transform.CompareTag("Player"))
            {
                Debug.Log(hit.transform.gameObject.name);
                if (hit.transform.GetComponent<PhotonView>().IsMine)
                {
                    Debug.Log("Young fly ");
                    directionModifier.enabled = true;
                    isDragging = true;
                    startPosition = transform.position;
                }
            }
        }
        if (_photonView.IsMine && isDragging && Input.GetMouseButton(0))
        {
            inputPlane.SetNormalAndPosition(Vector3.up, transform.position);
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (inputPlane.Raycast(ray, out var hit))
            {
                mousePos = ray.GetPoint(hit);
                currentDragDirection = (startPosition - mousePos).normalized;
            }
            var ballRotation = new Vector3(currentDragDirection.x, 0, currentDragDirection.z);
            var distance = Vector3.Distance(startPosition, mousePos);
            power = Mathf.Clamp(distance * 2, 0, 5);
            var newRotation = Quaternion.LookRotation(ballRotation, Vector3.up);
            transform.localRotation = newRotation;
        }
        if (_photonView.IsMine && isDragging && Input.GetMouseButtonUp(0))
        {
            lastSavedPosition = startPosition;
            directionModifier.DisableAll();
            directionModifier.enabled = false;
            if (power > 1)
            {
                rb.AddForce(direction.transform.forward * (power * 10), ForceMode.Impulse);
                player.IncreaseStroke();
                UpdateStrokeUI?.Invoke();
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
