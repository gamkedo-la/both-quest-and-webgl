﻿using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generict Object pool for unity Components
/// Objects are stored and given to requests as inactive
/// Objects need to be returned to the pool after use
/// </summary>
/// <typeparam name="T"></typeparam>
public class ObjectPool<T> where T : Component
{
	private List<T> pool;
	private int accessIndex;

	public ObjectPool()
	{
		pool = new List<T>();
		accessIndex = 0;
	}

	public void InitializeObjectPool(GameObject objectToPool, int count, Transform patent = null)
	{
		for (int i = 0; i < count; i++)
		{
			T poolObject = GameObject.Instantiate(objectToPool, patent).GetComponent<T>();
			poolObject.gameObject.SetActive(false);			

			pool.Add(poolObject);
		}
	}

	/// <summary>
	/// Returns next inactive object in pool
	/// </summary>
	/// <returns></returns>
	public T GetObject()
	{
		bool nextAvailableObjectFound = false;
		int startingIndex = accessIndex;

		while (nextAvailableObjectFound == false)
		{
			if (pool[accessIndex].gameObject.activeInHierarchy == false)
			{
				return pool[accessIndex];
			}
			
			accessIndex = (accessIndex + 1) % pool.Count;
			

			//If we look through the entire pool and have not found any available Objects 
			if(accessIndex == startingIndex)
			{
				//Throw exception, this is a big issue
				throw new NoAvailabePoolObjectsException("There are no free objects in pool. Make sure to return objects before getting new ones");

				//Alternatively, could increase pool size, e.g List<T>, but this could effect frame rate 
			}
		}

		return null;
	}

	public void ReturnObject(T pooledObject)
	{
		if(pool.Contains(pooledObject))
		{
			pooledObject.gameObject.SetActive(false);
		}
		else
		{
			Debug.LogError("Object is not from this Pool");
		}
	}
}

public class NoAvailabePoolObjectsException : Exception
{
	public NoAvailabePoolObjectsException(string message) : base(message)
	{
	}
}