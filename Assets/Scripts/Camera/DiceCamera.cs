using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiceCamera : Singleton<DiceCamera>
{
    [SerializeField] Player m_player = null;
    [SerializeField] Die m_die = null;
    [SerializeField] [Range(1.0f, 20.0f)] float m_trackingIntensity = 3.0f;
    [SerializeField] [Range(0.0f, 10.0f)] float m_shakeAmplitude = 8.0f;
    [SerializeField] [Range(0.0f, 50.0f)] float m_shakeRate = 20.0f;

    Vector3 m_actualPosition;
    Vector3 m_startingPosition;
    Vector3 m_offset = new Vector3(0.0f, 0.0f, 1.0f);
    Vector3 m_shake = Vector3.zero;
    float m_shakeAmount = 0.0f;

    private void Start()
    {
        m_actualPosition = transform.position;
        m_startingPosition = transform.position + (m_die.transform.position - m_player.transform.position) + m_offset;
    }

    private void Update()
    {
        m_shakeAmount -= Time.deltaTime;
        m_shakeAmount = Mathf.Clamp01(m_shakeAmount);

        float time = Time.time * m_shakeRate;

        m_shake.x = m_shakeAmount * m_shakeAmplitude * ((Mathf.PerlinNoise(time, 0.0f) * 2.0f) - 1.0f);
        m_shake.y = m_shakeAmount * m_shakeAmplitude * ((Mathf.PerlinNoise(0.0f, time) * 2.0f) - 1.0f);
        m_shake.z = 0.0f;
    }

    private void FixedUpdate()
    {
        m_actualPosition = m_die.transform.position - m_player.transform.position;
        m_actualPosition += m_startingPosition + m_offset;
        Vector3 newPosition = m_actualPosition + m_shake;

        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * m_trackingIntensity);
    }

    public void ShakeCamera(float amount, float amplitude = 0.0f, float rate = 0.0f)
    {
        if (amplitude > 0.0f)
        {
            m_shakeAmplitude = Mathf.Clamp(amplitude, 0.0f, 10.0f);
        }
        if (rate > 0.0f)
        {
            m_shakeRate = Mathf.Clamp(rate, 0.0f, 50.0f);
        }

        m_shakeAmount += amount;
    }
}
