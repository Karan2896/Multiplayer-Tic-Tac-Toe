using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing : MonoBehaviour
{

    float speed = 3f;
    Vector3 targetPosition;

    // Start is called before the first frame update
    void Start()
    {
        targetPosition = transform.position;
    }
    // Update is called once per frame
    void Update()
    {
        MoveBox();
    }

    void MoveBox()
    {

        // Vector3 mousex = Camera.main.ScreenToWorldPoint(Input.GetMouseButtonDown(0));
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.point);
                targetPosition = hit.point;
                transform.position = targetPosition;
            }
        }




    }
}
