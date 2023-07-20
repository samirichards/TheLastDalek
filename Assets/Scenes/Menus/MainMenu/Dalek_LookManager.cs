using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Dalek_LookManager : MonoBehaviour
{
    [SerializeField] GameObject DalekProp;
    private LookAtAnimator dalekLookAnimator;
    [SerializeField] private Camera thisCamera;

    private RaycastHit hit;
    private Ray ray;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        ray = thisCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit))
        {
            Transform objectHit = hit.transform;

            DalekProp.GetComponent<LookAtAnimator>().SetLookTarget(objectHit);
        }
    }
}
