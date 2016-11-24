using UnityEngine;

namespace Assets.Scripts.General.UnityLayer
{
    public class CameraFollow : MonoBehaviour
    {

        public float followSmoothing = 1f;

        private GameObject followTarget;

        public void Update()
        {
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
