using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public List<Building> buildings;
    public Building currentlySelectedBuilding;

    public GameObject spawnedBuildingInstance;

    private void Start()
    {
        foreach (Building building in buildings) building.OnConstructionStarted += HandleConstructionStarted;
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
        currentlySelectedBuilding = building;
        spawnedBuildingInstance = Instantiate(building.assetReference);
    }
}