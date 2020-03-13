using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour
{

	[SerializeField]
	private PoolObject poolObject;

	private const int poolSize = 40;

	private readonly Queue<PoolObject> _pool = new Queue<PoolObject>(poolSize);

	private static readonly Quaternion NoRotation = Quaternion.Euler(0, 0, 0);
	
	public T Place<T>(Vector2 position) where T : PoolObject{
		return (T)Place(position);
	}

	public PoolObject Place(Vector2 position){
		PoolObject obj;
		if(_pool.Count > 0){
			obj = _pool.Dequeue();
			obj.gameObject.SetActive(true);
			obj.transform.SetPositionAndRotation(position, NoRotation);
			obj.Init();
		}
		else{
			obj = Instantiate(poolObject, position, NoRotation);
			obj.pool = this;
			obj.Init();
		}

		return obj;
	}


	public T Place<T>(Vector2 position, Quaternion rotation) where T : PoolObject {
		return (T)Place(position, rotation);
	}

	public PoolObject Place(Vector2 position, Quaternion rotation) {
		PoolObject obj;
		if (_pool.Count > 0) {
			obj = _pool.Dequeue();
			obj.gameObject.SetActive(true);
			obj.transform.SetPositionAndRotation(position, rotation);
			obj.Init();
		}
		else {
			obj = Instantiate(poolObject, position, rotation);
			obj.pool = this;
			obj.Init();
		}

		return obj;
	}


	public void Return(PoolObject obj){
		obj.gameObject.SetActive(false);
		_pool.Enqueue(obj);
	}
}
