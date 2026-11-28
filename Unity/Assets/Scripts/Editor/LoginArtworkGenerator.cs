#if UNITY_EDITOR
using System.IO;
using UnityEditor;
using UnityEngine;

namespace CorruptStateRP.Editor {
    public static class LoginArtworkGenerator {
        const int Width = 1600;
        const int Height = 900;

        [MenuItem("Corrupt State RP/Generate Login Artwork")]
        public static void Generate() {
            var texture = new Texture2D(Width, Height, TextureFormat.RGB24, false);
            texture.name = "CorruptStateLoginBackground";
            var pixels = new Color[Width * Height];
            var top = new Color(.035f, .055f, .12f);
            var horizon = new Color(.72f, .27f, .22f);
            var road = new Color(.035f, .045f, .065f);
            for (int y = 0; y < Height; y++) {
                float t = y / (float)(Height - 1);
                Color sky = Color.Lerp(top, horizon, Mathf.Pow(t, 2.1f) * .82f);
                for (int x = 0; x < Width; x++) {
                    float dx = (x - Width * .48f) / (Width * .46f);
                    float dy = (y - Height * .43f) / (Height * .38f);
                    float glow = Mathf.Clamp01(1f - Mathf.Sqrt(dx * dx + dy * dy));
                    Color c = Color.Lerp(sky, new Color(1f, .48f, .25f), glow * .22f);
                    if (y < Height * .19f) c = Color.Lerp(road, new Color(.12f, .13f, .16f), y / (Height * .19f));
                    pixels[y * Width + x] = c;
                }
            }
            texture.SetPixels(pixels);
            var random = new System.Random(60128);
            DrawSkyline(texture, random);
            DrawPalm(texture, 80, 260, 1f);
            DrawPalm(texture, 1450, 240, .82f);
            DrawRoad(texture);
            DrawCar(texture, 80, 65);
            texture.Apply(false, false);

            const string directory = "Assets/Resources/Art";
            Directory.CreateDirectory(directory);
            File.WriteAllBytes(directory + "/LoginBackground.png", texture.EncodeToPNG());
            Object.DestroyImmediate(texture);
            AssetDatabase.ImportAsset(directory + "/LoginBackground.png", ImportAssetOptions.ForceSynchronousImport | ImportAssetOptions.ForceUpdate);
            var importer = AssetImporter.GetAtPath(directory + "/LoginBackground.png") as TextureImporter;
            if (importer != null) {
                importer.textureType = TextureImporterType.Sprite;
                importer.spriteImportMode = SpriteImportMode.Single;
                importer.mipmapEnabled = false;
                importer.maxTextureSize = 2048;
                importer.textureCompression = TextureImporterCompression.Compressed;
                importer.compressionQuality = 85;
                importer.wrapMode = TextureWrapMode.Clamp;
                importer.filterMode = FilterMode.Bilinear;
                importer.SaveAndReimport();
            }
            Debug.Log("Generated premium Corrupt State RP login artwork: " + directory + "/LoginBackground.png");
        }

        static void DrawSkyline(Texture2D t, System.Random random) {
            int x = -20;
            while (x < Width + 20) {
                int w = random.Next(48, 112);
                int h = random.Next(150, 420);
                int baseY = 250 + random.Next(-20, 45);
                Color building = new Color(.035f, .055f, .095f, 1f);
                Rect(t, x, baseY, w, h, building);
                Rect(t, x + 5, baseY + h, w - 10, 8, new Color(.09f, .12f, .17f));
                int rows = h / 28;
                int cols = w / 18;
                for (int row = 1; row < rows; row++) {
                    for (int col = 1; col < cols; col++) {
                        if (random.NextDouble() > .56) {
                            Color window = random.NextDouble() > .72
                                ? new Color(1f, .62f, .28f, 1f)
                                : new Color(.34f, .56f, .78f, 1f);
                            Rect(t, x + col * 16, baseY + row * 25, 5, 10, window);
                        }
                    }
                }
                x += w + random.Next(7, 20);
            }
            // A taller downtown tower anchors the skyline.
            Rect(t, 675, 250, 150, 500, new Color(.025f, .045f, .08f));
            Rect(t, 720, 750, 60, 34, new Color(.04f, .065f, .11f));
            for (int y = 300; y < 720; y += 34)
                for (int xw = 695; xw < 805; xw += 25)
                    Rect(t, xw, y, 8, 14, new Color(.95f, .57f, .25f));
        }

