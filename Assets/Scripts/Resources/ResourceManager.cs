using System.Collections.Generic;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Resources
{
    public class ResourceManager : MonoBehaviour
    {
        private int randomResourceCount;
        private List<IResource> resources;

        private void Awake()
        {
            resources = new List<IResource>();
            randomResourceCount = new Random().NextInt(1, 10);
        }

        private void Start()
        {
            GenerateResourceDeposits();
        }

        private void GenerateResourceDeposits()
        {
            for (int i = 0; i < randomResourceCount; i++)
            {
                Stones stones = new();
                resources.Add(stones);
            }

            foreach (IResource resource in resources)
            {
                float randomX = new Random().NextFloat(-100, 100);
                float randomZ = new Random().NextFloat(-100, 100);
                Vector3 randomLocation = new(randomX, 0, randomZ);
                Instantiate(resource.AssetReference).transform.position = randomLocation;
            }
        }
    }

    public enum ResourceType
    {
        Wood
        , Stone
    }


    public interface IResource
    {
        public GameObject AssetReference { get; set; }
        ResourceType ResourceType { get; set; }
        int Amount { get; set; }
        int Remaining { get; set; }
    }

/*
    ResourceManager:
    Global Resource List
    - Wood
    - Stone
*/
}