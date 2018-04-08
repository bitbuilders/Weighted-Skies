using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] Player m_player = null;
    [SerializeField] [Range(1.0f, 20.0f)] float m_attentiveness = 4.0f;
    [SerializeField] [Range(0.0f, 50.0f)] float m_distanceFromPlayer = 15.0f;
    [SerializeField] [Range(0.0f, 10.0f)] float m_shakeAmplitude = 8.0f;
    [SerializeField] [Range(0.0f, 50.0f)] float m_shakeRate = 20.0f;

    Vector3 m_actualPosition;
    Vector3 m_startingDirection;
    Vector3 m_startingPosition;
    Vector3 m_shake = Vector3.zero;
    float m_shakeAmount = 0.0f;

    private void Start()
    {
        m_actualPosition = transform.position;
        m_startingPosition = transform.position;
        Vector3 pos = m_actualPosition;
        pos.y = 0.0f;
        m_startingDirection = pos - m_player.transform.position;
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
        Vector3 position = m_startingDirection.normalized * m_distanceFromPlayer;
        position = m_player.transform.position + position + (Vector3.up * m_startingPosition.y);

        m_actualPosition = position;
    }

    private void LateUpdate()
    {
        Vector3 position = m_actualPosition + m_shake;

        transform.position = Vector3.Lerp(transform.position, position, Time.deltaTime * m_attentiveness);
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
