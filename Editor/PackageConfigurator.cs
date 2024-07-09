/****************************************************************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: #COMPANY#
 * Project: #PROJECTNAME#
 * Creation Date: #DATE#
 * 
 * Description: Was having issues getting this script to work but the purpose was to properly
 *                     handle package dependencies that are version specific. This script was
 *                     supposed to change the version of the package used but it wasn't working
 *                     and I had other things to do. I'm leaving this here for now.
****************************************************************************/
/*
using System.Collections.Generic;
using System;
using System.IO;
using System.Runtime.Hosting;
using UnityEditor;
using UnityEngine;


[InitializeOnLoad]
public static class PackageConfigurator
{
    static PackageConfigurator()
    {
        ConfigurePackage();
    }

    private static void ConfigurePackage()
    {
        string packagePath = "Packages/com.virtusense.sharedcodeassets/package.json";

        if (!File.Exists(packagePath))
        {
            if (GetProjectName() != "SharedCodeAssetsProject")
            {
                Debug.LogError($"Package file not found at {packagePath}");
            }
            return;
        }

        string packageJson = File.ReadAllText(packagePath);
        PackageJson packageData = JsonUtility.FromJson<PackageJson>(packageJson);

        string unityVersion = Application.unityVersion;

        if (unityVersion.StartsWith("2020") || unityVersion.StartsWith("2021"))
        {
            packageData.dependencies["com.unity.textmeshpro"] = "3.0.6";
        }
        else if (unityVersion.StartsWith("2019"))
        {
            packageData.dependencies["com.unity.textmeshpro"] = "2.1.6";
        }

        string updatedPackageJson = JsonUtility.ToJson(packageData, true);
        updatedPackageJson = FixJson(updatedPackageJson);
        File.WriteAllText(packagePath, updatedPackageJson);

        AssetDatabase.Refresh();
    }

    private static string FixJson(string json)
    {
        // Remove extra quotes and escape sequences
        json = json.Replace("\\\"", "\"");
        json = json.Replace("\"{", "{").Replace("}\"", "}");
        return json;
    }

    /*
    string packagePath = "Packages/com.virtusense.sharedcodeassets/package.json";

    if (!File.Exists(packagePath))
    {
        if(GetProjectName() != "SharedCodeAssetsProject")
        {
            Debug.LogError($"Package file not found at {packagePath}");
        }
        return;
    }

    string packageJson = File.ReadAllText(packagePath);
    PackageJson packageData = JsonUtility.FromJson<PackageJson>(packageJson);

    if (Application.unityVersion.StartsWith("2020") || Application.unityVersion.StartsWith("2021"))
    {
        packageData.dependencies.Add("com.unity.textmeshpro", "3.0.6");
    }
    else if (Application.unityVersion.StartsWith("2019"))
    {
        packageData.dependencies.Add("com.unity.textmeshpro", "2.1.6");
    }

    string updatedPackageJson = JsonUtility.ToJson(packageData, true);
    File.WriteAllText(packagePath, updatedPackageJson);

    AssetDatabase.Refresh();
}

private static string GetProjectName()
    {
        string projectPath = Application.dataPath;
        string projectName = new DirectoryInfo(projectPath).Parent.Name;
        
        return projectName;
    }
}

[Serializable]
public class PackageJson
{
    public string name;
    public string version;
    public string displayName;
    public string description;
    public Dictionary<string, string> dependencies = new Dictionary<string, string>();
}

[Serializable]
public class SerializableDictionary<TKey, TValue> : ISerializationCallbackReceiver
{
    [SerializeField]
    private List<TKey> keys = new List<TKey>();

    [SerializeField]
    private List<TValue> values = new List<TValue>();

    private Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

    public void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();
        foreach (var kvp in dictionary)
        {
            keys.Add(kvp.Key);
            values.Add(kvp.Value);
        }
    }

    public void OnAfterDeserialize()
    {
        dictionary = new Dictionary<TKey, TValue>();

        if (keys.Count != values.Count)
            throw new Exception($"Mismatched key/value count: {keys.Count} keys, {values.Count} values");

        for (int i = 0; i < keys.Count; i++)
        {
            dictionary[keys[i]] = values[i];
        }
    }

    public Dictionary<TKey, TValue> ToDictionary()
    {
        return dictionary;
    }

    public void Add(TKey key, TValue value)
    {
        dictionary[key] = value;
    }
}*/
