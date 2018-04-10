using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public struct DifficultyValue
    {
        public DifficultyValue(float s, float lMin, float lMax, float fR, float lF, float lA)
        {
            speed = s;
            laserMin = lMin;
            laserMax = lMax;
            fireRate = fR;
            launchForce = lF;
            launchAngle = lA;
        }

        public float speed;
        public float laserMin;
        public float laserMax;
        public float fireRate;
        public float launchForce;
        public float launchAngle;
    }

    [SerializeField] [Range(1.0f, 50.0f)] float m_speed = 5.0f;
    [SerializeField] [Range(0.0f, 10.0f)] float m_laserMinSpawnRate = 2.0f;
    [SerializeField] [Range(0.0f, 10.0f)] float m_laserMaxSpawnRate = 4.0f;
    [SerializeField] [Range(0.0f, 10.0f)] float m_fireRate = 1.0f;
    [SerializeField] [Range(1.0f, 500.0f)] float m_launchForce = 25.0f;
    [SerializeField] [Range(-90.0f, 90.0f)] float m_launchAngle = -45.0f;
    [SerializeField] Transform m_shotLocation = null;
    [SerializeField] Transform m_minPos = null;
    [SerializeField] Transform m_maxPos = null;

    DifficultyValue[] m_difficultyLevels = new DifficultyValue[]
    {
        new DifficultyValue(70.0f, 1.25f, 2.25f, 0.4f, 110.0f, 35.0f),
        new DifficultyValue(60.0f, 1.40f, 2.6f, 0.52f, 115.0f, 37.0f),
        new DifficultyValue(50.0f, 1.55f, 2.95f, 0.64f, 120.0f, 39.0f),
        new DifficultyValue(40.0f, 1.7f, 3.3f, 0.76f, 125.0f, 41.0f),
        new DifficultyValue(30.0f, 1.85f, 3.65f, 0.88f, 130.0f, 43.0f),
        new DifficultyValue(20.0f, 2.0f, 4.0f, 1.0f, 135.0f, 45.0f)
    };

    BallPool m_ballPool;
    LaserPool m_laserPool;
    Animator m_animator;
    Vector3 m_targetPosition;
    Vector3 m_pickPosition;
    float m_laserSpawnRate = 2.5f;
    float m_laserSpawnTime = 0.0f;
    float m_shotTime = -0.25f;
    float m_direction = 1.0f;

    private void Start()
    {
        m_animator = GetComponent<Animator>();
        m_ballPool = BallPool.Instance;
        m_laserPool = LaserPool.Instance;
        m_targetPosition = transform.position;
        m_pickPosition = m_targetPosition;
        
        UpdateDifficulty();
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

        m_laserSpawnTime += Time.deltaTime;
        if (m_laserSpawnTime >= m_laserSpawnRate)
        {
            m_laserSpawnTime = 0.0f;
            GetNewSpawnRate();

            GameObject laser = m_laserPool.Get();
            laser.SetActive(true);
            laser.GetComponent<Laser>().Spawn();
        }

        float distance = m_targetPosition.x - transform.position.x;
        if (Mathf.Abs(distance) > 0.1f)
        {
            Vector3 direction = m_targetPosition - transform.position;
            Vector3 velocity = direction.normalized * m_speed;
            velocity = velocity * Time.deltaTime;
            transform.position += velocity;
        }

        m_animator.SetFloat("FireRate", (1.0f - m_fireRate) + 1.0f);
    }

    private void UpdateDifficulty()
    {
        SetDifficulty(m_difficultyLevels[DifficultyManager.Instance.Difficulty - 1]);
    }

    private void SetDifficulty(DifficultyValue value)
    {
        m_speed = value.speed;
        m_laserMinSpawnRate = value.laserMin;
        m_laserMaxSpawnRate = value.laserMax;
        m_fireRate = value.fireRate;
        m_launchForce = value.launchForce;
        m_launchAngle = value.launchAngle;
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

    private void GetNewSpawnRate()
    {
        m_laserSpawnRate = Random.Range(m_laserMinSpawnRate, m_laserMaxSpawnRate);
    }

    private void PickNewPosition()
    {
        float min = m_pickPosition.x - 2.0f;
        float max = m_pickPosition.x + 2.0f;
        float newX = Random.Range(min, max);
        newX = Mathf.Clamp(newX, m_minPos.position.x, m_maxPos.position.x);
        m_targetPosition.x = newX;
    }

    public void Shoot()
    {
        m_shotTime = 0.0f;
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
