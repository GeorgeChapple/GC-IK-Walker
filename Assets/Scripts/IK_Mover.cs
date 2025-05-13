using UnityEngine;

public class IK_Mover : MonoBehaviour
{
    [SerializeField] private Vector3 gravity;
    [SerializeField] private float speed;

    private Vector3 directionOfLocalMovement;
    private CharacterController controller;
    [SerializeField] private IK_Walker_Manager manager;

    private void Awake()
    {
        //manager = transform.GetComponentInChildren<IK_Walker_Manager>();
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        AddGravity();
        manager.direction = directionOfLocalMovement;
    }

    public void HandleMovement()
    {
        float verticalMovementInput = Input.GetAxisRaw("Vertical");
        float horizontalMovementInput = Input.GetAxisRaw("Horizontal");
        directionOfLocalMovement = new Vector3(horizontalMovementInput, 0, verticalMovementInput).normalized;
        controller.Move(directionOfLocalMovement * speed * Time.deltaTime);
    }

    public void AddGravity()
    {
        if (!controller.isGrounded)
        {
            controller.SimpleMove(gravity);
        }
    }
}
