using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using BehaviorDesigner.Runtime;
using UnityEngine;

public class BehaviorSelection : MonoBehaviour
{
	private enum BehaviorSelectionType
	{
		MoveTowards = 0,
		RotateTowards = 1,
		Seek = 2,
		Flee = 3,
		Pursue = 4,
		Evade = 5,
		Follow = 6,
		Patrol = 7,
		Cover = 8,
		Wander = 9,
		Search = 10,
		WithinDistance = 11,
		CanSeeObject = 12,
		CanHearObject = 13,
		Flock = 14,
		LeaderFollow = 15,
		Queue = 16,
		Last = 17
	}

	public GameObject marker;

	public GameObject mainBot;

	public GameObject flockGroup;

	public GameObject followGroup;

	public GameObject queueGroup;

	public GameObject[] waypoints;

	public GameObject[] waypointsA;

	public GUISkin descriptionGUISkin;

	private Vector3[] flockGroupPosition;

	private Vector3[] followGroupPosition;

	private Vector3[] queueGroupPosition;

	private Quaternion[] flockGroupRotation;

	private Quaternion[] followGroupRotation;

	private Quaternion[] queueGroupRotation;

	private Dictionary<int, BehaviorTree> behaviorTreeGroup = new Dictionary<int, BehaviorTree>();

	private BehaviorSelectionType selectionType;

	private BehaviorSelectionType prevSelectionType;

	public void Start()
	{
		BehaviorTree[] components = mainBot.GetComponents<BehaviorTree>();
		for (int i = 0; i < components.Length; i++)
		{
			behaviorTreeGroup.Add(components[i].Group, components[i]);
		}
		components = Camera.main.GetComponents<BehaviorTree>();
		for (int j = 0; j < components.Length; j++)
		{
			behaviorTreeGroup.Add(components[j].Group, components[j]);
		}
		flockGroupPosition = new Vector3[flockGroup.transform.childCount];
		flockGroupRotation = new Quaternion[flockGroup.transform.childCount];
		for (int k = 0; k < flockGroup.transform.childCount; k++)
		{
			flockGroup.transform.GetChild(k).gameObject.SetActive(value: false);
			flockGroupPosition[k] = flockGroup.transform.GetChild(k).transform.position;
			flockGroupRotation[k] = flockGroup.transform.GetChild(k).transform.rotation;
		}
		followGroupPosition = new Vector3[followGroup.transform.childCount];
		followGroupRotation = new Quaternion[followGroup.transform.childCount];
		for (int l = 0; l < followGroup.transform.childCount; l++)
		{
			followGroup.transform.GetChild(l).gameObject.SetActive(value: false);
			followGroupPosition[l] = followGroup.transform.GetChild(l).transform.position;
			followGroupRotation[l] = followGroup.transform.GetChild(l).transform.rotation;
		}
		queueGroupPosition = new Vector3[queueGroup.transform.childCount];
		queueGroupRotation = new Quaternion[queueGroup.transform.childCount];
		for (int m = 0; m < queueGroup.transform.childCount; m++)
		{
			queueGroup.transform.GetChild(m).gameObject.SetActive(value: false);
			queueGroupPosition[m] = queueGroup.transform.GetChild(m).transform.position;
			queueGroupRotation[m] = queueGroup.transform.GetChild(m).transform.rotation;
		}
		SelectionChanged();
	}

