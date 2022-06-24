using System.IO;
using UnityEngine;

namespace GKitForUnity.Unity.Utility {
    public static class AssetUtility {
        //Resource
        public static T GetResource<T>(this string path) where T : UnityEngine.Object {
            return GResourceUtility.Get<T>(path);
        }

        public static Texture2D GetTexture(this string localPath, TextureFormat format = TextureFormat.RGBA32, bool generateMipmap = true) {
            if (File.Exists(localPath)) {
                byte[] binary = File.ReadAllBytes(localPath);
                Texture2D tex = new Texture2D(2, 2, format, generateMipmap);
                tex.LoadRawTextureData(binary);
                return tex;
            }

            return null;
        }
    }
}