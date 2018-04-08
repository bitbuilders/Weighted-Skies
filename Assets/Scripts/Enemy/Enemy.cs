using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] [Range(1.0f, 50.0f)] float m_speed = 5.0f;
    [SerializeField] [Range(0.0f, 10.0f)] float m_fireRate = 1.0f;
    [SerializeField] [Range(1.0f, 100.0f)] float m_launchForce = 25.0f;
    [SerializeField] [Range(-90.0f, 90.0f)] float m_launchAngle = -45.0f;
    [SerializeField] Transform m_shotLocation = null;
    [SerializeField] Transform m_minPos = null;
    [SerializeField] Transform m_maxPos = null;

    BallPool m_ballPool;
    Animator m_animator;
    Vector3 m_targetPosition;
    Vector3 m_pickPosition;
    float m_shotTime = 0.0f;
    float m_direction = 1.0f;

    private void Start()
    {
        m_animator = GetComponent<Animator>();
        m_ballPool = BallPool.Instance;
        m_targetPosition = transform.position;
        m_pickPosition = m_targetPosition;
    }

    private void Update()
    {
        IncrementPickPosition();

        m_shotTime += Time.deltaTime;
        if (m_shotTime >= m_fireRate)
        {
            m_shotTime = 0.0f;

            m_animator.SetTrigger("Shoot");
            PickNewPosition();
        }
        
        Vector3 direction = m_targetPosition - transform.position;
        Vector3 velocity = direction.normalized * m_speed;
        velocity = velocity * Time.deltaTime;
        transform.position += velocity;
    }

    private void IncrementPickPosition()
    {
        m_pickPosition.x += m_speed * Time.deltaTime * m_direction;

        if (m_pickPosition.x >= m_maxPos.position.x)
        {
            m_direction = -1.0f;
        }
        else if (m_pickPosition.x <= m_minPos.position.x)
        {
            m_direction = 1.0f;
        }
    }

    private void PickNewPosition()
    {
        float min = m_pickPosition.x - 2.0f;
        float max = m_pickPosition.x + 2.0f;
        float newX = Random.Range(min, max);
        m_targetPosition.x = newX;
    }

    public void Shoot()
    {
        GameObject ball = m_ballPool.Get();
        ball.SetActive(true);
        ball.transform.position = m_shotLocation.position;
        Rigidbody rigidbody = ball.GetComponent<Rigidbody>();
        rigidbody.velocity = Vector3.zero;

        Vector3 force = transform.forward * m_launchForce;
        Quaternion arcAngle = Quaternion.Euler(Vector3.right * m_launchAngle);
        force = arcAngle * force;

        rigidbody.AddForce(force, ForceMode.Impulse);
    }
}
