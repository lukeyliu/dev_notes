using UnityEngine;
using System.Collections;

public static class FileUtils {

    public static string ToAssetPath(string path)
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            return path.Substring(path.IndexOf("Assets\\"));
        }
        else
        {
            return path.Substring(path.IndexOf("Assets/"));
        }
    }

}
