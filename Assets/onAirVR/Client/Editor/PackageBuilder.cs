using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class PackageBuilder {
    private const string PackageFilename = "onAirVRClient.unitypackage";

    [MenuItem("onAirVR/Export onAirVR Client...")]
    public static void ExportAirVRClient() {
        string targetPath = EditorUtility.SaveFilePanel("Export onAirVR Client...", "", "onAirVRClient", "unitypackage");
        if (string.IsNullOrEmpty(targetPath)) {
            return;
        }

        if (File.Exists(targetPath)) {
            File.Delete(targetPath);
        }
        string[] folders = { "Assets/onAirVR" };
        string[] guids = AssetDatabase.FindAssets("", folders);
        List<string> assets = new List<string>();
        foreach (string guid in guids) {
            assets.Add(AssetDatabase.GUIDToAssetPath(guid));
        }
        assets.Add("Assets/Plugins/Android/assets/client.license");
        assets.Add("Assets/Plugins/Android/onAirVRClientPlugin.jar");
        assets.Add("Assets/Plugins/Android/libonAirVRClientPlugin.so");
        AssetDatabase.ExportPackage(assets.ToArray(), targetPath);

        EditorUtility.DisplayDialog("Congratulation!", "The package is exported successfully.", "Thanks.");
    }
}
