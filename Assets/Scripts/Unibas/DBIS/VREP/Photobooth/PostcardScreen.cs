using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Unibas.DBIS.VREP.Photobooth
{
    public class PostcardScreen : MonoBehaviour
    {
        private string url;

        private float width;
        private float height;

        private void Start()
        {
            _renderer = GetComponent<MeshRenderer>();
        }

        public void Display(string url, float width, float height)
        {
            this.url = url;
            this.width = width;
            this.height = height;
            StartCoroutine(GetText(url));
            Rescale(width, height);
        }
        
        private void Rescale(float width, float height)
        {
            float ratio = width / height;
            float newWidth = 5f; // TODO make configurable
            float newHeight = newWidth / ratio;
            transform.localScale = new Vector3(newWidth, newHeight);
            transform.localPosition = new Vector3(transform.localPosition.x, (-transform.localPosition.y + newHeight)/2f, transform.localPosition.z);
        }
        
        private MeshRenderer _renderer;

        private IEnumerator LoadImage(string url)
        {
            Texture2D tex = new Texture2D(512, 512, TextureFormat.ARGB32, true);
            var hasError = false;
            using (WWW www = new WWW(url))
            {
                yield return www;
                if (string.IsNullOrEmpty(www.error)) {
                    www.LoadImageIntoTexture(tex);
                    GetComponent<Renderer>().material.mainTexture = tex;
                    GC.Collect();
                } else {
                    Debug.LogError(www.error);
                    Debug.LogError(www.url);
                    Debug.LogError(www.responseHeaders);
                    hasError = true;
                }
			
            }

            if (hasError)
            {
                _renderer.material.mainTexture = Resources.Load<Texture>("Textures/not-available");
            }
            else
            {
                _renderer.material.mainTexture = tex;
			
            }
		
            Rescale(width, height);
        }
        
        IEnumerator GetText(string url)
        {
            using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(url))
            {
                yield return uwr.SendWebRequest();

                if (uwr.isNetworkError || uwr.isHttpError)
                {
                    Debug.Log(uwr.error);
                }
                else
                {
                    // Get downloaded asset bundle
                    var texture = DownloadHandlerTexture.GetContent(uwr);
                    _renderer.material.mainTexture = texture;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        public void ReloadImage(string url)
        {
            StartCoroutine(LoadImage(url));
        }
    }
}