using System.Threading.Tasks;
using UnityEngine;

public class RockSpawner : MonoBehaviour
{
    public GameObject Rock;
    async void Start()
    {
        for(int i = 0; i < 10; i++)
        {
            for (float angle = 0; angle < 360; angle += 6)
            {
                var radianAngle = angle * Mathf.Deg2Rad;
                var direction = new Vector2(Mathf.Cos(radianAngle), Mathf.Sin(radianAngle));

                var ball = PoolManager.instance.Spawn(Rock.name, transform.position, Quaternion.identity, false);
                var ballRb = ball.GetComponent<Rigidbody2D>();

                ballRb.AddForce(direction * 200);
            }
            await Task.Delay(1000);
        }  
    }
}
