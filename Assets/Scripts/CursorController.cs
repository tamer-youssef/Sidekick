using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Windows.Kinect;
using System;
using System.Runtime.InteropServices;
using System.Drawing;

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

    [DllImport("user32.dll", SetLastError = true)]
    static extern uint SendInput(uint nInputs, [MarshalAs(UnmanagedType.LPArray), In] INPUT[] pInputs, int cbSize);

    public KinectSensor kinectSensor;
    public float cursorSpeed = 1000f;
    public float clickThreshold = 0.3f; // Duration threshhold to detect a closed fist

    // Constants for mouse input type and flags
    private const int INPUT_MOUSE = 0;
    private const int MOUSEEVENTF_LEFTDOWN = 0x02;
    private const int MOUSEEVENTF_LEFTUP = 0x04;

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
        Debug.Log("left clicking!");
        Debug.LogFormat("Performing click action at position ({0}, {1})", targetPosition.x, targetPosition.y);

        // Set up an array of two INPUT structs representing a left mouse button down event and a left mouse button up event
        INPUT[] inputs = new INPUT[2];

        // Set up the first struct with MOUSEINPUT data representing a left mouse button down event
        inputs[0].type = INPUT_MOUSE;
        inputs[0].mi.dx = (int)targetPosition.x;
        inputs[0].mi.dy = Screen.height - (int)targetPosition.y;
        inputs[0].mi.dwFlags = MOUSEEVENTF_LEFTDOWN;

        // Set up the second struct with MOUSEINPUT data representing a left mouse button up event
        inputs[1].type = INPUT_MOUSE;
        inputs[1].mi.dx = (int)targetPosition.x;
        inputs[1].mi.dy = Screen.height - (int)targetPosition.y;
        inputs[1].mi.dwFlags = MOUSEEVENTF_LEFTUP;

        //Debug.Log("dx: " + inputs[0].mi.dx);
        //Debug.Log("dy: " + inputs[0].mi.dy);
        // Send the input events to the operating system
        uint result = SendInput(2, inputs, Marshal.SizeOf(typeof(INPUT)));
        if (result == 0)
        {
            Debug.Log("good");
        } 
        else if (result == 2)
        {
            Debug.Log("bad");
        }
    }
}