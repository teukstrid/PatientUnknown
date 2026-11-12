
using UnityEngine;

public class TooltipBillboard : MonoBehaviour
{
    private Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    private void LateUpdate()
    {
        if (_mainCamera == null)
            return;

        transform.rotation = Quaternion.LookRotation(
            transform.position - _mainCamera.transform.position
        );
    }
}