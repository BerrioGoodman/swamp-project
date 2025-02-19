using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraController : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform playerTarget;//donde mira la camara
    [SerializeField] private PlayerInput playerControls;
    [SerializeField] private GameObject player;

    [Header("Parametros")]
    //angulo de rotacion de la camara con respecto a la persona
    private Vector2 angle = new Vector2(90 * Mathf.Deg2Rad, -15 * Mathf.Deg2Rad);
    private float maxDistance = 4; //Distancia del jugador a la camara
    public float maxAngleCameraY = 80;
    public float minAngleCameraY = 60;
    [Range(1, 50)] public float sensibility;

    [Header("Variables")]
    private Vector2 nearPlaneSize; //calculo del plano mas cercano de la colision de la camara
    private float horizontalLookRotation;
    void Start()
    {
        playerControls = GetComponentInParent<PlayerInput>();//obtiene el componente del padre en la jerarquia
        playerCamera = GetComponent<Camera>();
        CalculateNearPlaneSize();
    }

    // Update is called once per frame
    void Update()
    {
        RotateCamera();
    }

    public void RotateCamera() 
    {
        Vector2 lookHV = playerControls.actions["Look"].ReadValue<Vector2>();
        //rotaremos al personaje
        //horizontalLookRotation += lookHV.x * Time.deltaTime * sensibility;

        if (lookHV.x != 0) //si el mouse esta mirando en algun lado en el eje X
        {
            //Se convierte el angulo de vision en radianes y se multiplica por el tiempo y los frames de pantalla
            angle.x += lookHV.x * Mathf.Deg2Rad * sensibility * Time.deltaTime;
        }

        if (lookHV.y != 0) 
        {
            angle.y += lookHV.y * Mathf.Deg2Rad * sensibility * Time.deltaTime;
            //limita el valor dentro de un rango especifico
            //Se pone negativo el valor para que no rote donde no debe
            angle.y = Mathf.Clamp(angle.y, -maxAngleCameraY * Mathf.Deg2Rad, minAngleCameraY * Mathf.Deg2Rad);
        }

        //rotacion en el eje Y(horizontal, izquiera, derecha)
        //rota en el eje X segun el valor de la variable
        //quaternion.euler transforma las rotaciones a euler
        //player.transform.rotation = Quaternion.Euler(0, horizontalLookRotation, 0);
    }

    private void LateUpdate()
    {
        //calcular la orbita de la camara
        //calcula de forma trigonometrica
        Vector3 orbit = new Vector3(
            Mathf.Cos(angle.x) * Mathf.Cos(angle.y),
            -Mathf.Sin(angle.y),
            -Mathf.Sin(angle.x) * Mathf.Cos(angle.y)
        );

        float cameraDistance = maxDistance;
        RaycastHit hit; //calcular los puntos de colision de la camara
        Vector3[] _point = GetCameraCollitionPoint(orbit);

        //puntos de colision
        foreach (Vector3 point in _point) 
        {
            if (Physics.Raycast(point,orbit, out hit, maxDistance))//si el punto de colision esta en la orbita
            {
                //la distancia nueva de la camara cambia depende del punto de colision
                cameraDistance = Mathf.Min((hit.point - playerTarget.position).magnitude, cameraDistance);
            }
        }

        transform.position = playerTarget.position + orbit * cameraDistance;
        transform.rotation = Quaternion.LookRotation(playerTarget.position - transform.position);
    }


    //esta funcion detecta todos los puntos de colision de la camara
    private Vector3[] GetCameraCollitionPoint(Vector3 orbit) 
    {
        Vector3 _position = playerTarget.position;

        Vector3 _center = _position + orbit * (playerCamera.nearClipPlane + 0.2f);

        Vector3 _right = transform.right * nearPlaneSize.x;
        Vector3 _up = transform.up * nearPlaneSize.y;

        return new Vector3[]
        {
            _center - _right + _up,
            _center + _right + _up,
            _center - _right - _up,
            _center + _right - _up,
        };
    }

    private void CalculateNearPlaneSize() //funcionamiento de la colision de la camara con el entorno
    {
        float _ver = Mathf.Tan(playerCamera.fieldOfView * Mathf.Deg2Rad / 2) * playerCamera.nearClipPlane;
        float _hor = _ver * playerCamera.aspect;

        nearPlaneSize = new Vector2(_hor, _ver);
    }

}
