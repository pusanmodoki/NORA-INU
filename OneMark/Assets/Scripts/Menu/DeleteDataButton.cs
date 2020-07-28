using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteDataButton : BaseSelectedObject
{
	[SerializeField]
	Color m_selectedColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
	[SerializeField]
	Color m_nonSelectedColor = new Color(0.7f, 0.7f, 0.7f, 1.0f);
	[SerializeField]
	UnityEngine.UI.Image m_nonSelectImage = null;
	[SerializeField]
	UnityEngine.UI.Image m_selectImage = null;
	[SerializeField]
	AudioSource m_pushSource = null;
	[SerializeField]
	AudioSource m_enterSource = null;
	[SerializeField]
	VolumeChangeManager[] m_volumeChangeManagers = null;
	[SerializeField]
	float m_selectSeconds = 2.0f;
	[SerializeField]
	float m_nonSelectSeconds = 1.0f;

	Timer m_nonSelectTimer = new Timer();
	float m_bgmVolume = 0.0f;
	bool m_isOnCursor = false;

	public void CheckEndPushAudio()
	{
		if (m_pushSource.isPlaying)
		{
			m_pushSource.Stop();
			AudioManager.instance.bgmVolume = m_bgmVolume;
		}
	}

	public override void AwakeCursor()
	{
		m_nonSelectImage.color = m_selectedColor;
		m_isOnCursor = true;
	}

	public override void OffCursor()
	{
		m_nonSelectImage.color = m_nonSelectedColor;
		m_isOnCursor = false;
		m_selectImage.fillAmount = 0.0f;
		m_nonSelectTimer.Stop();

		if (m_pushSource.isPlaying)
		{
			m_pushSource.Stop();
			AudioManager.instance.bgmVolume = m_bgmVolume;
		}
	}

	public override void OnCursor()
	{
		m_nonSelectImage.color = m_selectedColor;
		m_isOnCursor = true;
	}

	public override void OnEnter()
	{
	}

	void Update()
	{
		if (!m_isOnCursor || m_nonSelectTimer.elapasedTime < m_nonSelectSeconds) return;

		if (Input.GetButton("Fire1"))
		{
			m_selectImage.fillAmount += Time.deltaTime / m_selectSeconds;

			if (!m_pushSource.isPlaying)
			{
				m_pushSource.Play();
				m_bgmVolume = AudioManager.instance.bgmVolume;
				AudioManager.instance.bgmVolume = VolumeChangeManager.cVolumes[0];
			}

			if (m_selectImage.fillAmount >= 1.0f)
			{
				m_selectImage.fillAmount = 0.0f;
				m_nonSelectTimer.Start();

				m_pushSource.Stop();
				AudioManager.instance.bgmVolume = m_bgmVolume;

				m_enterSource.PlayOneShot(m_enterSource.clip);

				DataManager.instance.ResetSaveData();
				DataManager.instance.WriteSaveData();

				foreach (var manager in m_volumeChangeManagers)
					manager.Start();
			}
		}
		else
		{
			m_selectImage.fillAmount = 0.0f;

			if (m_pushSource.isPlaying)
			{
				m_pushSource.Stop();
				AudioManager.instance.bgmVolume = m_bgmVolume;
			}
		}
	}
}
