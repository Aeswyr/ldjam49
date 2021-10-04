using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Unstable
{
	/// <summary>
	/// Manages Ambiance audio in a scene
	/// </summary>
	[RequireComponent(typeof(AudioSource))]
	public class AmbianceMgr : MonoBehaviour, IAudioPlayer
	{
		private AudioSource m_ambianceSrc;

		private AudioSrcMgr.AudioLoopPair m_stashedAudio;
		private AudioData m_currData;
		private Queue<AudioSrcMgr.AudioLoopPair> m_audioQueue;

		#region Unity Callbacks

		private void Awake()
		{
			m_ambianceSrc = this.GetComponent<AudioSource>();
		}

		#endregion

		#region IAudioPlayer

		/// <summary>
		/// For longer sounds
		/// </summary>
		/// <param name="clipID"></param>
		public void PlayAudio(string clipID, bool loop = false)
		{
			LoadAmbianceAudio(clipID);
			m_ambianceSrc.loop = loop;
			m_ambianceSrc.Play();
		}

		public bool IsPlayingAudio()
		{
			return m_ambianceSrc.isPlaying;
		}

		public void StopAudio()
		{
			m_ambianceSrc.Stop();
		}
		public void ResumeAudio()
		{
			m_ambianceSrc.Play();
		}

		// Saves the current audio for later
		public void StashAudio()
		{
			m_stashedAudio = new AudioSrcMgr.AudioLoopPair(m_currData, m_ambianceSrc.loop);
		}

		// Saves the current audio for later
		public void ResumeStashedAudio()
		{
			if (m_stashedAudio.Data == null) { return; }

			AudioSrcMgr.LoadAudio(m_ambianceSrc, m_stashedAudio.Data);
			m_ambianceSrc.loop = m_stashedAudio.Loop;
			m_ambianceSrc.Play();
		}

		#endregion

		#region Helper Methods

		private void LoadAmbianceAudio(string clipID)
		{
			var data = AudioDb.GetAudioData(clipID);
			AudioSrcMgr.LoadAudio(m_ambianceSrc, data);
			m_currData = data;
		}

		#endregion
	}
}
