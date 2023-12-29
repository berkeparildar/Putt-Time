using Cinemachine;
using UnityEngine;

public class CameraRotate : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera virtualCamera;

    [SerializeField]
    private bool isOnPlayer;

    [SerializeField]
    private CinemachineOrbitalTransposer orbitalTransposer;

    private void Start()
    {
        orbitalTransposer = virtualCamera.GetCinemachineComponent<CinemachineOrbitalTransposer>();
    }

    void Update()
    {
        RotateWithInput();
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
        }
        if (Input.GetMouseButton(0) && !isOnPlayer)
        {
            orbitalTransposer.m_XAxis.m_InputAxisName = "Mouse X";
        }
        else
        {
            orbitalTransposer.m_XAxis.m_InputAxisName = null;
        }
        if (isOnPlayer && Input.GetMouseButtonUp(0))
        {
            isOnPlayer = false;
        }
    }
}
