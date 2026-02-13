using UnityEngine;

namespace GeoPlaySample.InfiniteRunner
{
    public class PlayerAnimationEvents : MonoBehaviour
    {
        [SerializeField] private ParticleSystem leftFootImpactParticles;
        [SerializeField] private ParticleSystem rightFootImpactParticles;

        private SpriteRenderer mountLeftFoot;
        private SpriteRenderer mountRightFoot;
        private SpriteRenderer mountDuck;

        private void Start()
        {
            if (mountLeftFoot == null || mountRightFoot == null || mountDuck == null) return;

            mountLeftFoot.enabled = false;
            mountRightFoot.enabled = false;
            mountDuck.enabled = false;
        }

        public void OnMoveLeftFoot()
        {
            if (leftFootImpactParticles == null) return;

            leftFootImpactParticles.gameObject.SetActive(true);
            leftFootImpactParticles.Stop();
            leftFootImpactParticles.Play();

            if (mountLeftFoot == null || mountRightFoot == null || mountDuck == null) return;

            mountLeftFoot.enabled = true;
            mountRightFoot.enabled = false;
            mountDuck.enabled = false;
        }

        public void OnMoveRightFoot()
        {
            if (rightFootImpactParticles == null) return;

            leftFootImpactParticles.gameObject.SetActive(true);
            rightFootImpactParticles.Stop();
            rightFootImpactParticles.Play();

            if (mountLeftFoot == null || mountRightFoot == null || mountDuck == null) return;

            mountLeftFoot.enabled = false;
            mountRightFoot.enabled = true;
            mountDuck.enabled = false;
        }

        public void OnJump()
        {
            if (leftFootImpactParticles != null)
                leftFootImpactParticles.gameObject.SetActive(false);
            if (rightFootImpactParticles != null)
                rightFootImpactParticles.gameObject.SetActive(false);

            if (mountLeftFoot == null || mountRightFoot == null || mountDuck == null) return;

            mountLeftFoot.enabled = true;
            mountRightFoot.enabled = false;
            mountDuck.enabled = false;
        }

        public void OnDuck()
        {
            if (mountLeftFoot == null || mountRightFoot == null || mountDuck == null) return;

            mountLeftFoot.enabled = false;
            mountRightFoot.enabled = false;
            mountDuck.enabled = true;
        }

        public void SetMounts(
            SpriteRenderer mountMove0,
            SpriteRenderer mountMove1,
            SpriteRenderer mountDuck
            )
        {
            mountLeftFoot = mountMove0;
            mountRightFoot = mountMove1;
            this.mountDuck = mountDuck;
        }

        public void SetMountMove0(SpriteRenderer mount)
        {
            mountLeftFoot = mount;
        }

        public void SetMountMove1(SpriteRenderer mount)
        {
            mountRightFoot = mount;
        }

        public void SetMountDuck(SpriteRenderer mount)
        {
            this.mountDuck = mount;
        }
    }
}
