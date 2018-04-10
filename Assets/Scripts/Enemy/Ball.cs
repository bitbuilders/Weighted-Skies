using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    [SerializeField] [Range(1.0f, 900.0f)] float m_turnSpeed = 90.0f;
    [SerializeField] [Range(1.0f, 10.0f)] float m_jumpResistance = 3.0f;
    [SerializeField] [Range(1.0f, 10.0f)] float m_fallMultiplier = 3.0f;

    Rigidbody m_rigidbody;
    AudioManager m_audioManager;

    private void Start()
    {
        m_rigidbody = GetComponent<Rigidbody>();
        m_audioManager = AudioManager.Instance;
    }

    private void Update()
    {
        float multiplier = m_rigidbody.velocity.magnitude * 0.25f + 5.0f;
        float turnSpeed = m_turnSpeed * multiplier;
        Vector3 angle = Vector3.left * turnSpeed;
        Quaternion rotation = Quaternion.Euler(angle * Time.deltaTime);
        transform.rotation = rotation * transform.rotation;
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

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Death"))
        {
            StartCoroutine(StartDeactivationTimer(0.5f));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        m_audioManager.PlayClip("Bounce", transform.position, false, m_audioManager.transform);
        if (collision.gameObject.CompareTag("Player"))
        {
            if (!collision.gameObject.GetComponent<Player>().m_hasWon)
                Game.Instance.EndGame();
        }
    }

    IEnumerator StartDeactivationTimer(float time)
    {
        yield return new WaitForSeconds(time);

        gameObject.SetActive(false);
    }
}
