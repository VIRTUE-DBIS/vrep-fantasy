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

        public Material Material;

        public void Capture(Action<byte[]> handler)
        {
            StartCoroutine(DoCapture(handler));
        }


        private IEnumerator DoCapture(Action<byte[]> handler)
        {
            yield return new WaitForEndOfFrame();
            var tex = Material.mainTexture.Convert();
            byte[] bytes = tex.EncodeToPNG();
            Destroy(tex);
            handler.Invoke(bytes);
        }
        
        
    }
}