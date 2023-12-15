using System;
using System.IO.Ports;
using System.Threading;

public class InputService : Service
{
    public PlayerControls playerInputActions;

    private readonly SerialPort serialPort = new("COM3", 9600);

    public event Action JumpStarted;
    public event Action JumpCancelled;
    public int HorizontalAxis { get { return horizontalAxis; } }
    private int horizontalAxis = 0;

    private bool jumpPressed = false;
    private bool jumpWasPressed = false;

    private bool jumpStartedEvent = false;
    private bool jumpCancelledEvent = false;

    private readonly Thread arduinoThread;
    private bool arduinoThreadTerminated = false;

    public InputService()
    {
        playerInputActions = new PlayerControls();
        playerInputActions.Enable();

        serialPort.Open();
        serialPort.ReadTimeout = 10;

        arduinoThread = new Thread(ArduinoInputThread);
        arduinoThread.Start();
    }

    public void DisablePlayerInput()
    {
        playerInputActions.Disable();
    }

    public void EnablePlayerInput()
    {
        playerInputActions.Enable();
    }

    public override void OnDisable()
    {
        arduinoThreadTerminated = true;
        if (arduinoThread != null && arduinoThread.IsAlive)
        {
            arduinoThread.Join();
        }
    }

    private void ArduinoInputThread()
    {
        while (!arduinoThreadTerminated)
        {
            if (serialPort.IsOpen)
            {
                try
                {
                    jumpWasPressed = jumpPressed;
                    jumpPressed = false;
                    horizontalAxis = 0;

                    int inputFlag = serialPort.ReadByte();
                    if ((inputFlag & 1 << 0) != 0)
                    {
                        horizontalAxis -= 1;
                    }
                    if ((inputFlag & 1 << 1) != 0)
                    {
                        horizontalAxis += 1;
                    }
                    if ((inputFlag & 1 << 2) != 0)
                    {
                        jumpPressed = true;
                    }

                    if (jumpPressed && !jumpWasPressed)
                    {
                        jumpStartedEvent = true;
                    }
                    else if (jumpWasPressed && !jumpPressed)
                    {
                        jumpCancelledEvent = true;
                    }
                }
                catch (Exception)
                {

                }
            }


            Thread.Sleep(10);
        }
    }

    public override void OnUpdate()
    {
        if (jumpStartedEvent)
        {
            jumpStartedEvent = false;
            JumpStarted?.Invoke();
        }
        if (jumpCancelledEvent)
        {
            jumpCancelledEvent = false;
            JumpCancelled?.Invoke();
        }
    }
}

