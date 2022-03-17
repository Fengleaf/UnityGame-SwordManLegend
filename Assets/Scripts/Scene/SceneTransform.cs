using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransform : MonoBehaviour
{
    public int nextSceneIndex;
    public Vector2 nextPosition;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (nextSceneIndex == 2 && !GameManager.defeatedDemon)
                return;
            GameManager.instance.playerLoadedPosition = nextPosition;
            SceneManager.LoadScene(nextSceneIndex);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        
    }

}
