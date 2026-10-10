using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class StarSparkleGenerator
{
    const float bodyTip    = 0.25f;
    const float flareLen   = 0.42f;
    const float flareThick = 0.012f;

    [MenuItem("Tools/Generate Sparkle Star Sprite")]
    static void Generate()
    {
        Make("Assets/_Sprites/UI/SparkleStar.png", 256, 4, (nx, ny) =>
        {
            float ax = Mathf.Abs(nx), ay = Mathf.Abs(ny);
            if (Mathf.Max(ax, ay) > flareLen + 0.02f) return 0f;

            bool body = Mathf.Sqrt(ax / bodyTip) + Mathf.Sqrt(ay / bodyTip) <= 1f;

            bool flareH = ax <= flareLen && ay <= flareThick * (1f - ax / flareLen);
            bool flareV = ay <= flareLen && ax <= flareThick * (1f - ay / flareLen);

            return (body || flareH || flareV) ? 1f : 0f;
        });

        Make("Assets/_Sprites/UI/SparkleGlow.png", 256, 1, (nx, ny) =>
        {
            float r = Mathf.Sqrt(nx * nx + ny * ny);
            float a = Mathf.Exp(-(r * r) / 0.14f);
            return a * (1f - Mathf.SmoothStep(0.8f, 1f, r));
        });
    }

    static void Make(string path, int size, int ss, Func<float, float, float> alphaAt)
    {
        var tex = new Texture2D(size, size, TextureFormat.RGBA32, false);
        var px = new Color[size * size];

        for (int y = 0; y < size; y++)
        for (int x = 0; x < size; x++)
        {
            float sum = 0f;
            for (int sy = 0; sy < ss; sy++)
            for (int sx = 0; sx < ss; sx++)
            {
                float fx = (x + (sx + 0.5f) / ss) / size * 2f - 1f;
                float fy = (y + (sy + 0.5f) / ss) / size * 2f - 1f;
                sum += alphaAt(fx, fy);
            }
            px[y * size + x] = new Color(1f, 1f, 1f, sum / (ss * ss));
        }

        tex.SetPixels(px);
        tex.Apply();
        File.WriteAllBytes(path, tex.EncodeToPNG());
        UnityEngine.Object.DestroyImmediate(tex);
        AssetDatabase.ImportAsset(path);

        var imp = (TextureImporter)AssetImporter.GetAtPath(path);
        imp.textureType = TextureImporterType.Sprite;
        imp.alphaIsTransparency = true;
        imp.mipmapEnabled = false;
        imp.filterMode = FilterMode.Bilinear;
        imp.wrapMode = TextureWrapMode.Clamp;
        imp.textureCompression = TextureImporterCompression.Uncompressed;
        imp.SaveAndReimport();
        Debug.Log("Đã tạo " + path);
    }
}