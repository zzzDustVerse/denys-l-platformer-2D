using UnityEngine;

public class DragObjects2D : MonoBehaviour
{
    private Camera mainCamera;
    private Transform selectedObject;
    private Rigidbody2D selectedRb;
    private Vector3 offset;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(
                Input.mousePosition
            );

            RaycastHit2D hit = Physics2D.Raycast(
                mousePosition, Vector2.zero
            );

            if (hit.collider != null &&
                hit.collider.CompareTag("Floor"))
            {
                selectedObject = hit.collider.transform;
                selectedRb = selectedObject.GetComponent<Rigidbody2D>();

                offset = selectedObject.position -
                         new Vector3(mousePosition.x, mousePosition.y,
                                     selectedObject.position.z);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            selectedObject = null;
            selectedRb = null;
        }
    }

    void FixedUpdate()
    {
        if (selectedObject != null && selectedRb != null)
        {
            Vector3 mousePosition = mainCamera.ScreenToWorldPoint(
                Input.mousePosition
            );

            Vector2 targetPosition = new Vector2(
                mousePosition.x + offset.x,
                mousePosition.y + offset.y
            );

            selectedRb.MovePosition(targetPosition);
        }
    }
}