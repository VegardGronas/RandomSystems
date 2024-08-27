using System.Collections.Generic;
using System;
using UnityEngine;

public enum CameraLookMode { Slerp, Instant }
public class CameraManager : MonoBehaviour
{
    [SerializeField]
    private Camera m_Camera;

    public Camera Camera
    {
        get => m_Camera;
        set => m_Camera = value;
    }

    [SerializeField]
    private MovementManager m_MovementManager;

    public MovementManager MovementManager
    {
        get => m_MovementManager;
    }

    [SerializeField]
    private Transform m_CameraSlot;

    public Transform CameraSlot => m_CameraSlot;

    [SerializeField]
    private Transform m_YawTransform;

    public Transform YawTransform => m_YawTransform;

    [SerializeField]
    private Transform m_PitchTransform;

    public Transform PitchTransform => m_PitchTransform;

    [SerializeField]
    private FirstPersonCameraSettings m_FirstPersonCameraSettings;

    public FirstPersonCameraSettings FirstPersonCameraSettings
    {
        get => m_FirstPersonCameraSettings;
    }

    [SerializeField]
    private ThirdPersonCameraSettings m_ThirdPersonCameraSettings;

    public ThirdPersonCameraSettings ThirdPersonCameraSettings
    {
        get => m_ThirdPersonCameraSettings;
    }

    [SerializeField]
    private CameraSettings m_CameraSettings;

    public CameraSettings CameraSettings => m_CameraSettings;

    private float m_CurrentPitch;
    private float m_CurrentYaw;

    private Vector3 m_YawTargetRotation;
    private Vector3 m_PitchTargetRotation;

    private CameraLookMode currentCameraLookMode;
    private MovementMode currentMovementMode;

    private void Awake()
    {
        // Ensure all required components are not null
        Debug.Assert(m_MovementManager != null, "MovementManager is not assigned!");
        Debug.Assert(m_CameraSettings != null, "CameraSettings is not assigned!");

        // Initialize the current camera look mode based on the initial movement mode
        UpdateCameraLookMode();

        if (m_Camera == null) m_Camera = Camera.main;
        SlotCamera();
    }

    private void Start()
    {
        UpdateCameraLookMode();
    }

    private void SlotCamera()
    {
        if (m_CameraSlot != null)
        {
            m_Camera.transform.SetParent(m_CameraSlot.transform);
            m_Camera.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(Vector3.zero));
        }
    }

    public void SetMovementMode(MovementMode mode)
    {
        if (m_CameraSlot != null)
        {
            switch (mode)
            {
                case MovementMode.FirstPerson:
                    m_CameraSlot.localPosition = Vector3.zero;

                    foreach(GameObject obj in m_FirstPersonCameraSettings.ObjectsToHide)
                    {
                        obj.SetActive(false);
                    }

                    foreach (GameObject obj in m_FirstPersonCameraSettings.ObjectsToShow)
                    {
                        obj.SetActive(true);
                    }

                    break;
                case MovementMode.ThirdPerson:
                    m_CameraSlot.localPosition = m_CameraSettings.thirdPersonView;


                    foreach (GameObject obj in m_ThirdPersonCameraSettings.ObjectsToHide)
                    {
                        obj.SetActive(false);
                    }

                    foreach (GameObject obj in m_ThirdPersonCameraSettings.ObjectsToShow)
                    {
                        obj.SetActive(true);
                    }

                    break;
            }
        }
    }

    public void Rotate(float yaw, float pitch)
    {
        if (Mathf.Abs(yaw) > 0)
        {
            YawRotate(yaw);
        }

        if (Mathf.Abs(pitch) > 0)
        {
            PitchRotate(pitch);
        }
    }

    public void YawRotate(float value)
    {
        switch (m_MovementManager.MovementSettings.MovementMode)
        {
            case MovementMode.FirstPerson:
                m_CurrentYaw += value * m_FirstPersonCameraSettings.horizontalMouseSensitivity;
                break;
            case MovementMode.ThirdPerson:
                m_CurrentYaw += value * m_ThirdPersonCameraSettings.horizontalMouseSensitivity;
                break;
        }

        m_YawTargetRotation = new Vector3(0, m_CurrentYaw, 0);
    }

    public void PitchRotate(float value)
    {
        switch (m_MovementManager.MovementSettings.MovementMode)
        {
            case MovementMode.FirstPerson:
                m_CurrentPitch += value * m_FirstPersonCameraSettings.verticalMouseSensitivity;
                break;
            case MovementMode.ThirdPerson:
                m_CurrentPitch += value * m_ThirdPersonCameraSettings.verticalMouseSensitivity;
                break;
        }

        m_CurrentPitch = Mathf.Clamp(m_CurrentPitch, m_CameraSettings.minClampedPitch, m_CameraSettings.maxClampedPitch);
        m_PitchTargetRotation = new Vector3(m_CurrentPitch, 0, 0);
    }

    public void SetLocation(Vector3 newLocation)
    {
        transform.position = newLocation + m_CameraSettings.riggOffset;
    }

    private void UpdateCameraLookMode()
    {
        currentMovementMode = m_MovementManager.MovementSettings.MovementMode;

        switch (currentMovementMode)
        {
            case MovementMode.FirstPerson:
                currentCameraLookMode = m_FirstPersonCameraSettings.CameraUpdateLookMode;
                break;
            case MovementMode.ThirdPerson:
                currentCameraLookMode = m_ThirdPersonCameraSettings.CameraUpdateLookMode;
                break;
        }
    }

    private void Update()
    {
        // Update the camera look mode if the movement mode changes
        if (HasMovementModeChanged()) // Implement this method if needed
        {
            UpdateCameraLookMode();
        }

        // Apply the rotation based on the cached camera look mode
        switch (currentCameraLookMode)
        {
            case CameraLookMode.Instant:
                InstantRotation();
                break;
            case CameraLookMode.Slerp:
                SlerpRotation();
                break;
        }
    }

    private bool HasMovementModeChanged()
    {
        if(currentMovementMode != m_MovementManager.MovementSettings.MovementMode)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void SlerpRotation()
    {
        if (m_PitchTransform != null)
        {
            m_PitchTransform.localRotation = Quaternion.Slerp(m_PitchTransform.localRotation, Quaternion.Euler(m_PitchTargetRotation), m_CameraSettings.SlerpRotationSpeed * Time.deltaTime);
        }

        if (m_YawTransform != null)
        {
            m_YawTransform.rotation = Quaternion.Slerp(m_YawTransform.rotation, Quaternion.Euler(m_YawTargetRotation), m_CameraSettings.SlerpRotationSpeed * Time.deltaTime);
        }
    }

    private void InstantRotation()
    {
        if (m_PitchTransform != null)
        {
            m_PitchTransform.localRotation = Quaternion.Euler(m_PitchTargetRotation);
        }

        if (m_YawTransform != null)
        {
            m_YawTransform.rotation = Quaternion.Euler(m_YawTargetRotation);
        }
    }
}

