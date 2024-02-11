using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public AudioClip buttonPres;

    [Header("First")]
    public bool FirstButton;

    public GameObject upwall;
    public GameObject downwall;
    public AudioClip wallSound;
    public Transform SoundPoint;

    [Space(20),
        Header("Second")]
    public bool SecondButton;
    public GameObject door;
    public GameObject door2;
    public AudioClip doorSound;

    AudioSource source;

    [Space(20),
        Header("Third")]
    public bool ThirdButton;
    public AudioClip doorsSound;
    public GameObject[] ForActive;
    public GameObject[] ForDeactive;

    private void Start()
    {
        source = gameObject.AddComponent<AudioSource>();
    }
    public void ButtonPress()
    {
        source.PlayOneShot(buttonPres);
        if(FirstButton)
        {
            FirstButtonFunc();        
        }
        else if(SecondButton)
        {
            SecondButtonFunc();
        }
        else if(ThirdButton)
        {
            ThirdButtonFunc();
        }
        GetComponent<SpriteRenderer>().DOFade(0, 0.5f).OnComplete(() => 
        { 
            gameObject.SetActive(false);
            GetComponent<SpriteMask>().enabled = false;
        });
    }

    public async void FirstButtonFunc()
    {
        // Üst duvar yukarý doðru hareket etsin
        upwall.transform.DOMoveY(upwall.transform.position.y + 2f, 7f);

        // Alt duvar aþaðý doðru hareket etsin
        downwall.transform.DOMoveY(downwall.transform.position.y - 2f, 7f);

        GameObject audio = new GameObject("Audio");
        AudioSource _source = audio.AddComponent<AudioSource>();
        _source.clip = wallSound;
        _source.Play();

        await Task.Delay(200);

        for(int i = 0; i < 10; i++)
        {
            HandleSound(SoundPoint.position);
            await Task.Delay(500);
        }
    }

    async void SecondButtonFunc()
    {
        door.SetActive(false);
        door2.SetActive(false);

        GameObject audio = new GameObject("Audio");
        AudioSource _source = audio.AddComponent<AudioSource>();
        _source.clip = doorSound;
        _source.Play();

        await Task.Delay(200);

        for (int i = 0; i < 2; i++)
        {
            HandleSound(door.transform.position);
            await Task.Delay(200);
            HandleSound(door2.transform.position);
            await Task.Delay(200);
        }
    }

    void ThirdButtonFunc()
    {
        foreach (GameObject enemy in ForDeactive)
        {
            enemy.SetActive(false);
        }

        foreach (GameObject enemy in ForActive)
        {
            enemy.SetActive(true);
        }

        // Kapý sesini çal
        AudioSource.PlayClipAtPoint(doorsSound, transform.position);
    }

    public void HandleSound(Vector3 origin)
    {
        int index = 0;
        for (float angle = 30; angle < 390; angle += 60)
        {
            var radianAngle = angle * Mathf.Deg2Rad;
            radianAngle += index;
            var direction = new Vector3(Mathf.Cos(radianAngle), Mathf.Sin(radianAngle));

            var ball = PoolManager.instance.Spawn("FlyBall", origin, Quaternion.identity, false);
            var ballRb = ball.GetComponent<Rigidbody2D>();
            var ballSound = ball.GetComponent<SoundController>();

            ballSound.maxLifeTime = 3;

            ballRb.AddForce(direction * 300);

            index += Random.Range(15,20);
        }
    }
}
