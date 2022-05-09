using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseUtilitiesAssistancesFactory : MonoBehaviour
{
    private static MouseUtilitiesAssistancesFactory m_instance;
    public static MouseUtilitiesAssistancesFactory Instance { get { return m_instance; } }

    public MouseAssistanceDialog m_refDialogAssistance;
    public MouseAssistanceBasic m_refCube;

    private void Awake()
    {
        if (m_instance != null && m_instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            m_instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public MouseAssistanceDialog createDialogNoButton(string title, string description, Transform parent)
    {
        Transform dialogView = Instantiate(m_refDialogAssistance.transform, parent);
        MouseAssistanceDialog dialogController = dialogView.GetComponent<MouseAssistanceDialog>();
        dialogController.setTitle(title);
        float sizeDescriptionText = -0.002f * description.Length + 0.38f;
        dialogController.setDescription(description, sizeDescriptionText);
        dialogController.enableBillboard(true);

        return dialogController;
    }

    public MouseAssistanceBasic createCube(string texture, Transform parent)
    {
        Transform cubeView = Instantiate(m_refCube.transform, parent);
        MouseAssistanceBasic cubeController = cubeView.GetComponent<MouseAssistanceBasic>();
        cubeController.setMaterialToChild(texture);

        return cubeController;
    }
}
