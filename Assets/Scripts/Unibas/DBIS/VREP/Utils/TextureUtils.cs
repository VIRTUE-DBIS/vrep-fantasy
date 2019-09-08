using UnityEngine;

namespace DefaultNamespace
{
    public static class TextureUtils
    {
        public static Texture2D Convert(this Texture tex)
        {
            Texture2D image = new Texture2D(tex.width, tex.height, TextureFormat.RGB24, false);
            
            RenderTexture activeBackup = RenderTexture.active;
            
            RenderTexture renderTexture = new RenderTexture(tex.width, tex.height/2, 24);
            Graphics.Blit(tex, renderTexture);
            RenderTexture.active = renderTexture;
            image.ReadPixels(new Rect(0,renderTexture.height/2f,renderTexture.width, renderTexture.height/2f), 0,0);
            image.Apply();

            RenderTexture.active = activeBackup;
            
            return image;
        }
    }
}