
namespace Assets.Scripts.Physics.Colliders
{
    public class ABCircleCollider : ABCollider
    {
        public float Radius { get; set; }


        public ABCircleCollider(ABRigidBody _body) : base(_body)
        {

        }
    }
}
