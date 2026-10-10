using System.IO;
using UnityEditor;
using UnityEngine;

public static class PauseHoverCoreGenerator
{
    // Match the canvas and frame geometry of PauseHoverGenerator.
    private const int Width = 720;
    private const int Height = 132;
    private const float FramePadding = 10f;
    private const float CornerCut = 12f;

    // Adjust these values, then run the generator again.
    private const float CoreOpacity = 0.26f;
    private const float EdgeSoftness = 18f;
    private const float BorderGap = 2f;

    private static readonly Color Tint =
        new Color(0.91f, 0.90f, 0.96f, 1f);

    [MenuItem("Tools/Pause Menu/Generate Hover Core Sprite")]
    private static void Generate()
    {
        const string path = "Assets/_Sprites/UI/PauseHoverCore.png";
        Directory.CreateDirectory("Assets/_Sprites/UI");

        var texture = new Texture2D(
            Width, Height, TextureFormat.RGBA32, false);

        try
        {
            var pixels = new Color[Width * Height];
            float halfWidth = Width * 0.5f - FramePadding;
            float halfHeight = Height * 0.5f - FramePadding;

            for (int y = 0; y < Height; y++)
            for (int x = 0; x < Width; x++)
            {
                float px = x + 0.5f - Width * 0.5f;
                float py = y + 0.5f - Height * 0.5f;
                float ax = Mathf.Abs(px);
                float ay = Mathf.Abs(py);

                float cornerDistance =
                    (ax + ay - (halfWidth + halfHeight - CornerCut))
                    / Mathf.Sqrt(2f);

                float distance = Mathf.Max(
                    Mathf.Max(ax - halfWidth, ay - halfHeight),
                    cornerDistance);

                // Keep the core inside the chamfered frame and soften its edges.
                float edgeMask = Mathf.SmoothStep(
                    0f, 1f,
                    Mathf.InverseLerp(BorderGap, EdgeSoftness, -distance));

                float nx = px / halfWidth;
                float ny = py / halfHeight;
                float centerLight = Mathf.Exp(
                    -(nx * nx * 1.2f + ny * ny * 2f));

                float alpha = CoreOpacity * edgeMask *
                    Mathf.Lerp(0.35f, 1f, centerLight);

                pixels[y * Width + x] = new Color(
                    Tint.r, Tint.g, Tint.b, Mathf.Clamp01(alpha));
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
            path, ImportAssetOptions.ForceSynchronousImport);

        var importer = AssetImporter.GetAtPath(path) as TextureImporter;
        if (importer == null)
        {
            Debug.LogError("Could not import hover core: " + path);
            return;
        }

        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.spritePixelsPerUnit = 100f;

        var settings = new TextureImporterSettings();
        importer.ReadTextureSettings(settings);
        settings.spriteMeshType = SpriteMeshType.FullRect;
        importer.SetTextureSettings(settings);

        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.filterMode = FilterMode.Bilinear;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.maxTextureSize = 1024;
        importer.SaveAndReimport();

        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);
        Selection.activeObject = sprite;
        EditorGUIUtility.PingObject(sprite);
        Debug.Log("Generated hover core: " + path);
    }
}
