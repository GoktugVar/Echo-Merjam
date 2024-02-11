using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum Ground
{
    Dirt,
    Gritty,
    Stone,
    StoneAlt,
    Wet,
    Wood
}

[System.Serializable]
public class FootStepSound
{
    public Ground Ground;
    public AudioClip[] FootStepSounds;
}

public class PlayerMovement : Singleton<PlayerMovement>
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float footstepInterval = 0.5f;
    [SerializeField] private float shiftSpeedMultiplier = 0.5f;
    [SerializeField] private float prepareJumpTime = 0.5f;
    [SerializeField] private float jumpCooldown = 1f;

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip trapSound;
    [SerializeField] private AudioClip crashedSound;
    [SerializeField] private AudioClip[] jumpSound;
    [SerializeField] private FootStepSound[] footStepSounds;

    [Header("Footstep Objects")]
    [SerializeField] private GameObject leftFoot;
    [SerializeField] private GameObject leftFootShift;
    [SerializeField] private GameObject rightFoot;
    [SerializeField] private GameObject rightFootShift;
    [SerializeField] private GameObject SpaceBar;

    [Header("Rock Settings")]
    [SerializeField] private GameObject rockPrefab;

    [Header("UI Settings")]
    [SerializeField] private Image panel;
    [SerializeField] private Image redpanel;

    private Vector2 moveDirection = Vector2.zero;
    private Vector2 lastMoveDirection = Vector2.zero;
    private Rigidbody2D rb;
    private int footCount = 0;
    private float jumpTimer = 0f;
    private Vector3 lastStepPosition;

    public bool isAlive = true;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
        rb = GetComponent<Rigidbody2D>();
        GameObject foot = SpawnFootstep(leftFoot);
        foot.GetComponent<SoundController>().maxLifeTime = 12;
        foot = SpawnFootstep(rightFoot);
        foot.GetComponent<SoundController>().maxLifeTime = 12;
    }

    void Update()
    {
        if (!isAlive)
        {
            rb.velocity = Vector2.zero;
            return;
        }
        if (HandleJump())
            HandleMovement();
    }

    void HandleMovement()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        moveDirection = new Vector2(horizontalInput, verticalInput).normalized;
        if (moveDirection != Vector2.zero) lastMoveDirection = moveDirection;

        rb.velocity = moveDirection * moveSpeed;

        float speedFactor = Input.GetKey(KeyCode.LeftShift) ? shiftSpeedMultiplier : 1f;

        float distanceToLastStep = Vector2.Distance(transform.position, lastStepPosition);

        if (distanceToLastStep > footstepInterval * speedFactor)
        {
            GameObject footPrefab = speedFactor == 1 ? (footCount % 2 == 1 ? leftFoot : rightFoot) :
                (footCount % 2 == 1 ? leftFootShift : rightFootShift);
            GameObject foot = SpawnFootstep(footPrefab);
            Transform child = foot.transform.GetChild(0);
            if (speedFactor == 1)
                audioSource.PlayOneShot(GetFootStepSound());
            MakeSound(speedFactor == 1 ? 1 : 0.25f, child.position);
            lastStepPosition = transform.position;
            footCount++;
        }
    }

    bool HandleJump()
    {
        if (Input.GetKeyUp(KeyCode.Space) && jumpTimer == 0)
        {
            audioSource.PlayOneShot(jumpSound[Mathf.Clamp(prepareJumpTime, 1, 3) == 1 ? 0 : Mathf.Clamp(prepareJumpTime, 1, 3) == 2 ? 1 : 2]);
            jumpTimer = jumpCooldown;
            SpawnFootstep(leftFoot);
            SpawnFootstep(rightFoot);
            MakeSound(prepareJumpTime, transform.position);
            lastStepPosition = transform.position;
            prepareJumpTime = 0;
            SpaceBar.transform.DOScaleX(0, 0.5f);
        }



        if (jumpTimer > 0) jumpTimer -= Time.deltaTime;
        else jumpTimer = 0f;
        if (Input.GetKey(KeyCode.Space) && jumpTimer == 0)
        {
            prepareJumpTime += Time.deltaTime * 2;
            rb.velocity = Vector2.zero;
            var scale = Mathf.Clamp(prepareJumpTime, 0, 3);
            SpaceBar.transform.localScale = new Vector3(scale / 3, 1, 1);
            return false;
        }

        return true;
    }

    GameObject SpawnFootstep(GameObject footPrefab)
    {
        var footDirection = Mathf.Atan2(lastMoveDirection.y, lastMoveDirection.x) * Mathf.Rad2Deg;
        var footRotation = Quaternion.Euler(0f, 0f, footDirection);
        var foot = PoolManager.instance.Spawn(footPrefab.name, transform.position, footRotation, false);

        return foot;
    }

    AudioClip GetFootStepSound()
    {
        foreach (var sound in footStepSounds)
        {
            if (sound.Ground == GetCurrentGround())
            {
                return sound.FootStepSounds[footCount % 4];
            }
        }
        return null;
    }

    Ground GetCurrentGround()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.down, 0.1f);

        if (hits.Length > 0)
        {
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider.gameObject.TryGetComponent(out GroundController obj))
                {
                    return obj.MyGround;
                }
            }
        }

        return Ground.Stone;
    }

    void MakeSound(float prepare, Vector3 origin)
    {
        prepare = Mathf.Clamp(prepare, 0.25f, 3);

        for (float angle = 0; angle < 360; angle += (12 * (1.5f / Mathf.Clamp(prepare, 1, 3))))
        {
            var radianAngle = angle * Mathf.Deg2Rad;
            var direction = new Vector2(Mathf.Cos(radianAngle), Mathf.Sin(radianAngle));

            var ball = PoolManager.instance.Spawn(rockPrefab.name, origin, Quaternion.identity, false);
            var ballRb = ball.GetComponent<Rigidbody2D>();
            var ballSound = ball.GetComponent<SoundController>();

            ballSound.maxLifeTime = prepare * 2;

            ballRb.AddForce(direction * 200 * (prepare == 3 ? 1.5f : 1));
        }
    }

    public void CheckFootHitbox(GameObject foot, Collider2D collision)
    {
        if (collision.tag == "Spike")
        {
            Vector2 contactPoint = collision.ClosestPoint(transform.position);
            lastStepPosition = contactPoint;
            foot.GetComponent<SpriteRenderer>().color = Color.red;
            audioSource.PlayOneShot(GetFootStepSound());
            MakeSound(1, foot.transform.position);
            footCount++;


            isAlive = false;
            audioSource.PlayOneShot(deathSound);
            gameObject.AddComponent<AudioSource>().PlayOneShot(trapSound);


            redpanel.gameObject.SetActive(true);
            panel.DOFade(1, 0);
            redpanel.DOFade(1, 1).OnComplete(() =>
            {
                panel.gameObject.SetActive(true);
                redpanel.DOFade(0, 1).OnComplete(() =>
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                });
            });
        }
        if (collision.tag == "End")
        {
            isAlive = false;
            panel.DOFade(1, 1).OnComplete(() => SceneManager.LoadScene(int.Parse(collision.name)));
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            GameObject footPrefab = (footCount % 2 == 1 ? leftFoot : rightFoot);
            GameObject foot = SpawnFootstep(footPrefab);
            Vector2 contactPoint = collision.ClosestPoint(transform.position);
            lastStepPosition = contactPoint;
            foot.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.red;
            audioSource.PlayOneShot(GetFootStepSound());
            var audio = gameObject.AddComponent<AudioSource>();
            audio.clip = crashedSound;
            audio.Play();
            MakeSound(1, foot.transform.GetChild(0).position);
            footCount++;

            isAlive = false;

            audioSource.PlayOneShot(deathSound);
            gameObject.AddComponent<AudioSource>().PlayOneShot(trapSound);

            redpanel.gameObject.SetActive(true);
            panel.DOFade(1, 0);
            redpanel.DOFade(1, 1).OnComplete(() =>
            {
                panel.gameObject.SetActive(true);
                redpanel.DOFade(0, 1).OnComplete(() =>
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                });
            });


        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            GameObject footPrefab = (footCount % 2 == 1 ? leftFoot : rightFoot);
            GameObject foot = SpawnFootstep(footPrefab);
            Vector2 contactPoint = collision.collider.ClosestPoint(transform.position);
            lastStepPosition = contactPoint;
            foot.transform.GetChild(0).GetComponent<SpriteRenderer>().color = Color.red;
            audioSource.PlayOneShot(GetFootStepSound());
            MakeSound(1, foot.transform.GetChild(0).position);
            footCount++;

            isAlive = false;


            audioSource.PlayOneShot(deathSound);
            gameObject.AddComponent<AudioSource>().PlayOneShot(trapSound);
            redpanel.DOFade(1, 1).OnComplete(() =>
            {
                panel.gameObject.SetActive(true);
                panel.DOFade(0, 1).OnComplete(() =>
                {
                    SceneManager.LoadScene(SceneManager.GetActiveScene().name);
                });
            });
        }
    }
}
