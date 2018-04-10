using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] [Range(1.0f, 100.0f)] float m_speed = 50.0f;
    [SerializeField] [Range(1.0f, 100.0f)] float m_jumpForce = 25.0f;
    [SerializeField] [Range(1.0f, 100.0f)] float m_throwForce = 15.0f;
    [SerializeField] [Range(-90.0f, 0.0f)] float m_throwAngle = -35.0f;
    [SerializeField] [Range(1.0f, 10.0f)] float m_jumpResistance = 3.0f;
    [SerializeField] [Range(1.0f, 10.0f)] float m_fallMultiplier = 3.0f;
    [SerializeField] [Range(1.0f, 10.0f)] float m_diceCheckRate = 3.0f;
    [SerializeField] [Range(1, 100)] public int m_throws = 1;
    [SerializeField] [Range(1, 100)] public int m_slams = 1;
    [SerializeField] [Range(1, 10)] public int m_numOfJumps = 1;
    [SerializeField] bool m_canMove = true;
    [SerializeField] bool m_canJump = true;
    [SerializeField] Transform m_groundTouchPoint = null;
    [SerializeField] LayerMask m_groundMask = 0;
    [SerializeField] Die m_die = null;
    [SerializeField] GameObject m_pauseMenu = null;

    Game m_game;
    Rigidbody m_rigidbody;
    Animator m_animator;
    bool m_onGround = true;
    public bool m_hasWon = false;
    int m_jumps;
    float m_diceTime = 0.0f;

    bool OnGround { get { return m_onGround; } set { m_onGround = value; } }

    private void Start()
    {
        m_game = Game.Instance;
        m_rigidbody = GetComponent<Rigidbody>();
        m_animator = GetComponent<Animator>();
        m_jumps = m_numOfJumps;
        m_slams = m_game.m_playerSlams;
        m_game.StartTimer();
        m_hasWon = false;
    }

    private void Update()
    {
        if (!m_game.Paused)
        {
            if (Input.GetButtonDown("Jump") && m_jumps > 0 && m_canJump)
            {
                Vector3 force = Vector3.up * m_jumpForce;
                m_rigidbody.AddForce(force, ForceMode.Impulse);
                m_jumps--;
            }
            else if (Input.GetButtonDown("Jab") && m_throws > 0)
            {
                m_animator.SetTrigger("Jab");
                m_throws--;
            }
            else if (Input.GetButtonDown("Heavy Slice"))
            {
                m_animator.SetTrigger("Windmill");
            }
            else if (Input.GetButtonDown("Slam") && m_slams > 0)
            {
                m_animator.SetTrigger("Slam");
                m_slams--;
            }
            else if (Input.GetButtonDown("Ability"))
            {
                m_animator.SetTrigger("Ability");
            }
            else if (Input.GetButtonDown("Pause"))
            {
                m_pauseMenu.SetActive(true);
            }

            if (m_die && m_die.Stationary)
            {
                m_diceTime += Time.deltaTime;

                if (m_diceTime >= m_diceCheckRate)
                {
                    DifficultyManager.Instance.Difficulty = m_die.Value;
                    m_game.StartLevelTransition("Level1", DifficultyManager.Instance.GetDifficultyWord());
                    gameObject.SetActive(false);
                }
            }
            else
            {
                m_diceTime = 0.0f;
            }
        }
    }

    public void Unpause()
    {
        m_game.Unpause();
        m_pauseMenu.SetActive(false);
    }

    public void MainMenu()
    {
        m_game.LoadScene("MainMenu");
    }

    private void FixedUpdate()
    {
        if (m_canMove)
        {
            Vector3 velocity = Vector3.zero;
            velocity.z = Input.GetAxis("Vertical");
            velocity.x = Input.GetAxis("Horizontal");
            velocity = velocity * m_speed * 100.0f * Time.deltaTime;
            m_rigidbody.AddForce(velocity, ForceMode.Force);

            if (velocity.magnitude > 0.0f)
            {
                Quaternion rotation = Quaternion.LookRotation(velocity, Vector3.up);
                transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * 3.0f);
            }

            if (m_rigidbody.velocity.y > 0.1f)
            {
                m_rigidbody.velocity += (Vector3.up * Physics.gravity.y) * (m_jumpResistance - 1.0f) * Time.deltaTime;
            }
            else if (m_rigidbody.velocity.y < -0.1f)
            {
                m_rigidbody.velocity += (Vector3.up * Physics.gravity.y) * (m_fallMultiplier - 1.0f) * Time.deltaTime;
            }

            m_animator.SetFloat("yVelocity", m_rigidbody.velocity.y);

            Collider[] points = Physics.OverlapSphere(m_groundTouchPoint.position, 0.2f, m_groundMask);
            m_onGround = points.Length > 0;
            m_animator.SetBool("OnGround", OnGround);

            Vector3 speed = new Vector3(m_rigidbody.velocity.x, 0.0f, m_rigidbody.velocity.z);
            float walkSpeed = speed.magnitude;
            float runSpeed = walkSpeed - 8.0f;
            if (walkSpeed <= 0.01f) walkSpeed = 0.0f;

            float animationSpeedWalk = walkSpeed * 0.5f;
            float animationSpeedRun = walkSpeed * 0.13f;

            m_animator.SetFloat("Walking", walkSpeed);
            m_animator.SetFloat("Running", runSpeed);
            m_animator.SetFloat("WalkSpeed", animationSpeedWalk);
            m_animator.SetFloat("RunSpeed", animationSpeedRun);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (OnGround)
            {
                m_jumps = m_numOfJumps;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Finish"))
        {
            m_hasWon = true;
            EndLevel.Instance.EndFade();
        }
    }

    public void Jab()
    {
        if (m_die != null)
        {
            Vector3 force = m_throwForce * transform.forward;
            Quaternion arcRotation = Quaternion.Euler(Vector3.right * m_throwAngle);
            force = arcRotation * force;

            m_die.Throw(force);
        }
    }

    public void Slam()
    {
        if (m_die != null)
        {
            Vector3 force = (m_throwForce * 0.75f) * Vector3.up;

            m_die.Shake(force);

            DiceCamera.Instance.ShakeCamera(0.4f, 13.0f, 5.0f);
        }
    }
}
