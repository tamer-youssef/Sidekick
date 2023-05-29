using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;
using System;
using System.Runtime.InteropServices;

public class CursorController : MonoBehaviour
{
    // Struct for mouse input
    [StructLayout(LayoutKind.Sequential)]
    public struct MOUSEINPUT
    {
        public int dx;
        public int dy;
        public uint mouseData;
        public uint dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    // Struct for input
    [StructLayout(LayoutKind.Explicit)]
    public struct INPUT
    {
        [FieldOffset(0)]
        public int type;
        [FieldOffset(4)]
        public MOUSEINPUT mi;
    }

    [DllImport("user32.dll")]
    static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, int dwExtraInfo);
    [DllImport("user32.dll")]
    static extern bool SetCursorPos(int x, int y);

    private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
    private const uint MOUSEEVENTF_LEFTUP = 0x0004;

    public KinectSensor kinectSensor;
    public float cursorSpeed = 1000f;
    public float clickThreshold = 0.3f; // Duration threshhold to detect a closed fist

    // Constants for mouse input type and flags
    private const int INPUT_MOUSE = 0;

    private Body[] bodies;
    private BodyFrameReader bodyFrameReader = null;
    private bool isHandClosed = false;
    private float handCloseTime = 0f;

    void Start()
    {
        if (this.kinectSensor == null)
        {
            this.kinectSensor = KinectSensor.GetDefault();
        }

        if (this.kinectSensor != null)
        {
            this.bodyFrameReader = this.kinectSensor.BodyFrameSource.OpenReader();

            if (!this.kinectSensor.IsOpen)
            {
                this.kinectSensor.Open();
            }
        }
    }
    
    void Update()
    {
        if (this.bodyFrameReader != null)
        {
            var frame = this.bodyFrameReader.AcquireLatestFrame();

            if (frame != null)
            {
                if (this.bodies == null)
                {
                    this.bodies = new Body[this.kinectSensor.BodyFrameSource.BodyCount];
                }

                frame.GetAndRefreshBodyData(this.bodies);
                frame.Dispose();

                foreach (var body in this.bodies)
                {
                    if (body.IsTracked)
                    {
                        var hand = body.Joints[JointType.HandRight];
                        var screenPos = Camera.main.WorldToScreenPoint(new Vector3(hand.Position.X * 100, hand.Position.Y * 100, hand.Position.Z * 10));


                        // Set the position of the cursor GameObject based on the tracked hand position
                        transform.position = Vector3.Lerp(transform.position, screenPos, Time.deltaTime * this.cursorSpeed);

                        // Check if the hand is in a closed fist
                        if (body.HandRightState == HandState.Closed)
                        {
                            Debug.Log("hand is closed.");
                            if (!isHandClosed)
                            {
                                // If the hand was previously open, mark the time when it was closed
                                isHandClosed = true;
                                handCloseTime = Time.time;
                            }
                            else
                            {
                                // If the hand has been closed for more than the click threshold time, perform a click action
                                if (Time.time - handCloseTime > clickThreshold)
                                {
                                    PerformClickAction(transform.position);
                                }
                            }
                        }
                        else
                        {
                            Debug.Log("hand is open");
                            // Reset the hand close state and time
                            isHandClosed = false;
                            handCloseTime = 0f;
                        }
                    }
                }
            }
        }
    }

    void OnApplicationQuit()
    {
        if (this.bodyFrameReader != null)
        {
            this.bodyFrameReader.Dispose();
        }

        if (this.kinectSensor != null && this.kinectSensor.IsOpen)
        {
            this.kinectSensor.Close();
        }
    }

    void PerformClickAction(Vector3 targetPosition)
    {
        Debug.Log("Moving cursor and left clicking!");
        Debug.LogFormat("Performing click action at position ({0}, {1})", targetPosition.x, targetPosition.y);

        // Simulate a left mouse button down event
        mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, 0);

        // Simulate a left mouse button up event
        mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, 0);
    }
}