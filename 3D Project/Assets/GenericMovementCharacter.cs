using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenericMovementCharacter : MonoBehaviour
{
    [Tooltip("Referencia del CharacterControler para obtener sus fisicas")]
    public CharacterController controller;

    private Animator anim;

    [Header("Propiedades del jugador")]

    public float speed = 0.0f;

    public float playeRotationspeed = 0.1f;

    public float rotationspeedSmooth = 0.1f;
  
    public float currentSpeed = 0f;

    private float speedMagnitude;

    [Tooltip("Fuerza de Salto")]
    public float jumpHeight = 3f;

    [Tooltip("Gravedad del jugador dentoro del juego")]
    public float gravity = -9.81f;

    [Tooltip("Collider: para saber si esta tocando el piso")]
    public Transform groundCheck;

    [Tooltip("Distancia del suelo")]
    public float groundDistance = 0.4f;

    [Tooltip("Layer para saber que esta pisando el jugador")]
    public LayerMask groundMask;

    public Camera cam;
    Vector3 velocity;
    bool isGrounded;
    public Vector3 desiredMoveDirection;


    private void Start(){
        anim = GetComponent<Animator>();
        cam = Camera.main;
    }


    void Update()
    {
        //Physic.CheckSaphere :Returns true if there are any colliders overlapping the sphere defined by position and radius in world coordinates.
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0f)
        {
            //Esto es para ponerle un limite en la velocidad de Y cuando el jugador esta cayendo (sino seria infinitamente negativo)
            velocity.y = -2f;
        }

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        var camera = Camera.main;
        var forward = cam.transform.forward;
        var right = cam.transform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        desiredMoveDirection = forward * z + right * x;


        if (desiredMoveDirection != Vector3.zero) {

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMoveDirection), rotationspeedSmooth);
            controller.Move(desiredMoveDirection * Time.deltaTime * speed);

        }

        //Player Jump

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);

      
        //Animation Setting

        speedMagnitude = new Vector2(x,z).sqrMagnitude;

        if (speedMagnitude > playeRotationspeed)
        {

            anim.SetFloat("Blend", speedMagnitude, 0.3f, Time.deltaTime);
            
        }

        if (speedMagnitude < playeRotationspeed)
        {
            Debug.Log("Dejo de rotar");
            anim.SetFloat("Blend", speedMagnitude, 0.15f, Time.deltaTime);
        }
    }
}
