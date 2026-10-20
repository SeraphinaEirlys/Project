using System.IO;
using UnityEditor;
using UnityEngine;

public static class HealthFrameGenerator
{
    private const float LeftCapFraction = 0.28f;
    private const float RightCapFraction = 0.07f;

    [MenuItem("Tools/Health HUD/Generate 128x64 Frame")]
    private static void Generate128x64() => Generate(128, 64);

    [MenuItem("Tools/Health HUD/Generate 256x128 Frame")]
    private static void Generate256x128() => Generate(256, 128);

    private static void Generate(int width, int height)
    {
        string sourceAssetPath = FindHealthHudAsset();
        if (string.IsNullOrEmpty(sourceAssetPath))
        {
            Debug.LogError("Không tìm thấy HealthHUD.png trong Assets của project hiện tại. " +
                           "Unity đang dùng thư mục Assets: " + Application.dataPath +
                           ". Hãy kiểm tra ảnh có nằm trong Assets của đúng project đang mở không.");
            return;
        }

        string projectRoot = Directory.GetParent(Application.dataPath).FullName;
        string sourceFullPath = Path.Combine(projectRoot, sourceAssetPath);
        var source = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        if (!ImageConversion.LoadImage(source, File.ReadAllBytes(sourceFullPath), false))
        {
            Object.DestroyImmediate(source);
            Debug.LogError("Không đọc được ảnh nguồn: " + sourceAssetPath);
            return;
        }

        float scale = Mathf.Min((float)width / source.width, (float)height / source.height);
        int contentWidth = Mathf.Max(1, Mathf.RoundToInt(source.width * scale));
        int contentHeight = Mathf.Max(1, Mathf.RoundToInt(source.height * scale));
        int offsetX = (width - contentWidth) / 2;
        int offsetY = (height - contentHeight) / 2;
        var output = new Texture2D(width, height, TextureFormat.RGBA32, false);
        var pixels = new Color[width * height];
        for (int y = 0; y < contentHeight; y++)
        {
            float v = (y + 0.5f) / contentHeight;
            for (int x = 0; x < contentWidth; x++)
                pixels[(y + offsetY) * width + x + offsetX] = source.GetPixelBilinear((x + 0.5f) / contentWidth, v);
        }
        output.SetPixels(pixels);
        output.Apply();
        Object.DestroyImmediate(source);

        string fileName = $"HealthHUD_{width}x{height}_9Slice.png";
        string sourceDirectory = Path.GetDirectoryName(sourceAssetPath).Replace('\\', '/');
        string assetPath = sourceDirectory + "/" + fileName;
        string fullPath = Path.Combine(projectRoot, assetPath);
        File.WriteAllBytes(fullPath, output.EncodeToPNG());
        Object.DestroyImmediate(output);
        AssetDatabase.ImportAsset(assetPath, ImportAssetOptions.ForceUpdate);
        var importer = AssetImporter.GetAtPath(assetPath) as TextureImporter;
        if (importer == null)
        {
            Debug.LogError("Không thể cấu hình Sprite Importer: " + assetPath);
            return;
        }

        int left = Mathf.RoundToInt(width * LeftCapFraction);
        int right = Mathf.RoundToInt(width * RightCapFraction);
        int verticalCap = Mathf.Max(1, height / 4);
        importer.textureType = TextureImporterType.Sprite;
        importer.spriteImportMode = SpriteImportMode.Single;
        importer.alphaIsTransparency = true;
        importer.mipmapEnabled = false;
        importer.filterMode = FilterMode.Bilinear;
        importer.wrapMode = TextureWrapMode.Clamp;
        importer.textureCompression = TextureImporterCompression.Uncompressed;
        importer.spritePixelsPerUnit = 100f;
        importer.spriteBorder = new Vector4(left, verticalCap, right, verticalCap);
        importer.SaveAndReimport();
        Debug.Log($"Đã tạo {assetPath} ({width}x{height}). Dùng UI Image với Image Type = Sliced; border trái/phải {left}/{right}px.");
    }

    private static string FindHealthHudAsset()
    {
        string[] guids = AssetDatabase.FindAssets("HealthHUD t:Texture2D", new[] { "Assets" });
        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (string.Equals(Path.GetFileName(path), "HealthHUD.png", System.StringComparison.OrdinalIgnoreCase))
                return path;
        }
        return null;
    }
}
