using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Displayer : MonoBehaviour
{
    private static Displayer instance;

    public static Displayer Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(Displayer)) as Displayer;
                if (instance == null)
                {
                    GameObject go = new GameObject("Displayer");
                    instance = go.AddComponent<Displayer>();
                }
            }
            return instance;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        //if (Instance != this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
    }
}
