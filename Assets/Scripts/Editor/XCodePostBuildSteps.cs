#if UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor.iOS.Xcode;

using System.IO;
public class XCodePostBuildSteps
{
    const string TrackingDescription =
    "This identifier will be used to deliver personalized content to you. ";

    [PostProcessBuild(9999999)] //to be called after MMNV PostProcess calls (NiceVibrations package)
    public static void OnPostprocessBuild(BuildTarget buildTarget, string pathToXcode)
    {
        Debug.Log("PostBuildStep Starting...");
        if (buildTarget == BuildTarget.iOS)
        {
            AddPListValues(pathToXcode);
            ModifyFrameworks(pathToXcode);
        }
        Debug.Log("PostBuildStep Finished!");
    }

    //public static void SetSwiftVersionWithRegex(string path)
    //{
    //    string projPath = PBXProject.GetPBXProjectPath(path);
    //    string projText = File.ReadAllText(projPath);
    //    projText = Regex.Replace(projText, "SWIFT_VERSION = [^;]*;", "SWIFT_VERSION = 5.0;", RegexOptions.Multiline);
    //    File.WriteAllText(projPath, projText);
    //}

    public static void AddPListValues(string pathToXcode)
    {
        // Get Plist from Xcode project 
        string plistPath = pathToXcode + "/Info.plist";
        // Read in Plist 
        PlistDocument plistObj = new PlistDocument();
        plistObj.ReadFromString(File.ReadAllText(plistPath));
        // set values from the root obj
        PlistElementDict plistRoot = plistObj.root;
        // Set value in plist
        plistRoot.SetString("NSUserTrackingUsageDescription", TrackingDescription); //App Tracking Transparency request string
        plistRoot.SetBoolean("ITSAppUsesNonExemptEncryption", false); //Bypasses missing compliance warning
        SetSkadNetworkItems(plistRoot); //SUPERSONIC todo remove if working with any other publisher
        // save
        File.WriteAllText(plistPath, plistObj.WriteToString());
    }

    private static void SetSkadNetworkItems(PlistElementDict plistRoot)
    {
        //adds SKAdNetworkItems for Supersonic Analytics
        PlistElementArray array = plistRoot.CreateArray("SKAdNetworkItems");

        PlistElementDict dict1 = array.AddDict();
        dict1["SKAdNetworkIdentifier"] = new PlistElementString("v9wttpbfk9.skadnetwork");
        PlistElementDict dict2 = array.AddDict();
        dict2["SKAdNetworkIdentifier"] = new PlistElementString("n38lu8286q.skadnetwork");
        plistRoot.values["SKAdNetworkItems"] = array;
    }

    private static void ModifyFrameworks(string path)
    {
        string projPath = PBXProject.GetPBXProjectPath(path);

        var project = new PBXProject();
        project.ReadFromFile(projPath);

        string mainTargetGuid = project.GetUnityMainTargetGuid();

        foreach (var targetGuid in new[] { mainTargetGuid, project.GetUnityFrameworkTargetGuid() })
        {
            project.SetBuildProperty(targetGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "NO");
            project.SetBuildProperty(targetGuid, "SWIFT_VERSION", "5.0");
            project.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");
        }
        project.SetBuildProperty(mainTargetGuid, "SWIFT_VERSION", "5.0");
        project.SetBuildProperty(mainTargetGuid, "ALWAYS_EMBED_SWIFT_STANDARD_LIBRARIES", "YES");


        project.WriteToFile(projPath);
    }
}
#endif