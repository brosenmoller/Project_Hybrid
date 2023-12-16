using System;
using System.IO.Ports;
using System.Threading;
using UnityEngine.InputSystem;

public enum InputHandlingMethod { Normal = 0, Arduino = 1 };

public class InputService : Service
{
    private PlayerControls playerInputActions;
    private InputHandlingMethod inputHandlingMethod;

    private readonly SerialPort serialPort = new("COM3", 9600);

    public event Action JumpStarted;
    public event Action JumpCancelled;

    public event Action DashStarted;
    
    public event Action AttackStarted;

    public event Action CrouchStarted;
    public event Action CrouchCancelled;

    public int HorizontalAxis { get { return horizontalAxis; } }
    private int horizontalAxis = 0;
    public int Direction { get { return direction; } }
    private int direction = 1;

    private bool jumpPressed = false;
    private bool jumpWasPressed = false;

    private bool jumpStartedEvent = false;
    private bool jumpCancelledEvent = false;

    private Thread arduinoThread;
    private bool arduinoThreadTerminated = false;

    public InputService(InputHandlingMethod inputHandlingMethod)
    {
        this.inputHandlingMethod = inputHandlingMethod;

        if (inputHandlingMethod == InputHandlingMethod.Normal)
        {
            SetupNormalInput();
        }
        else if (inputHandlingMethod == InputHandlingMethod.Arduino)
        {
            SetupArduinoInput();
        }
    }

    private void SetupNormalInput()
    {
        playerInputActions = new PlayerControls();
        playerInputActions.Enable();

        playerInputActions.Gameplay.HorizontalMovement.performed += UpdateMovementDirection;
        playerInputActions.Gameplay.HorizontalMovement.canceled += ResetMovementDirection;
        playerInputActions.Gameplay.Jump.started += JumpStartedEvent;
        playerInputActions.Gameplay.Jump.canceled += JumpCancelledEvent;
    }

    public void UpdateMovementDirection(InputAction.CallbackContext callbackContext) => horizontalAxis = (int)callbackContext.ReadValue<float>();

    public void ResetMovementDirection(InputAction.CallbackContext callbackContext) => horizontalAxis = 0;
    public void JumpStartedEvent(InputAction.CallbackContext callbackContext) => JumpStarted?.Invoke();
    public void JumpCancelledEvent(InputAction.CallbackContext callbackContext) => JumpCancelled?.Invoke();

    private void SetupArduinoInput()
    {
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
        if (inputHandlingMethod != InputHandlingMethod.Arduino) { return; }

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
            jumpWasPressed = jumpPressed;
            jumpPressed = false;
            horizontalAxis = 0;


            if (serialPort.IsOpen)
            {
                try
                {
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
                }
                catch (Exception) { }
            }

            if (jumpPressed && !jumpWasPressed)
            {
                jumpStartedEvent = true;
            }
            else if (jumpWasPressed && !jumpPressed)
            {
                jumpCancelledEvent = true;
            }

            Thread.Sleep(10);
        }
    }

    public override void OnUpdate()
    {
        if (inputHandlingMethod == InputHandlingMethod.Arduino)
        {
            CallArduinoEvents();
        }
    }

    private void CallArduinoEvents()
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

