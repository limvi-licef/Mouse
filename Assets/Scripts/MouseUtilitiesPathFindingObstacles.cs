using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.UI.BoundsControl;
using System;

public class MouseUtilitiesPathFindingObstacles : MonoBehaviour
{
    List<GameObject> m_cubes;

    private void Awake()
    {
        m_cubes = new List<GameObject>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void addCube(string name, Vector3 scaling, Vector3 position, string color, bool navMeshTag, bool callbackOnTouch, Transform parent)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);

        // Set parent
        cube.transform.parent = parent;

        // Add buttons to interface
        MouseUtilitiesAdminMenu.Instance.addButton("Bring " + name, delegate ()
        {
            MouseUtilities.bringObject(cube.transform);
        }, MouseUtilitiesAdminMenu.Panels.Obstacles);
        MouseUtilitiesAdminMenu.Instance.addSwitchButton("Hide " + name, delegate ()
        {
            MouseUtilities.showInteractionSurface(cube.transform, !cube.GetComponent<Renderer>().enabled);
        }, MouseUtilitiesAdminMenu.Panels.Obstacles);

        // Set color
        MouseUtilities.setColor(cube.transform.transform, color);

        // Set scaling and position
        cube.transform.position = position;
        cube.transform.localScale = scaling;

        // Set the manipulation features
        cube.AddComponent<ObjectManipulator>();
        cube.AddComponent<RotationAxisConstraint>().ConstraintOnRotation = Microsoft.MixedReality.Toolkit.Utilities.AxisFlags.XAxis | Microsoft.MixedReality.Toolkit.Utilities.AxisFlags.ZAxis;
        BoundsControl boundsControl = cube.AddComponent<BoundsControl>();
        boundsControl.ScaleHandlesConfig.ScaleBehavior = Microsoft.MixedReality.Toolkit.UI.BoundsControlTypes.HandleScaleMode.NonUniform;
        boundsControl.TranslationHandlesConfig.ShowHandleForX = true;
        boundsControl.TranslationHandlesConfig.ShowHandleForY = true;
        boundsControl.TranslationHandlesConfig.ShowHandleForZ = true;

        // Set optional features
        if (navMeshTag)
        {
            cube.AddComponent<NavMeshSourceTag>();
        }

        if (callbackOnTouch)
        { // As we will be adding the MouseAssistanceBasic, it requires to encapsulate the cube in an empty gameobject, and to rename the cube "Child"
            GameObject child = cube;
            child.name = "Child";
            cube = new GameObject(name);
            child.transform.parent = cube.transform;
            
            cube.AddComponent<MouseAssistanceBasic>();
        }

        m_cubes.Add(cube);
    }

    public List<GameObject> getCubes()
    {
        return m_cubes;
    }
}
