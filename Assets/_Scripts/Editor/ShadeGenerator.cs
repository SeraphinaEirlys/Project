using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class ShadeGenerator
{
    [MenuItem("Tools/Generate Shade Gradients")]
    static void Generate()
    {
        Make("Assets/_Sprites/UI/ShadeTop.png", 4, 256,
            (x, y, w, h) => Mathf.Pow(y / (float)(h - 1), 1.5f));

        Make("Assets/_Sprites/UI/ShadeLeft.png", 256, 4,
            (x, y, w, h) => Mathf.Pow(1f - x / (float)(w - 1), 1.5f));
    }

    static void Make(string path, int w, int h, Func<int, int, int, int, float> alpha)
    {
        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        var px = new Color[w * h];
        for (int y = 0; y < h; y++)
        for (int x = 0; x < w; x++)
            px[y * w + x] = new Color(1f, 1f, 1f, alpha(x, y, w, h));

        tex.SetPixels(px);
        tex.Apply();
        File.WriteAllBytes(path, tex.EncodeToPNG());
        UnityEngine.Object.DestroyImmediate(tex);
        AssetDatabase.ImportAsset(path);

        var imp = (TextureImporter)AssetImporter.GetAtPath(path);
        imp.textureType = TextureImporterType.Sprite;
        imp.alphaIsTransparency = true;
        imp.mipmapEnabled = false;
        imp.wrapMode = TextureWrapMode.Clamp;
        imp.textureCompression = TextureImporterCompression.Uncompressed;
        imp.SaveAndReimport();
        Debug.Log("Đã tạo " + path);
    }
}