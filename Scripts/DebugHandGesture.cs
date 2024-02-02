using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.Gestures;

public class DebugHandGesture : MonoBehaviour
{
    [SerializeField]
    XRHandTrackingEvents m_HandTrackingEvents;

    [SerializeField]
    UnityEvent m_GesturePerformed;

    [SerializeField]
    UnityEvent m_GestureEnded;

    [SerializeField]
    float m_MinimumHoldTime = 0.2f;

    [SerializeField]
    float m_GestureDetectionInterval = 0.1f;

    XRHandShape m_HandShape;
    XRHandPose m_HandPose;

    bool m_WasDetected;
    bool m_PerformedTriggered;
    float m_TimeOfLastConditionCheck;
    float m_HoldStartTime;

    public object UnityEditor { get; internal set; }

    public void OnEnable()
    {
        m_HandTrackingEvents.jointsUpdated.AddListener(OnJointsUpdated);
    }

    public void SetHandShape(XRHandShape shape)
    {
        m_HandShape = shape;
    }

    void OnDisable() => m_HandTrackingEvents.jointsUpdated.RemoveListener(OnJointsUpdated);

    void OnJointsUpdated(XRHandJointsUpdatedEventArgs eventArgs)
    {
        if (!isActiveAndEnabled || Time.timeSinceLevelLoad < m_TimeOfLastConditionCheck + m_GestureDetectionInterval)
            return;

        var detected =
            m_HandTrackingEvents.handIsTracked &&
            m_HandShape != null && m_HandShape.CheckConditions(eventArgs) ||
            m_HandPose != null && m_HandPose.CheckConditions(eventArgs);

        if (!m_WasDetected && detected)
        {
            m_HoldStartTime = Time.timeSinceLevelLoad;
        }
        else if (m_WasDetected && !detected)
        {
            m_PerformedTriggered = false;
            m_GestureEnded?.Invoke();
        }

        m_WasDetected = detected;
        if (!m_PerformedTriggered && detected)
        {
            var holdTimer = Time.timeSinceLevelLoad - m_HoldStartTime;
            if (holdTimer > m_MinimumHoldTime)
            {
                m_GesturePerformed?.Invoke();
                m_PerformedTriggered = true;
            }
        }

        m_TimeOfLastConditionCheck = Time.timeSinceLevelLoad;
    }
}