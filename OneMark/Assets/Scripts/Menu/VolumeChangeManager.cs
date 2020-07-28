using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeChangeManager : MonoBehaviour
{
	public static readonly float[] cVolumes = new float[5]{-10.0f, -5.0f, 0.0f, 5.0f, 10.0f};

	public VolumeChangeButton[] buttons { get { return m_buttons; } }

	[SerializeField]
	bool m_isBgm = true;
	[SerializeField]
	MenuInput m_input = null;
	[SerializeField]
	VolumeChangeButton[] m_buttons = null;

	public void SelectVolumeIndex(int index)
	{
		if (m_isBgm)
			DataManager.instance.saveData.SetBgmVolumeIndex(index);
		else
			DataManager.instance.saveData.SetSeVolumeIndex(index);

		DataManager.instance.WriteSaveData();

		ResetVolume();
	}

	public void Start()
	{
		for (int i = 0; i < m_buttons.Length; ++i)
			buttons[i].SetThisIndex(this, i);

		m_input.ForceSelect(m_isBgm ? DataManager.instance.saveData.bgmVolumeIndex : DataManager.instance.saveData.seVolumeIndex);

		ResetVolume();
	}

	void ResetVolume()
	{
		int volumeIndex = m_isBgm ? DataManager.instance.saveData.bgmVolumeIndex : DataManager.instance.saveData.seVolumeIndex;

		for (int i = 0; i < m_buttons.Length; ++i)
		{
			if (i <= volumeIndex) m_buttons[i].SetSelectColor(true);
			else m_buttons[i].SetSelectColor(false);
		}

		if (m_isBgm)
			AudioManager.instance.bgmVolume = cVolumes[volumeIndex];
		else
			AudioManager.instance.seVolume = cVolumes[volumeIndex];
	}
}
