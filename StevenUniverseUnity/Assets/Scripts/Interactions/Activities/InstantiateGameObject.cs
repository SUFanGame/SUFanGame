using UnityEngine;

namespace StevenUniverse.FanGame.Interactions.Activities
{
    [System.Serializable]
    public class InstantiateGameObject : Activity
    {
        [SerializeField] private string prefabResourcePath;
        [SerializeField] private Vector3 position;

        private GameObject instantiatedGameObject;

        public InstantiateGameObject(int id, bool allowsControl, string prefabResourcePath, Vector3 position)
            : base(id, allowsControl)
        {
            this.prefabResourcePath = prefabResourcePath;
            this.position = position;
        }

        public override void StartActivity()
        {
            base.StartActivity();

            instantiatedGameObject =
                (GameObject)
                GameObject.Instantiate(Resources.Load<GameObject>(prefabResourcePath), position, Quaternion.identity);
            IsComplete = true;
        }

        public GameObject InstantiatedGameObject
        {
            get { return instantiatedGameObject; }
        }
    }
}