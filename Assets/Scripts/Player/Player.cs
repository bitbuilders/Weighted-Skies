﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] [Range(1.0f, 100.0f)] float m_speed = 50.0f;
    [SerializeField] [Range(1.0f, 900.0f)] float m_turnSpeed = 90.0f;
    [SerializeField] [Range(1.0f, 100.0f)] float m_jumpForce = 25.0f;
    [SerializeField] [Range(1.0f, 10.0f)] float m_jumpResistance = 3.0f;
    [SerializeField] [Range(1.0f, 10.0f)] float m_fallMultiplier = 3.0f;
    [SerializeField] Transform m_groundTouchPoint = null;
    [SerializeField] LayerMask m_groundMask = 0;

    Rigidbody m_rigidbody;
    Animator m_animator;
    private bool m_onGround = true;

    bool OnGround { get { return m_onGround; } set { m_onGround = value; } }

    private void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Jump"))
        {
            Vector3 force = Vector3.up * m_jumpForce;
            m_rigidbody.AddForce(force, ForceMode.Impulse);
        }
    }

    private void FixedUpdate()
    {
        Vector3 turnAngle = Vector3.up * Input.GetAxis("Horizontal") *  m_turnSpeed;
        Quaternion rotation = Quaternion.Euler(turnAngle * Time.deltaTime);
        transform.rotation *= rotation;

        Vector3 velocity = Vector3.zero;
        velocity.z = Input.GetAxis("Vertical");
        velocity = velocity * m_speed * 100.0f * Time.deltaTime;
        m_rigidbody.AddRelativeForce(velocity, ForceMode.Force);

        if (m_rigidbody.velocity.y > 0.0f)
        {
            m_rigidbody.velocity += (Vector3.up * Physics.gravity.y) * (m_jumpResistance - 1.0f) * Time.deltaTime;
        }
        else if (m_rigidbody.velocity.y < 0.0f)
        {
            m_rigidbody.velocity += (Vector3.up * Physics.gravity.y) * (m_fallMultiplier - 1.0f) * Time.deltaTime;
        }

        m_animator.SetFloat("yVelocity", m_rigidbody.velocity.y);

        Collider[] points = Physics.OverlapSphere(m_groundTouchPoint.position, 0.2f, m_groundMask);
        m_onGround = points.Length > 0;
        m_onGround = true;
        m_animator.SetBool("OnGround", OnGround);
    }
}
