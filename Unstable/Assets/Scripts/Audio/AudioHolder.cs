using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unstable
{
    public class AudioHolder : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_audioHolderPrefab;

        // Start is called before the first frame update
        void Start()
        {
            if (GameObject.Find("AudioSrcMgr") == null)
            {
                Instantiate(m_audioHolderPrefab);
            }
            Destroy(this.gameObject);
        }
    }
}