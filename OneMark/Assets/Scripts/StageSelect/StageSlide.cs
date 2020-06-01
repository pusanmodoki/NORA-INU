using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageSlide : MonoBehaviour
{
	public Timer timer { get; private set; } = new Timer();
	public bool isSlide { get; private set; } = false;

    [SerializeField]
    private float interval = 40.0f;

    [SerializeField]
    private float scrollSeconds = 1.5f;

	public void StartSlide()
	{
		isSlide = true;
		timer.Start();
	}

    // Start is called before the first frame update
    void Start()
	{
		for (int i = 0; i < transform.childCount; ++i)
		{
			Vector3 localPos = transform.GetChild(i).transform.localPosition;
			localPos.x = i * interval;
			transform.GetChild(i).transform.localPosition = localPos;
		}
		timer.Start();
	}

    // Update is called once per frame
    void Update()
    {
		if (isSlide) ScrollUpdate();
    }

	void ScrollUpdate()
	{
		float pointx = (float)(-(StageSelectIndexer.index.x - 1)) * interval;
		float lerpx = Mathf.Lerp(transform.localPosition.x, pointx, timer.elapasedTime / scrollSeconds);

		Vector3 vec = transform.localPosition;
		vec.x = lerpx;
		transform.localPosition = vec;

		if (timer.elapasedTime >= scrollSeconds)
		{
			isSlide = false;
			timer.Start();
		}
	}
}
