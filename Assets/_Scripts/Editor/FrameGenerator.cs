using System.IO;
using UnityEditor;
using UnityEngine;

public static class FrameGenerator
{
    [MenuItem("Tools/Generate Menu Frame Sprite")]
    static void Generate()
    {
        const int w = 128, h = 48;
        const int corner = 14;
        Color line = new Color(0.93f, 0.93f, 0.96f);
        Color fill = new Color(0.07f, 0.07f, 0.09f);

        var tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        var px = new Color[w * h];

        for (int y = 0; y < h; y++)
        for (int x = 0; x < w; x++)
        {
            int cx = Mathf.Min(x, w - 1 - x);   // khoảng cách tới cạnh dọc gần nhất
            int cy = Mathf.Min(y, h - 1 - y);   // khoảng cách tới cạnh ngang gần nhất

            // viền mảnh 2px, riêng ở góc dày thành 4px (tạo cảm giác góc nhọn)
            bool horiz = cy < 2 || (cy < 4 && cx < corner);
            bool vert  = cx < 2 || (cx < 4 && cy < corner);

            if (horiz || vert)
            {
                bool inCorner = cx < corner && cy < corner;
                px[y * w + x] = new Color(line.r, line.g, line.b, inCorner ? 1f : 0.6f);
            }
            else
            {
                px[y * w + x] = new Color(fill.r, fill.g, fill.b, 0.7f);
            }
        }

        tex.SetPixels(px);
        tex.Apply();

        string path = "Assets/_Sprites/UI/MenuFrame.png";
        File.WriteAllBytes(path, tex.EncodeToPNG());
        Object.DestroyImmediate(tex);
        AssetDatabase.ImportAsset(path);

        var imp = (TextureImporter)AssetImporter.GetAtPath(path);
        imp.textureType = TextureImporterType.Sprite;
        imp.alphaIsTransparency = true;
        imp.mipmapEnabled = false;
        imp.filterMode = FilterMode.Bilinear;
        imp.wrapMode = TextureWrapMode.Clamp;
        imp.textureCompression = TextureImporterCompression.Uncompressed;
        imp.spriteBorder = new Vector4(corner, corner, corner, corner); // cho 9-slice
        imp.SaveAndReimport();
        Debug.Log("Đã tạo " + path);
    }
}