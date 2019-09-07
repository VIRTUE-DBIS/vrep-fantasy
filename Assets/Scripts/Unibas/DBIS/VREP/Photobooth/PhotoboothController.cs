using System;
using System.Collections.Generic;
using Unibas.DBIS.VREP.Photobooth.Models;
using UnityEngine;

namespace Unibas.DBIS.VREP.Photobooth
{
    public class PhotoboothController : MonoBehaviour, PhotoboothClientHandler
    {
        private bool activated = false;

        public ImageLoader PostcardScreen;
        private PhotoboothClient client;

        private List<string> capturedCards;
        private string[] availableCards;
        
        private void Start()
        {
            activated = VREPController.Instance.Settings.EnablePhotobooth;
        }

        private void Update()
        {
            
        }

        public void DisplayPostcard(string id)
        {
            PostcardScreen.ReloadImage(client.GetImageUrl(id));
        }

        public void HandleGetPostcards(PostcardsList list)
        {
            availableCards = list.postcards;
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
    }
}