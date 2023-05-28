using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAIposition : MonoBehaviour
{
    public GameObject ball;
    public GameObject playerAI;

    // Update is called once per frame
    void Update()
    {
        Vector3 newPosition = playerAI.transform.position; // Get the current position of the PlayerAI object
        newPosition.y = ball.transform.position.y; // Set the y-coordinate of the PlayerAI object to be the same as the Ball's y-coordinate
        playerAI.transform.position = newPosition; // Update the PlayerAI object's position to the new position
    }
}
