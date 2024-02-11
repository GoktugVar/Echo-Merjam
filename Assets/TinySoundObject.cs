using System.Threading.Tasks;
using UnityEngine;

public class TinySoundObject : MonoBehaviour
{
    public Collider2D spawnArea; // Spawn alan�na ait collider
    public Vector2 cooldown = new Vector2(250, 500);
    public GameObject rockPrefab;
    public bool water;
    public float life;

    [Range(0f, 1f)] 
    public float volume;

    Vector3 lastSpawn;
    private void Start()
    {
        SpawnObject();
    }

    public async void SpawnObject()
    {
        if(spawnArea == null)
        {
            await Task.Delay(Random.Range(250, 500));
            SpawnObject();
            return;
        }
        // Collider'�n s�n�rlar�n� al
        Bounds bounds = spawnArea.bounds;

        // Rastgele bir konum se�
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomY = Random.Range(bounds.min.y, bounds.max.y);

        // Se�ilen rastgele konumda objeyi spawn et
        Vector3 spawnPosition = new Vector3(randomX, randomY, 0);

        // Son spawn noktas� ile yeni spawn noktas� aras�ndaki mesafeyi kontrol et
        if (lastSpawn != Vector3.zero && Vector3.Distance(lastSpawn, spawnPosition) < 1)
        {
            // E�er minimum mesafeden daha yak�nsa yeni bir konum se�
            SpawnObject();
            return;
        }

        // Mesafe uygunsa, yeni spawn konumunu kullanarak objeyi spawn et
        if (water)
        {
            GetComponent<AudioSource>().Play();
        }
        HandleSound(spawnPosition);
        lastSpawn = spawnPosition;

        await Task.Delay(Random.Range((int)cooldown.x, (int)cooldown.y));
        SpawnObject(); // Yinelemeli olarak spawn etmek i�in SpawnObject() fonksiyonunu �a��r�n
    }


    public void HandleSound(Vector3 origin)
    {
        for (float angle = 0; angle < 360; angle += 60)
        {
            var radianAngle = angle * Mathf.Deg2Rad;
            var direction = new Vector3(Mathf.Cos(radianAngle), Mathf.Sin(radianAngle));

            var ball = PoolManager.instance.Spawn(rockPrefab.name, origin - (direction * volume), Quaternion.identity, false);
            var ballRb = ball.GetComponent<Rigidbody2D>();
            var ballSound = ball.GetComponent<SoundController>();

            ballSound.maxLifeTime = life == 0? 2 : life;

            ballRb.AddForce(direction * 100);
        }
    }
}
