using System;
using UnityEngine;
using Valve.VR;

namespace Unibas.DBIS.VREP.Photobooth
{
    /// <summary>
    /// Based on SteamVR_TestTrackedCamera
    /// </summary>
    public class MixedRealityCamera : MonoBehaviour
    {
        public MeshRenderer Screen;
        
        private Material material;
        private Transform target;

        public float ScalingFactor = 2f;
        
        
        private readonly bool undistorted = false;

        private void Awake()
        {
            material = Screen.material;
            target = Screen.transform;
        }

        private void OnEnable()
        {
            // The video stream must be symmetrically acquired and released in
            // order to properly disable the stream once there are no consumers.
            SteamVR_TrackedCamera.VideoStreamTexture source = SteamVR_TrackedCamera.Source(undistorted); // Empirically found
            source.Acquire();

            // Auto-disable if no camera is present.
            if (!source.hasCamera)
                enabled = false;
        }

        private void OnDisable()
        {
            // Clear the texture when no longer active.
            material.mainTexture = null;

            // The video stream must be symmetrically acquired and released in
            // order to properly disable the stream once there are no consumers.
            SteamVR_TrackedCamera.VideoStreamTexture source = SteamVR_TrackedCamera.Source(undistorted);
            source.Release();
        }

        private void Update()
        {
            SteamVR_TrackedCamera.VideoStreamTexture source = SteamVR_TrackedCamera.Source(undistorted);
            Texture2D texture = source.texture;
            if (texture == null)
            {
                return;
            }

            // Apply the latest texture to the material.  This must be performed
            // every frame since the underlying texture is actually part of a ring
            // buffer which is updated in lock-step with its associated pose.
            // (You actually really only need to call any of the accessors which
            // internally call Update on the SteamVR_TrackedCamera.VideoStreamTexture).
            material.mainTexture = texture;

            // Adjust the height of the quad based on the aspect to keep the texels square.
            float aspect = (float)texture.width / (texture.height / 2f);

            
            material.mainTextureOffset = Vector2.zero;
            material.mainTextureScale = new Vector2(ScalingFactor, -(ScalingFactor/2f));

            target.localScale = new Vector3(ScalingFactor, ScalingFactor / aspect, ScalingFactor);

        }
        
    }
}