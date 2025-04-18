using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TourPlacement : MonoBehaviour
{
    [SerializeField] private LayerMask placementCheckMask;
    [SerializeField] private LayerMask placementCollideMask;
    [SerializeField] private Camera playerCamera;
    private GameObject CurrentPlacingTower;


    void Update()
    {
        if (CurrentPlacingTower)
        {
            Ray camray = playerCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            bool didHit = Physics.Raycast(camray, out hit, 100f, placementCollideMask);

            if (didHit)
            {
                CurrentPlacingTower.transform.position = hit.point;
                if (Input.GetKeyDown(KeyCode.C)) // to cancel tower placement
                {
                    Destroy(CurrentPlacingTower);
                    CurrentPlacingTower = null;
                    return;
                }
                if (Input.GetMouseButtonDown(0))
                {
                    // Get the position where the ray hit
                    Vector3 hitPosition = hit.point;
                    // Perform a sphere or overlap cast to find all colliders at this point
                    Collider[] hitColliders = Physics.OverlapSphere(hitPosition, 0.1f);
                    // Look for a collider with the VirginCell tag
                    foreach (Collider col in hitColliders)
                    {
                        if (col.gameObject.CompareTag("VirginCell"))
                        {
                            GameManager.towersInGame.Add(CurrentPlacingTower.GetComponent<TowerBehavior>());
                            CurrentPlacingTower = null;
                            break;
                        }
                    }
                }
            }
        }
    }

    public void SetTowerToPlacement(GameObject tower)
    {
        // Instantiate new tower
        CurrentPlacingTower = Instantiate(tower, new Vector3(0, 1f, 0), Quaternion.identity);
        // Adjust the scale 
        CurrentPlacingTower.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
    }
}