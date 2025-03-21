using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TourPlacement : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    private GameObject CurrentPlacingTower;

    void Start()
    {
    }

    void Update()
    {
        if (CurrentPlacingTower )
        {
            Ray camray = playerCamera.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(camray, out RaycastHit hit, 100f))
            {
                CurrentPlacingTower.transform.position = hit.point;
            }


            if (Input.GetMouseButtonDown(0))
            {
                CurrentPlacingTower = null;
            }
        }
    }

    public void SetTowerToPlacement(GameObject tower)
    {
        // Instantiate new tower
        CurrentPlacingTower = Instantiate(tower, new Vector3(0, 0.15f, 0), Quaternion.identity);
        // Adjust the scale 
        CurrentPlacingTower.transform.localScale = new Vector3(0.9f, 0.9f, 0.9f);
    }
}