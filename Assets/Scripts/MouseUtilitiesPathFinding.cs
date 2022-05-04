using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using System.Reflection;
using System;

public class MouseUtilitiesPathFinding : MonoBehaviour
{
    public Transform m_origin;
    public Transform m_target;
    private NavMeshPath m_path;
    private float m_elapsed = 0.0f;
    

    // Start is called before the first frame update
    void Start()
    {
        m_path = new NavMeshPath();
        m_elapsed = 0.0f;

        //NavMeshBuilder.CollectSources()
    }

    // Update is called once per frame
    void Update()
    {
        m_elapsed += Time.deltaTime;
        if(m_elapsed > 1.0f)
        {
            m_elapsed -= 1.0f;
            NavMesh.CalculatePath(m_origin.position, m_target.position, NavMesh.AllAreas, m_path);

            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Update navmesh called. Number of points in the path: " + m_path.corners.Length);
        }
        for (int i = 0; i < m_path.corners.Length - 1; i ++)
        {
            MouseDebugMessagesManager.Instance.displayMessage(MethodBase.GetCurrentMethod().ReflectedType.Name, MethodBase.GetCurrentMethod().Name, MouseDebugMessagesManager.MessageLevel.Info, "Line drew between : " + m_path.corners[i] + " and " + m_path.corners[i + 1]);
            Debug.DrawLine(m_path.corners[i], m_path.corners[i + 1], Color.red);
        }
    }
}