	public void OnGUI()
	{
		GUILayout.BeginVertical(GUILayout.Width(300f));
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("<-"))
		{
			prevSelectionType = selectionType;
			selectionType = (BehaviorSelectionType)((int)(selectionType - 1) % 17);
			if (selectionType < BehaviorSelectionType.MoveTowards)
			{
				selectionType = BehaviorSelectionType.Queue;
			}
			SelectionChanged();
		}
		GUILayout.Box(SplitCamelCase(selectionType.ToString()), GUILayout.Width(220f));
		if (GUILayout.Button("->"))
		{
			prevSelectionType = selectionType;
			selectionType = (BehaviorSelectionType)((int)(selectionType + 1) % 17);
			SelectionChanged();
		}
		GUILayout.EndHorizontal();
		GUILayout.Box(Description(), descriptionGUISkin.box);
		if (selectionType == BehaviorSelectionType.CanHearObject && GUILayout.Button("Play Sound"))
		{
			marker.GetComponent<AudioSource>().Play();
		}
		GUILayout.EndVertical();
	}

	private string Description()
	{
		string result = "";
		switch (selectionType)
		{
		case BehaviorSelectionType.MoveTowards:
			result = "The Move Towards task will move the agent towards the target (without pathfinding). In this example the green agent is moving towards the red dot.";
			break;
		case BehaviorSelectionType.RotateTowards:
			result = "The Rotate Towards task rotate the agent towards the target. In this example the green agent is rotating towards the red dot.";
			break;
		case BehaviorSelectionType.Seek:
			result = "The Seek task will move the agent towards the target with pathfinding. In this example the green agent is seeking the red dot (which moves).";
			break;
		case BehaviorSelectionType.Flee:
			result = "The Flee task will move the agent away from the target with pathfinding. In this example the green agent is fleeing from red dot (which moves).";
			break;
		case BehaviorSelectionType.Pursue:
			result = "The Pursue task is similar to the Seek task except the Pursue task predicts where the target is going to be in the future. This allows the agent to arrive at the target earlier than it would have with the Seek task.";
			break;
		case BehaviorSelectionType.Evade:
			result = "The Evade task is similar to the Flee task except the Evade task predicts where the target is going to be in the future. This allows the agent to flee from the target earlier than it would have with the Flee task.";
			break;
		case BehaviorSelectionType.Follow:
			result = "The Follow task will allow the agent to continuously follow a GameObject. In this example the green agent is following the red dot.";
			break;
		case BehaviorSelectionType.Patrol:
			result = "The Patrol task moves from waypoint to waypint. In this example the green agent is patrolling with four different waypoints (the white dots).";
			break;
		case BehaviorSelectionType.Cover:
			result = "The Cover task will move the agent into cover from its current position. In this example the agent sees cover in front of it so takes cover behind the wall.";
			break;
		case BehaviorSelectionType.Wander:
			result = "The Wander task moves the agent randomly throughout the map with pathfinding.";
			break;
		case BehaviorSelectionType.Search:
			result = "The Search task will search the map by wandering until it finds the target. It can find the target by seeing or hearing the target. In this example the Search task is looking for the red dot.";
			break;
		case BehaviorSelectionType.WithinDistance:
			result = "The Within Distance task is a conditional task that returns success when another object comes within distance of the current agent. In this example the Within Distance task is paired with the Seek task so the agent will move towards the target as soon as the target within distance.";
			break;
		case BehaviorSelectionType.CanSeeObject:
			result = "The Can See Object task is a conditional task that returns success when it sees an object in front of the current agent. In this example the Can See Object task is paired with the Seek task so the agent will move towards the target as soon as the target is seen.";
			break;
		case BehaviorSelectionType.CanHearObject:
			result = "The Can Hear Object task is a conditional task that returns success when it hears another object. Press the \"Play\" button to emit a sound from the red dot. If the red dot is within range of the agent when the sound is played then the agent will move towards the red dot with the Seek task.";
			break;
		case BehaviorSelectionType.Flock:
			result = "The Flock task moves a group of objects together in a pattern (which can be adjusted). In this example the Flock task is controlling all 30 objects. There are no colliders on the objects - the Flock task is completing managing the position of all of the objects";
			break;
		case BehaviorSelectionType.LeaderFollow:
			result = "The Leader Follow task moves a group of objects behind a leader object. There are two behavior trees running in this example - one for the leader (who is patrolling the area) and one for the group of objects. Again, there is are no colliders on the objects.";
			break;
		case BehaviorSelectionType.Queue:
			result = "The Queue task will move a group of objects through a small space in an organized way. In this example the Queue task is controlling all of the objects. Another way to move all of the objects through the doorway is with the seek task, however with this approach the objects would group up at the doorway.";
			break;
		}
		return result;
	}

	private static string SplitCamelCase(string s)
	{
		s = new Regex("(?<=[A-Z])(?=[A-Z][a-z])|(?<=[^A-Z])(?=[A-Z])|(?<=[A-Za-z])(?=[^A-Za-z])", RegexOptions.IgnorePatternWhitespace).Replace(s, " ");
		return (char.ToUpper(s[0]) + s.Substring(1)).Trim();
	}

	private void SelectionChanged()
	{
		DisableAll();
		switch (selectionType)
		{
		case BehaviorSelectionType.MoveTowards:
			marker.transform.position = new Vector3(20f, 1f, -20f);
			marker.SetActive(value: true);
			mainBot.transform.position = new Vector3(-20f, 1f, -20f);
			mainBot.transform.eulerAngles = new Vector3(0f, 180f, 0f);
			mainBot.SetActive(value: true);
			break;
		case BehaviorSelectionType.RotateTowards:
			marker.transform.position = new Vector3(20f, 1f, 10f);
			marker.SetActive(value: true);
			mainBot.transform.position = new Vector3(0f, 1f, -20f);
			mainBot.transform.eulerAngles = new Vector3(0f, 180f, 0f);
			mainBot.SetActive(value: true);
			break;
		case BehaviorSelectionType.Seek:
			marker.transform.position = new Vector3(20f, 1f, 20f);
			marker.SetActive(value: true);
			marker.GetComponent<Animation>()["MarkerSeek"].time = 0f;
			marker.GetComponent<Animation>()["MarkerSeek"].speed = 1f;
			marker.GetComponent<Animation>().Play("MarkerSeek");
			mainBot.transform.position = new Vector3(-20f, 1f, -20f);
			mainBot.transform.eulerAngles = new Vector3(0f, 180f, 0f);
			mainBot.SetActive(value: true);
			break;
		case BehaviorSelectionType.Flee:
			marker.transform.position = new Vector3(20f, 1f, 20f);
			marker.SetActive(value: true);
			marker.GetComponent<Animation>()["MarkerFlee"].time = 0f;
			marker.GetComponent<Animation>()["MarkerFlee"].speed = 1f;
			marker.GetComponent<Animation>().Play("MarkerFlee");
			mainBot.transform.position = new Vector3(10f, 1f, 18f);
			mainBot.transform.eulerAngles = new Vector3(0f, 180f, 0f);
			mainBot.SetActive(value: true);
			break;
		case BehaviorSelectionType.Pursue:
			marker.transform.position = new Vector3(20f, 1f, 20f);
			marker.SetActive(value: true);
			marker.GetComponent<Animation>()["MarkerPersue"].time = 0f;
			marker.GetComponent<Animation>()["MarkerPersue"].speed = 1f;
			marker.GetComponent<Animation>().Play("MarkerPersue");
			mainBot.transform.position = new Vector3(-20f, 1f, 0f);
			mainBot.transform.eulerAngles = new Vector3(0f, 90f, 0f);
			mainBot.SetActive(value: true);
			break;
		case BehaviorSelectionType.Evade:
			marker.transform.position = new Vector3(20f, 1f, 20f);
			marker.SetActive(value: true);
			marker.GetComponent<Animation>()["MarkerEvade"].time = 0f;
			marker.GetComponent<Animation>()["MarkerEvade"].speed = 1f;
			marker.GetComponent<Animation>().Play("MarkerEvade");
			mainBot.transform.position = new Vector3(0f, 1f, 18f);
			mainBot.transform.eulerAngles = new Vector3(0f, 180f, 0f);
			mainBot.SetActive(value: true);
			break;
		case BehaviorSelectionType.Follow:
			marker.transform.position = new Vector3(20f, 1f, 20f);
			marker.SetActive(value: true);
			marker.GetComponent<Animation>()["MarkerFollow"].time = 0f;
			marker.GetComponent<Animation>()["MarkerFollow"].speed = 1f;
			marker.GetComponent<Animation>().Play("MarkerFollow");
			mainBot.transform.position = new Vector3(20f, 1f, 15f);
			mainBot.transform.eulerAngles = new Vector3(0f, 0f, 0f);
			mainBot.SetActive(value: true);
			break;
		case BehaviorSelectionType.Patrol:
		{
			for (int m = 0; m < waypoints.Length; m++)
			{
				waypoints[m].SetActive(value: true);
			}
			mainBot.transform.position = new Vector3(-20f, 1f, 20f);
			mainBot.transform.eulerAngles = new Vector3(0f, 180f, 0f);
			mainBot.SetActive(value: true);
			break;
		}
		case BehaviorSelectionType.Cover:
			mainBot.transform.position = new Vector3(-5f, 1f, -10f);
			mainBot.transform.eulerAngles = new Vector3(0f, 0f, 0f);
			mainBot.SetActive(value: true);
			break;
		case BehaviorSelectionType.Wander:
			mainBot.transform.position = new Vector3(-20f, 1f, -20f);
			mainBot.transform.eulerAngles = new Vector3(0f, 0f, 0f);
			mainBot.SetActive(value: true);
			break;
		case BehaviorSelectionType.Search:
			marker.transform.position = new Vector3(20f, 1f, 20f);
			marker.SetActive(value: true);
			mainBot.transform.position = new Vector3(-20f, 1f, -20f);
			mainBot.transform.eulerAngles = new Vector3(0f, 0f, 0f);
			mainBot.SetActive(value: true);
			break;
		case BehaviorSelectionType.WithinDistance:
			marker.transform.position = new Vector3(20f, 1f, 20f);
			marker.GetComponent<Animation>()["MarkerPersue"].time = 0f;
			marker.GetComponent<Animation>()["MarkerPersue"].speed = 1f;
			marker.GetComponent<Animation>().Play("MarkerPersue");
			marker.SetActive(value: true);
			mainBot.transform.position = new Vector3(-15f, 1f, 2f);
			mainBot.transform.eulerAngles = new Vector3(0f, 0f, 0f);
			mainBot.SetActive(value: true);
			break;
		case BehaviorSelectionType.CanSeeObject:
			marker.transform.position = new Vector3(20f, 1f, 20f);
			marker.GetComponent<Animation>()["MarkerPersue"].time = 0f;
			marker.GetComponent<Animation>()["MarkerPersue"].speed = 1f;
			marker.GetComponent<Animation>().Play("MarkerPersue");
			marker.SetActive(value: true);
			mainBot.transform.position = new Vector3(-15f, 1f, -10f);
			mainBot.transform.eulerAngles = new Vector3(0f, 0f, 0f);
			mainBot.SetActive(value: true);
			break;
		case BehaviorSelectionType.CanHearObject:
			marker.transform.position = new Vector3(20f, 1f, 20f);
			marker.GetComponent<Animation>()["MarkerPersue"].time = 0f;
			marker.GetComponent<Animation>()["MarkerPersue"].speed = 1f;
			marker.GetComponent<Animation>().Play("MarkerPersue");
			marker.SetActive(value: true);
			mainBot.transform.position = new Vector3(-15f, 1f, -10f);
			mainBot.transform.eulerAngles = new Vector3(0f, 0f, 0f);
			mainBot.SetActive(value: true);
			break;
		case BehaviorSelectionType.Flock:
		{
			Camera.main.transform.position = new Vector3(0f, 90f, -150f);
			for (int l = 0; l < flockGroup.transform.childCount; l++)
			{
				flockGroup.transform.GetChild(l).gameObject.SetActive(value: true);
			}
			break;
		}
		case BehaviorSelectionType.LeaderFollow:
		{
			for (int j = 0; j < waypointsA.Length; j++)
			{
				waypointsA[j].SetActive(value: true);
			}
			mainBot.transform.position = new Vector3(0f, 1f, -130f);
			mainBot.SetActive(value: true);
			Camera.main.transform.position = new Vector3(0f, 90f, -150f);
			for (int k = 0; k < followGroup.transform.childCount; k++)
			{
				followGroup.transform.GetChild(k).gameObject.SetActive(value: true);
			}
			break;
		}
		case BehaviorSelectionType.Queue:
		{
			marker.transform.position = new Vector3(45f, 1f, 0f);
			for (int i = 0; i < queueGroup.transform.childCount; i++)
			{
				queueGroup.transform.GetChild(i).gameObject.SetActive(value: true);
			}
			break;
		}
		}
		StartCoroutine("EnableBehavior");
	}

	private void DisableAll()
	{
		StopCoroutine("EnableBehavior");
		behaviorTreeGroup[(int)prevSelectionType].DisableBehavior();
		if (prevSelectionType == BehaviorSelectionType.LeaderFollow)
		{
			behaviorTreeGroup[17].DisableBehavior();
		}
		marker.GetComponent<Animation>().Stop();
		marker.SetActive(value: false);
		mainBot.SetActive(value: false);
		Camera.main.transform.position = new Vector3(0f, 90f, 0f);
		for (int i = 0; i < flockGroup.transform.childCount; i++)
		{
			flockGroup.transform.GetChild(i).gameObject.SetActive(value: false);
			flockGroup.transform.GetChild(i).transform.position = flockGroupPosition[i];
			flockGroup.transform.GetChild(i).transform.rotation = flockGroupRotation[i];
		}
		for (int j = 0; j < followGroup.transform.childCount; j++)
		{
			followGroup.transform.GetChild(j).gameObject.SetActive(value: false);
			followGroup.transform.GetChild(j).transform.position = followGroupPosition[j];
			followGroup.transform.GetChild(j).transform.rotation = followGroupRotation[j];
		}
		for (int k = 0; k < queueGroup.transform.childCount; k++)
		{
			queueGroup.transform.GetChild(k).gameObject.SetActive(value: false);
			queueGroup.transform.GetChild(k).transform.position = queueGroupPosition[k];
			queueGroup.transform.GetChild(k).transform.rotation = queueGroupRotation[k];
		}
		for (int l = 0; l < waypoints.Length; l++)
		{
			waypoints[l].SetActive(value: false);
		}
		for (int m = 0; m < waypointsA.Length; m++)
		{
			waypointsA[m].SetActive(value: false);
		}
	}

	private IEnumerator EnableBehavior()
	{
		yield return new WaitForSeconds(0.5f);
		behaviorTreeGroup[(int)selectionType].EnableBehavior();
		if (selectionType == BehaviorSelectionType.LeaderFollow)
		{
			behaviorTreeGroup[17].EnableBehavior();
		}
	}
}
