﻿using UnityEngine;

public abstract class Spawner : MonoBehaviour
{
	[Header("Pool")]
	[SerializeField]
	protected GameObject pooledPrefab;
	[SerializeField]
	protected int pooledCount;

	[Header("Spawning")]
	[SerializeField]
	[Tooltip("Spawned every x serconds")]
	private float spawnRate;

	[SerializeField]
	private Transform spawnLocationParent;
	[SerializeField]
	private Transform spawnLocationMin;
	[SerializeField]
	private Transform spwnLocationMax;

	public bool ShouldStartSpawning { get; set; }

	private float timer = 0f;

	protected ComponentPool componentPool;

	protected virtual void Start()
	{
		//Pool
		componentPool = new ComponentPool();

		//Spawn at the start
		timer = spawnRate;

		//Remove this so that we can have waves of enemies;
		//ShouldStartSpawning = true;
	}

	private void Update()
	{
		if (ShouldStartSpawning)
		{
			if (timer >= spawnRate)
			{
				//Spawn Robo Car
				Spawn();

				timer = timer - spawnRate;
			}

			timer += Time.deltaTime;
		}
	}

	protected virtual void Spawn() { }

	protected Transform GetSpawnLocation()
	{
		GameObject spawnLocationObject = new GameObject("Spawn Location Object");
		spawnLocationObject.transform.parent = spawnLocationParent;

		Vector3 spawnLocation = Vector3.zero;

		spawnLocation.x = UnityEngine.Random.Range(spawnLocationMin.position.x, spwnLocationMax.position.x);
		spawnLocation.y = UnityEngine.Random.Range(spawnLocationMin.position.y, spwnLocationMax.position.y);
		spawnLocation.z = UnityEngine.Random.Range(spawnLocationMin.position.z, spwnLocationMax.position.z);

		spawnLocationObject.transform.position = spawnLocation;

		return spawnLocationObject.transform;
	}

}
