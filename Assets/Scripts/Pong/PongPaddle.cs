using UnityEngine;

public class PongPaddle : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 startPos;

    void OnMouseDown()
    {
        // Store the starting position and flag the object as being dragged.
        isDragging = true;
        startPos = transform.position;
    }

    void OnMouseUp()
    {
        // Reset the drag flag when the mouse button is released.
        isDragging = false;
    }

    void Update()
    {
        // If the object is being dragged, update its position based on the mouse input.
        if (isDragging)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 newPos = new Vector3(startPos.x, mousePos.y, startPos.z); // Constrain to y-axis only
            transform.position = newPos;
        }
    }
}