using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public InputAction MoveAction;

    public float walkSpeed = 2.5f;
    public float turnSpeed = 10f;

    Rigidbody m_Rigidbody;
    Animator m_Animator;
    Vector2 m_InputVector;

    void Start()
    {
        m_Rigidbody = GetComponent<Rigidbody>();
        m_Animator = GetComponentInChildren<Animator>();

        if (m_Animator == null)
        {
            Debug.LogError("[PlayerMovement] No se encontró Animator en Player ");
        }
        else
        {
            Debug.Log("[PlayerMovement] Animator encontrado: " + m_Animator.gameObject.name);
            m_Animator.updateMode = AnimatorUpdateMode.Fixed;
            m_Animator.applyRootMotion = false;
        }

        if (m_Rigidbody == null)
        {
            Debug.LogError("[PlayerMovement] No se encontró Rigidbody en Player!");
        }
        else
        {
            m_Rigidbody.constraints = RigidbodyConstraints.FreezeRotationX |
                                       RigidbodyConstraints.FreezeRotationZ;
        }

        MoveAction.Enable();
    }

    void Update()
    {
        m_InputVector = MoveAction.ReadValue<Vector2>();

        if (m_InputVector.sqrMagnitude > 0.0001f)
            Debug.Log($"[Input] x:{m_InputVector.x:F2} y:{m_InputVector.y:F2} mag:{m_InputVector.magnitude:F2}");
    }

    void FixedUpdate()
    {
        bool isMoving = m_InputVector.magnitude > 0.1f; 

        if (isMoving)
        {
            Vector3 movement = new Vector3(m_InputVector.x, 0f, m_InputVector.y).normalized;

            Vector3 newPosition = m_Rigidbody.position + movement * walkSpeed * Time.fixedDeltaTime;
            m_Rigidbody.MovePosition(newPosition);

            if (movement.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(movement);
                Quaternion newRotation = Quaternion.Lerp(
                    m_Rigidbody.rotation,
                    targetRotation,
                    turnSpeed * Time.fixedDeltaTime
                );
                m_Rigidbody.MoveRotation(newRotation);
            }
        }

        if (m_Animator != null)
        {
            float speedParam = m_InputVector.magnitude;
            m_Animator.SetFloat("Speed", speedParam);
           
        }
    }

    void OnDisable()
    {
        MoveAction.Disable();
    }
}
