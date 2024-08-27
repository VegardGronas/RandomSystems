using System;
using UnityEngine;

public enum MovementMode { ThirdPerson, FirstPerson }

[RequireComponent (typeof(CharacterController))]
public class MovementManager : MonoBehaviour
{
    [SerializeField]
    CameraManager m_CameraManager;

    public CameraManager CameraManager
    {
        get => m_CameraManager;
        set => m_CameraManager = value;
    }

    [SerializeField]
    CharacterController m_CharacterController;

    public CharacterController CharacterController
    {
        get => m_CharacterController;
    }

    [SerializeField]
    MovementSettings m_MovementSettings;

    public MovementSettings MovementSettings
    {
        get => m_MovementSettings;
    }

    Vector2 m_MoveInputValue;

    private void Start()
    {
        SetMovementMode(m_MovementSettings.MovementMode);
    }

    public void SetMoveInputVector(Vector2 inputValue)
    {
        m_MoveInputValue = inputValue;
    }

    public void SetMovementMode(MovementMode mode)
    {
        m_MovementSettings.MovementMode = mode;
        m_CameraManager.SetMovementMode(m_MovementSettings.MovementMode);
    }

    public void ChangeMovementMode()
    {
        switch(m_MovementSettings.MovementMode)
        {
            case MovementMode.FirstPerson:
                m_MovementSettings.MovementMode = MovementMode.ThirdPerson;
                break;
            case MovementMode.ThirdPerson:
                m_MovementSettings.MovementMode = MovementMode.FirstPerson;
                break;
        }

        m_CameraManager.SetMovementMode(m_MovementSettings.MovementMode);
    }

    private void Update()
    {
        switch (m_MovementSettings.MovementMode)
        {
            case MovementMode.FirstPerson:
                MoveFirstPerson();
                break;
            case MovementMode.ThirdPerson:
                MoveThirdPerson();
                break;
        }
    }

    private void MoveFirstPerson()
    {
        Vector3 forward = m_CameraManager.YawTransform.forward * m_MoveInputValue.y;
        Vector3 right = m_CameraManager.YawTransform.right * m_MoveInputValue.x;
        Vector3 direction = forward + right;

        m_CharacterController.Move(direction * m_MovementSettings.MoveSpeed * Time.deltaTime);
    }

    private void MoveThirdPerson()
    {
        if(m_MoveInputValue.magnitude > 0)
        {
            Vector3 forward = m_CameraManager.YawTransform.forward * m_MoveInputValue.y;
            Vector3 right = m_CameraManager.YawTransform.right * m_MoveInputValue.x;
            Vector3 direction = forward + right;

            transform.rotation = Quaternion.LookRotation(direction);

            m_CharacterController.Move(transform.forward * m_MovementSettings.MoveSpeed * Time.deltaTime);
        }
    }
}

[Serializable]
public class MovementSettings
{
    public float MoveSpeed = 2f;
    public float Gravity = -9f;
    public bool UseGravity = true;
    public MovementMode MovementMode;
}