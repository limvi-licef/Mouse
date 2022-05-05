using UnityEngine;
using UnityEngine.AI;

// Use physics raycast hit from mouse click to set agent destination
[RequireComponent(typeof(NavMeshAgent))]
public class ClickToMove : MonoBehaviour
{
    NavMeshAgent m_Agent;
    RaycastHit m_HitInfo = new RaycastHit();

    bool m_drawLine = false;

    void Start()
    {
        m_Agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftShift))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray.origin, ray.direction, out m_HitInfo))
            {
                m_Agent.destination = m_HitInfo.point;
                m_drawLine = true;
            }
                
        }
    }

   private void OnDrawGizmos()
    {
// Debug.Log("Called");

        if (m_drawLine)
        {
            m_drawLine = false;

            /*Debug.Log("Number of corner: " + m_Agent.path.corners.Length);

            for (int i = 1; i < m_Agent.path.corners.Length; i++)
            {
                Vector3 cornerStart = m_Agent.path.corners[i - 1];
                Vector3 cornerEnd = m_Agent.path.corners[i];
                Gizmos.DrawLine(cornerStart, cornerEnd);
                Debug.Log("Corner start: " + cornerStart + " corner end: " + cornerEnd);
            }*/

            Vector3 target = new Vector3(m_Agent.transform.position.x + 100, m_Agent.transform.position.y, m_Agent.transform.position.z + 50);

            target = m_Agent.pathEndPosition;

            NavMeshPath path = new NavMeshPath();
            NavMesh.CalculatePath(m_Agent.transform.position, target, NavMesh.AllAreas, path);


            Debug.Log("Number of corner: " + path.corners.Length + " Start position: " + m_Agent.transform.position + " Target position: " + target);

            LineRenderer lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.positionCount = path.corners.Length;

            for (int i = 0; i < path.corners.Length; i++)
            {
                // Vector3 cornerStart = path.corners[i - 1];
                Vector3 corner = path.corners[i];

                lineRenderer.SetPosition(i, corner);
                //lineRenderer.SetPosition(i, cornerEnd);

                Debug.Log("Corner start: " + corner);
                //Gizmos.DrawLine(cornerStart, cornerEnd);
                //Debug.Log("Corner start: " + cornerStart + " corner end: " + cornerEnd);
            }
        }
    }
}
