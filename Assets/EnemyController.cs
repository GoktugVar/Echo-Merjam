using UnityEngine;
using DG.Tweening;
using System.Threading.Tasks;


public class EnemyController : MonoBehaviour
{
    public float moveSpeed = 5f;

    private Vector3 target;
    private AudioSource audioSource;
    public GameObject rockPrefab;
    float lastSound;

    int index;
    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        target = transform.position;
    }

    private void Update()
    {
        // Hedefe doðru hareket et
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

        // Hedefe doðru hareket ederken ve ses çalmýyorken sesi çal
        if (Vector3.Distance(transform.position, target) > 0.1f)
        {
            if (Time.time - lastSound > 0.1f)
                HandleSound(transform.position);

            if (audioSource.isPlaying == false)
            {
                audioSource.Play();
                audioSource.DOFade(.5f, 1);
            }
        }

        // Hedefe ulaþýldýðýnda ve ses çalýyorken sesi durdur
        if (Vector3.Distance(transform.position, target) < 0.01f && audioSource.isPlaying == true)
        {
            audioSource.DOFade(0, 1).OnComplete(() => audioSource.Stop());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ball"))
        {
            target = collision.GetComponent<SoundController>().SpawnPos;
        }
    }

    public async void TestSound()
    {
        HandleSound(transform.position);
        await Task.Delay(100);
        TestSound();
    }

    public void HandleSound(Vector3 origin)
    {
        lastSound = Time.time;
        for (float angle = 0; angle < 360; angle += 72)
        {
            var radianAngle = angle * Mathf.Deg2Rad;
            radianAngle += index;
            var direction = new Vector3(Mathf.Cos(radianAngle), Mathf.Sin(radianAngle));

            var ball = PoolManager.instance.Spawn(rockPrefab.name, origin + (direction * .2f), Quaternion.identity, false);
            var ballRb = ball.GetComponent<Rigidbody2D>();
            var ballSound = ball.GetComponent<SoundController>();

            ballSound.maxLifeTime = 1f;
            ballRb.AddForce(direction * 100);    
        }
        index += 10;
    }
}
