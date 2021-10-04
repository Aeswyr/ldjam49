using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unstable
{
    public class AudioDb : MonoBehaviour
    {
        private Dictionary<string, AudioData> m_audioMap;

        [SerializeField]
        private AudioData[] m_audioData;

        public static AudioDb instance;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(this.transform.parent.gameObject);
            }
            else if (instance != this)
            {
                Destroy(this.gameObject);
            }
        }

        public static AudioData GetAudioData(string id)
        {
            // initialize the map if it does not exist
            if (instance.m_audioMap == null)
            {
                instance.m_audioMap = new Dictionary<string, AudioData>();
                foreach (AudioData data in instance.m_audioData)
                {
                    instance.m_audioMap.Add(data.ID, data);
                }
            }
            if (instance.m_audioMap.ContainsKey(id))
            {
                return instance.m_audioMap[id];
            }
            else
            {
                throw new KeyNotFoundException(string.Format("No Audio " +
                    "with id `{0}' is in the database", id
                ));
            }
        }

    }
}
