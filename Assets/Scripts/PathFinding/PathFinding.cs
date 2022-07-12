using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using Microsoft.MixedReality.Toolkit.Experimental.SpatialAwareness;
using System.Reflection;
using System;

namespace MATCH
{
    namespace PathFinding
    {
        public class PathFinding : MonoBehaviour
        {
            private NavMeshPath m_path;

            NavMeshData m_NavMesh;
            AsyncOperation m_Operation;
            NavMeshDataInstance m_Instance;
            List<NavMeshBuildSource> m_Sources = new List<NavMeshBuildSource>();

            Transform m_interactionSurfaceView;
            Assistances.InteractionSurface m_interactionSurfaceController;

            Obstacles m_obstaclesManager;

            private void Awake()
            {

            }

            // Start is called before the first frame update
            IEnumerator Start()
            {
                // Get children and components
                m_interactionSurfaceView = transform.Find("AssistanceInteractionSurface");
                m_interactionSurfaceController = m_interactionSurfaceView.GetComponent<Assistances.InteractionSurface>();
                m_obstaclesManager = GetComponent<Obstacles>();

                // Set admin buttons
                AdminMenu.Instance.addButton("Add obstacle", callbackAddObstacle, AdminMenu.Panels.Obstacles);
                m_interactionSurfaceController.SetAdminButtons("path surface");
                m_interactionSurfaceController.SetColor("Mouse_Cyan_Glowing");
                m_interactionSurfaceView.position = new Vector3(0.3f, -0.16f, -5f);
                m_interactionSurfaceController.SetScaling(new Vector3(0.1f, 1f, 0.1f));
                m_interactionSurfaceController.ShowInteractionSurfaceTable(true);
                m_interactionSurfaceController.SetObjectResizable(true);
                m_interactionSurfaceController.SetPreventResizeY(true);

                // Add component to the interaction surface to define walkable surface
                m_interactionSurfaceController.GetInteractionSurface().gameObject.AddComponent<NavMeshSourceTag>();

                // Add callbacks
                m_obstaclesManager.s_moved += callbackObstacleResizedOrMoved;
                m_obstaclesManager.s_resized += callbackObstacleResizedOrMoved;

                // Nav mesh computation
                while (true)
                {
                    UpdateNavMesh(true);
                    yield return m_Operation;
                }
            }

            void OnEnable()
            {
                m_interactionSurfaceView = transform.Find("AssistanceInteractionSurface");
                m_interactionSurfaceController = m_interactionSurfaceView.GetComponent<Assistances.InteractionSurface>();

                // Construct and add navmesh
                m_NavMesh = new NavMeshData();
                m_Instance = NavMesh.AddNavMeshData(m_NavMesh);
                /*if (m_Tracked == null)
                    m_Tracked = transform;*/
                UpdateNavMesh(false);
            }

            void OnDisable()
            {
                // Unload navmesh and clear handle
                m_Instance.Remove();
            }

            void UpdateNavMesh(bool asyncUpdate = false)
            {
                NavMeshSourceTag.Collect(ref m_Sources);
                var defaultBuildSettings = NavMesh.GetSettingsByID(0);
                var bounds = QuantizedBounds();

                if (asyncUpdate)
                    m_Operation = NavMeshBuilder.UpdateNavMeshDataAsync(m_NavMesh, defaultBuildSettings, m_Sources, bounds);
                else
                    NavMeshBuilder.UpdateNavMeshData(m_NavMesh, defaultBuildSettings, m_Sources, bounds);
            }

            public Vector3[] computePath(Transform origin, Transform destination)
            {
                DebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, DebugMessagesManager.MessageLevel.Info, "Number of registered meshes: " + NavMesh.GetSettingsCount());

                NavMeshPath path = new NavMeshPath();
                if (NavMesh.CalculatePath(origin.position, destination.position, NavMesh.AllAreas, path) == false)
                {
                    DebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, DebugMessagesManager.MessageLevel.Warning, "Cannot compute the path");
                }

                DebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, DebugMessagesManager.MessageLevel.Info, "Number of corner: " + path.corners.Length + " Start position: " + origin.position + " Target position: " + destination.position);

                return path.corners;
            }

            Bounds QuantizedBounds()
            {
                Vector3 center = new Vector3(0, 0, 0);

                if (m_interactionSurfaceController.GetInteractionSurface() != null)
                {
                    center = m_interactionSurfaceController.GetInteractionSurface().position; //new Vector3(1.0f, 0, 2.94f)
                                                                                              //MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Center: " + center);

                }
                else
                {
                    //MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Warning, "Interaction surface object not yet initialized - center initialized to (0,0,0)");
                }


                Vector3 size = new Vector3(20f, 20f, 20f);

                // Quantize the bounds to update only when theres a 10% change in size
                return new Bounds(center, size);
            }

            // Update is called once per frame
            void Update()
            {

            }

            void callbackAddObstacle()
            {
                Vector3 position = new Vector3(Camera.main.transform.position.x + 0.5f, Camera.main.transform.position.y, Camera.main.transform.position.z);

                Vector3 scaling = new Vector3(0.1f, 0.1f, 0.1f);

                m_obstaclesManager.addCube("Obstacle " + (m_obstaclesManager.getCubes().Count + 1).ToString(), scaling, position, "Mouse_White_Transparent", true, false, transform);
            }

            void callbackObstacleResizedOrMoved(System.Object sender, EventArgs e)
            {
                DebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, DebugMessagesManager.MessageLevel.Info, "Called");

                GameObject cube = (GameObject)sender;
                Transform interactionSurface = m_interactionSurfaceController.GetInteractionSurface();

                float max = cube.transform.position.y + cube.transform.localScale.y / 2.0f;
                float newPosY = (max - interactionSurface.position.y) / 2.0f + interactionSurface.position.y;
                float newScalingY = (max - interactionSurface.position.y);

                cube.transform.localScale = new Vector3(cube.transform.localScale.x, newScalingY, cube.transform.localScale.z);
                cube.transform.position = new Vector3(cube.transform.position.x, newPosY, cube.transform.position.z);

            }
        }

    }
}

