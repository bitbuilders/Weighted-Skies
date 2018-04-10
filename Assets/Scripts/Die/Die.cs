using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Die : MonoBehaviour
{
    [SerializeField] Transform m_side1 = null;
    [SerializeField] Transform m_side2 = null;
    [SerializeField] Transform m_side3 = null;
    [SerializeField] Transform m_side4 = null;
    [SerializeField] Transform m_side5 = null;
    [SerializeField] Transform m_side6 = null;
    [SerializeField] LayerMask m_groundMask = 0;
    [SerializeField] [Range(1.0f, 10.0f)] float m_jumpResistance = 3.0f;
    [SerializeField] [Range(1.0f, 10.0f)] float m_fallMultiplier = 3.0f;

    public int Value { get { return GetSide(); } }
    public bool Stationary { get { return m_stationary; } }

    const float DEAD_SPEED = 0.5f;
    const float UPDATE_TIMER = 1.0f;

    Rigidbody m_rigidbody;
    bool m_stationary = true;
    float m_updateTime = 0.0f;
    float m_xRotation = 0.0f;
    float m_zRotation = 0.0f;
    float m_xRotationSpeed = 50.0f;
    float m_zRotationSpeed = 25.0f;

    private void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();

        m_stationary = false;
        m_rigidbody.useGravity = false;
    }

    private void Update()
    {
        m_updateTime += Time.deltaTime;
        
        if (m_updateTime >= UPDATE_TIMER && m_rigidbody.useGravity)
        {
            m_updateTime = 0.0f;

            Vector3 speed = m_rigidbody.velocity;
            m_stationary = speed.magnitude <= DEAD_SPEED;
        }

        Vector3 angle = new Vector3(m_xRotation * m_xRotationSpeed, 0.0f, m_zRotation * m_zRotationSpeed);
        Quaternion rotation = Quaternion.Euler(angle * Time.deltaTime);
        transform.rotation = rotation * transform.rotation;

        m_xRotation -= Time.deltaTime;
        m_zRotation -= Time.deltaTime;

        m_xRotation = Mathf.Clamp(m_xRotation, 0.0f, 10.0f);
        m_zRotation = Mathf.Clamp(m_zRotation, 0.0f, 10.0f);
    }

    private void FixedUpdate()
    {
        if (m_rigidbody.velocity.y > 0.1f)
        {
            m_rigidbody.velocity += (Vector3.up * Physics.gravity.y) * (m_jumpResistance - 1.0f) * Time.deltaTime;
        }
        else if (m_rigidbody.velocity.y < -0.1f)
        {
            m_rigidbody.velocity += (Vector3.up * Physics.gravity.y) * (m_fallMultiplier - 1.0f) * Time.deltaTime;
        }
    }

    private int GetSide()
    {
        int side = 0;

        if (IsSideTouching(m_side1)) side = 6;
        else if (IsSideTouching(m_side2)) side = 5;
        else if (IsSideTouching(m_side3)) side = 4;
        else if (IsSideTouching(m_side4)) side = 3;
        else if (IsSideTouching(m_side5)) side = 2;
        else if (IsSideTouching(m_side6)) side = 1;

        return side;
    }

    private bool IsSideTouching(Transform side)
    {
        Collider[] points = Physics.OverlapSphere(side.position, 0.2f, m_groundMask);

        return points.Length > 0;
    }

    public void Throw(Vector3 force)
    {
        m_rigidbody.useGravity = true;

        m_rigidbody.AddForce(force, ForceMode.Impulse);

        m_xRotation = 1.5f;
        m_zRotation = 1.5f;

        m_xRotationSpeed = Random.Range(90.0f, 360.0f);
        int x = Random.Range(0, 2) == 1 ? -1 : 1;
        m_zRotationSpeed = Random.Range(45.0f, 180.0f) * x;
    }

    public void Shake(Vector3 upwardForce)
    {
        m_rigidbody.AddForce(upwardForce, ForceMode.Impulse);

        m_xRotation = 1.5f;
        m_zRotation = 1.5f;

        m_xRotationSpeed = Random.Range(-180.0f, 180.0f);
        m_zRotationSpeed = Random.Range(-90.0f, 90.0f);
    }
}
