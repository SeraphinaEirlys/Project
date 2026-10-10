using System.IO;
using UnityEditor;
using UnityEngine;

public static class SettingsRowHighlightGenerator
{
    private const int Width = 1024;
    private const int Height = 128;

    // Tăng/giảm độ sáng nền được tạo ra.
    private const float MaxAlpha = 0.95f;

    private static readonly Color Tint =
        new Color(0.82f, 0.82f, 0.88f, 1f);

    [MenuItem("Tools/Generate Settings Row Highlight")]
    private static void Generate()
    {
        const string folder = "Assets/_Sprites/UI";
        const string path = folder + "/SettingsRowHighlight.png";

        Directory.CreateDirectory(folder);

        var texture = new Texture2D(
            Width,
            Height,
            TextureFormat.RGBA32,
            false);

        try
        {
            var pixels = new Color[Width * Height];

            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    float u = (x + 0.5f) / Width;
                    float v = (y + 0.5f) / Height;

                    // Dải sáng tập trung ở giữa theo chiều dọc,
                    // rồi mờ dần về mép trên và dưới.
                    float vertical = v - 0.5f;
                    float verticalFade =
                        Mathf.Exp(-(vertical * vertical) * 34f);

                    // Hai đầu dải sáng mờ dần theo chiều ngang.
                    float leftFade = Mathf.SmoothStep(0f, 0.10f, u);
                    float rightFade =
                        1f - Mathf.SmoothStep(0.90f, 1f, u);

                    float alpha = MaxAlpha *
                                  verticalFade *
                                  leftFade *
                                  rightFade;

                    pixels[y * Width + x] = new Color(
                        Tint.r,
                        Tint.g,
                        Tint.b,
                        Mathf.Clamp01(alpha));
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();

            File.WriteAllBytes(path, texture.EncodeToPNG());
        }
        finally
        {
            Object.DestroyImmediate(texture);
        }

        AssetDatabase.ImportAsset(
            path,
            ImportAssetOptions.ForceSynchronousImport);

        var importer = AssetImporter.GetAtPath(path) as TextureImporter;

        if (importer == null)
        {
            Debug.LogError("Không thể import ảnh: " + path);
            return;
        }

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = 100f;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.filterMode = FilterMode.Bilinear;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.textureCompression =
            TextureImporterCompression.Uncompressed;

        var settings = new TextureImporterSettings();
        importer.ReadTextureSettings(settings);
        settings.spriteMeshType = SpriteMeshType.FullRect;
        importer.SetTextureSettings(settings);

        importer.SaveAndReimport();

        var sprite =
            AssetDatabase.LoadAssetAtPath<Sprite>(path);

        Selection.activeObject = sprite;
        EditorGUIUtility.PingObject(sprite);

        Debug.Log("Đã tạo dải sáng: " + path);
    }
}