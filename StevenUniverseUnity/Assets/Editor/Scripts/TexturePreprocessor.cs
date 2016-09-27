using UnityEditor;
using UnityEngine;

namespace StevenUniverse.FanGameEditor
{
    public class TexturePreprocessor : AssetPostprocessor
    {
        private const int ALIGNMENT_BOTTOM_LEFT = 6;

        void OnPreprocessTexture()
        {
            TextureImporter importer = (TextureImporter) assetImporter;
            TextureImporterSettings settings = new TextureImporterSettings();

            importer.ReadTextureSettings(settings);
            settings.readable = true;
            settings.filterMode = FilterMode.Point;
            settings.textureFormat = TextureImporterFormat.AutomaticTruecolor;

            //Directory-specific settings
            string texturePath = importer.assetPath;
            if (texturePath.Contains("Assets/Editor/Textures/EditorInstances/"))
            {
                settings.spriteAlignment = ALIGNMENT_BOTTOM_LEFT;
                settings.spritePixelsPerUnit = 16;
                settings.maxTextureSize = 32;
            }

            importer.SetTextureSettings(settings);
        }
    }
}