using Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    [SerializeField] private bool isOnPlayer;
    [SerializeField] private CinemachineOrbitalTransposer orbitalTransposer;
    [SerializeField] private float yValue;
    [SerializeField] private float zValue;
    [SerializeField] private Vector3 startPosition;

    private void Start()
    {
        orbitalTransposer = virtualCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
    }

    private void GetCameraValues()
    {
        yValue = orbitalTransposer.m_FollowOffset.y;
        zValue = orbitalTransposer.m_FollowOffset.z;
    }

    private void Update()
    {
        GetCameraValues();
        RotateWithInput();
        ZoomWithInput();
    }

    private void RotateWithInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit) && hit.transform.CompareTag("Player"))
            {
                isOnPlayer = true;
            }
            else
            {
                startPosition = Input.mousePosition;
            }
        }
        if (Input.GetMouseButton(0) && !isOnPlayer)
        {
            orbitalTransposer.m_XAxis.m_InputAxisName = "Mouse X";
            if (Input.mousePosition.y < startPosition.y) // 350 355
            {
                var incrementValue = (Input.mousePosition.y - startPosition.y) * 0.05f;
                if (!(yValue - incrementValue > 30))
                {
                    orbitalTransposer.m_FollowOffset.y -= incrementValue; // -0.05
                } 
                startPosition = Input.mousePosition;

            }
            else if (Input.mousePosition.y > startPosition.y) // 355 350
            {
                var decrementValue = (Input.mousePosition.y - startPosition.y) * 0.05f;
                if (!(yValue - decrementValue < 10))
                {
                    orbitalTransposer.m_FollowOffset.y -= decrementValue; // -0.05
                } 
                startPosition = Input.mousePosition;
            }
        }
        else
        {
            orbitalTransposer.m_XAxis.m_InputAxisName = null;
            orbitalTransposer.m_XAxis.m_InputAxisValue = 0;
        }

        if (isOnPlayer && Input.GetMouseButtonUp(0))
        {
            isOnPlayer = false;
        }
    }

    private void ZoomWithInput()
    {
        if (Input.mouseScrollDelta.y > 0 && yValue >= 12)
        {
            orbitalTransposer.m_FollowOffset.y -= 2;
            orbitalTransposer.m_FollowOffset.z += 2;
        }
        else if (Input.mouseScrollDelta.y < 0 && yValue <= 28)
        {
            orbitalTransposer.m_FollowOffset.y += 2;
            orbitalTransposer.m_FollowOffset.z -= 2;
        }
    }
}
