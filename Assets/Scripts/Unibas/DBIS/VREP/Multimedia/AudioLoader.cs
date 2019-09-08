using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace DefaultNamespace
{
    
    public class AudioLoader : MonoBehaviour
    {
        private AudioSource audioSource;

        private bool _loaded = false;
        private string _lastUrl = null;


        // Use this for initialization
        void Start()
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        IEnumerator LoadClip(string url)
        {
            if (_loaded && _lastUrl.Equals(url))
            {
                Play();
                yield break;
            }
            
            using (UnityWebRequest uwr = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.OGGVORBIS))
            {
                yield return uwr.SendWebRequest();

                if (uwr.isNetworkError || uwr.isHttpError)
                {
                    Debug.Log(uwr.error);
                }
                else
                {
                    // Get downloaded asset bundle
                    var audioClip = DownloadHandlerAudioClip.GetContent(uwr);
                    audioSource.clip = audioClip;
                    audioSource.volume = 0.2f;
                    audioSource.loop = true;
                    audioSource.Play();
                    _loaded = true;
                    _lastUrl = url;
                }
            }
        }

        private IEnumerator LoadAudio(string url)
        {
            if (_loaded && _lastUrl.Equals(url))
            {
                Play();
                yield break;
            }

            using (WWW www = new WWW(url))
            {
                yield return www;

                if (www.isDone)
                {
                    AudioClip audioClip = www.GetAudioClip(false, true) as AudioClip;
                    this.audioSource.clip = audioClip;
                    this.audioSource.volume = 0.2f;
                    this.audioSource.loop = true;
                    this.audioSource.Play();
                    _loaded = true;
                    _lastUrl = url;
                }
            }
        }

        /// <summary>
        ///  Plays the audio which was previosuly loaded via ReloadAudio().
        /// </summary>
        public void Play()
        {
            if (_loaded)
            {
                audioSource.Play();
            }
            else
            {
                ReloadAudio(_lastUrl);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void Stop()
        {
            audioSource.Stop();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        public void ReloadAudio(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                StartCoroutine(LoadClip(url));
            }
        }
    }
}