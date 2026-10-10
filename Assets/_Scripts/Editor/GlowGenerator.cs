using System.IO;
using UnityEditor;
using UnityEngine;

public static class GlowGenerator
{
    [MenuItem("Tools/Generate Tab Glow Sprite")]
    static void Generate()
    {
        const int width = 256;
        const int height = 64;
        Color glow = new Color(0.75f, 0.8f, 1f);

        var tex = new Texture2D(width, height, TextureFormat.RGBA32, false);
        var pixels = new Color[width * height];

        for (int y = 0; y < height; y++)
        for (int x = 0; x < width; x++)
        {
            float nx = (x + 0.5f) / width * 2f - 1f;
            float ny = (y + 0.5f) / height * 2f - 1f;
            float d = Mathf.Sqrt(nx * nx + ny * ny);
            float a = Mathf.Clamp01(1f - d);
            a = Mathf.Pow(a, 1.5f);
            pixels[y * width + x] = new Color(glow.r, glow.g, glow.b, a);
        }

        tex.SetPixels(pixels);
        tex.Apply();

        string path = "Assets/_Sprites/UI/TabGlow.png";
        File.WriteAllBytes(path, tex.EncodeToPNG());
        Object.DestroyImmediate(tex);
        AssetDatabase.ImportAsset(path);

        var importer = (TextureImporter)AssetImporter.GetAtPath(path);
        importer.textureType = TextureImporterType.Sprite;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.SaveAndReimport();
        Debug.Log("Đã tạo " + path);
    }
}