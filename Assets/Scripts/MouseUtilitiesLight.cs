using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseUtilitiesLight : MonoBehaviour
{
    public bool m_followUser = false;
    public GameObject m_hologramToLookAt;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (m_followUser)
        {
            /*if (Vector3.Distance(Camera.main.transform.position, transform.position) > 1)
            {*/
            Vector3 cameraPosition = Camera.main.transform.position;
            Vector3 cubePosition = m_hologramToLookAt.transform.parent.position;

            Vector3 direction = (cubePosition - cameraPosition).normalized;

            Vector3 positionFinal = cubePosition - direction * 1.5f;

            gameObject.transform.position = positionFinal;
            gameObject.transform.LookAt(m_hologramToLookAt.transform.parent.position);
            //}
        }
    }
}
