/****************************************************************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: #COMPANY#
 * Project: #PROJECTNAME#
 * Creation Date: #DATE#
 * 
 * Description: TODO
****************************************************************************/
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
            if(GetProjectName() != "SharedCodeAssetsProject")
            {
                Debug.LogError($"Package file not found at {packagePath}");
            }
            return;
        }

        string packageJson = File.ReadAllText(packagePath);
        dynamic packageData = Newtonsoft.Json.JsonConvert.DeserializeObject(packageJson);

        if (Application.unityVersion.StartsWith("2020") || Application.unityVersion.StartsWith("2021"))
        {
            packageData.dependencies["com.unity.textmeshpro"] = "3.0.6";
        }
        else if (Application.unityVersion.StartsWith("2019"))
        {
            packageData.dependencies["com.unity.textmeshpro"] = "2.1.6";
        }

        string updatedPackageJson = Newtonsoft.Json.JsonConvert.SerializeObject(packageData, Newtonsoft.Json.Formatting.Indented);
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
