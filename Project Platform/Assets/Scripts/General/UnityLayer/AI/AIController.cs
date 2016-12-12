using Assets.Scripts.General.UnityLayer.Physics_Components;
using Assets.Scripts.Physics.Colliders;
using UnityEngine;

namespace Assets.Scripts.General.UnityLayer.AI
{
    public class AIController : MonoBehaviour
    {

        public CircleColliderComponent aiCollider;

        private LevelManager levelManager;

        private PathfinderAgent pathAgent;

        public void Start()
        {
            aiCollider.Collider.CollisionListener.RegisterOnCollisionCallback(OnCollision);

            levelManager = FindObjectOfType<LevelManager>();
            pathAgent = GetComponent<PathfinderAgent>();
        }

        public void Update()
        {
            var playerObject = GameObject.FindGameObjectWithTag("Player");

            if(pathAgent != null && playerObject != null)
            {
                if(pathAgent.currentPath == null)
                {
                    pathAgent.StartPathing(transform.position, playerObject.transform);
                }
            }
        }

        public void OnCollision(ABCollider _collider)
        {
            if(_collider.RigidBody.GameObject.tag == "Player")
            {
                if(levelManager != null)
                {
                    levelManager.OnPlayerCaught();
                }
            }
        }
    }
}
