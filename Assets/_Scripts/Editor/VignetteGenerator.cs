using System.IO;
using UnityEditor;
using UnityEngine;

public static class VignetteGenerator
{
    [MenuItem("Tools/Generate Vignette Sprite")]
    static void Generate()
    {
        const int width = 1024;
        const int height = 576;
        const float inner = 0.5f;
        const float outer = 1.35f;
        const float maxAlpha = 0.9f;
        Color tint = new Color(0.04f, 0.03f, 0.08f);

        var tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        var pixels = new Color[width * height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float nx = (x + 0.5f) / width * 2f - 1f;
                float ny = (y + 0.5f) / height * 2f - 1f;
                float d = Mathf.Sqrt(nx * nx + ny * ny);

                float t = Mathf.InverseLerp(inner, outer, d);
                t = t * t * (3f - 2f * t);

                pixels[y * width + x] = new Color(tint.r, tint.g, tint.b, t * maxAlpha);
            }
        }

        tex.SetPixels(pixels);
        tex.Apply();

        string path = "Assets/_Sprites/UI/Vignette.png";
        Directory.CreateDirectory(Path.GetDirectoryName(path));
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