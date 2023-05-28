using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

public class Player1position : MonoBehaviour
{
    // Import the Windows API function for getting the cursor position
    [DllImport("user32.dll")]
    static extern bool GetCursorPos(out POINT lpPoint);

    // Define the POINT structure used by the Windows API function
    struct POINT
    {
        public int x;
        public int y;
    }

    void Update()
    {
        // Declare variables for storing the cursor position and the position of this game object
        POINT cursorPosition;
        Vector3 gameObjectPosition = transform.position;

        // Call the Windows API function to get the cursor position
        if (GetCursorPos(out cursorPosition))
        {
            // Map the cursor position to the y position of this game object
            gameObjectPosition.y = (float)Screen.height - cursorPosition.y;

            // Set the position of this game object
            transform.position = gameObjectPosition;
        }
    }
}