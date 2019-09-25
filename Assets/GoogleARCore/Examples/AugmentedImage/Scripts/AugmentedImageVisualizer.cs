//-----------------------------------------------------------------------
// <copyright file="AugmentedImageVisualizer.cs" company="Google">
//
// Copyright 2018 Google Inc. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

namespace GoogleARCore.Examples.AugmentedImage
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using GoogleARCore;
    using GoogleARCoreInternal;
    using UnityEngine;
    using UnityEngine.Video;

    /// <summary>
    /// Uses 4 frame corner objects to visualize an AugmentedImage.
    /// </summary>
    public class AugmentedImageVisualizer : MonoBehaviour
    {
        /// <summary>
        /// The AugmentedImage to visualize.
        /// </summary>
        public AugmentedImage Image;

        public AudioSource audioSource;

        GameObject ChairButton;
        GameObject CouchButton;
        GameObject Table1Button;
        GameObject Table2Button;
        GameObject Table3Button;

        /// <summary>
        /// A model for the lower left corner of the frame to place when an image is detected.
        /// </summary>
        public GameObject videoPlane;

        bool playing = false;
        

        VideoPlayer _videoPlayer;

        public void Start()
        {
            ChairButton = GameObject.Find("ChairButton");
            CouchButton = GameObject.Find("CouchButton");
            Table1Button = GameObject.Find("Table1Button");
            Table2Button = GameObject.Find("Table2Button");
            Table3Button = GameObject.Find("Table3Button");

            if (!AugmentedImageExampleController.Instance.buttonStatus[0]) Table1Button.SetActive(false);
            if (!AugmentedImageExampleController.Instance.buttonStatus[1]) Table2Button.SetActive(false);
            if (!AugmentedImageExampleController.Instance.buttonStatus[2]) Table3Button.SetActive(false);
            if (!AugmentedImageExampleController.Instance.buttonStatus[3]) ChairButton.SetActive(false);
            if (!AugmentedImageExampleController.Instance.buttonStatus[4]) CouchButton.SetActive(false);

            _videoPlayer = videoPlane.GetComponent<VideoPlayer>();
            _videoPlayer.loopPointReached += OnStop;
        }

        void OnStop(UnityEngine.Video.VideoPlayer source)
        {
            _videoPlayer.Stop();
            videoPlane.SetActive(false);
        }

        /// <summary>
        /// The Unity Update method.
        /// </summary>
        public void Update()
        {
            if (Image == null || Image.TrackingState != TrackingState.Tracking || Image.TrackingMethod!=AugmentedImageTrackingMethod.FullTracking)
            {
                _videoPlayer.Stop();
                videoPlane.SetActive(false);
                return;
            }

            if (!playing)
            {
                switch (Image.DatabaseIndex)
                {
                    case 0:
                        ChairButton.SetActive(true); AugmentedImageExampleController.Instance.buttonStatus[3] = true;
                        break;
                    case 1:
                        CouchButton.SetActive(true); AugmentedImageExampleController.Instance.buttonStatus[4] = true;
                        break;
                    case 2:
                        Table1Button.SetActive(true); AugmentedImageExampleController.Instance.buttonStatus[0] = true;
                        Table2Button.SetActive(true); AugmentedImageExampleController.Instance.buttonStatus[1] = true;
                        Table3Button.SetActive(true); AugmentedImageExampleController.Instance.buttonStatus[2] = true;
                        break;
                }
                audioSource.Play();
                videoPlane.SetActive(true);
                _videoPlayer.Play();
                playing = true;
            }

            videoPlane.transform.localScale = new Vector3(Image.ExtentZ*0.1f, 1, Image.ExtentX*0.1f);
            
        }
    }
}
