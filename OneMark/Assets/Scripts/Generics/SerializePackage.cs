using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializePackage<T>
{
	public List<T> list = new List<T>();

	public SerializePackage()
	{
		list = new List<T>();
	}
	public SerializePackage(List<T> list)
	{
		this.list = list;
	}

	public T this[int index]
	{
		set { list[index] = value; }
		get { return list[index]; }
	}
}

[System.Serializable]
public class SerializePackageString : SerializePackage<string>
{
}
