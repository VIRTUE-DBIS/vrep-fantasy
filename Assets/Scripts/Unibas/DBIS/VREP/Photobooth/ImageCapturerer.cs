﻿using System;
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

        public void Capture(Action<byte[]> handler)
        {
            StartCoroutine(DoCapture(handler));
        }


        private IEnumerator DoCapture(Action<byte[]> handler)
        {
            yield return new WaitForEndOfFrame();
            var tex = Renderer.material.mainTexture.Convert();
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