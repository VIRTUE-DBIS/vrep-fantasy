using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unibas.DBIS.VREP.Photobooth.Models;
using UnityEngine;

namespace Unibas.DBIS.VREP.Photobooth
{
    public class PhotoboothClient : MonoBehaviour
    {
        public string ServerUrl;
        public string ImageHostUrl;

        public PhotoboothClientHandler Handler;

        private string suffix;

        private string response;

        private const string GET_POSTCARDS_ACTION = "postcards";
        private const string GET_POSTCARD_ACTION = "postcard/:id";
        private const string ID_PARAMETER_NAME = ":id";
        private const string GET_HISTORY_ACTION = "history";
        private const string GET_PRINT_ACTION = "print/:id";
        private const string POST_SNAPSHOT_ACTION = "snapshot";

        private Action<string> responseProcessor;


        /// <summary>
        /// Requests an exhibition and calls the processor, once the exhibition is loaded.
        /// </summary>
        /// <param name="exhibitionId">The ID of the exhibition</param>
        /// <param name="processor">An Action which processes VREM's response. If null is passed to that action, an error occurred</param>
        public void RequestExhibition(string exhibitionId, Action<string> processor)
        {
            // TODO Refactor Action to a proper interface
            this.suffix = exhibitionId;
            responseProcessor = processor;
//            StartCoroutine(DoExhibitionRequest());
        }

        /// <summary>
        /// Queries the server for a list of postcard ids, requestable as images using GET_POSTCARD_ACTION
        /// </summary>
        public void GetPostcards()
        {
            StartCoroutine(RequestGet<PostcardsList>(ServerUrl + GET_POSTCARDS_ACTION, Handler.HandleGetPostcards,
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
        

        private static IEnumerator<WWW> RequestGet<O>(string url, Action<O> processor,
            Action<string> errorHandler = null)
        {
            WWW www = new WWW(url);
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