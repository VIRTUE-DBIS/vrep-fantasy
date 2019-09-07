using System;
using Unibas.DBIS.VREP.Photobooth.Models;
using UnityEngine;

namespace Unibas.DBIS.VREP.Photobooth
{
    public class PhotoboothController : MonoBehaviour, PhotoboothClientHandler
    {
        private bool activated = false;
        
        private void Start()
        {
            activated = VREPController.Instance.Settings.EnablePhotobooth;
        }

        private void Update()
        {
            
        }

        public void HandleGetPostcards(PostcardsList list)
        {
            throw new NotImplementedException();
        }

        public void HandlePostSnapshot(IdObject idObject)
        {
            throw new NotImplementedException();
        }

        public void HandleGetHistory(HistoryList list)
        {
            throw new NotImplementedException();
        }

        public void HandleGetPrint(SuccessResponse response)
        {
            throw new NotImplementedException();
        }

        public void HandleError(string msg)
        {
            throw new NotImplementedException();
        }
    }
}