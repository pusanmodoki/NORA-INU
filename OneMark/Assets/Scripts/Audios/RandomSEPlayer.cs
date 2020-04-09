//作成者 : 植村将太
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ランダムに登録SEを再生するRandomSEPlayer class
/// </summary>
public class RandomSEPlayer : SEPlayer
{
    List<int> m_indexs = new List<int>();
	
    /// <summary>
    /// [PlaySE]
    /// SEを再生(loop不可)
    /// </summary>
    public void PlayRandomSE()
    {
		//play index
		int index = 0;

		//全て再生済みでClear
		if (m_indexs.Count >= base.numSources)
			m_indexs.Clear();

		//Randomでindex取得, 未再生index->break
		while (true)
		{
			index = Random.Range(0, numSources);
			if (!m_indexs.Contains(index))
				break;
		}
		//PlaySE成功でindexリストに追加
        if (PlaySE(index, false))
			m_indexs.Add(index);
    }
}
