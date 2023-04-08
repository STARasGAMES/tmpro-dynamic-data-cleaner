#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;

namespace GitIntegration
{
    [InitializeOnLoad]
    public class SmartMergeRegistrator
    {
        const string SmartMergeRegistratorEditorPrefsKey = "smart_merge_installed";
        const int Version = 1;
        static string VersionKey = $"{Version}_{Application.unityVersion}";

        [MenuItem("Tools/Git/SmartMerge registration")]
        static void SmartMergeRegister()
        {
            try
            {
                var UnityYAMLMergePath = EditorApplication.applicationContentsPath + "/Tools" + "/UnityYAMLMerge.exe";
                Utils.ExecuteGitWithParams("config merge.unityyamlmerge.name \"Unity SmartMerge (UnityYamlMerge)\"");
                Utils.ExecuteGitWithParams($"config merge.unityyamlmerge.driver \"\\\"{UnityYAMLMergePath}\\\" merge -h -p --force --fallback none %O %B %A %A\"");
                Utils.ExecuteGitWithParams("config merge.unityyamlmerge.recursive binary");
                EditorPrefs.SetString(SmartMergeRegistratorEditorPrefsKey, VersionKey);
                Debug.Log($"Succesfuly registered UnityYAMLMerge with path {UnityYAMLMergePath}");
            }
            catch (Exception e)
            {
                Debug.Log($"Fail to register UnityYAMLMerge with error: {e}");
            }
        }

        [MenuItem("Tools/Git/SmartMerge unregistration")]
        static void SmartMergeUnRegister()
        {
            Utils.ExecuteGitWithParams("config --remove-section merge.unityyamlmerge");
            Debug.Log($"Succesfuly unregistered UnityYAMLMerge");
        }

        //Unity calls the static constructor when the engine opens
        static SmartMergeRegistrator()
        {
            var instaledVersionKey = EditorPrefs.GetString(SmartMergeRegistratorEditorPrefsKey);
            if (instaledVersionKey != VersionKey)
                SmartMergeRegister();
        }
    }
}
#endif