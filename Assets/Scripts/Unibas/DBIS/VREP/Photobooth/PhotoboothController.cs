using System;
using System.Collections;
using System.Collections.Generic;
using Unibas.DBIS.VREP.Photobooth.Models;
using UnityEngine;
using UnityEngine.Networking;

namespace Unibas.DBIS.VREP.Photobooth
{
    public class PhotoboothController : MonoBehaviour, PhotoboothClientHandler
    {
        private bool activated = false;

        public ImageCapturerer Capturerer;
        public PostcardScreen Screen;
        
        private PhotoboothClient client;

        private List<string> capturedCards = new List<string>();
        private string[] availableCards;

        private void Awake()
        {
            client = gameObject.AddComponent<PhotoboothClient>();
            client.SetServerURL("http://192.168.92.22:5002");
            client.Handler = this;
        }

        private void Start()
        {
            activated = VREPController.Instance.Settings.EnablePhotobooth;
        }

        private bool first = true;

        private void Update()
        {
            if (first)
            {
                RequestRandomPostcard();
                first = false;
            }
        }


        public void RequestRandomPostcard()
        {
            client.GetRandomPostcard();
        }

        public void UploadImage()
        {
            Capturerer.Capture((bytes =>
            {
                Debug.Log("Uplaoding image");
                client.PostSnapshot(bytes, "C4823_1"); // TODO fix this
                Debug.Log("Sent bytes...");
            }));
        }
        
        public void DisplayPostcard(string id)
        {
            
            client.GetPostcardInfo(id);
        }

        public void PollForCompletion(string id, int interval, Action<string> completionHandler)
        {
            StartCoroutine(PollForImage(client.GetImageUrl(id), 60, s => Debug.Log("Done for " + s)));
        }
        
        private IEnumerator PollForImage(string url, int interval, Action<string> completionHandler)
        {
            using (UnityWebRequest uwr = UnityWebRequest.Get(url))
            {
                yield return uwr.SendWebRequest();

                if (uwr.isNetworkError || uwr.isHttpError)
                {
                    Debug.Log(uwr.error);
                }
                else
                {
                    if (uwr.responseCode == 200)
                    {
                        completionHandler.Invoke(url);
                    }
                    else
                    {
                        yield return new WaitForSecondsRealtime(interval);
                    }
                }
            }
        }

        public void HandleGetPostcards(PostcardsList list)
        {
            availableCards = list.postcards;
        }

        public void HandleRandomPostcard(PostcardsList list)
        {
            Debug.Log("Will display image with id: "+list.postcards[0]);
            DisplayPostcard(list.postcards[0]);
        }

        public void HandlePostSnapshot(IdObject idObject)
        {
            capturedCards.Add(idObject.id);
        }

        public void HandleGetHistory(HistoryList list)
        {
            throw new NotImplementedException();
        }

        public void HandleGetPrint(SuccessResponse response)
        {
            // we just ignore it for now
        }

        public void HandleError(string msg)
        {
            Debug.LogError(msg);
        }

        public void HandlePostcardInfo(ImageInfo obj)
        {
            Screen.Display(client.GetImageUrl(obj.id), obj.width, obj.height);
        }
    }
}