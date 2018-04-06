using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceCamera : MonoBehaviour
{
    [SerializeField] Player m_player = null;
    [SerializeField] Die m_die = null;
    [SerializeField] [Range(1.0f, 20.0f)] float m_trackingIntensity = 3.0f;

    Vector3 m_startingPosition;
    Vector3 m_offset = new Vector3(0.0f, 0.0f, 1.0f);

    private void Start()
    {
        m_startingPosition = transform.position + (m_die.transform.position - m_player.transform.position) + m_offset;
    }

    private void LateUpdate()
    {
        Vector3 newPosition = m_die.transform.position - m_player.transform.position;
        newPosition += m_startingPosition + m_offset;

        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * m_trackingIntensity);
    }
}
