﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoboCarSpawner : MonoBehaviour
{
	private readonly float TARGET_Z_POS = 69.149f;


	[SerializeField]
	private GameObject roboCarPrefab;
	[SerializeField]
	private Transform spawnLocationMin;
	[SerializeField]
	private Transform spwnLocationMax;

	[SerializeField]
	[Tooltip("Spawned every x serconds")]
	private float spawnRate;
	
	[SerializeField]
	private PotatoPile potatoPile;

	private float timer = 0f;

	private ObjectPool<RoboCar> roboCarPool;

	private void Start()
	{
		//Spawn at the start
		timer = spawnRate;

		roboCarPool = new ObjectPool<RoboCar>();
		roboCarPool.InitializeObjectPool(roboCarPrefab, 20, transform);
	}

	private void Update()
	{
		if(timer >= spawnRate)
		{
			//Spawn Robo Car
			SpawnRoboCar();

			timer = timer - spawnRate;
		}

		timer += Time.deltaTime;
	}

	private void SpawnRoboCar()
	{
		RoboCar roboCar = roboCarPool.GetObject();

		Vector3 spawnLocation = GetSpawnLocation();
		roboCar.transform.position = spawnLocation;

		spawnLocation.z = TARGET_Z_POS;
		roboCar.Initialize(potatoPile, spawnLocation);

		roboCar.DestroyedAction += OnRoboCarDestroyed;

		roboCar.gameObject.SetActive(true);
	}

	private void OnRoboCarDestroyed(RoboCar roboCar)
	{
		roboCar.DestroyedAction -= OnRoboCarDestroyed;

		roboCarPool.ReturnObject(roboCar);
	}

	private Vector3 GetSpawnLocation()
	{
		Vector3 spawnLocation = Vector3.zero;

		spawnLocation.x = UnityEngine.Random.Range(spawnLocationMin.position.x, spwnLocationMax.position.x);
		spawnLocation.y = UnityEngine.Random.Range(spawnLocationMin.position.y, spwnLocationMax.position.y);
		spawnLocation.z = UnityEngine.Random.Range(spawnLocationMin.position.z, spwnLocationMax.position.z);

		return spawnLocation;
	}

}
