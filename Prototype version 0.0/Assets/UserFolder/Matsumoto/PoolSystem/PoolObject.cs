using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PoolObject : MonoBehaviour
{
	public Pool pool { private get; set; }

	public abstract void Init();

	protected new void Destroy(Object obj){
		ReturnToPool();
	}

	public void ReturnToPool(){
		pool.Return(this);
	}

}
