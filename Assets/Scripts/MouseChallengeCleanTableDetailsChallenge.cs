using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseChallengeCleanTableDetailsChallenge : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.LookAt(Camera.main.transform);
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(Camera.main.transform.position, transform.position) > 1 )
        {
            transform.LookAt(Camera.main.transform);
        }
        
    }
}
