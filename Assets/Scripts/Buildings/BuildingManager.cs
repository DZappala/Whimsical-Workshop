using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Buildings
{
    public class BuildingManager : MonoBehaviour
    {
        public List<Building> buildings;
        public Building currentlySelectedBuilding;
        public GameObject spawnedBuildingInstance;
        public LayerMask groundLayer;
        private Camera cameraMain;

        private void Start()
        {
            foreach (Building building in buildings) building.OnConstructionStarted += HandleConstructionStarted;
            cameraMain = Camera.main;
        }

        private void OnDestroy()
        {
            foreach (Building building in buildings) building.OnConstructionStarted -= HandleConstructionStarted;
        }

        public void HandleConstruct()
        {
            spawnedBuildingInstance = null;
            currentlySelectedBuilding = null;
        }

        private void HandleConstructionStarted(Building building)
        {
            if (!cameraMain) return;
            currentlySelectedBuilding = building;
            spawnedBuildingInstance = Instantiate(building.assetReference);
            Ray ray = cameraMain.ScreenPointToRay(Mouse.current.position.ReadValue());

            // If the mouse is not over the ground, return
            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {
                //destroy the instance of spawnedBuildingInstance
                Destroy(spawnedBuildingInstance);
                return;
            }

            spawnedBuildingInstance.transform.position = hit.point;
        }
    }
}