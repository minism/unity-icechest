using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Presets;
using UnityEngine;

namespace Ice {

  // Uses preset assets in the current or parent directory for automatically applying
  // asset settings.
  public class PresetDirectoryProcessor : AssetPostprocessor {
    private SpriteSheetSettings spriteSheetSettings;

    void OnPreprocessAsset() {
      // Make sure we are applying presets the first time an asset is imported.
      if (assetImporter.importSettingsMissing) {
        // Get the current imported asset folder.
        var path = Path.GetDirectoryName(assetPath);
        while (!string.IsNullOrEmpty(path)) {
          // Find all Preset assets in this folder.
          var presetGuids = AssetDatabase.FindAssets("t:Preset", new[] { path });
          foreach (var presetGuid in presetGuids) {
            // Make sure we are not testing Presets in a subfolder.
            string presetPath = AssetDatabase.GUIDToAssetPath(presetGuid);
            if (Path.GetDirectoryName(presetPath) == path) {
              // Load the Preset and try to apply it to the importer.
              var preset = AssetDatabase.LoadAssetAtPath<Preset>(presetPath);
              if (preset.ApplyTo(assetImporter))
                MaybeLoadSpriteSheetSettings(Path.GetDirectoryName(presetPath));
                return;
            }
          }

          // Try again in the parent folder.
          path = Path.GetDirectoryName(path);
        }
      }
    }

    void OnPostprocessTexture(Texture2D texture) {
      if (spriteSheetSettings == null) {
        return;
      }
      var textureImporter = assetImporter as TextureImporter;
      if (textureImporter.textureType != TextureImporterType.Sprite) {
        return;
      }

      var w = spriteSheetSettings.cellWidth;
      var h = spriteSheetSettings.cellHeight;
      int cols = texture.width / w;
      int rows = texture.height / h;
      List<SpriteMetaData> metadatas = new List<SpriteMetaData>();
      for (int r = 0; r < rows; r++) {
        for (int c = 0; c < cols; c++) {
          var metadata = new SpriteMetaData();
          metadata.rect = new Rect(c * w, r * h, w, h);
          metadata.name = $"{c}-{r}";
          metadatas.Add(metadata);
        }
      }

      Debug.Log($"Ice: Automatically sliced sprite {assetPath} according to settings.");
      ((TextureImporter)assetImporter).spritesheet = metadatas.ToArray();
      AssetDatabase.Refresh();
    }

    void MaybeLoadSpriteSheetSettings(string dirname) {
      var guids = AssetDatabase.FindAssets("t:SpriteSheetSettings", new[] { dirname });
      if (guids.Length > 0) {
        spriteSheetSettings = AssetDatabase.LoadAssetAtPath<SpriteSheetSettings>(
            AssetDatabase.GUIDToAssetPath(guids[0]));
      }
    }
  }

} // namespace Ice
