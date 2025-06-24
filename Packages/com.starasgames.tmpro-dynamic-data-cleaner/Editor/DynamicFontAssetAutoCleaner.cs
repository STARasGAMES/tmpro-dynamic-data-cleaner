// TextMeshPro dynamic font assets have a very annoying habit of saving their dynamically generated binary data in the
// same text file as their configuration data. This causes massive headaches for version control.
//
// This script addresses the above issue. It runs whenever any assets in the project are about to be saved. If any of
// those assets are a TMP dynamic font asset, they will have their dynamically generated data cleared before they are
// saved, which prevents that data from ever polluting the version control.
//
// For more information, see this post by @cxode: https://forum.unity.com/threads/tmpro-dynamic-font-asset-constantly-changes-in-source-control.1227831/#post-8934711

using System;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace TMProDynamicDataCleaner.Editor
{
    internal class DynamicFontAssetAutoCleaner : UnityEditor.AssetModificationProcessor
    {
        static string[] OnWillSaveAssets(string[] paths)
        {
            foreach (string path in paths)
            {
                try
                {
                    var assetType = AssetDatabase.GetMainAssetTypeAtPath(path);

                    // GetMainAssetTypeAtPath() sometimes returns null, for example, when path leads to .meta file
                    if (assetType == null)
                        continue;

                    // TMP_FontAsset is not marked as sealed class, so also checking for subclasses just in case
                    if (assetType != typeof(TMP_FontAsset) && assetType.IsSubclassOf(typeof(TMP_FontAsset)) == false)
                        continue;

                    // Loading the asset only when we sure it is a font asset
                    var fontAsset = AssetDatabase.LoadMainAssetAtPath(path) as TMP_FontAsset;

                    // Theoretically this case is not possible due to asset type check above, but to be on the safe side check for null
                    if (fontAsset == null)
                        continue;

#if UNITY_2022_1_OR_NEWER
                    bool isDynamic = fontAsset.atlasPopulationMode is AtlasPopulationMode.Dynamic or AtlasPopulationMode.DynamicOS;
#else
                    bool isDynamic = fontAsset.atlasPopulationMode is AtlasPopulationMode.Dynamic;
#endif

                    if (!isDynamic)
                        continue;
                        
                    // Debug.Log("Clearing font asset data at " + path);
                    fontAsset.ClearFontAssetData(setAtlasSizeToZero: true);
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                    Debug.LogError($"Something went wrong while clearing dynamic font data. For more info look at previous log message. Font asset path: '{path}'");
                }
            }

            return paths;
        }
    }
}
