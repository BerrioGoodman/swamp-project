using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Referencias")]
    public PlayerInput playerControls;
    public Transform cameraTransform;
    private CharacterController characterController;
    public Animator tPAnimator;

    [Header("Estadisticas del personaje")]
    [SerializeField] private float walkSpeed = 7f;
    [SerializeField] private float runSpeed = 12f;
    [SerializeField] private float jumpHeight = 2.5f;
    private float raycastDistance = 0.2f;

    [Header("Variables del jugador")]
    [SerializeField] private float speed;
    [SerializeField] private bool isGrounded = true;
    [SerializeField] private float currentSpeed;
    private Vector3 lastPosition;

    private Vector3 velocity;

    [Header("Estidisticas del mundo")]
    [SerializeField] private float gravity = 9.8f;
    private void Awake()
    {
        //Le damos el componente de characterContoller a la variable
        characterController = GetComponent<CharacterController>();
        //Le damos el componente de playerInput a la variable
        playerControls = GetComponent<PlayerInput>();

        speed = walkSpeed;
        lastPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = characterController.isGrounded;//Esta o no en el piso
        tPAnimator.SetBool("IsGround", isGrounded);

        if (isGrounded && velocity.y < 0) 
        {
            velocity.y = -2.5f; //si el personaje esta en el suelo no se le aplica mas fuerza
        }

        Move();
        Jump();
        HandleCursosLock();
    }

    public void Move()
    {
        //Estamos capturando el movimiento de player controls
        Vector2 _moveHV = playerControls.actions["Move"].ReadValue<Vector2>();

        //la derecha de la camara y el frente serán el movimiento en x y en Y
        Vector3 move = cameraTransform.right * _moveHV.x + cameraTransform.forward * _moveHV.y;

        move.y = 0f;//Asegurar de que el movimiento sea solo horizontal
        move.Normalize();//trabajamos solo con numeros enteros

        //si el shift esta oprimido corre, sino, camina
        speed = playerControls.actions["Run"].IsInProgress() ? runSpeed : walkSpeed;

        characterController.Move(move * speed * Time.deltaTime);//el movimiento del personaje

        //calcular velocidad actual
        currentSpeed = (transform.position - lastPosition).magnitude / Time.deltaTime;
        lastPosition = transform.position;

        //rotar al personaje
        if (_moveHV.magnitude > 0.001f) 
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(move), 0.1f);//rotar al personaje
        }

        tPAnimator.SetFloat("Speed", currentSpeed);
    }

    public void Jump() 
    {
        if (playerControls.actions["Jump"].WasPressedThisFrame() && isGrounded) 
        {
            
            velocity.y = Mathf.Sqrt(jumpHeight * gravity);
            
        }
        
        velocity.y -= gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);

    }

    public void HandleCursosLock() 
    {
        //si oprimimos el click izquierdo desaparece el cursor
        if (playerControls.actions["LeftClick"].WasPressedThisFrame()) 
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        //si oprimimos esc volvemos
        if (playerControls.actions["RightClick"].WasPressedThisFrame() )
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private bool CheckIfGrounded() 
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, raycastDistance)) 
        {
            return true;
        }
        return false;
    }
}
