using System;
using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Manages player input and interactions with the camera and movement components.
/// 
/// This class is designed to work out of the box with the provided camera and movement components.
/// It serves as a demonstration of how these components integrate and interact with each other.
/// 
/// For customization or personal modifications, you are encouraged to create a new player class 
/// that extends or adapts this functionality to suit your needs.
/// </summary>
public class Player : MonoBehaviour
{
    /// <summary>
    /// The component used to move this gameobject.
    /// </summary>
    [SerializeField]
    private MovementManager m_MovementManager;

    public MovementManager MovementManager => m_MovementManager;

    /// <summary>
    /// Component/script placed on a child or other object of the camera rigg
    /// </summary>
    [SerializeField]
    private CameraManager m_CameraManager;

    public CameraManager CameraManager => m_CameraManager;

    /// <summary>
    /// This value will be set in start, and stored for later occations.
    /// If paused game cursor will show. If default was set to hidden. The cursor will be set to hidden when unpaused again.
    /// </summary>
    [SerializeField]
    private CursorLockMode m_DefaultCursorLockMode;

    public CursorLockMode DefaultCursorLockMode => m_DefaultCursorLockMode;

    /// <summary>
    /// Referencing the class at the bottom of this scrip. 
    /// Class used to seperate some variables from this class. Makes it more clean in the inspector.
    /// </summary>
    [SerializeField]
    private PlayerInputActions m_PlayerInputActions;

    public PlayerInputActions PlayerInputActions => m_PlayerInputActions;

    /// <summary>
    /// Boolean to check if the game is paused. 
    /// Input and timescale are changed if the current value changes
    /// </summary>
    private bool m_GamePaused = false;

    /// <summary>
    /// Boolean to check if the game is paused. 
    /// Input and timescale are changed if the current value changes
    /// </summary>
    public bool GamePaused
    {
        get => m_GamePaused;
        set
        {
            m_GamePaused = value;
            if (m_GamePaused)
            {
                Time.timeScale = 0;
                DisableInput();
                SetCursor(CursorLockMode.None);
            }
            else
            {
                Time.timeScale = 1;
                EnableInput();
                SetCursorToDefault();
            }
        }
    }

    /// <summary>
    /// If we find the camera remove it as a child.
    /// We will set the camerarigs position in the late update to track the players position instead
    /// </summary>
    private void Awake()
    {
        if (m_CameraManager != null)
        {
            m_CameraManager.transform.SetParent(null);
        }
    }

    /// <summary>
    /// Enable the inputs, Ignore this if you use unity events
    /// </summary>
    private void OnEnable()
    {
        if (m_PlayerInputActions.UseInputClass)
        {
            EnableInput();
        }
    }

    /// <summary>
    /// Disabeling the inputs, Ignore this if you use unity events
    /// </summary>
    private void OnDisable()
    {
        if (m_PlayerInputActions.UseInputClass)
        {
            DisableInput();

            // Unsub to input to pause the game
            if (m_PlayerInputActions.PauseAction)
                m_PlayerInputActions.PauseAction.action.performed -= PauseInput;
        }
    }

    /// <summary>
    /// Looking for the camera in the scene if not set in the inspector.
    /// </summary>
    private void Start()
    {
#if UNITY_6000_0_OR_NEWER
        // Unity 6 and newer
        if (m_CameraManager == null)
        {
            m_CameraManager = FindFirstObjectByType<CameraManager>();
        }
#else
        // Older Unity versions
        if (m_CameraManager == null)
        {
            m_CameraManager = FindObjectOfType<CameraManager>();
        }
#endif

        // Assigning the camera to the movment component.
        // The movement are based on the camera directions.
        if (m_MovementManager != null && m_MovementManager.CameraManager == null)
        {
            m_MovementManager.CameraManager = m_CameraManager;
        }

        //Settings default cursor
        Cursor.lockState = m_DefaultCursorLockMode;
    }

