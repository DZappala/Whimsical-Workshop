using Cinemachine;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputActionAsset actionAsset;
    public BuildingManager buildingManager; // Reference to BuildingManager

    [UsedImplicitly] public CinemachineFreeLook freeLook;
    [UsedImplicitly] public GameObject target;
    public LayerMask groundLayer; // LayerMask to identify the ground

    public float panSpeed = 1f;
    public float rotateSpeed = 1f;
    public float zoomSpeed = 10f;

    public float hoverHeight = 1.5f; // The height above the ground at which the building will hover
    public float hoverSpeed = 1f; // The speed at which the building will hover up and down
    public float hoverScale = 1f; // The scale of the hover effect (larger values mean larger hover distance)

    public float buildingRotateSpeed = 0.05f;
    public float buildingTranslateSpeed = 0.05f;
    private Camera cameraMain;

    private float hoverSeed; // The seed for the Perlin noise function

    private Vector2 pan;
    private InputAction panAction;
    private Vector2 rotate;
    private InputAction rotateAction;
    private bool rotatingBuilding;
    private float zoom;
    private InputAction zoomAction;


    private void Awake()
    {
        panAction = actionAsset.FindAction("Pan");
        rotateAction = actionAsset.FindAction("Rotate");
        zoomAction = actionAsset.FindAction("Zoom");

        panAction.performed += ctx => pan = ctx.ReadValue<Vector2>();
        panAction.canceled += ctx => pan = Vector2.zero;

        rotateAction.performed += ctx => rotate = ctx.ReadValue<Vector2>();
        rotateAction.canceled += ctx => rotate = Vector2.zero;

        zoomAction.performed += ctx => zoom = ctx.ReadValue<float>();
        hoverSeed = Random.Range(0f, 100f); // Generate a random seed for the Perlin noise function
    }

    private void Start()
    {
        cameraMain = Camera.main;
    }

    private void Update()
    {
        HandleMovement();
        if (buildingManager.currentlySelectedBuilding) HandleConstruct();
    }

    private void OnEnable()
    {
        panAction.Enable();
        rotateAction.Enable();
        zoomAction.Enable();
    }

    private void OnDisable()
    {
        panAction.Disable();
        rotateAction.Disable();
        zoomAction.Disable();
    }

    private void HandleConstruct()
    {
        if (!cameraMain) return;
        Ray ray = cameraMain.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer)) return;

        // Move the building to the mouse position
        Vector3 targetPosition = hit.point;
        targetPosition += new Vector3(Mathf.PerlinNoise(hoverSeed, Time.time * hoverSpeed) - 0.5f,
            hoverHeight + Mathf.PerlinNoise(Time.time * hoverSpeed, hoverSeed) * hoverScale,
            Mathf.PerlinNoise(Time.time * hoverSpeed, hoverSeed) - 0.5f) * hoverScale;
        buildingManager.spawnedBuildingInstance.transform.position =
            Vector3.Lerp(buildingManager.spawnedBuildingInstance.transform.position, targetPosition,
                buildingTranslateSpeed);

        // On left click, place the building
        if (Mouse.current.leftButton.wasPressedThisFrame && buildingManager.currentlySelectedBuilding)
            buildingManager.HandleConstruct();

        // On right click, rotate the building
        if (!Mouse.current.rightButton.isPressed) return;
        rotatingBuilding = true;
        Vector3 directionToLookAt = hit.point - buildingManager.spawnedBuildingInstance.transform.position;
        Quaternion rotation = Quaternion.LookRotation(directionToLookAt);
        buildingManager.spawnedBuildingInstance.transform.rotation = Quaternion.Lerp(
            buildingManager.spawnedBuildingInstance.transform.rotation, rotation,
            buildingRotateSpeed);
    }

    private void HandleMovement()
    {
        //Translate the target object based on the pan input
        Vector3 panMovement = new Vector3(pan.x, 0, pan.y) * (panSpeed * Time.deltaTime);
        target.transform.Translate(panMovement, Space.Self);


        if (!rotatingBuilding)
        {
            Vector3 rotationMovement = new Vector3(0, rotate.x, 0) * (rotateSpeed * Time.deltaTime);
            freeLook.m_XAxis.Value += -rotationMovement.y;
        }

        Vector3 zoomMovement = new Vector3(0, 0, zoom) * (zoomSpeed * Time.deltaTime);
        freeLook.m_YAxis.Value += zoomMovement.z;
    }
}