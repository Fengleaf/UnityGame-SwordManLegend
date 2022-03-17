using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffect : MonoBehaviour
{
    public float parallaxEffect;
    private float startX;
    // Start is called before the first frame update
    void Start()
    {
        startX = transform.position.x;
    }

    // Update is called once per frame
    void Update()
    {
        float distance = Camera.main.transform.position.x * parallaxEffect;
        transform.position = new Vector3(startX + distance, transform.position.y, transform.position.z);
    }
}
