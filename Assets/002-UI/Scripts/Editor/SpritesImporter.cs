using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SpritesImporter : AssetPostprocessor
{
    void OnPreprocessTexture() {
        
        var textureImporter = (TextureImporter) assetImporter;

        if (!textureImporter.assetPath.Contains("Sprites")) return;
        textureImporter.textureCompression = TextureImporterCompression.CompressedHQ;
        textureImporter.textureType = TextureImporterType.Sprite;
        textureImporter.spriteImportMode = SpriteImportMode.Single;
    }
}
