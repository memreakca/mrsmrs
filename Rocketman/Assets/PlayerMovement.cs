using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    public int HighScore;
    public float jumpForce = 10f;
    public float jetpackForce = 5f;
    public Transform groundCheck;
    public float checkRadius = 0.2f;
    public float speedLimit = 5f;

    public float checkIsCloseToPlatform;
    public Transform startPos;

    public LayerMask groundLayer;
    public float fuel;
    public float maxFuel = 100f;
    public float fuelConsumptionRate = 10f;

    private Rigidbody2D rb;
    public bool isGrounded;
    private bool usingJetpack;
    public bool isCloseToPlatform;

    public float rotationSpeed = 100f; 
    public float maxRotationAngle = 15f;
    public float rotationResetSpeed = 50f;

    public bool isDead;

    public GameObject deadPanel;

    private Vector3 targetPosition;
    public float slideSpeed = 2f;

    [SerializeField] private TextMeshProUGUI highScoreText;
    private bool hasTriggeredPlatformEvent;
    private GameObject currentPlatform;

    [SerializeField] private Image fuelBar;
    void Start()
    {
        HighScore = 0;
        currentPlatform = null;
        fuel = maxFuel;
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isDead)
        {
            return;
        }
        highScoreText.text = $"Score = {HighScore}";
        fuelBar.fillAmount = fuel / 100;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        isCloseToPlatform = Physics2D.OverlapCircle(groundCheck.position, checkIsCloseToPlatform, groundLayer);


        if (isGrounded && Input.GetKeyDown(KeyCode.W))
        {
            Destroy(currentPlatform); 
            Debug.Log("jumped");
            rb.velocity = Vector2.up * jumpForce;
            hasTriggeredPlatformEvent = false;
        }

        if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D)) && fuel > 0 && !isGrounded)
        {
            usingJetpack = true;
        }
        else
        {
            usingJetpack = false;
        }


        if (isGrounded)
        {
            SetTargetPositionToPlatformCenter();
            transform.position = Vector3.Lerp(transform.position, targetPosition, slideSpeed * Time.deltaTime);
            fuel = maxFuel;
        }
    }

    void FixedUpdate()
    {
        
        if (usingJetpack)
        {
            if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))
            {
                ApplyJetpackForce(Vector2.up);
            }
            else if (Input.GetKey(KeyCode.A))
            {
                ApplyJetpackForce(new Vector2(1, 1).normalized);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                ApplyJetpackForce(new Vector2(-1, 1).normalized);
            }

            fuel -= fuelConsumptionRate * Time.deltaTime;
        }
        float rotation = 0f;
        if (!isCloseToPlatform)
        {
            
            if (Input.GetKey(KeyCode.A))
            {
                rotation = -rotationSpeed * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.D))
            {
                rotation = rotationSpeed * Time.deltaTime;
            }
            else if (Input.GetKey(KeyCode.D) && Input.GetKey(KeyCode.A))
            {
                rotation = rotationSpeed * Time.deltaTime;
            }
            else
            {
                float currentRotation = transform.rotation.eulerAngles.z;
                if (currentRotation > 180f)
                {
                    currentRotation -= 360f;
                }
                rotation = Mathf.MoveTowards(currentRotation, 0, rotationResetSpeed * Time.deltaTime) - currentRotation;
            }
            RotatePlayer(rotation);
        }
        else
        {
            float currentRotation = transform.rotation.eulerAngles.z;
            if (currentRotation > 180f)
            {
                currentRotation -= 360f;
            }
            rotation = Mathf.MoveTowards(currentRotation, 0, rotationResetSpeed * Time.deltaTime) - currentRotation;
            RotatePlayer(rotation);      
        }
      
    }

    void ApplyJetpackForce(Vector2 direction)
    {
        rb.AddForce(direction * jetpackForce);
    }
    void RotatePlayer(float rotation)
    {
        float currentRotation = transform.rotation.eulerAngles.z;
        if (currentRotation > 180f)
        {
            currentRotation -= 360f;
        }

        float newRotation = Mathf.Clamp(currentRotation + rotation, -maxRotationAngle, maxRotationAngle);
        transform.rotation = Quaternion.Euler(0, 0, newRotation);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        
        if (collision.relativeVelocity.magnitude > speedLimit)
        {
            KillPlayer();
        }
        else if (collision.gameObject.tag == "Platform")
        {
            if (!hasTriggeredPlatformEvent && !isDead)
            {
                hasTriggeredPlatformEvent = true;
                currentPlatform = collision.gameObject;
                PlatformEvents.PlayerLanded(collision.gameObject);
                HighScore++;
            }

        }
        else if (collision.gameObject.tag == "Death")
        {
            KillPlayer();
        }
    }
    void SetTargetPositionToPlatformCenter()
    {
        Collider2D platformCollider = Physics2D.OverlapCircle(groundCheck.position, checkIsCloseToPlatform, groundLayer);
        if (platformCollider != null)
        {
            Bounds platformBounds = platformCollider.bounds;
            Vector3 platformCenter = platformBounds.center;
            targetPosition = new Vector3(platformCenter.x, transform.position.y, transform.position.z);
        }
    }
    public void KillPlayer()
    {
        isDead = true;
        Destroy(PlatformManager.instance.deathPlatform);
        deadPanel.SetActive(true);
        Time.timeScale = 0;

    }
    public void Respawn()
    {
        
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        isDead = false;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(groundCheck.position, checkIsCloseToPlatform);
        
    }
}
