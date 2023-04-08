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
            try
            {
                foreach (string path in paths)
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

                    if (fontAsset.atlasPopulationMode != AtlasPopulationMode.Dynamic)
                        continue;

                    // Debug.Log("Clearing font asset data at " + path);
                    fontAsset.ClearFontAssetData(setAtlasSizeToZero: true);
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Debug.LogError("Something went wrong while clearing dynamic font data. For more info look at log message above.");
            }

            return paths;
        }
    }
}