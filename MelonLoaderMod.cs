using MelonLoader;
using UnityEngine;

[assembly: MelonGame(null, null)]

namespace NoGravity
{
    public static class BuildInfo
    {
        public const string Name = "NoGravity";
        public const string Description = "Makes all Physics objects floaty";
        public const string Author = "Exil_S";
        public const string Company = null;
        public const string Version = "1.0.0";
        public const string DownloadLink = null;
    }

    public class NoGravity : MelonMod
    {
        private class AntiGravityObject
        {
            public GameObject gameObject;
            public Rigidbody rigidbody;
        }

        private AntiGravityObject[] antiGravityObjects;
        private bool isNoGravityEnabled = false;
        private float maxDistance = 10f;
        private float antiGravityForce = 9.8f;

        public override void OnApplicationStart()
        {
            antiGravityObjects = FindAntiGravityObjects();
        }

        public override void OnUpdate()
        {
            if (Input.GetKeyDown(KeyCode.G))
            {
                ToggleNoGravity();
            }
        }

        private void EnableNoGravity()
        {
            isNoGravityEnabled = true;
        }

        private void DisableNoGravity()
        {
            isNoGravityEnabled = false;
        }

        private void ToggleNoGravity()
        {
            if (isNoGravityEnabled)
            {
                DisableNoGravity();
            }
            else
            {
                EnableNoGravity();
            }
        }

        public override void OnFixedUpdate()
        {
            if (isNoGravityEnabled)
            {
                foreach (AntiGravityObject agObject in antiGravityObjects)
                {
                    Collider[] colliders = Physics.OverlapSphere(agObject.gameObject.transform.position, maxDistance);
                    foreach (Collider collider in colliders)
                    {
                        if (collider.gameObject != agObject.gameObject)
                        {
                            Rigidbody otherRb = collider.GetComponent<Rigidbody>();
                            if (otherRb != null)
                            {
                                Vector3 forceDirection = (agObject.gameObject.transform.position - collider.transform.position).normalized;
                                float distance = Vector3.Distance(agObject.gameObject.transform.position, collider.transform.position);
                                float strength = Mathf.InverseLerp(maxDistance, 0f, distance);
                                Vector3 antigravityForce = forceDirection * antiGravityForce * strength;
                                otherRb.AddForce(antigravityForce, ForceMode.Force);
                            }
                        }
                    }
                }
            }
        }

        private AntiGravityObject[] FindAntiGravityObjects()
        {
            Rigidbody[] rigidbodies = UnityEngine.Object.FindObjectsOfType<Rigidbody>();
            AntiGravityObject[] agObjects = new AntiGravityObject[rigidbodies.Length];
            for (int i = 0; i < rigidbodies.Length; i++)
            {
                agObjects[i] = new AntiGravityObject
                {
                    gameObject = rigidbodies[i].gameObject,
                    rigidbody = rigidbodies[i]
                };
            }
            return agObjects;
        }
    }
}
