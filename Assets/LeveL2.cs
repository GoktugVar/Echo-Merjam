using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeveL2 : MonoBehaviour
{
    public float timer;
    private float start = 0;
    public GameObject panel;
    public KeyCode key;
    void Update()
    {
        if (Input.GetKey(key)) { start += Time.deltaTime; }
        if (Input.GetKeyDown(key))
        {
            if(start <= timer)
                panel.SetActive(false);
            start = 0;
        }
    }
}
