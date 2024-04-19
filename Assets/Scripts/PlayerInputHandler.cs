using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public static PlayerInputHandler Instance { get; private set; }

    [Header("Input Action Asset")]
    [SerializeField] private InputActionAsset playerControls;

    [Header("Action Map Name References")]
    [SerializeField] private string actionMapName = "Player";

    [Header("Action Name References")]
    [SerializeField] private string move = "Move";
    [SerializeField] private string shoot = "Shoot";
    [SerializeField] private string skill1 = "Skill1";
    [SerializeField] private string skill2 = "Skill2";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        moveAction = playerControls.FindActionMap(actionMapName).FindAction(move);
        shootAction = playerControls.FindActionMap(actionMapName).FindAction(shoot);
        skill1Action = playerControls.FindActionMap(actionMapName).FindAction(skill1);
        skill2Action = playerControls.FindActionMap(actionMapName).FindAction(skill2);

        RegisterInputActions();
    }
    
    private void Update()
    {
        
    }
    private void RegisterInputActions()
    {
        moveAction.performed += context => MoveInput = context.ReadValue<Vector2>();
        moveAction.canceled += context => MoveInput = Vector2.zero;
        shootAction.performed += context => ShootInput = true;
        shootAction.canceled += context => ShootInput = false;
        skill1Action.performed += context => Skill1Input = true;
        skill1Action.canceled += context => Skill1Input = false;
        skill2Action.performed += context => Skill2Input = true;
        skill2Action.canceled += context => Skill2Input = false;
    }

    private void OnEnable()
    {
        moveAction.Enable();
        shootAction.Enable();
        skill1Action.Enable();
        skill2Action.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        shootAction.Disable();
        skill1Action.Disable();
        skill2Action.Disable(); 
    }

    public Vector2 MoveInput { get; private set; }
    public bool ShootInput { get; private set; }
    public bool Skill1Input { get; private set; }
    public bool Skill2Input { get; private set; }

    private InputAction moveAction;
    private InputAction shootAction;
    private InputAction skill1Action;
    private InputAction skill2Action;

    public Vector3 GetLocalMousePosition()
    { return Input.mousePosition; }
}
