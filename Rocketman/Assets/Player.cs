using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Player: MonoBehaviour
{
    public static Player instance;

    public int HighScore;

    [Header("Jetpack Attributes")]
    public float jumpForce = 10f;
    public float jetpackForce = 5f;
    public float checkRadius = 0.2f;
    public float speedLimit = 5f;
    public float fuel;
    public float maxFuel = 100f;
    public float fuelConsumptionRate = 10f;

    [Header("Checks")]
    public LayerMask groundLayer;
    public Transform groundCheck;
    public Transform startPos;
    public bool isGrounded;
    public float checkIsCloseToPlatform;
    public bool isCloseToPlatform;
    public bool isDead;
    private bool usingJetpack;
    private Vector3 targetPosition;
    public float slideSpeed = 2f;
    public float jumpInterval;
    public bool canJump;
    private float jumpTimer;

    private GameObject currentPlatform;
    private bool hasTriggeredPlatformEvent;

    [Header("---------")]
    private Rigidbody2D rb;
    public float rotationSpeed = 100f;
    public float maxRotationAngle = 15f;
    public float rotationResetSpeed = 50f;

    [Header("UI")]
    public GameObject deadPanel;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private Image fuelBar;
    

    [Header("AnimationSprites")]
    [SerializeField] private SpriteRenderer playerSprite;
    [SerializeField] private Sprite midAirSprite;
    [SerializeField] private Sprite landedSprite;
    [SerializeField] private GameObject LeftRocketEffect;
    [SerializeField] private GameObject RightRocketEffect;

    private void Awake()
    {
        instance = this;
    }
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


        if (isGrounded && Input.GetKeyDown(KeyCode.Space) && canJump)
        {
            
            JumpedFromPlatform();
            rb.velocity = Vector2.up * jumpForce;
            hasTriggeredPlatformEvent = false;
            jumpTimer = jumpInterval;
            canJump = false;
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
            jumpTimer -= Time.deltaTime;

            if (jumpTimer < 0)
            {
                canJump = true;
            }
            SetTargetPositionToPlatformCenter();
            transform.position = Vector3.Lerp(transform.position, targetPosition, slideSpeed * Time.deltaTime);
            fuel = maxFuel;
        }
       
    }
    public void JumpedFromPlatform()
    {
        Invoke("DestroyPlatform", 2);
        Rigidbody2D pltfrmrb = currentPlatform.GetComponent<Rigidbody2D>();
        pltfrmrb.velocity = Vector2.down * jumpForce;
    }
    public void DestroyPlatform()
    {
        Destroy(currentPlatform);
    }
    void FixedUpdate()
    {

        if (usingJetpack)
        {
            if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))
            {
                LeftRocketEffect.SetActive(true);
                RightRocketEffect.SetActive(true);
                ApplyJetpackForce(Vector2.up);
            }
            else if (Input.GetKey(KeyCode.A))
            {
                LeftRocketEffect.SetActive(true);
                ApplyJetpackForce(new Vector2(1, 1).normalized);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                RightRocketEffect.SetActive(true);
                ApplyJetpackForce(new Vector2(-1, 1).normalized);
            }

            fuel -= fuelConsumptionRate * Time.deltaTime;
        }
        else
        {
            LeftRocketEffect.SetActive(false);
            RightRocketEffect.SetActive(false);
        }
        float rotation = 0f;
        if (!isCloseToPlatform)
        {

            playerSprite.sprite = midAirSprite;
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

            playerSprite.sprite = landedSprite;
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
