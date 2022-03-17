using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlUV : MonoBehaviour
{
    private Material material;
    // Start is called before the first frame update
    void Start()
    {
        material = GetComponent<SpriteRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        float uvX = material.GetVector("_Offset").x;
        float uvY = material.GetVector("_Offset").y;

        uvX += Time.deltaTime * 0.05f;
        uvY += Time.deltaTime * 0.05f;

        material.SetVector("_Offset", new Vector4(uvX, uvY, 0.0f, 0.0f));
    }
}
