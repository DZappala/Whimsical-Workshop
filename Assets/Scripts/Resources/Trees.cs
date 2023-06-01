using UnityEngine;

namespace Resources
{
    [CreateAssetMenu(fileName = "Trees", menuName = "ScriptableObjects/Resources/Trees")]
    public class Trees : ScriptableObject, IResource
    {
        public Trees()
        {
            ResourceType = ResourceType.Wood;
            Amount = 100;
            Remaining = Amount;
        }

        public GameObject AssetReference { get; set; }
        public ResourceType ResourceType { get; set; }
        public int Amount { get; set; }
        public int Remaining { get; set; }
    }
}