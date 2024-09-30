using UnityEngine;
using UnityEngine.InputSystem;

public static class InputManager
{
    // user input
    private static UserInput _userInput = new UserInput();

    // input direction
    private static Vector3 _inputDirection = Vector3.zero;
    public static Vector3 InputDirection { get { return _inputDirection; } }


    // these input events do not return any value nor pass it through params
    // thus can each use a single delegate
    public delegate void InputEvent();

    // fire primary event
    public static event InputEvent OnFirePrimary;

    // fire special event
    public static event InputEvent OnFireSpecial;

    // pause game event
    public static event InputEvent OnPauseGame;

    #region InitializeEvents
    // call method after scenes have loaded. Initializes the input manager
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void OnStart() {
        _userInput.Enable();
        SubscribeToInputEvents();
    }

    // subscribe relevant events to corresponding new input events
    private static void SubscribeToInputEvents() {
        _userInput.GameInput.Move.performed += OnMovePerformed;
        _userInput.GameInput.FirePrimary.performed += OnFirePrimaryPerformed;
        _userInput.GameInput.FireSpecial.performed += OnFireSpecialPerformed;
        _userInput.GameInput.PauseGame.performed += OnPauseGamePerformed;
    }
    #endregion

    #region InputValueEventListeners
    private static void OnMovePerformed(InputAction.CallbackContext value) {
        _inputDirection = value.ReadValue<Vector2>();
    }
    #endregion

    #region InputButtonEvents
    // I am uncertain if calling these static events from new input system events is good practice,
    // but I am doing so to avoid having to create an instance of UserInput in every script that needs
    // to listen to input events, nor do I want to have to add callbackContext to each method
    private static void OnFirePrimaryPerformed(InputAction.CallbackContext value) {
        // not using ?.Invoke because this is more readable and I'm scared teachers will yell at me :(
        if(OnFirePrimary != null)
        {
            OnFirePrimary.Invoke();
        }
    }

    private static void OnFireSpecialPerformed(InputAction.CallbackContext value) {
        if(OnFireSpecial != null)
        {
            OnFireSpecial.Invoke();
        }
    }

    private static void OnPauseGamePerformed(InputAction.CallbackContext value) {
        if(OnPauseGame != null)
        {
            OnPauseGame.Invoke();
        }
    }
    #endregion
}