        static void DrawPalm(Texture2D t, int x, int y, float scale) {
            Color dark = new Color(.018f, .03f, .05f);
            Line(t, x, y, x + (int)(12 * scale), y + (int)(220 * scale), 10, dark);
            for (int i = 0; i < 7; i++) {
                float a = (i / 6f) * Mathf.PI - Mathf.PI * .12f;
                int ex = x + (int)(Mathf.Cos(a) * 145 * scale);
                int ey = y + (int)(Mathf.Sin(a) * 70 * scale) + (int)(220 * scale);
                Line(t, x + (int)(12 * scale), y + (int)(220 * scale), ex, ey, 7, dark);
            }
        }

        static void DrawRoad(Texture2D t) {
            Rect(t, 0, 0, Width, 175, new Color(.025f, .032f, .045f));
            Rect(t, 0, 168, Width, 7, new Color(.23f, .25f, .29f));
            for (int x = 0; x < Width; x += 190) Rect(t, x, 52, 92, 7, new Color(.72f, .63f, .42f, .65f));
            Rect(t, 0, 172, Width, 3, new Color(.95f, .52f, .23f, .35f));
        }

        static void DrawCar(Texture2D t, int x, int y) {
            Color body = new Color(.012f, .02f, .035f);
            Rect(t, x, y + 20, 500, 90, body);
            Rect(t, x + 105, y + 92, 250, 80, new Color(.018f, .03f, .05f));
            Rect(t, x + 125, y + 105, 95, 52, new Color(.12f, .2f, .29f));
            Rect(t, x + 235, y + 105, 100, 52, new Color(.1f, .17f, .25f));
            Circle(t, x + 105, y + 28, 48, new Color(.008f, .01f, .018f));
            Circle(t, x + 105, y + 28, 24, new Color(.18f, .2f, .23f));
            Circle(t, x + 385, y + 28, 48, new Color(.008f, .01f, .018f));
            Circle(t, x + 385, y + 28, 24, new Color(.18f, .2f, .23f));
            Rect(t, x + 455, y + 54, 36, 18, new Color(1f, .78f, .42f));
            Rect(t, x + 4, y + 58, 24, 24, new Color(.75f, .12f, .1f));
        }

        static void Rect(Texture2D t, int x, int y, int w, int h, Color c) {
            int x0 = Mathf.Clamp(x, 0, t.width), x1 = Mathf.Clamp(x + w, 0, t.width);
            int y0 = Mathf.Clamp(y, 0, t.height), y1 = Mathf.Clamp(y + h, 0, t.height);
            for (int py = y0; py < y1; py++)
                for (int px = x0; px < x1; px++) t.SetPixel(px, py, c);
        }

        static void Circle(Texture2D t, int cx, int cy, int radius, Color c) {
            int r2 = radius * radius;
            for (int y = -radius; y <= radius; y++)
                for (int x = -radius; x <= radius; x++)
                    if (x * x + y * y <= r2) {\n                        int px = cx + x, py = cy + y;\n                        if (px >= 0 && px < t.width && py >= 0 && py < t.height) t.SetPixel(px, py, c);\n                    }
        }

        static void Line(Texture2D t, int x0, int y0, int x1, int y1, int thickness, Color c) {
            int dx = Mathf.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
            int dy = -Mathf.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
            int err = dx + dy;
            while (true) {
                Circle(t, x0, y0, thickness / 2, c);
                if (x0 == x1 && y0 == y1) break;
                int e2 = 2 * err;
                if (e2 >= dy) { err += dy; x0 += sx; }
                if (e2 <= dx) { err += dx; y0 += sy; }
            }
        }
    }
}
#endif