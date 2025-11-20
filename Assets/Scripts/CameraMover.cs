using UnityEngine;

[RequireComponent(typeof(InputReader))]
public class CameraMover : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 100.0f;
    [SerializeField] private float _zoomSpeed = 100.0f;
    [SerializeField] private Vector2 _positiveBoundaries = new Vector2(380f, 380f);
    [SerializeField] private Vector2 _negativeBoundaries = new Vector2(-380f, -380f);
    [SerializeField] private float _minZoomValue = 10.0f;
    [SerializeField] private float _maxZoomValue = 55.0f;

    private InputReader _reader;
    private Vector2 _movementDirection = Vector2.zero;

    private void Awake()
    {
        _reader = GetComponent<InputReader>();
    }

    private void OnEnable()
    {
        _reader.Moved += ChangeDirection;
        _reader.Zoomed += Zoom;
    }

    private void OnDisable()
    {
        _reader.Moved -= ChangeDirection;
        _reader.Zoomed -= Zoom;
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        if (transform.position.x + _movementDirection.x >= _positiveBoundaries.x 
            || transform.position.x + _movementDirection.x <= _negativeBoundaries.x)
            _movementDirection.x = 0;

        if (transform.position.z + _movementDirection.y >= _positiveBoundaries.y 
            || transform.position.z + _movementDirection.y <= _negativeBoundaries.y)
            _movementDirection.y = 0;

        Vector3 newPosition = new Vector3(transform.position.x + _movementDirection.x, transform.position.y, transform.position.z + _movementDirection.y);
        transform.position = Vector3.Lerp(transform.position, newPosition, _moveSpeed * Time.deltaTime);
    }

    private void ChangeDirection(Vector2 direction)
    {
        _movementDirection = direction;
    }

    private void Zoom(float direction)
    {
        if (transform.position.y + direction >= _maxZoomValue 
            || transform.position.y + direction <= _minZoomValue)
            direction = 0;

        Vector3 newPosition = new Vector3(transform.position.x, transform.position.y + direction, transform.position.z);
        transform.position = Vector3.Lerp(transform.position, newPosition, _zoomSpeed * Time.deltaTime);
    }
}
