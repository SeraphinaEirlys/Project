using System.IO;
using UnityEditor;
using UnityEngine;

public static class PauseHoverGenerator
{
    // Ảnh gấp đôi kích thước nút 360 x 66.
    private const int Width = 720;
    private const int Height = 132;

    // Các giá trị bên dưới có thể chỉnh rồi generate lại.
    private const float FillOpacity = 0.12f;
    private const float BorderOpacity = 0.48f;
    private const float GlowOpacity = 0.10f;

    private const float BorderThickness = 1.6f;
    private const float GlowSoftness = 5f;
    private const float CornerCut = 12f;

    // Màu trắng bạc hơi lạnh.
    private static readonly Color Tint =
        new Color(0.91f, 0.90f, 0.96f, 1f);

    [MenuItem("Tools/Pause Menu/Generate Hover Sprite")]
    private static void Generate()
    {
        const string path = "Assets/_Sprites/UI/PauseHover.png";

        Directory.CreateDirectory("Assets/_Sprites/UI");

        var texture = new Texture2D(
            Width,
            Height,
            TextureFormat.RGBA32,
            false);

        var pixels = new Color[Width * Height];

        // Chừa khoảng trống quanh khung để chứa glow.
        float halfFrameWidth = Width * 0.5f - 10f;
        float halfFrameHeight = Height * 0.5f - 10f;

        for (int y = 0; y < Height; y++)
        {
            for (int x = 0; x < Width; x++)
            {
                float px = x + 0.5f - Width * 0.5f;
                float py = y + 0.5f - Height * 0.5f;

                float ax = Mathf.Abs(px);
                float ay = Mathf.Abs(py);

                // Khoảng cách tới khung chữ nhật có góc vát.
                // Âm: bên trong. Dương: bên ngoài.
                float verticalDistance = ax - halfFrameWidth;
                float horizontalDistance = ay - halfFrameHeight;

                float cornerDistance =
                    (ax + ay -
                    (halfFrameWidth + halfFrameHeight - CornerCut))
                    / Mathf.Sqrt(2f);

                float distance = Mathf.Max(
                    Mathf.Max(verticalDistance, horizontalDistance),
                    cornerDistance);

                // Viền có cạnh mềm để hạn chế răng cưa.
                float border = 1f - Mathf.SmoothStep(
                    BorderThickness * 0.5f,
                    BorderThickness * 0.5f + 1f,
                    Mathf.Abs(distance));

                // Glow mềm quanh đường viền.
                float glowDistance = distance / GlowSoftness;
                float glow = Mathf.Exp(
                    -glowDistance * glowDistance);

                // Nền sáng nhẹ, tập trung ở giữa nút.
                float nx = px / halfFrameWidth;
                float ny = py / halfFrameHeight;

                float centerLight = Mathf.Exp(
                    -(nx * nx * 1.8f + ny * ny * 2.4f));

                float inside = 1f - Mathf.SmoothStep(
                    -1f,
                    1f,
                    distance);

                float fill = inside * FillOpacity *
                    Mathf.Lerp(0.35f, 1f, centerLight);

                // Kết hợp các lớp alpha.
                float alpha = fill;

                alpha = 1f - (1f - alpha) *
                    (1f - glow * GlowOpacity);

                alpha = 1f - (1f - alpha) *
                    (1f - border * BorderOpacity);

                // Fade về trong suốt ở mép ảnh.
                float edgeDistance = Mathf.Min(
                    Mathf.Min(x, Width - 1 - x),
                    Mathf.Min(y, Height - 1 - y));

                alpha *= Mathf.SmoothStep(0f, 4f, edgeDistance);

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
        Object.DestroyImmediate(texture);

        AssetDatabase.ImportAsset(
            path,
            ImportAssetOptions.ForceSynchronousImport);

        var importer =
            AssetImporter.GetAtPath(path) as TextureImporter;

        var settings = new TextureImporterSettings();

        if (importer != null)
        {
            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.spritePixelsPerUnit = 100f;
            importer.ReadTextureSettings(settings);
            settings.spriteMeshType = SpriteMeshType.FullRect;
            importer.SetTextureSettings(settings);

            importer.alphaIsTransparency = true;
            importer.mipmapEnabled = false;
            importer.filterMode = FilterMode.Bilinear;
            importer.wrapMode = TextureWrapMode.Clamp;
            importer.textureCompression =
                TextureImporterCompression.Uncompressed;

            importer.maxTextureSize = 1024;
            importer.SaveAndReimport();
        }

        var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(path);

        Selection.activeObject = sprite;
        EditorGUIUtility.PingObject(sprite);

        Debug.Log("Đã tạo sprite hover: " + path);
    }
}