using UnityEditor;

public class AssetBundleBuilder
{
    // New AssetBundle builder in menu item
    [MenuItem("Assets/AssetBundle Builders/Build All AssetBundles (Windows x64)")]
    static void CreateAssetBundles_win_x64()
    {
        // Build AssetBundle and save in local directory for windows x64
		BuildPipeline.BuildAssetBundles("Assets/AssetBundles", 
            BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows64);
    }	
}

