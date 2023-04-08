using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using TMPro;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace TMProDynamicDataCleaner.Tests
{
    public class DynamicFontAssetAutoCleanerTests
    {
        [Test]
        public void DynamicFontAsset_HasZeroSizeAtlasTexture_WhenSavedToDisk()
        {
            var beforeTestAllAssetPaths = AssetDatabase.GetAllAssetPaths();

            var allGuids = AssetDatabase.FindAssets("t:font");
            Assert.That(allGuids.Length, Is.GreaterThan(0), () => "Your project doesn't have any font asset to create temporary TMP_FontAsset.");
            
            var allPaths = allGuids.Select(AssetDatabase.GUIDToAssetPath).ToArray();

            string fontPath = allPaths.First();

            // Try to use font asset that TMP uses by default
            foreach (var path in allPaths)
                if (path.Contains("LiberationSans.ttf"))
                {
                    fontPath = path;
                    break;
                }

            var font = AssetDatabase.LoadAssetAtPath<Font>(fontPath);

            // Creating temporary TMP_FontAsset 
            Selection.objects = new Object[] { font };
            TMP_FontAsset_CreationMenu.CreateFontAsset();
            var afterTestAllAssetPaths = AssetDatabase.GetAllAssetPaths();
            var needToCleanAssetPaths = afterTestAllAssetPaths.Where(x => Array.IndexOf(beforeTestAllAssetPaths, x) == -1).ToArray();

            try
            {
                TMP_FontAsset tmpFontAsset = null;
                foreach (var path in needToCleanAssetPaths)
                {
                    if (AssetDatabase.GetMainAssetTypeAtPath(path) != typeof(TMP_FontAsset))
                        continue;
                    tmpFontAsset = AssetDatabase.LoadMainAssetAtPath(path) as TMP_FontAsset;
                }

                if (tmpFontAsset != null)
                {
                    // The actual test
                    Assert.That(tmpFontAsset.atlasTexture.width, Is.EqualTo(0));
                    tmpFontAsset.TryAddCharacters("abcd");
                    Assert.That(tmpFontAsset.atlasTexture.width, Is.Not.EqualTo(0));
                    AssetDatabase.SaveAssets();
                    Assert.That(tmpFontAsset.atlasTexture.width, Is.EqualTo(0), () => "TMP_FontAsset atlas texture is not cleared.");
                }
                else
                {
                    Debug.LogError("Cannot find newly created TMP_FontAsset");
                }
            }
            finally
            {
                // Cleanup
                // Debug.Log($"Need to delete next assets:\n{string.Join("\n", needToCleanAssetPaths)}");
                var failed = new List<string>();
                AssetDatabase.DeleteAssets(needToCleanAssetPaths, failed);
                if (failed.Count > 0)
                    Debug.LogError($"Something went wrong. Some temporary test assets were not deleted:\n {string.Join("\n", failed)}");
            }
        }
    }
}