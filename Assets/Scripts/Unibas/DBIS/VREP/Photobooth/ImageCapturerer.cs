using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.Networking;

namespace Unibas.DBIS.VREP.Photobooth
{
    public class ImageCapturerer : MonoBehaviour
    {

        public Renderer Renderer;
        
        public float ScalingFactorX = 2f;
        public float ScalingFactorY = 3f;
        public float TextureScalingFactorX = 0.2f;
        public float TextureScalingFactorY = 0.35f;
        public float TextureOffsetX = 0.35f;
        public float TextureOffsetY = 0.4f;
       
        public void Capture(Action<byte[]> handler)
        {
            StartCoroutine(DoCapture(handler));
        }


        private IEnumerator DoCapture(Action<byte[]> handler)
        {
            yield return new WaitForEndOfFrame();
            var material = Renderer.material;
                
            material.mainTextureOffset = new Vector2(TextureOffsetX, TextureOffsetY);
            material.mainTextureScale = new Vector2(1 * TextureScalingFactorX, -(1 / 2f) * TextureScalingFactorY);

            var tex = material.mainTexture.Convert();
            var newTex = RotateAndCrop(tex);
            byte[] bytes = newTex.EncodeToPNG();
            Destroy(tex);
            Destroy(newTex);
            handler.Invoke(bytes);
        }

        private Texture2D RotateAndCrop(Texture2D tex)
        {
            var newHeight = tex.height / 2;
            Color[] pixels = new Color[newHeight*tex.width];
            Texture2D resized = new Texture2D(tex.width, newHeight, TextureFormat.RGB24, false);
            
            for(int y = 0; y<newHeight; y++)
            {
                for (int x = 0; x < tex.width; x++)
                {
                    var col = tex.GetPixel(x, y + newHeight);
                    pixels[(newHeight - 1 - y) * tex.width + x] = col;
                }
            }
            
            resized.SetPixels(pixels);
            resized.Apply();
            Destroy(tex);
            return resized;
        }
        
        
    }
}