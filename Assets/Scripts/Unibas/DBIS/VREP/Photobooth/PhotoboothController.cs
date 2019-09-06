using System;
using UnityEngine;

namespace Unibas.DBIS.VREP.Photobooth
{
    public class PhotoboothController : MonoBehaviour
    {
        private bool activated = false;
        
        private void Start()
        {
            activated = VREPController.Instance.Settings.EnablePhotobooth;
        }

        private void Update()
        {
            
        }
    }
}