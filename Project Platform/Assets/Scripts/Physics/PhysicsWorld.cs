using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Physics
{
    public class PhysicsWorld
    {

        /// <summary>
        /// World gravity per axis. (m/s)
        /// </summary>
        public Vector2 Gravity { get; set; }

        /// <summary>
        /// Size of a single unit in pixels. Normally 100
        /// </summary>
        public int UnitSize { get; set; }

        /// <summary>
        /// A list of rigid bodies for the current physics world.
        /// </summary>
        public List<ABRigidBody> RigidBodies { get; private set; }

        /// <summary>
        /// Initialize the physics world with physics settings.
        /// </summary>
        /// <param name="_gravity">Gravity value in meters per second.</param>
        public void Initialize(Vector2 _gravity)
        {
            RigidBodies = new List<ABRigidBody>();
            Gravity = _gravity;
        }

        public void Step()
        {
            // Do physics stuff here
            // delta time = Time.fixedDeltaTime

            for(var i = 0; i < RigidBodies.Count; i++)
            {
                var body1 = RigidBodies[i];
                for(var j = i + 1; j < RigidBodies.Count; j++)
                {
                    var body2 = RigidBodies[j];
                    // Check collisions between body1 and body2.
                }
            }
        }
    }
}
