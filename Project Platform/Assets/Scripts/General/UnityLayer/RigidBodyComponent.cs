using Assets.Scripts.Physics;
using UnityEngine;

namespace Assets.Scripts.General.UnityLayer
{
    /// <summary>
    /// Rigid body component to allow custom rigid body class to interact with Unity component system.
    /// </summary>
    public class RigidBodyComponent : MonoBehaviour
    {

        public ABRigidBody RigidBody { get; private set; }

        public void Init(ABRigidBody _rigidBody)
        {
            RigidBody = _rigidBody;
        }
    }
}
