using UnityEngine;

public class CameraController : MonoBehaviour
{
	public float _speed;

	public Vector2 _rotation;
	public float _rotateSpeed;
	[Range(0, 1)]
	public float _rotateSlerp;

	private bool _lockCursor;

	private void Update()
	{
		if (Input.GetMouseButtonDown(1))
		{
			_lockCursor = !_lockCursor;
			Cursor.lockState = _lockCursor ? CursorLockMode.Locked : CursorLockMode.None;
		}
	}

	private void FixedUpdate()
	{
		if (_lockCursor)
		{
			_rotation.x -= Input.GetAxis("Mouse Y") * _rotateSpeed * Time.deltaTime;
			_rotation.y += Input.GetAxis("Mouse X") * _rotateSpeed * Time.deltaTime;

			_rotation.x = Mathf.Clamp(_rotation.x, -90, 90);

			transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(_rotation), _rotateSlerp);
		}

		Vector3 movmentForward = Input.GetAxis("Horizontal") * Time.deltaTime * transform.right;
		Vector3 movmentRight   = Input.GetAxis("Vertical") * Time.deltaTime * transform.forward;
		float movmentUp = 0;
		if (Input.GetKey(KeyCode.Space))
			movmentUp++;
		if (Input.GetKey(KeyCode.LeftShift))
			movmentUp--;

		movmentForward.y = movmentRight.y = 0;

		movmentForward.Normalize();
		movmentRight.Normalize();

		movmentForward *= _speed;
		movmentRight *= _speed;
		movmentUp *= _speed;

		transform.localPosition += movmentForward + movmentRight + new Vector3 { y = movmentUp };
	}
}
