using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unibas.DBIS.VREP.Photobooth.Models;
using UnityEngine;
using UnityEngine.Networking;

namespace Unibas.DBIS.VREP.Photobooth
{
    public class PhotoboothClient : MonoBehaviour
    {
        public string ServerUrl;

        public PhotoboothClientHandler Handler;

        private const string LIST_POSTCARDS_ACTION = "postcard/list";
        private const string GET_POSTCARDS_RANDOM_ACTION = "postcard/random";
        private const string GET_POSTCARD_ACTION = "postcard/image/:id";
        private const string GET_POSTCARD_AUDIO_ACTION = "postcard/audio/:id";
        private const string ID_PARAMETER_NAME = ":id";
        private const string GET_HISTORY_ACTION = "history/list";
        private const string GET_HISTORY_IMAGE_ACITON = "history/image/:id";
        private const string GET_PRINT_ACTION = "print/:id";
        private const string GET_GENERATE_ACTION = "generate/:id";
        private const string POST_SNAPSHOT_ACTION = "snapshot";
        private const string FORM_FIELD_IMAGE_NAME = "file";

        private Action<string> responseProcessor;


        /// <summary>
        /// Queries the server for a list of postcard ids, requestable as images using GET_POSTCARD_ACTION
        /// </summary>
        public void GetPostcards()
        {
            StartCoroutine(RequestGet<PostcardsList>(ServerUrl + LIST_POSTCARDS_ACTION, Handler.HandleGetPostcards,
                Handler.HandleError));
        }

        public void SetServerURL(string url)
        {
            ServerUrl = url;
            SanitizeServerUrl();
        }
        
        public void GetRandomPostcard()
        {
            StartCoroutine(RequestGet<PostcardsList>(ServerUrl + GET_POSTCARDS_RANDOM_ACTION, Handler.HandleRandomPostcard,
                Handler.HandleError));
        }

        public void GetHistory()
        {
            StartCoroutine(RequestGet<HistoryList>(ServerUrl + GET_HISTORY_ACTION, Handler.HandleGetHistory,
                Handler.HandleError));
        }

        public void GetPrint(string id)
        {
            StartCoroutine(RequestGet<SuccessResponse>(ServerUrl + GET_PRINT_ACTION.Replace(ID_PARAMETER_NAME, id), Handler.HandleGetPrint,
                Handler.HandleError));
        }

        public void PostSnapshot(string data)
        {
            StartCoroutine(RequestPost<string, IdObject>(ServerUrl + POST_SNAPSHOT_ACTION,data, Handler.HandlePostSnapshot,
                Handler.HandleError));
        }

        public void PostSnapshot(byte[] data, string id)
        {
            StartCoroutine(UploadBytesDirectly(data, ServerUrl + GET_GENERATE_ACTION.Replace(ID_PARAMETER_NAME, id), Handler.HandlePostSnapshot,
                Handler.HandleError));
        }

        private IEnumerator UploadBytesDirectly(byte[] bytes, string url, Action<IdObject> processor, Action<string> errorHandler)
        {
            WWW www = new WWW(url, bytes);
            yield return www;
            if (www.error == null && processor != null)
            {
                processor.Invoke(JsonUtility.FromJson<IdObject>(www.text));
            }
            else
            {
                Debug.LogError(www.error);
                if (errorHandler != null)
                {
                    errorHandler.Invoke(www.error);
                }
            }
        }

        private IEnumerator UploadBytes(byte[] bytes, string url)
        {
            WWWForm form = new WWWForm();
            form.AddBinaryData(FORM_FIELD_IMAGE_NAME, bytes, "image.png", "image/png");

            using (var w = UnityWebRequest.Post(url, form))
            {
                yield return w.SendWebRequest();
                if (w.isNetworkError || w.isHttpError)
                {
                    Handler.HandleError(w.error);
                }
                else
                {
                    string response = w.downloadHandler.text;
                    Handler.HandlePostSnapshot(JsonUtility.FromJson<IdObject>(response));
                }
            }
        }

        private static IEnumerator<WWW> RequestGet<O>(string url, Action<O> processor,
            Action<string> errorHandler = null)
        {
            Debug.Log("RequestGet to "+url);
            WWW www = new WWW(url);
            yield return www;
            if (www.error == null && processor != null)
            {
                Debug.Log("Received: "+www.text);
                processor.Invoke(JsonUtility.FromJson<O>(www.text));
            }
            else
            {
                Debug.LogError(www.error);
                if (errorHandler != null)
                {
                    errorHandler.Invoke(www.error);
                }
            }
        }

        private static IEnumerator<WWW> RequestPost<I, O>(string url, I input, Action<O> processor,
            Action<string> errorHandler = null)
        {
            WWW www = GenerateJSONPostRequest(url, JsonUtility.ToJson(input));
            yield return www;
            if (www.error == null && processor != null)
            {
                processor.Invoke(JsonUtility.FromJson<O>(www.text));
            }
            else
            {
                Debug.LogError(www.error);
                if (errorHandler != null)
                {
                    errorHandler.Invoke(www.error);
                }
            }
        }

        public string GetImageUrl(string id)
        {
            return ServerUrl + GET_POSTCARD_ACTION.Replace(ID_PARAMETER_NAME, id);
        }

        public string GetAudioUrl(string id)
        {
            return ServerUrl + GET_POSTCARD_AUDIO_ACTION.Replace(ID_PARAMETER_NAME, id);
        }

        private void SanitizeServerUrl()
        {
            if (!ServerUrl.StartsWith("http://"))
            {
                ServerUrl = "http://" + ServerUrl;
            }

            if (!ServerUrl.EndsWith("/"))
            {
                ServerUrl = ServerUrl + "/";
            }
        }

        /**
             * Generates a WWW object with given params
             * 
             * @param url - A string which represents the url
             * @param json - The json data to send, as a string
             * 
             */
        public static WWW GenerateJSONPostRequest(string url, string json)
        {
            Hashtable headers = new Hashtable();
            headers.Add("Content-Type", "application/json");
            byte[] postData = Encoding.ASCII.GetBytes(json.ToCharArray());
            return new WWW(url,
                postData);
        }
    }
}