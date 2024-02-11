using DG.Tweening;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public float maxLifeTime;
    private float lifeTime;
    public SpriteRenderer spriteRenderer;
    public SpriteRenderer spriteRenderer2;
    public Rigidbody2D rb;

    public bool deneme;
    public GameObject[] trails;

    public TrailRenderer trailRenderer;

    public Vector3 SpawnPos;
    void Start()
    {
        SpawnPos = transform.position;
        lifeTime = maxLifeTime;
    }

    void Update()
    {
        if (lifeTime == -1)
            return;

        lifeTime -= Time.deltaTime;

        if (lifeTime <= 0)
        {
            lifeTime = -1;
            LifeEnd();
        }

        float alpha = Mathf.Clamp01(lifeTime / maxLifeTime);
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);

        if(rb)
        {
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                trailRenderer.colorGradient.colorKeys,
                new GradientAlphaKey[] { new GradientAlphaKey(alpha, 0f), new GradientAlphaKey(0, 1f) }
            );
            trailRenderer.colorGradient = gradient;
        }
    }

    void LifeEnd()
    {
        if (rb != null)
        {
            DOTween.To(() => (Vector3)rb.velocity, x => rb.velocity = x, Vector3.zero, 1).OnComplete(() =>
            {
                lifeTime = maxLifeTime;
                trailRenderer.Clear();               
                PoolManager.instance.Despawn(this.gameObject);
            });
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("End"))
        {
            lifeTime = 5000;
            transform.localScale *= 2;
            trailRenderer.Clear();
            GetComponent<TrailRenderer>().widthMultiplier = transform.localScale.x;
        }
        else if (collision.TryGetComponent(out GroundController groundController))
        {
            if (groundController.MyGround == Ground.Wet)
            {
                lifeTime *= 2;
            }
        }
        if (collision.tag == "Enemy" && gameObject.tag != "Enemy" && lifeTime > 0.1f)
        {
            lifeTime = 0.1f;
        }

        if (deneme)
        {
            if (collision.tag == "Spike")
            {
                trails[0].SetActive(true);
            }
            else if (collision.tag == "Button")
            {
                trails[1].SetActive(true);
            }
            else if (collision.tag == "Water")
            {
                trails[2].SetActive(true);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("End"))
        {
            lifeTime = maxLifeTime/2;
            transform.localScale /= 2;
            GetComponent<TrailRenderer>().widthMultiplier = transform.localScale.x;
        }
        if (deneme)
        {
            if (collision.tag == "Spike")
            {
                trails[0].SetActive(false);
            }
            else if (collision.tag == "Button")
            {
                trails[1].SetActive(false);
            }
            else if (collision.tag == "Water")
            {
                trails[2].SetActive(false);
            }
        }
    }
}
