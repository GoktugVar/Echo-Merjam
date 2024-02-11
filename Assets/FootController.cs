using UnityEngine;

public class FootController : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerMovement.instance.CheckFootHitbox(this.gameObject, collision);

        if(collision.tag == "Button")
        {
            collision.GetComponent<ButtonController>().ButtonPress();
        }
    }
}
