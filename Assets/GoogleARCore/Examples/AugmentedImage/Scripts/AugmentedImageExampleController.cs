//-----------------------------------------------------------------------
// <copyright file="AugmentedImageExampleController.cs" company="Google">
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
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using GoogleARCore;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// Controller for AugmentedImage example.
    /// </summary>
    /// <remarks>
    /// In this sample, we assume all images are static or moving slowly with
    /// a large occupation of the screen. If the target is actively moving,
    /// we recommend to check <see cref="AugmentedImage.TrackingMethod"/> and
    /// render only when the tracking method equals to
    /// <see cref="AugmentedImageTrackingMethod.FullTracking"/>.
    /// See details in <a href="https://developers.google.com/ar/develop/c/augmented-images/">
    /// Recognize and Augment Images</a>
    /// </remarks>
    public class AugmentedImageExampleController : MonoBehaviour
    {
        /// <summary>
        /// A prefab for visualizing an AugmentedImage.
        /// </summary>
        public AugmentedImageVisualizer AugmentedImageVisualizerPrefab;

        bool scanComplete = false;

        GameObject ChairButton;
        GameObject CouchButton;
        GameObject Table1Button;
        GameObject Table2Button;
        GameObject Table3Button;

        public bool[] buttonStatus = new bool[5];

        private static AugmentedImageExampleController s_Instance = null;

        public static AugmentedImageExampleController Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    var augImageControllers = FindObjectsOfType<AugmentedImageExampleController>();
                    if (augImageControllers.Length > 0)
                    {
                        s_Instance = augImageControllers[0];
                    }
                    else
                    {
                        Debug.LogError("No instance of AugmentedImageExampleController exists in the scene.");
                    }
                }

                return s_Instance;
            }
        }

        /// <summary>
        /// The overlay containing the fit to scan user guide.
        /// </summary>
        public GameObject FitToScanOverlay;

        private Dictionary<int, AugmentedImageVisualizer> m_Visualizers
            = new Dictionary<int, AugmentedImageVisualizer>();

        private List<AugmentedImage> m_TempAugmentedImages = new List<AugmentedImage>();

        public void Start()
        {
            ChairButton = GameObject.Find("ChairButton");
            CouchButton = GameObject.Find("CouchButton");
            Table1Button = GameObject.Find("Table1Button");
            Table2Button = GameObject.Find("Table2Button");
            Table3Button = GameObject.Find("Table3Button");

            Table1Button.SetActive(false);
            Table2Button.SetActive(false);
            Table3Button.SetActive(false);
            ChairButton.SetActive(false);
            CouchButton.SetActive(false);

            for(int i=0;i<5;i++)
            {
                buttonStatus[i] = false;
            }
        }

        /// <summary>
        /// The Unity Update method.
        /// </summary>
        public void Update()
        {
            // Exit the app when the 'back' button is pressed.
            //if (Input.GetKey(KeyCode.Escape))
            //{
            //    Application.Quit();
            //}

            // Get updated augmented images for this frame.
            Session.GetTrackables<AugmentedImage>(
                m_TempAugmentedImages, TrackableQueryFilter.Updated);

            // Create visualizers and anchors for updated augmented images that are tracking and do
            // not previously have a visualizer. Remove visualizers for stopped images.
            foreach (var image in m_TempAugmentedImages)
            {
                AugmentedImageVisualizer visualizer = null;
                m_Visualizers.TryGetValue(image.DatabaseIndex, out visualizer);
                if (image.TrackingState == TrackingState.Tracking && image.TrackingMethod == AugmentedImageTrackingMethod.FullTracking && visualizer == null)
                {
                    // Create an anchor to ensure that ARCore keeps tracking this augmented image.
                    Anchor anchor = image.CreateAnchor(image.CenterPose);
                    visualizer = (AugmentedImageVisualizer)Instantiate(
                        AugmentedImageVisualizerPrefab, anchor.transform);
                    visualizer.Image = image;
                    Table1Button.SetActive(true);
                    Table2Button.SetActive(true);
                    Table3Button.SetActive(true);
                    ChairButton.SetActive(true);
                    CouchButton.SetActive(true);
                    m_Visualizers.Add(image.DatabaseIndex, visualizer);
                }
                else if ((image.TrackingState == TrackingState.Stopped || image.TrackingMethod != AugmentedImageTrackingMethod.FullTracking) && visualizer != null)
                {
                    m_Visualizers.Remove(image.DatabaseIndex);
                    GameObject.Destroy(visualizer.gameObject);
                }
            }

            // Show the fit-to-scan overlay if there are no images that are Tracking.
            foreach (var visualizer in m_Visualizers.Values)
            {
                if (visualizer.Image.TrackingState == TrackingState.Tracking)
                {
                    FitToScanOverlay.SetActive(false);
                    scanComplete = true;
                    return;
                }
            }
            if(!scanComplete)
                FitToScanOverlay.SetActive(true);
        }
    }
}
