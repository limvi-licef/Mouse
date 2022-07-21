/*Copyright 2022 Guillaume Spalla

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.*/

using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Timers;
using System.Reflection;
using System.Linq;

namespace MATCH
{
    namespace Utilities
    {
        public class PhysicalObjectInformation
        {
            string Name; //object name
            Vector3 Coord; //coordinates
            Vector3 FirstCorner; //boundingboxcorner
            Vector3 SecondCorner; //boundingboxcorner

            public string GetObjectName()
            {
                return Name;
            }
            /*
            public PhysicalObjectVectors getObjectBoundaries()
            {
                PhysicalObjectVectors vectors = new PhysicalObjectVectors();
                vectors.setVectors(m_coord, m_firstCorner, m_secondCorner);
                return vectors;
            }
            */
            public void SetObjectParams(string name, Vector3 center, Vector3 firstBoundary, Vector3 secondBoundary)
            {
                Name = name;
                Coord = center;
                FirstCorner = firstBoundary;
                SecondCorner = secondBoundary;
            }

            public Vector3 GetCenter()
            {
                return Coord;
            }

            public Vector3 GetFirstBoundary()
            {
                return FirstCorner;
            }

            public Vector3 GetSecondBoundary()
            {
                return SecondCorner;
            }

        }
        /*
        public class PhysicalObjectVectors
        {
            Vector3 m_coord; //coordinates
            Vector3 m_firstCorner; //boundingboxcorner
            Vector3 m_secondCorner; //boundingboxcorner

            public void setVectors(Vector3 center, Vector3 firstBoundary, Vector3 secondBoundary)
            {
                m_coord = center;
                m_firstCorner = firstBoundary;
                m_secondCorner = secondBoundary;
            }

            public void getVectors(out Vector3 center,out Vector3 firstBoundary,out Vector3 secondBoundary)
            {
                center = m_coord;
                firstBoundary = m_firstCorner;
                secondBoundary = m_secondCorner;
            }
        }*/
    }
}
