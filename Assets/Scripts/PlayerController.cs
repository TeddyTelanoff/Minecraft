using UnityEngine;

public class PlayerController : MonoBehaviour
{
	public float _jumpForce = 500;
	public float _airBoost = 1.25f;
	public float _speed = 50;

	public Transform _cameraTransform;
	public Vector2 _rotation = Vector2.zero;
	public float _rotateSpeed = 625;
	[Range(0, 1)]
	public float _rotateSlerp = 0.99f;

	public Transform _groundCheck;
	public float _groundReach = 0.4f;

	private Rigidbody _rigidbody;
	private bool _lockCursor;
	private bool _onGround;

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
		_onGround = Physics.CheckSphere(_groundCheck.position, _groundReach, LayerMask.NameToLayer("Ground"));
		

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
		movmentUp *= _jumpForce;

		if (!_onGround)
        {
			movmentForward *= _airBoost;
			movmentRight *= _airBoost;
            movmentUp = 0;
		}

		_rigidbody.AddForce(movmentForward + movmentRight + new Vector3 { y = movmentUp });
	}

    private void OnDrawGizmos()
    {
		Gizmos.color = Color.cyan;
		Gizmos.DrawWireSphere(_groundCheck.position, _groundReach);
    }
}
