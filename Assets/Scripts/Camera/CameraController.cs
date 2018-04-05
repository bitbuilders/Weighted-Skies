using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] [Range(0.0f, 25.0f)] float m_spaceFromTarget = 10.0f;
    [SerializeField] [Range(0.0f, 30.0f)] float m_verticalOffset = 3.0f;
    [SerializeField] [Range(1.0f, 20.0f)] float m_lockIntensity = 10.0f;
    [SerializeField] [Range(1.0f, 20.0f)] float m_attentiveness = 10.0f;
    [SerializeField] [Range(1.0f, 900.0f)] float m_turnAngle = 45.0f;
    [SerializeField] GameObject m_target = null;

    Vector3 m_newPosition = Vector3.zero;
    float m_turnTime = 0.0f;

    private void Start()
    {
        m_newPosition = transform.position;
    }

    private void Update()
    {
        float turnDir = Input.GetButtonDown("RotateCamera") ? 1.0f : 0.0f;
        if (Input.GetAxis("RotateCamera") < 0.0f)
        {
            turnDir *= -1.0f;
        }

        m_turnTime += Time.deltaTime;
        if (m_turnTime > 0.0f)
        {
            m_newPosition = transform.position;
        }
        
        if (turnDir != 0.0f && m_turnTime > 0.0f)
        {
            Vector3 direction = transform.position - m_target.transform.position;
            float angle = m_turnAngle * turnDir;
            Quaternion rot = Quaternion.AngleAxis(angle, Vector3.up);
            Vector3 pos = rot * direction;
            Vector3 newPosition = m_target.transform.position + pos;
            m_newPosition = newPosition;

            m_turnTime = -0.5f;
        }
    }

    void LateUpdate()
    {
        Vector3 dir = m_newPosition - m_target.transform.position;
        dir.y = 0.0f;
        Vector3 offset = dir.normalized * m_spaceFromTarget;
        Vector3 targetPosition = m_target.transform.position + offset;
        targetPosition.y = m_target.transform.position.y + m_verticalOffset;
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * m_attentiveness);

        Vector3 lookDir = m_target.transform.position + Vector3.up * 2.0f - transform.position;
        Quaternion rotation = Quaternion.LookRotation(lookDir.normalized, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * m_lockIntensity);
    }
}
