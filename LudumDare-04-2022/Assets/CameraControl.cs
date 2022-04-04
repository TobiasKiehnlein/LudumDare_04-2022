using System;
using System.Collections;
using System.Linq;
using EntitySystem;
using Unity.VisualScripting;
using UnityEngine;
using Object = System.Object;

public class CameraControl : MonoBehaviour
{
    public float m_DampTime = 0.2f; // Approximate time for the camera to refocus.
    public float m_ScreenEdgeBuffer = 4f; // Space between the top/bottom most target and the screen edge.
    public float m_MinSize = 6.5f; // The smallest orthographic size the camera can be.
    [HideInInspector] public Vector3[] m_Targets; // All the targets the camera needs to encompass.
    private Vector3[] overrideTargets;


    private Camera m_Camera; // Used for referencing the camera.
    private float m_ZoomSpeed; // Reference speed for the smooth damping of the orthographic size.
    private Vector3 m_MoveVelocity; // Reference velocity for the smooth damping of the position.
    private Vector3 m_DesiredPosition; // The position the camera is moving towards.


    private void Awake()
    {
        m_Camera = GetComponentInChildren<Camera>();
    }

    private void Start()
    {
    }


    private void FixedUpdate()
    {
        m_Targets = overrideTargets is {Length: > 0} ? overrideTargets : FindObjectsOfType<Human>().Select(x => x.transform.position).ToArray();

        // Move the camera towards a desired position.
        Move();

        // Change the size of the camera based.
        Zoom();
    }


    private void Move()
    {
        // Find the average position of the targets.
        FindAveragePosition();

        // Smoothly transition to that position.
        transform.position = Vector3.SmoothDamp(transform.position, m_DesiredPosition, ref m_MoveVelocity, m_DampTime);
    }


    private void FindAveragePosition()
    {
        if (m_Targets.Length == 0) return;
        var maxX = m_Targets.Max(x => x.x);
        var minX = m_Targets.Min(x => x.x);
        var maxY = m_Targets.Max(x => x.y);
        var minY = m_Targets.Min(x => x.y);

        var averagePos = new Vector3((maxX + minX) / 2, (maxY + minY) / 2)
        {
            // Keep the same y value.
            z = transform.position.z
        };

        averagePos.y -= 128.4f;

        // The desired position is the average position;
        m_DesiredPosition = averagePos;
    }


    private void Zoom()
    {
        // Find the required size based on the desired position and smoothly transition to that size.
        float requiredSize = FindRequiredSize();
        m_Camera.orthographicSize = Mathf.SmoothDamp(m_Camera.orthographicSize, requiredSize, ref m_ZoomSpeed, m_DampTime);
    }


    private float FindRequiredSize()
    {
        // Find the position the camera rig is moving towards in its local space.
        Vector3 desiredLocalPos = transform.InverseTransformPoint(m_DesiredPosition);

        // Start the camera's size calculation at zero.
        float size = 0f;

        // Go through all the targets...
        foreach (var t in m_Targets)
        {
            // Otherwise, find the position of the target in the camera's local space.
            var targetLocalPos = transform.InverseTransformPoint(t);

            // Find the position of the target from the desired position of the camera's local space.
            var desiredPosToTarget = targetLocalPos - desiredLocalPos;

            // Choose the largest out of the current size and the distance of the tank 'up' or 'down' from the camera.
            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.y));

            // Choose the largest out of the current size and the calculated size based on the tank being to the left or right of the camera.
            size = Mathf.Max(size, Mathf.Abs(desiredPosToTarget.x) / m_Camera.aspect);
        }

        // Add the edge buffer to the size.
        size += m_ScreenEdgeBuffer;

        // Make sure the camera's size isn't below the minimum.
        size = Mathf.Max(size, m_MinSize);

        return size;
    }


    public void SetStartPositionAndSize()
    {
        // Find the desired position.
        FindAveragePosition();

        // Set the camera's position to the desired position without damping.
        transform.position = m_DesiredPosition;

        // Find and set the required size of the camera.
        m_Camera.orthographicSize = FindRequiredSize();
    }

    public void OverridePositions(Vector3[] targets, float duration = 5)
    {
        overrideTargets = targets;
        StartCoroutine(ResetTargets(duration));
    }

    private IEnumerator ResetTargets(float duration)
    {
        yield return new WaitForSeconds(duration);
        overrideTargets = null;
    }
}