    /// <summary>
    /// Manages input callbacks using the new Unity Input System.
    /// 
    /// This method sets up input callbacks programmatically, which is my preferred approach. While many developers might choose to configure these callbacks directly in the Unity Inspector using Unity events, this approach provides more flexibility and control in code.
    /// 
    /// If this method does not suit your preferences, you can:
    /// - Create a new player class to handle inputs differently.
    /// - Disable automatic input enabling by unchecking the corresponding boolean option, and instead assign the input actions to the methods in this class through the Inspector.
    /// 
    /// There is no single "correct" approach—this is simply a personal preference for managing input callbacks.
    /// </summary>
    public void EnableInput()
    {
        if (m_PlayerInputActions != null)
        {
            // Sub to rotation on the camera
            if (m_PlayerInputActions.RotateInputAction)
                m_PlayerInputActions.RotateInputAction.action.performed += LookRotationInput;

            // Sub to input to change movement modes
            if(m_PlayerInputActions.SetMovementModeAction)
                m_PlayerInputActions.SetMovementModeAction.action.performed += SetMovementModeInput;

            // Sub to input to set movement direction
            if(m_PlayerInputActions.MoveInputAction)
            {
                m_PlayerInputActions.MoveInputAction.action.performed += MoveInput;
                m_PlayerInputActions.MoveInputAction.action.canceled += MoveInput;
            }

            // Sub to input to pause the game
            if(m_PlayerInputActions.PauseAction)
                m_PlayerInputActions.PauseAction.action.performed += PauseInput;
        }
    }

    /// <summary>
    /// Manages input callbacks using the new Unity Input System.
    /// </summary>
    public void DisableInput()
    {
        if (m_PlayerInputActions != null)
        {
            // Unsub to rotation on the camera
            if (m_PlayerInputActions.RotateInputAction)
                m_PlayerInputActions.RotateInputAction.action.performed -= LookRotationInput;

            // Unsub to input to change movement modes
            if (m_PlayerInputActions.SetMovementModeAction)
                m_PlayerInputActions.SetMovementModeAction.action.performed -= SetMovementModeInput;

            // Unsub to input to set movement direction
            if (m_PlayerInputActions.MoveInputAction)
            {
                m_PlayerInputActions.MoveInputAction.action.performed -= MoveInput;
                m_PlayerInputActions.MoveInputAction.action.canceled -= MoveInput;
            }
        }
    }

    /// <summary>
    /// Change the state of the cursor
    /// </summary>
    /// <param name="newLockMode">Descides the new cursor lock mode</param>
    public void SetCursor(CursorLockMode newLockMode)
    {
        Cursor.lockState = newLockMode;
    }

    /// <summary>
    /// Set cursor to the default cursor. This value is a varaible in this script.
    /// </summary>
    public void SetCursorToDefault()
    {
        Cursor.lockState = m_DefaultCursorLockMode;
    }

    private void LateUpdate()
    {
        SetCameraPosition();
    }

    /// <summary>
    /// Updates the camera position to follow the player.
    /// 
    /// To ensure proper camera behavior, the camera should be disconnected from the player and independently track the player's position.
    /// This is important because, in a third-person perspective, the player would need to face the camera when moving backward.
    /// If the camera is a child of the player, the entire camera rig might rotate, causing undesirable behavior like the player spinning endlessly.
    /// 
    /// For an alternative setup, consider placing the camera rig outside the player and making it track the player’s position from a separate game object.
    /// </summary>
    private void SetCameraPosition()
    {
        if (m_CameraManager != null)
        {
            m_CameraManager.SetLocation(transform.position);
        }
    }

