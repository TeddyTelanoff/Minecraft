using UnityEngine;

public class CameraController : MonoBehaviour
{
	public float _speed;

	public Transform _cameraTransform;
	public Vector2 _rotation;
	public float _rotateSpeed;
	[Range(0, 1)]
	public float _rotateSlerp;

	private Rigidbody _rigidbody;
	private bool _lockCursor;
	private int _groundsTouching;
	private bool _onGround { get => _groundsTouching > 0; }

    private void Start() =>
		_rigidbody = GetComponent<Rigidbody>();

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

			transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(new Vector3 { y = _rotation.y }), _rotateSlerp);
			_cameraTransform.localRotation = Quaternion.Slerp(_cameraTransform.localRotation, Quaternion.Euler(new Vector3 { x = _rotation.x }), _rotateSlerp);
		}

		var movmentForward = Input.GetAxisRaw("Horizontal") * Time.deltaTime * transform.right;
		var movmentRight   = Input.GetAxisRaw("Vertical") * Time.deltaTime * transform.forward;
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

		if (!_onGround)
			movmentUp = 0;

		_rigidbody.AddForce(movmentForward + movmentRight + new Vector3 { y = movmentUp });
	}

	private void OnTriggerEnter(Collider other) => _groundsTouching++;

	private void OnTriggerExit(Collider other) => _groundsTouching--;
}
