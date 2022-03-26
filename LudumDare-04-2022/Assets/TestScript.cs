using UnityEngine;

public class TestScript : MonoBehaviour
{
	[SerializeField] private float Speed = 100f;

	// Update is called once per frame
	void Update()
	{
		transform.Rotate(Vector3.up * Time.deltaTime * Speed);
	}
}