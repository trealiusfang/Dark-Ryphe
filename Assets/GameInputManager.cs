using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameInputManager : MonoBehaviour
{
    Camera _camera;
    [SerializeField] private LayerMask clickableLayer;
    private void Awake()
    {
        _camera = Camera.main;
    }
    public void Initialize(Camera camera)
    {
        _camera = camera;
        GetComponent<PlayerInput>().actions.Enable();
        GetComponent<PlayerInput>().camera = camera;
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (IsPointerOverUI())
            return;

        Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, 100000))
        {
            var clickable = hit.collider.GetComponent<IClickable>();
            Debug.Log(hit.transform.name);
            if (clickable != null)
            {
                clickable.OnClick();
            }
        }
    }

    public void OnInspect(InputAction.CallbackContext context)
    {
        if (!context.performed) return;

        if (IsPointerOverUI())
            return;

        Ray ray = _camera.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (Physics.Raycast(ray, out RaycastHit hit, 100000))
        {
            var clickable = hit.collider.GetComponent<IInspectable>();
            Debug.Log(hit.transform.name);

            if (clickable != null)
            {
                clickable.OnInspect();
            }
        }
    }

    public static bool IsPointerOverUI()
    {
        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Mouse.current.position.ReadValue();

        List<RaycastResult> results = new List<RaycastResult>();
        if (EventSystem.current == null) { Debug.Log("No event system?"); return false; }
        
        EventSystem.current.RaycastAll(pointerData, results);
        if (results.Count > 0) Debug.Log("On UI MF");
        return results.Count > 0;
    }
}
public interface IClickable
{
    void OnClick();
}

public interface IInspectable
{
    void OnInspect();
}
