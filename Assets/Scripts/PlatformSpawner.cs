using UnityEngine;

using System.Collections;
using System.Collections.Generic;

namespace TofuGirl
{
    /// <summary>
    /// Spawner for platforms.
    /// </summary>
    public class PlatformSpawner : MonoBehaviour
    {
        public static PlatformSpawner Instance { get; private set; }

        [SerializeField] private Camera m_MainCamera = null;

        [SerializeField] private Transform m_ParentTransform = null;
        [SerializeField] private PlatformController m_PlatformPrefab = null;

        // Might not be applicable if we're planning to show the entire platform stack on game over.
        [SerializeField] private bool m_UseObjectPooling = false;

        [Header("Platform Configs")]
        [SerializeField] private float m_PlatformSpeed = 1000.0f;

        [SerializeField] private float m_PlatformWidth = 400.0f;
        [SerializeField] private float m_PlatformHeight = 200.0f;

        private bool m_SpawnRight = false;
        private List<PlatformController> m_PlatformControllers = new List<PlatformController>();

        public void Awake()
        {
            Instance = this;
        }

        public void Start()
        {
            // Always spawn on right.
            m_SpawnRight = true;
            SpawnPlatform(m_SpawnRight);
        }

        /// <summary>
        /// Spawn platform with alternating position. (left ~ right)
        /// </summary>
        public void SpawnPlatform()
        {
            m_ParentTransform.transform.position += Vector3.down * m_PlatformHeight;

            m_SpawnRight = !m_SpawnRight;
            SpawnPlatform(m_SpawnRight);
        }

        /// <summary>
        /// Resets the platform spawner.
        /// </summary>
        public void ResetSpawner()
        {
            for (int i = 0; i < m_PlatformControllers.Count; i++)
            {
                Destroy(m_PlatformControllers[i].gameObject);
            }

            m_PlatformControllers.Clear();
            m_ParentTransform.localPosition = Vector3.zero;

            // Always spawn on right.
            m_SpawnRight = true;
            SpawnPlatform(m_SpawnRight);
        }

        /// <summary>
        /// Used to spawn platforms.
        /// </summary>
        /// <param name="spawnRight">Should spawn on the right camera view.</param>
        private void SpawnPlatform(bool spawnRight)
        {
            PlatformController platformController = GetPlatformController();

            if (spawnRight)
            {
                platformController.transform.position = new Vector3((Screen.width * 0.5f) + (m_PlatformWidth * 0.5f), transform.position.y, 0.0f);
                platformController.Initialize(m_PlatformSpeed, Vector3.left);
            }
            else
            {
                platformController.transform.position = new Vector3((Screen.width * -0.5f) - (m_PlatformWidth * 0.5f), transform.position.y, 0.0f);
                platformController.Initialize(m_PlatformSpeed, Vector3.right);
            }

            platformController.gameObject.SetActive(true);
        }

        /// <summary>
        /// Used in object pooling.
        /// Re-uses platforms that are out of camera view.
        /// </summary>
        /// <returns>Platform Controller Instance</returns>
        private PlatformController GetPlatformController()
        {
            PlatformController platformController = null;

            if (m_UseObjectPooling)
            {
                for (int i = 0; i < m_PlatformControllers.Count; i++)
                {
                    Vector3 max = m_PlatformControllers[i].PlatformBounds.max;
                    float screenYMin = Screen.height * -0.5f;

                    if(m_MainCamera != null)
                    {
                        screenYMin = m_MainCamera.ViewportToWorldPoint(m_MainCamera.rect.min).y;
                    }

                    if (max.y <= screenYMin)
                    {
                        platformController = m_PlatformControllers[i];
                        break;
                    }
                }
            }

            if (platformController == null)
            {
                platformController = Instantiate<PlatformController>(m_PlatformPrefab, m_ParentTransform);
                m_PlatformControllers.Add(platformController);
            }

            return platformController;
        }
    }
}
