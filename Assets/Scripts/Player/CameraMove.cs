using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraMove : MonoBehaviour
{
    public float[] leftBound;
    public float[] rightBound;

    public bool stop;

    private int sceneIndex;

    private static CameraMove instance;

    public static CameraMove Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType(typeof(CameraMove)) as CameraMove;
                //if (instance == null)
                //{
                //    GameObject go = new GameObject("CameraMove");
                //    instance = go.AddComponent<CameraMove>();
                //}
            }
            return instance;
        }
    }

    private void Start()
    {
        if (Instance != this) Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        sceneIndex = SceneManager.GetActiveScene().buildIndex - 1;
        stop = false;
    }

    public void UpdateCameraMove()
    {
        sceneIndex = SceneManager.GetActiveScene().buildIndex - 1;
        float x = Player.Instance.transform.position.x;
        if (x >= leftBound[sceneIndex] && x <= rightBound[sceneIndex])
            Instance.transform.position = new Vector3(x, 0, -10);
        else if (x < leftBound[sceneIndex])
            Instance.transform.position = new Vector3(leftBound[sceneIndex], Instance.transform.position.y, -10);
        else if (x > rightBound[sceneIndex])
            Instance.transform.position = new Vector3(rightBound[sceneIndex], Instance.transform.position.y, -10);
        if (sceneIndex == 1)
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        else if (sceneIndex == 2)
            transform.position = new Vector3(transform.position.x, 1.75f, transform.position.z);
        else if (sceneIndex == 3 || sceneIndex == 4)
            transform.position = new Vector3(transform.position.x, 1.32f, transform.position.z);
        else if (sceneIndex == 5)
            transform.position = new Vector3(transform.position.x, -1.31f, transform.position.z);
        else
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
        if (!stop && Time.timeScale != 0)
        {
            float x = Player.Instance.transform.position.x;
            Vector3 pos = Instance.transform.position;
            if (x >= leftBound[SceneManager.GetActiveScene().buildIndex - 1] && x <= rightBound[SceneManager.GetActiveScene().buildIndex - 1])
            {
                if (SceneManager.GetActiveScene().buildIndex == 2)
                    transform.position = new Vector3(x, 0, -10);
                else if (SceneManager.GetActiveScene().buildIndex == 3)
                    Instance.transform.position = new Vector3(x, 1.75f, -10);
                else if (SceneManager.GetActiveScene().buildIndex == 4 || SceneManager.GetActiveScene().buildIndex == 5)
                    Instance.transform.position = new Vector3(x, 1.32f, -10);
                else if (SceneManager.GetActiveScene().buildIndex == 6)
                    Instance.transform.position = new Vector3(x, -1.31f, -10);
                else
                    Instance.transform.position = new Vector3(x, 0, -10);
            }
            else if (x < leftBound[SceneManager.GetActiveScene().buildIndex - 1])
            {
                pos.x = leftBound[SceneManager.GetActiveScene().buildIndex - 1];
                Instance.transform.position = pos;
            }
            else if (x > rightBound[SceneManager.GetActiveScene().buildIndex - 1])
            {
                pos.x = rightBound[SceneManager.GetActiveScene().buildIndex - 1];
                Instance.transform.position = pos;
            }
        }
    }
}
