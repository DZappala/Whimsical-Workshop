using UnityEngine;

namespace Resources
{
    [CreateAssetMenu(fileName = "Stones", menuName = "ScriptableObjects/Resources/Stones")]
    public class Stones : ScriptableObject, IResource
    {
        public Stones()
        {
            ResourceType = ResourceType.Stone;
            Amount = 100;
            Remaining = Amount;
        }

        public GameObject AssetReference { get; set; }
        public ResourceType ResourceType { get; set; }
        public int Amount { get; set; }
        public int Remaining { get; set; }
    }
}