[Serializable]
public class ThirdPersonCameraSettings
{
        /// <summary>
    /// Sensitivity of the camera's horizontal movement based on mouse input.
    /// </summary>
    public float horizontalMouseSensitivity = .5f;

    /// <summary>
    /// Sensitivity of the camera's vertical movement based on mouse input.
    /// </summary>
    public float verticalMouseSensitivity = .5f;

    /// <summary>
    /// Whether to use smooth (slerp) rotation or instant rotation.
    /// </summary>
    public CameraLookMode CameraUpdateLookMode = CameraLookMode.Instant;

    /// <summary>
    /// If using a rigged character you can hide for example their body
    /// </summary>
    public List<GameObject> ObjectsToHide;

    /// <summary>
    /// Add objects here to re enable or show objects when in first person
    /// </summary>
    public List<GameObject> ObjectsToShow;
}

[Serializable]
public class FirstPersonCameraSettings
{
    /// <summary>
    /// Sensitivity of the camera's horizontal movement based on mouse input.
    /// </summary>
    public float horizontalMouseSensitivity = .5f;

    /// <summary>
    /// Sensitivity of the camera's vertical movement based on mouse input.
    /// </summary>
    public float verticalMouseSensitivity = .5f;

    /// <summary>
    /// Whether to use smooth (slerp) rotation or instant rotation.
    /// </summary>
    public CameraLookMode CameraUpdateLookMode = CameraLookMode.Instant;

    /// <summary>
    /// If using a rigged character you can hide for example their body
    /// </summary>
    public List<GameObject> ObjectsToHide;

    /// <summary>
    /// Add objects here to re enable or show objects when in third person
    /// </summary>
    public List<GameObject> ObjectsToShow;
}

[Serializable]
public class CameraSettings
{
    /// <summary>
    /// Maximum clamping value for the camera's pitch rotation.
    /// </summary>
    public float maxClampedPitch = 90;

    /// <summary>
    /// Minimum clamping value for the camera's pitch rotation.
    /// </summary>
    public float minClampedPitch = -90;

    /// <summary>
    /// Position of the camera in third-person view relative to the player.
    /// </summary>
    public Vector3 thirdPersonView = new(0, 0, -2.5f);

    /// <summary>
    /// Offset of the camera rig from the player's position.
    /// </summary>
    public Vector3 riggOffset = new(0, 1, 0);

    /// <summary>
    /// Speed of the smooth (slerp) rotation. Adjusted in the Inspector.
    /// </summary>
    [Range(1, 30)]
    public float SlerpRotationSpeed = 15f;
}