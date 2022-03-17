using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AssetBundleLoader : MonoBehaviour
{
    public Transform m_GORoot;  // Root parent for instantiated game objects
    public string[] m_Url;      // List of AssetBundle URL

    private AssetBundle p_AssetBundle;  // AssetBundle loaded from URL

    public Image image;

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    StartCoroutine(LoadAssetBundle());
        //}
    }

    IEnumerator LoadAssetBundle ()
    {
        // Load AssetBundle from given URL
        WWW www = new WWW(Application.dataPath + "/AssetBundles/AssetBundles 1/hspecial");
        yield return www;
        Debug.Log("Load Finished");
        // Get and assign current AssetBundle
        p_AssetBundle = www.assetBundle;

        // Load Cube asset/object from AssetBundle and instantiate in the scene
        Sprite sprite = p_AssetBundle.LoadAsset("mx.png") as Sprite;
        image.sprite = sprite;
    }

    public void SpawnAssetBundles (int groupNum)
    {
        // Destroy all objects and unload current AssetBundle
        UnloadAll();

        // Start loading AssetBundle by group number
        StartCoroutine("LoadAssetBundle", groupNum);
    }

    public void UnloadAll ()
    {
        if (p_AssetBundle != null)
        {
            //Destroy all child game object under root
            foreach (Transform child in m_GORoot.transform)
            {
                Destroy(child.gameObject);
            }

            // Unload current AssetBundle
            p_AssetBundle.Unload(true);
            // p_AssetBundle becomes null after unload
        }
    }

    public void UnLoadAssetBundle (bool flag)
    {
        if (p_AssetBundle != null)
        {
            p_AssetBundle.Unload(flag);
        }
        else
        {
            Debug.Log("Current AssetBundle is null");
        }
    }
}
