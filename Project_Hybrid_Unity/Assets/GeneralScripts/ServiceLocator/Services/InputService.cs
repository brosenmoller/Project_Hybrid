using System;
using System.IO.Ports;
using System.Threading;
using UnityEngine.InputSystem;

public enum InputHandlingMethod { Normal = 0, Arduino = 1 };

public class InputService : Service
{
    private PlayerControls playerInputActions;
    private readonly InputHandlingMethod inputHandlingMethod;

    private readonly SerialPort serialPort = new("COM3", 9600);

    public event Action JumpStarted;
    public event Action JumpCancelled;

    public event Action DashStarted;
    
    public event Action AttackStarted;

    public event Action CrouchStarted;
    public event Action CrouchCancelled;

    public int HorizontalAxis { 
        get { return horizontalAxis; }
        private set 
        { 
            horizontalAxis = value; 
            if (horizontalAxis != 0) { direction = horizontalAxis; } 
        }
    }
    private int horizontalAxis = 0;
    public int Direction { get { return direction; } }
    private int direction = 1;

    private bool jumpPressed = false;
    private bool jumpStartedEvent = false;
    private bool jumpCancelledEvent = false;

    private bool crouchPressed = false;
    private bool crouchStartedEvent = false;
    private bool crouchCancelledEvent = false;

    private bool attackStartedEvent = false;

    private bool dashStartedEvent = false;

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
        playerInputActions.Gameplay.Crouch.started += CrouchStartedEvent;
        playerInputActions.Gameplay.Crouch.canceled += CrouchCancelledEvent;
        playerInputActions.Gameplay.Attack.started += AttackStartedEvent;
        playerInputActions.Gameplay.Dash.started += DashStartedEvent;
    }

    public void UpdateMovementDirection(InputAction.CallbackContext callbackContext) => HorizontalAxis = (int)callbackContext.ReadValue<float>();

    public void ResetMovementDirection(InputAction.CallbackContext callbackContext) => HorizontalAxis = 0;
    public void JumpStartedEvent(InputAction.CallbackContext callbackContext) => JumpStarted?.Invoke();
    public void JumpCancelledEvent(InputAction.CallbackContext callbackContext) => JumpCancelled?.Invoke();
    public void CrouchStartedEvent(InputAction.CallbackContext callbackContext) => CrouchStarted?.Invoke();
    public void CrouchCancelledEvent(InputAction.CallbackContext callbackContext) => CrouchCancelled?.Invoke();
    public void AttackStartedEvent(InputAction.CallbackContext callbackContext) => AttackStarted?.Invoke();
    public void DashStartedEvent(InputAction.CallbackContext callbackContext) => DashStarted?.Invoke();

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
            bool jumpWasPressed = jumpPressed;
            jumpPressed = false;

            bool crouchWasPressed = crouchPressed;
            crouchPressed = false;

            HorizontalAxis = 0;

            if (serialPort.IsOpen)
            {
                try
                {
                    int inputFlag = serialPort.ReadByte();
                    if ((inputFlag & 1 << 0) != 0) // Left
                    {
                        HorizontalAxis -= 1;
                    }
                    if ((inputFlag & 1 << 1) != 0) // Right
                    {
                        HorizontalAxis += 1;
                    }
                    if ((inputFlag & 1 << 2) != 0) // Jump
                    {
                        jumpPressed = true;
                    }
                    if ((inputFlag & 1 << 3) != 0) // Crouch
                    {
                        crouchPressed = true;
                    }
                    if ((inputFlag & 1 << 4) != 0) // Attack
                    {
                        attackStartedEvent = true;
                    }
                    if ((inputFlag & 1 << 5) != 0) // Dash
                    {
                        dashStartedEvent = true;
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

            if (crouchPressed && !crouchWasPressed)
            {
                crouchStartedEvent = true;
            }
            else if (crouchWasPressed && !crouchPressed)
            {
                crouchCancelledEvent = true;
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

        if (crouchStartedEvent)
        {
            crouchStartedEvent = false;
            CrouchStarted?.Invoke();
        }
        if (crouchCancelledEvent)
        {
            crouchCancelledEvent = false;
            CrouchCancelled?.Invoke();
        }

        if (attackStartedEvent)
        {
            attackStartedEvent = false;
            AttackStarted?.Invoke();
        }

        if (dashStartedEvent)
        {
            dashStartedEvent = false;
            DashStarted?.Invoke();
        }
    }
}

