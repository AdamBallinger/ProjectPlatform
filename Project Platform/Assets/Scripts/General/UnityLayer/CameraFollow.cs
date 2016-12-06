using UnityEngine;

namespace Assets.Scripts.General.UnityLayer
{
    public class CameraFollow : MonoBehaviour
    {

        public float followSmoothing = 1f;

        private GameObject followTarget;

        public void Update()
        {
            var pos = transform.position;

            if (pos.x < 0.0f) pos.x = 0.0f;
            if (pos.x > World.Current.Width) pos.x = World.Current.Width;
            if (pos.y < 0.0f) pos.y = 0.0f;
            if (pos.y > World.Current.Height) pos.y = World.Current.Height;

            transform.position = pos;

            if(followTarget == null)
            {
                followTarget = GameObject.FindGameObjectWithTag("Player");
                return;
            }

            var interpPos = Vector3.MoveTowards(transform.position, followTarget.transform.position, followSmoothing * Time.deltaTime);
            interpPos.z = -10;
            transform.position = interpPos;
        }
    }
}
