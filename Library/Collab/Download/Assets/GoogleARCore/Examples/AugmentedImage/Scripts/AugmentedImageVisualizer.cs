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

        GameObject ChairButton;
        GameObject CouchButton;
        GameObject Table1Button;
        GameObject Table2Button;
        GameObject Table3Button;

        /// <summary>
        /// A model for the lower left corner of the frame to place when an image is detected.
        /// </summary>
        public GameObject FrameLowerLeft;

        /// <summary>
        /// A model for the lower right corner of the frame to place when an image is detected.
        /// </summary>
        //public GameObject FrameLowerRight;

        ///// <summary>
        ///// A model for the upper left corner of the frame to place when an image is detected.
        ///// </summary>
        //public GameObject FrameUpperLeft;

        ///// <summary>
        ///// A model for the upper right corner of the frame to place when an image is detected.
        ///// </summary>
        //public GameObject FrameUpperRight;

        VideoPlayer _videoPlayer;

        public void Start()
        {
            ChairButton = GameObject.Find("Button4");
            CouchButton = GameObject.Find("Button5");
            Table1Button = GameObject.Find("Button1");
            Table2Button = GameObject.Find("Button2");
            Table3Button = GameObject.Find("Button3");

            Table1Button.SetActive(false);
            Table2Button.SetActive(false);
            Table3Button.SetActive(false);
            ChairButton.SetActive(false);
            CouchButton.SetActive(false);

            _videoPlayer = GetComponentInChildren<VideoPlayer>();
            _videoPlayer.loopPointReached += OnStop;
        }

        private void OnStop(VideoPlayer source)
        {
            FrameLowerLeft.SetActive(false);
        }

        /// <summary>
        /// The Unity Update method.
        /// </summary>
        public void Update()
        {
            if (Image == null || Image.TrackingState != TrackingState.Tracking)
            {
                FrameLowerLeft.SetActive(false);
                //FrameLowerRight.SetActive(false);
                //FrameUpperLeft.SetActive(false);
                //FrameUpperRight.SetActive(false);
                return;
            }

            if (!_videoPlayer.isPlaying)
            {
                switch (Image.DatabaseIndex)
                {
                    case 0:
                        ChairButton.SetActive(true);
                        break;
                    case 1:
                        CouchButton.SetActive(true);
                        break;
                    case 2:
                        Table1Button.SetActive(true);
                        Table2Button.SetActive(true);
                        Table3Button.SetActive(true);
                        break;
                }
                _videoPlayer.Play();
            }

            FrameLowerLeft.transform.localScale = new Vector3(Image.ExtentX*0.125f, 1, Image.ExtentZ*0.125f);

            float halfWidth = Image.ExtentX / 2;
            float halfHeight = Image.ExtentZ / 2;
            FrameLowerLeft.transform.localPosition =
                (halfWidth * Vector3.left) + (halfHeight * Vector3.back);
            //FrameLowerRight.transform.localPosition =
            //    (halfWidth * Vector3.right) + (halfHeight * Vector3.back);
            //FrameUpperLeft.transform.localPosition =
            //    (halfWidth * Vector3.left) + (halfHeight * Vector3.forward);
            //FrameUpperRight.transform.localPosition =
            //    (halfWidth * Vector3.right) + (halfHeight * Vector3.forward);

            FrameLowerLeft.SetActive(true);
            //FrameLowerRight.SetActive(true);
            //FrameUpperLeft.SetActive(true);
            //FrameUpperRight.SetActive(true);
        }
    }
}
