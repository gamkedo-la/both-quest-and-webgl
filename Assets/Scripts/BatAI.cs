﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class BatAI : MonoBehaviour
{
	enum State
	{
		Approach,
		Steal,
		Escape
	}

	public float batSpeed = 5f;
	public float batHoverAmt = 0.3f;
	public float batHoverSpeed = 4.0f;
	public float batHoverDist = 3.0f;
	public float batStealTime = 2.0f;

	//Doing State with Primitives First, could refactor to Enumerators later
	private const int B_State_Approach = 0;
	private const int B_State_Stealing = 1;
	private const int B_State_Retreat = 2;
	private int stateNow = B_State_Approach;
	private Vector3 stealPos = Vector3.zero;
	private float stealStartTime = 0.0f;
	private PotatoPile potatoes;
	private Coroutine stealRoutine;
	private GameObject potato;
	private State batState = State.Approach;

	public Action<BatAI> DestroyedAction;

	private bool initialized;
	private bool takenDamageThisFrame;
	private int maxHitPoints;

	[SerializeField]
	private Transform clawTransform;

	[SerializeField]
	private float moveSpeed;
	[SerializeField]
	private float stealTime;

	[SerializeField]
	private int hitPoints;

	[SerializeField]
	private List<CollisionReporter> collisionReporters;


	private void Awake()
	{
		foreach (var reporter in collisionReporters)
		{
			reporter.OnCollitionEvent += OnCollisionHandler;
		}

		maxHitPoints = hitPoints;
	}


	public void Initialize(PotatoPile potatoPile)
	{
		potatoes = potatoPile;

		hitPoints = maxHitPoints;

		stealRoutine = null;

		initialized = true;
	}

	// Update is called once per frame
	void Update()
	{
		//if (stateNow == B_State_Approach)
		//{
		//	transform.LookAt(FromAnywhereSingleton.instance.transform.position);

		//	transform.position += transform.forward * Time.deltaTime * batSpeed;

		//	if (Vector3.Distance(transform.position, FromAnywhereSingleton.instance.transform.position) < batHoverDist)
		//	{
		//		stateNow++;
		//		stealPos = transform.position;

		//		stealStartTime = Time.timeSinceLevelLoad;
		//	}
		//}
		//else if (stateNow == B_State_Stealing)
		//{
		//	float timeStealing = Time.timeSinceLevelLoad - stealStartTime;
		//	transform.position = stealPos + batHoverAmt * Vector3.up * Mathf.Cos(timeStealing * batHoverSpeed);

		//	if (timeStealing > batStealTime)
		//	{
		//		stateNow++;
		//	}
		//}
		//else if (stateNow == B_State_Retreat)
		//{
		//	transform.LookAt(FromAnywhereSingleton.instance.transform.position);

		//	transform.position -= transform.forward * Time.deltaTime * batSpeed;
		//}

		if (initialized)
		{
			switch (batState)
			{
				case State.Approach:
					transform.LookAt(FromAnywhereSingleton.instance.transform.position);
					transform.position += transform.forward * Time.deltaTime * batSpeed;

					if (Vector3.Distance(transform.position, FromAnywhereSingleton.instance.transform.position) < batHoverDist)
					{
						batState = State.Steal;
					}
					break;

				case State.Steal:
					if (stealRoutine == null)
					{
						stealRoutine = StartCoroutine(StealPotatoRoutine());
					}
					break;

				case State.Escape:
					transform.LookAt(FromAnywhereSingleton.instance.transform.position);

					transform.position -= transform.forward * Time.deltaTime * batSpeed;
					break;
			}


		}

		takenDamageThisFrame = false;
	}

	private void OnCollisionHandler(Collision collision)
	{
		TakeDamage();
	}

	private void TakeDamage()
	{
		if (takenDamageThisFrame == false)
		{
			takenDamageThisFrame = true;

			hitPoints--;

			if (hitPoints <= 0)
			{
				Destroyed();
			}

			//Play on hit animatino/vfx
		}
	}

	private void Destroyed()
	{
		//Play death animation
		if (potato)
		{
			potatoes.ReturnPotato(potato);
			potato = null;
		}
		StopAllCoroutines();
		initialized = false;

		DestroyedAction?.Invoke(this);
	}

	private IEnumerator StealPotatoRoutine()
	{
		//TODO: check if there potato is already used
		potato = potatoes.StealPotato();

		if (potato != null)
		{
			transform.LookAt(potato.transform);

			while (Vector3.Distance(clawTransform.position, potato.transform.position) > 0.1f)
			{
				Vector3 stealDir = potato.transform.position - clawTransform.position;
				potato.transform.position -= stealDir.normalized * 1.5f * Time.deltaTime;
				yield return new WaitForEndOfFrame();
			}
			potato.transform.SetParent(transform);
			potatoes.PotatoStolenSuccessfully(potato);
		}

		batState = State.Escape;

		stealRoutine = null;
	}
}
