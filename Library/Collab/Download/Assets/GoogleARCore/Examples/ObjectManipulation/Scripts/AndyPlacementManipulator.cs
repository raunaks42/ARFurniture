//-----------------------------------------------------------------------
// <copyright file="AndyPlacementManipulator.cs" company="Google">
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

namespace GoogleARCore.Examples.ObjectManipulation
{
    using GoogleARCore;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using System.Collections.Generic;

    /// <summary>
    /// Controls the placement of Andy objects via a tap gesture.
    /// </summary>
    public class AndyPlacementManipulator : Manipulator
    {
        /// <summary>
        /// The first-person camera being used to render the passthrough camera image (i.e. AR
        /// background).
        /// </summary>
        public Camera FirstPersonCamera;

        /// <summary>
        /// A model to place when a raycast from a user touch hits a plane.
        /// </summary>
        public GameObject AndyPrefab;

        public GameObject Table1Prefab;
        public GameObject Table2Prefab;
        public GameObject Table3Prefab;
        public GameObject ChairPrefab;
        public GameObject CouchPrefab;
        int prefab = 1;

        /// <summary>
        /// Manipulator prefab to attach placed objects to.
        /// </summary>
        public GameObject ManipulatorPrefab;

        public void SelectTable1()
        {
            AndyPrefab = Table1Prefab;
            prefab = 1;
        }

        public void SelectTable2()
        {
            AndyPrefab = Table2Prefab;
            prefab = 2;
        }

        public void SelectTable3()
        {
            AndyPrefab = Table3Prefab;
            prefab = 3;
        }

        public void SelectChair()
        {
            AndyPrefab = ChairPrefab;
            prefab = 4;
        }

        public void SelectCouch()
        {
            AndyPrefab = CouchPrefab;
            prefab = 5;
        }

        //When Touching UI
        private bool IsPointerOverUIObject()
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
            return results.Count > 0;
        }


        /// <summary>
        /// Returns true if the manipulation can be started for the given gesture.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <returns>True if the manipulation can be started.</returns>
        protected override bool CanStartManipulationForGesture(TapGesture gesture)
        {
            if (gesture.TargetObject == null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Function called when the manipulation is ended.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        protected override void OnEndManipulation(TapGesture gesture)
        {
            if (gesture.WasCancelled)
            {
                return;
            }

            // If gesture is targeting an existing object we are done.
            if (gesture.TargetObject != null)
            {
                return;
            }

            // Raycast against the location the player touched to search for planes.
            TrackableHit hit;
            TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon;

            if (Frame.Raycast(
                gesture.StartPosition.x, gesture.StartPosition.y, raycastFilter, out hit)) 
            {
                if (!IsPointerOverUIObject())
                {
                    // Use hit pose and camera pose to check if hittest is from the
                    // back of the plane, if it is, no need to create the anchor.
                    if ((hit.Trackable is DetectedPlane) &&
                        Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position,
                            hit.Pose.rotation * Vector3.up) < 0)
                    {
                        Debug.Log("Hit at back of the current DetectedPlane");
                    }
                    else
                    {
                        // Instantiate Andy model at the hit pose.
                        var andyObject = Instantiate(AndyPrefab, hit.Pose.position, hit.Pose.rotation);

                        // Instantiate manipulator.
                        var manipulator =
                            Instantiate(ManipulatorPrefab, hit.Pose.position, hit.Pose.rotation);

                        // Make Andy model a child of the manipulator.
                        andyObject.transform.parent = manipulator.transform;

                        // Create an anchor to allow ARCore to track the hitpoint as understanding of
                        // the physical world evolves.
                        var anchor = hit.Trackable.CreateAnchor(hit.Pose);

                        // Make manipulator a child of the anchor.
                        manipulator.transform.parent = anchor.transform;

                        // Select the placed object.
                        manipulator.GetComponent<Manipulator>().Select();

                        switch(prefab)
                        {
                            case 1: manipulator.GetComponentInChildren<Transform>().localScale = new Vector3(10, 10, 10);
                                    break;
                            case 2: manipulator.GetComponentInChildren<Transform>().localScale = new Vector3(5, 5, 5);
                                    break;
                            case 3: manipulator.GetComponentInChildren<Transform>().localScale = new Vector3(11f, 11f, 11f);
                                    break;
                            case 4: manipulator.GetComponentInChildren<Transform>().localScale = new Vector3(2.15f, 2.15f, 2.15f);
                                    break;
                            case 5: manipulator.GetComponentInChildren<Transform>().localScale = new Vector3(11, 11, 11);
                                    break;
                        }
                    }
                }
            }
        }
    }
}
