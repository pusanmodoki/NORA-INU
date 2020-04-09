//制作者: 植村将太
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// イテレータ検索等を行うIteratorExtension
/// </summary>
public static class IteratorExtension
{
	/// <summary>
	/// [Compare]<delegate>
	/// 比較式, 使用する式は各関数で異なる
	/// return: result
	/// 引数1: left
	/// 引数2: right
	/// </summary>
	public delegate bool Compare<T>(T left, T right);

	/// <summary>
	/// [GetBeginEnumerator]
	/// 最初の要素となるイテレータを取得する
	/// throw: Count == 0
	/// return: Begin iterator
	/// 引数1: <this>
	/// </summary>
	public static List<T>.Enumerator GetBeginEnumerator<T>(this List<T> self)
	{
		if (self.Count == 0) throw new System.ArgumentNullException();

		var result = self.GetEnumerator();
		result.MoveNext();
		return result;
	}

	/// <summary>
	/// [FindMin]
	/// 最小要素を検索する
	/// throw: Count == 0
	/// return: Min element iterator
	/// 引数1: <this>
	/// 引数2: 比較式, フォーマット: left ＜ right
	/// </summary>
	public static List<T>.Enumerator FindMin<T>(this List<T> self, Compare<T> compare)
	{
		List<T>.Enumerator iterator;

		try { iterator = self.GetBeginEnumerator(); }
		catch(System.Exception) { throw; }

		if (self.Count <= 1) return iterator;
		
		var result = iterator;
		while(iterator.MoveNext())
		{
			if (compare(iterator.Current, result.Current))
				result = iterator;
		}

		return result;
	}
	/// <summary>
	/// [FindMinIndex]
	/// 最小要素を検索する
	/// return: Min element index, Count == 0 -> -1
	/// 引数1: <this>
	/// 引数2: 比較式, フォーマット: left ＜ right
	/// </summary>
	public static int FindMinIndex<T>(this List<T> self, Compare<T> compare)
	{
		int result = 0, i = 0, count = self.Count;
		if (count < 1) return count;

		for (; i < count; ++i)
		{
			if (compare(self[i], self[result]))
				result = i;
		}

		return result;
	}

	/// <summary>
	/// [FindMax]
	/// 最小要素を検索する
	/// throw: Count == 0
	/// return: Min element iterator
	/// 引数1: <this>
	/// 引数2: 比較式, フォーマット: left ＜ right
	/// </summary>
	public static List<T>.Enumerator FindMax<T>(this List<T> self, Compare<T> compare)
	{
		List<T>.Enumerator iterator;

		try { iterator = self.GetBeginEnumerator(); }
		catch (System.Exception) { throw; }

		if (self.Count <= 1) return iterator;

		var result = iterator;
		while (iterator.MoveNext())
		{
			if (compare(result.Current, iterator.Current))
				result = iterator;
		}

		return result;
	}
	/// <summary>
	/// [FindMaxIndex]
	/// 最小要素を検索する
	/// return: Min element index, Count == 0 -> -1
	/// 引数1: <this>
	/// 引数2: 比較式, フォーマット: left ＜ right
	/// </summary>
	public static int FindMaxIndex<T>(this List<T> self, Compare<T> compare)
	{
		int result = 0, i = 0, count = self.Count;
		if (count < 1) return count;

		for (; i < count; ++i)
		{
			if (compare(self[result], self[i]))
				result = i;
		}

		return result;
	}

	/// <summary>
	/// [SwapIndex]
	/// this[left]とthis[right]をswapする
	/// 引数1: <this>
	/// 引数2: left
	/// 引数3: right
	/// </summary>
	public static void SwapIndex<T>(this List<T> self, int left, int right)
	{
		T temp = self[left];
		self[left] = self[right];
		self[right] = temp;
	}
}