    /// <summary>
    /// Sends the input value to the camera component to rotate the camera rig.
    /// An optional approach is to use the Yaw and Pitch rotations separately for finer control.
    /// Example:
    ///     m_CameraManager.YawRotate(value.x);
    ///     m_CameraManager.PitchRotate(value.y);
    /// </summary>
    /// <param name="context">Input context containing the rotation values (Vector2). Note: Adjust this in the input action asset if needed.</param>
    private void LookRotationInput(InputAction.CallbackContext context)
    {
        Vector2 value = context.ReadValue<Vector2>();
        if (m_CameraManager != null)
        {
            m_CameraManager.Rotate(value.x, value.y);
        }
    }

    /// <summary>
    /// Changes the player movement mode based on input.
    /// Available modes: { FirstPerson, ThirdPerson }.
    /// To set the movement mode manually, use the method SetMovementMode(MovementMode mode) 
    /// on the m_MovementManager component.
    /// </summary>
    /// <param name="context">Input context for the action that triggers the mode change.</param>
    private void SetMovementModeInput(InputAction.CallbackContext context)
    {
        if (m_MovementManager != null)
        {
            m_MovementManager.ChangeMovementMode();
        }
    }

    /// <summary>
    /// Pauses the game. 
    /// </summary>
    /// <param name="context">Input context for the action that triggers the mode change.</param>
    private void PauseInput(InputAction.CallbackContext context)
    {
        PauseGame();
    }

    /// <summary>
    /// Whatever that happends when puased can be placed here.
    /// </summary>
    protected virtual void PauseGame()
    {
        GamePaused = !GamePaused;
    }

    /// <summary>
    /// Receives movement input from the action and forwards it to the MovementManager.
    /// 
    /// This method reads a Vector2 input value from the action, which represents the movement direction and magnitude.
    /// The MovementManager uses this input to calculate and apply movement based on the current movement mode (FirstPerson or ThirdPerson) and camera orientation.
    /// </summary>
    /// <param name="context">Input context containing the Vector2 value for movement input.</param>
    private void MoveInput(InputAction.CallbackContext context)
    {
        Vector2 inputValue = context.ReadValue<Vector2>();
        if (m_MovementManager != null)
        {
            m_MovementManager.SetMoveInputVector(inputValue);
        }
    }
}

/// <summary>
/// Contains references to input actions used by the player.
/// 
/// This class defines input actions for movement, rotation, and changing movement mode using Unity's new Input System.
/// 
/// By default, it is set up to work with input callbacks programmatically, which is my preferred method for flexibility and control.
/// However, if you prefer, you can:
/// - Disable the automatic input management by unchecking the <see cref="m_UseInputClass"/> boolean.
/// - Configure the input actions directly in the Unity Inspector and assign them to methods in your player class.
/// 
/// There is no single "correct" approach—this class provides a structured way to handle input actions, but you can adapt it to fit your needs.
/// </summary>
[Serializable]
public class PlayerInputActions
{
    [SerializeField]
    private bool m_UseInputClass = true;

    /// <summary>
    /// Indicates whether the input class is used for managing input callbacks programmatically.
    /// If set to false, you should configure the input actions directly in the Unity Inspector.
    /// </summary>
    public bool UseInputClass => m_UseInputClass;

    [SerializeField]
    private InputActionReference m_MoveInputAction;

    /// <summary>
    /// Reference to the input action for movement.
    /// </summary>
    public InputActionReference MoveInputAction => m_MoveInputAction;

    [SerializeField]
    private InputActionReference m_RotateInputAction;

    /// <summary>
    /// Reference to the input action for rotation.
    /// </summary>
    public InputActionReference RotateInputAction => m_RotateInputAction;

    [SerializeField]
    private InputActionReference m_SetMovementModeAction;

    /// <summary>
    /// Reference to the input action for changing the movement mode.
    /// </summary>
    public InputActionReference SetMovementModeAction => m_SetMovementModeAction;

    [SerializeField]
    private InputActionReference m_PuaseAction;

    /// <summary>
    /// Reference to the input action for changing the movement mode.
    /// </summary>
    public InputActionReference PauseAction => m_PuaseAction;
}
