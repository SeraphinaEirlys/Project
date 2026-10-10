using System.IO;
using UnityEditor;
using UnityEngine;

public static class CloudGlowGenerator
{
    [MenuItem("Tools/Generate Cloud Glow Sprite")]
    static void Generate()
    {
        const int width = 512;
        const int height = 256;
        const float softness = 2.4f;
        const float maxAlpha = 1f;

        var tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        var pixels = new Color[width * height];

        for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
        {
            float nx = (x + 0.5f) / width * 2f - 1f;
            float ny = (y + 0.5f) / height * 2f - 1f;
            float d = Mathf.Sqrt(nx * nx + ny * ny);

            float a = Mathf.Exp(-(d * softness) * (d * softness));

            a *= 1f - Mathf.SmoothStep(0.75f, 1f, d);

            pixels[y * width + x] = new Color(1f, 1f, 1f, a * maxAlpha);
        }

        tex.SetPixels(pixels);
        tex.Apply();

        string path = "Assets/_Sprites/UI/TabCloud.png";
        File.WriteAllBytes(path, tex.EncodeToPNG());
        Object.DestroyImmediate(tex);
        AssetDatabase.ImportAsset(path);

        var importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.textureType = TextureImporterType.Sprite;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.SaveAndReimport();
        Debug.Log("Đã tạo " + path);
    }
}