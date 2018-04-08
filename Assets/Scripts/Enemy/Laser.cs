using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    [SerializeField] [Range(1.0f, 1000.0f)] float m_speed = 20.0f;
    [SerializeField] [Range(1.0f, 20.0f)] float m_spawnTime = 5.0f;
    [SerializeField] Transform m_destination = null;
    [SerializeField] Transform m_start = null;

    Material m_material;
    Color m_color;
    float m_opacity;
    bool m_paused = true;

    private void Start()
    {

    }

    private void OnEnable()
    {
        m_material = new Material(GetComponent<Renderer>().material);
        GetComponent<Renderer>().material = m_material;
        m_color = m_material.GetColor("_TintColor");
        if (m_color.a > 0.05f) m_opacity = m_color.a;
        m_color.a = 0.0f;
        SetColor();
    }

    private void Update()
    {
        if (!m_paused)
        {
            Vector3 direction = m_destination.position - transform.position;
            Vector3 velocity = direction.normalized * m_speed;
            velocity = velocity * Time.deltaTime;

            transform.position += velocity;
        }
    }

    public void Spawn()
    {
        //print("HI");
        ResetPosition();
        StartCoroutine(FadeAndRaid());
    }

    IEnumerator FadeAndRaid()
    {
        for (float i = 0.0f; i <= m_opacity; i += Time.deltaTime / m_spawnTime)
        {
            m_color.a = i;
            SetColor();
            //print(m_material.GetColor("_TintColor").a);

            yield return null;
        }

        m_color.a = m_opacity;
        SetColor();

        ResetPosition();
        m_paused = false;
    }

    private void ResetPosition()
    {
        transform.position = m_start.position;
    }

    private void SetColor()
    {
        m_material.SetColor("_TintColor", m_color);
    }
}
