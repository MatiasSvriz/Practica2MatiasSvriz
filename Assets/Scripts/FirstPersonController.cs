using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FirstPersonController : MonoBehaviour
{
    [Header("Movement")] 
    [SerializeField] private float movementSpeed;
    [SerializeField] private float jumpHeight;
    [SerializeField] private float gravityScale;

    [Header("Ground Detection")]
    [SerializeField] private Transform feet;
    [SerializeField] private float detectionRadius;
    [SerializeField] private LayerMask whatIsGround;

    private CharacterController controller;
    private Camera cam;

    private bool isGrounded;
    private Vector2 inputVector;
    private Vector3 verticalMovement;
    
    private PlayerInput playerInput;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        GetComponent<PlayerInput>();
        cam = Camera.main;
        
        Cursor.lockState = CursorLockMode.Locked;
    }

    // OnEnable se ejecuta por cada vez que se activa este gameObject
    private void OnEnable()
    {
        playerInput.actions["Jump"].started += Jump;
        playerInput.actions["Move"].performed += UpdateMovement;
        playerInput.actions["Move"].canceled += UpdateMovement;
    }

    // OnEnable se ejecuta por cada vez que se desactiva este gameObject
    private void OnDisable()
    {
        playerInput.actions["Jump"].started -= Jump;
        playerInput.actions["Move"].performed -= UpdateMovement;
        playerInput.actions["Move"].canceled -= UpdateMovement;
    }

    private void UpdateMovement(InputAction.CallbackContext ctx)
    {
        // inputVector.x corresponde con la horizontal del teclado
        // inputVector.y corresponde con la vertical del teclado
        inputVector = ctx.ReadValue<Vector2>();
    }
    
    private void Jump(InputAction.CallbackContext ctx)
    {
        if (isGrounded)
        {
            verticalMovement.y = Mathf.Sqrt(-2 * gravityScale * jumpHeight);
        }
    }

    void Update()
    {
        GroundCheck();
        ApplyGravity();
        MoveAndRotate();
    }

    private void MoveAndRotate()
    {
        // Rotar el cuerpo con la cámara
        transform.rotation = Quaternion.Euler(0, cam.transform.eulerAngles.y, 0);
        
        // Calcular movimiento
        Vector3 movement = Vector3.zero;
        
        if (inputVector.magnitude > 0)
        {
            // Calcular ángulo basado en input y cámara
            float angle = Mathf.Atan2(inputVector.x, inputVector.y) * Mathf.Rad2Deg + cam.transform.eulerAngles.y;
            
            // Calcular velocidad (respeta magnitud del joystick)
            float speed = movementSpeed * inputVector.magnitude;
            
            // Rotar Vector3.forward al ángulo calculado
            Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            
            movement = direction * speed;
        }
        
        // Aplicar movimiento
        controller.Move((movement + verticalMovement) * Time.deltaTime);
    }

    private void ApplyGravity()
    {
        if (isGrounded && verticalMovement.y < 0)
        {
            verticalMovement.y = -2f;
        }
        else
        {
            verticalMovement.y += gravityScale * Time.deltaTime;
        }
    }

    private void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(feet.position, detectionRadius, whatIsGround);
    }

    private void OnDrawGizmos()
    {
        if (feet != null)
        {
            Gizmos.DrawSphere(feet.position, detectionRadius);
        }
    }
}
