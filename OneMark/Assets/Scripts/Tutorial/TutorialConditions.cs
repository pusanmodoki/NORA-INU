using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialConditions : MonoBehaviour
{
	public enum Conditions
	{
		Start,
		StageT1,
		StageT2,
		LinkMarkPoint,
		UnlinkMarkPoint,
		GameClear,
		GameOver,
		LinkCheckPoint,
		UnlinkCheckPoint,
		StageT3,
		StageT4,
		StageTutorial,
		MarkPointChikaChika,
		ServantEmpty,
		Max
	}

	public bool isGameStart { get; set; } 

	FollowObject m_camera = null;
	bool isLinkMarkPoint = false, isUnlinkMarkPoint = false;

	bool isGameClear = false, isGameOver = false;

	bool isLinkCheckPoint = false, isUnlinkCheckPoint = false;

	public bool IsCondition(Conditions condition)
	{
		switch (condition)
		{
			case Conditions.Start:
				return isGameStart;
			case Conditions.StageT1:
				return OneMarkSceneManager.instance.nowStageSceneIndex == new Vector2Int(0, 1);
			case Conditions.StageT2:
				return OneMarkSceneManager.instance.nowStageSceneIndex == new Vector2Int(0, 2);
			case Conditions.LinkMarkPoint:
				if (isLinkMarkPoint) { isLinkMarkPoint = false; return true; }
				else return false;
			case Conditions.UnlinkMarkPoint:
				if (isUnlinkMarkPoint) { isUnlinkMarkPoint = false; return true; }
				else return false;
			case Conditions.GameClear:
				if (MainGameManager.instance == null) return false;
				if (!isGameClear && MainGameManager.instance.resultState == MainGameManager.ResultState.GameClear)
					return isGameClear = true;
				else return false;
			case Conditions.GameOver:
				if (!isGameOver && MainGameManager.instance.resultState == MainGameManager.ResultState.GameEnd)
					return isGameOver = true;
				else return false;
			case Conditions.LinkCheckPoint:
				if (isLinkCheckPoint) { isLinkCheckPoint = false; return true; }
				else return false;
			case Conditions.UnlinkCheckPoint:
				if (isUnlinkCheckPoint) { isUnlinkCheckPoint = false; return true; }
				else return false;
			case Conditions.StageT3:
				return OneMarkSceneManager.instance.nowStageSceneIndex == new Vector2Int(0, 3);
			case Conditions.StageT4:
				return OneMarkSceneManager.instance.nowStageSceneIndex == new Vector2Int(0, 4);
			case Conditions.StageTutorial:
				return OneMarkSceneManager.instance.nowStageSceneIndex.x == 0;
			case Conditions.MarkPointChikaChika:
				if (PlayerAndTerritoryManager.instance == null) return false;
				foreach(var point in PlayerAndTerritoryManager.instance.mainPlayer.allTerritorys)
					if (point.isLinked && point.effectiveCounter01 <= 0.5f) return true;
				return false;
			case Conditions.ServantEmpty:
				foreach (var servant in ServantManager.instance.servantByMainPlayer)
					if (servant.isAccompanyingPlayer) return false;
				return true;
			default: return false;
		}
	}

	public void StartCondition()
	{
		m_camera = Camera.main.GetComponent<FollowObject>();
		PlayerAndTerritoryManager.instance.mainPlayer.changeTerritoryCallback += ChangeTerritory;
		CheckPointManager.instance.changeStateCallback += ChangeCheckPoint;

		isGameClear = isGameOver = isGameStart = false;
	}

	void ChangeTerritory(bool isAdd)
	{
		if (isAdd) isLinkMarkPoint = true;
		else isUnlinkMarkPoint = true;
	}
	void ChangeCheckPoint(int playerID, int pointID, bool isLink)
	{
		if (isLink) isLinkCheckPoint = true;
		else isUnlinkCheckPoint = true;
	}
}
