using UnityEngine;
using System.Collections;
using UnityEditor;
using System.IO;
using Spine;
using UnityEditor.Animations;

public class SpineDataImport : Editor {

	private static string SpineDataInputPath 			= "Assets/../SpineData";

    private static string SpineDataOutputPath           = "Assets/Resources/SpineData";

    private static string SpinePrefabOutputPath         = "Assets/Prefabs/Spine";

	[MenuItem("Custom/Spine/ImportSpineData")]
	static void SpineImport() 
    {
        Debug.Log("ImportAnimation ...");
        //如果项目中不存在路径则先创建
        if (!Directory.Exists(SpineDataOutputPath))
        {
            Directory.CreateDirectory(SpineDataOutputPath);
        }

		DirectoryInfo info = new DirectoryInfo (SpineDataInputPath);
        foreach( DirectoryInfo child in info.GetDirectories())
        {
            ImportAndRenameAtlas(child);
        }

        AssetDatabase.Refresh();

        Debug.Log("ImportAnimation Complete");
	}

    [MenuItem("Custom/Spine/CreateAnimationPrefabs")]
    static void CreateAnimationPrefabs()
    {
        Debug.Log("CreateAnimationPrefabs ...");

        DirectoryInfo fabPath = new DirectoryInfo(SpinePrefabOutputPath);
        if (!fabPath.Exists)
        {
            fabPath.Create();
        }

        DirectoryInfo info = new DirectoryInfo(SpineDataOutputPath);
        foreach (DirectoryInfo child in info.GetDirectories())
        {
            CreateAnimationPrefab(child);
        }

        Debug.Log("CreateAnimationPrefabs Complete");
    }

    [MenuItem("Custom/Spine/ImportAndCreatePrefabs")]
    static void ImportAndCreatePrefabs()
    {
        SpineImport();
        CreateAnimationPrefabs();
    }

    //重命名Atlas文件 并导入资源到项目
    static void ImportAndRenameAtlas( DirectoryInfo info) 
    {
        Debug.Log("ImportAnimation : " + info.Name);

        DirectoryInfo output = new DirectoryInfo(SpineDataOutputPath + "/" + info.Name);
        if (!output.Exists)
        {
            output.Create();
        }

        FileInfo [] files = info.GetFiles();

        foreach (FileInfo file in files)
        {
            if ( file.Name.LastIndexOf(".atlas") > 0 )
            {
                file.CopyTo(output.FullName + "/" + file.Name + ".txt",true);
            }
            else
            {
                file.CopyTo(output.FullName + "/" + file.Name, true);
            }
        }
    }
    
    static void CreateAnimationPrefab(DirectoryInfo info)
    {
        FileInfo[] files = info.GetFiles("*_SkeletonData.asset");
        if (files.Length > 0)
        {
            foreach (FileInfo file in files)
            {
                SkeletonDataAsset asset = AssetDatabase.LoadAssetAtPath<SkeletonDataAsset>(FileUtils.ToAssetPath(file.FullName));
                if ( asset != null )
                {
                    SkeletonData data = asset.GetSkeletonData(false);
                    //保存为prefab
                    GameObject go = new GameObject();
                    go.name = asset.name;
                    SkeletonAnimation sa = go.AddComponent<SkeletonAnimation>();
                    sa.skeletonDataAsset = asset;
                    if (data.Skins.Count > 1)
                    {
                        sa.initialSkinName = data.Skins.Items[1].Name;
                    }
                    string savePath = SpinePrefabOutputPath + "/" + asset.skeletonJSON.name + ".prefab";
                    PrefabUtility.CreatePrefab(savePath, go);
                    AssetDatabase.SaveAssets();
                    DestroyImmediate(go);
                }
            }
        }
        else 
        {
            Debug.Log("[ Error ] : " + info.FullName + " no exist SkeletonData");
        }
    }

}
