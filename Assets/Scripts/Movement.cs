using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
	CombatOrganizer co;
	bool isMoving = false;

	[Header("UI"),SerializeField] 
	Transform navigationPanel;
	[SerializeField]
	GameObject combatPanel;

	[Header("Movement Speeds")]
	[SerializeField] float TurnSpeed = 1;
	[SerializeField] float moveSpeed = 5;

	[Header("Current Orientation"),SerializeField]
	int facingDirection = 0;
	[SerializeField] DungeonRoom CurrentRoom;

	private void Start()
	{
		co = FindObjectOfType<CombatOrganizer>();
		transform.position = CurrentRoom.transform.position;
		CurrentRoom.OnRoomEnter();
	}

	#region Inputs
	public void OnRotate(InputAction.CallbackContext context)
	{
		float contextResult = context.ReadValue<float>();
		if (contextResult != 0)
			OnRotate(90 * Mathf.Sign(contextResult));
	}
	public void OnRotate(float direction)
	{
		if (!isMoving)
			StartCoroutine(RotateCam(direction));
	}

	public void OnMove(InputAction.CallbackContext context)
	{
		float contextResult = context.ReadValue<float>();
		if (contextResult != 0)
			OnMove(contextResult > 0);
	}
	public void OnMove(bool forward)
	{
		DungeonRoom newRoom = CurrentRoom.ConnectedRooms[(forward) ? facingDirection : (facingDirection + 2) % 4];
		if (!isMoving && newRoom != null)
			StartCoroutine(MoveCam(newRoom));
	}
	#endregion

	private void OnDestroy()
	{
		CurrentRoom.LeaveRoom();
	}

	public void EndCombat()
	{
		navigationPanel.gameObject.SetActive(true);
		combatPanel.SetActive(false);
	}

	IEnumerator RotateCam(float direction)
	{
		float t = 0;
		Quaternion StartDirection = transform.rotation;
		Quaternion DesiredDirection = Quaternion.Euler( transform.rotation.eulerAngles + new Vector3(0, direction));
		Button[] navbutts = navigationPanel.GetComponentsInChildren<Button>();

		isMoving = true;
		foreach (Button butt in navbutts)
			butt.interactable = false;

		while (t < 1)
		{
			transform.rotation = Quaternion.Lerp(StartDirection, DesiredDirection, t);
			t += Time.deltaTime * TurnSpeed;
			yield return null;
		}

		transform.rotation = DesiredDirection;
		facingDirection = Mod(facingDirection + 1 * (int)Mathf.Sign(direction), 4);

		isMoving = false;
		foreach (Button butt in navbutts)
			butt.interactable = true;
	}

	IEnumerator MoveCam(DungeonRoom destination)
	{
		isMoving = true;
		Button[] navbutts = navigationPanel.GetComponentsInChildren<Button>();
		foreach (Button butt in navbutts)
			butt.interactable = false;

		Vector3 destinationPos = destination.transform.position;
		Vector3 direction = (destinationPos - transform.position).normalized * moveSpeed;
		while ((transform.position - destinationPos).magnitude > 0.1f)
		{
			transform.position += direction * Time.deltaTime;
			//t += Time.deltaTime * TurnSpeed;
			yield return null;
		}

		// set up room stuff
		transform.position = destinationPos;
		CurrentRoom.LeaveRoom();
		destination.OnRoomEnter();
		CurrentRoom = destination;

		isMoving = false;
		foreach (Button butt in navbutts)
			butt.interactable = true;

		if (co.StartAFight(CurrentRoom.PresentEnemies))
		{
			navigationPanel.gameObject.SetActive(false);
			combatPanel.SetActive(true);
		}
	}
	int Mod(int a, int n) => (a % n + n) % n;

}
