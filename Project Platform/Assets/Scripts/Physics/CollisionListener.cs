using System;
using System.Collections.Generic;
using Assets.Scripts.Physics.Colliders;

namespace Assets.Scripts.Physics
{
    public class CollisionListener
    {

        /// <summary>
        /// Callback function executed when a collider that is a trigger, enters the collider for this listener.
        /// This callback is called once and not again until a new trigger enters, or the current has left and rentered.
        /// </summary>
        public Action<ABCollider> OnTriggerEnter;

        /// <summary>
        /// Callback function executed when a collider that is a trigger, stays (colliding for more than 1 physics step) inside the collider for this listener.
        /// </summary>
        public Action<ABCollider> OnTriggerStay;

        /// <summary>
        /// Callback function executed when a collider that is a trigger, leaves the collider for this listener.
        /// </summary>
        public Action<ABCollider> OnTriggerLeave;

        /// <summary>
        /// Callback function executed when a collider that is not a trigger collides with this listener's collider.
        /// </summary>
        public Action<ABCollider> OnCollision;

        public ABCollider Collider { get; private set; }

        /// <summary>
        /// Store a list of colliders that this listeners collider had collided with in the last physics step.
        /// </summary>
        public List<ABCollider> LastStepCollisions { get; private set; }


        public CollisionListener(ABCollider _collider)
        {
            Collider = _collider;
            LastStepCollisions = new List<ABCollider>();
        }

        /// <summary>
        /// Handle the collision for this listener against another collider.
        /// </summary>
        /// <param name="_colliding"></param>
        public void Handle(ABCollider _colliding)
        {
            // Don't allow colliders of the same object root to trigger each other.
            if (Collider.RigidBody.GameObject.transform.root == _colliding.RigidBody.GameObject.transform.root)
                return;

            if(_colliding.IsTrigger)
            {
                HandleTrigger(_colliding);
            }
            else
            {
                HandleCollider(_colliding);
            }
        }

        private void HandleTrigger(ABCollider _trigger)
        {
            // if the colliding object didn't collide with this collider last physics step
            if(!LastStepCollisions.Contains(_trigger))
            {
                // then execute the trigger enter callback
                if(OnTriggerEnter != null)
                    OnTriggerEnter(_trigger);

                LastStepCollisions.Add(_trigger);
            }
            else
            {
                if(OnTriggerStay != null)
                    OnTriggerStay(_trigger);
            }
        }

        private void HandleCollider(ABCollider _collider)
        {
            // Call OnCollision callback only if the collider hasnt previously been 
            // colliding with this object before leaving / seperating from it.
            if(!LastStepCollisions.Contains(_collider))
            {
                if(OnCollision != null)
                {
                    OnCollision(_collider);
                }

                LastStepCollisions.Add(_collider);
            }
        }

        /// <summary>
        /// Check if a collider is exiting this listeners collider.
        /// </summary>
        /// <param name="_collider"></param>
        public void HandleExit(ABCollider _collider)
        {
            if (Collider.RigidBody.GameObject.transform.root == _collider.RigidBody.GameObject.transform.root)
                return;

            // If the collider wasn't previously colliding with this listeners collider, then break out.
            if (!LastStepCollisions.Contains(_collider))
            {
                return;
            }

            // Otherwise handle exit callback based on collider type.
            if(_collider.IsTrigger)
            {
                if (OnTriggerLeave != null)
                {
                    OnTriggerLeave(_collider);
                }
            }

            // Then remove the collider from the last colliding colliders list.
            LastStepCollisions.Remove(_collider);
        }

        /// <summary>
        /// Register a function callback for when a trigger enters this collider.
        /// </summary>
        /// <param name="_callback"></param>
        public void RegisterTriggerEnterCallback(Action<ABCollider> _callback)
        {
            OnTriggerEnter += _callback;
        }

        /// <summary>
        /// Register a function callback for when a trigger has been colliding with this collider for
        /// more than 1 physics step.
        /// </summary>
        /// <param name="_callback"></param>
        public void RegisterTriggerStayCallback(Action<ABCollider> _callback)
        {
            OnTriggerStay += _callback;
        }

        /// <summary>
        /// Register a function callback for when a trigger leaves a previous collision with this collider.
        /// </summary>
        /// <param name="_callback"></param>
        public void RegisterTriggerLeaveCallback(Action<ABCollider> _callback)
        {
            OnTriggerLeave += _callback;
        }

        /// <summary>
        /// Register a function callback for when a none trigger collider collides with this collider.
        /// </summary>
        /// <param name="_callback"></param>
        public void RegisterOnCollisionCallback(Action<ABCollider> _callback)
        {
            OnCollision += _callback;
        }
    }
}
