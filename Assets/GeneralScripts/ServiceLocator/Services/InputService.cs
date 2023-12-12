
public class InputService : Service
{
    public PlayerControls playerInputActions;
    
    public InputService()
    {
        playerInputActions = new PlayerControls();
        playerInputActions.Enable();
    }

    public void DisablePlayerInput()
    {
        playerInputActions.Disable();
    }

    public void EnablePlayerInput()
    {
        playerInputActions.Enable();
    }
}

