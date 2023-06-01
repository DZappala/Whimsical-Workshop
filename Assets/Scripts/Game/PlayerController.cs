using Buildings;
using Cinemachine;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game
{
    public class PlayerController : MonoBehaviour
    {
        public InputActionAsset actionAsset;
        public BuildingManager buildingManager; // Reference to BuildingManager

        [UsedImplicitly] public CinemachineFreeLook freeLook;
        [UsedImplicitly] public GameObject target;
        public LayerMask groundLayer; // LayerMask to identify the ground

        public float panSpeed;
        public float rotateSpeed;
        public float zoomSpeed;

        public float buildingRotateSpeed;
        public float buildingTranslateSpeed;
        public float zoomSmoothTime = 0.2f; // Tune this value to get the desired smoothness
        private Camera cameraMain;
        private float currentZoomVelocity;

        private Vector2 pan;
        private InputAction panAction;
        private Vector2 rotate;
        private InputAction rotateAction;
        private bool rotatingBuilding;

        private float targetZoomLevel;
        private float zoom;
        private InputAction zoomAction;


        private void Awake()
        {
            panAction = actionAsset.FindAction("Pan");
            rotateAction = actionAsset.FindAction("Rotate");
            zoomAction = actionAsset.FindAction("Zoom");

            panAction.performed += ctx => pan = ctx.ReadValue<Vector2>();
            panAction.canceled += _ => pan = Vector2.zero;

            rotateAction.performed += ctx => rotate = ctx.ReadValue<Vector2>();
            rotateAction.canceled += _ => rotate = Vector2.zero;

            zoomAction.performed += ctx => zoom = ctx.ReadValue<float>();
            zoomAction.canceled += _ => zoom = 0;

            targetZoomLevel = freeLook.m_YAxis.Value;
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
            if (!rotatingBuilding)
            {
                if (!cameraMain) return;
                Ray ray = cameraMain.ScreenPointToRay(Mouse.current.position.ReadValue());

                // If the mouse is not over the ground, return
                if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer)) return;

                // Move the building to the mouse position
                Vector3 targetPosition = hit.point;
                buildingManager.spawnedBuildingInstance.transform.position =
                    Vector3.Lerp(buildingManager.spawnedBuildingInstance.transform.position, targetPosition,
                        buildingTranslateSpeed);
            }

            // On left click, place the building
            if (Mouse.current.leftButton.wasPressedThisFrame && buildingManager.currentlySelectedBuilding)
                buildingManager.HandleConstruct();

            // On right click, rotate the building.
            // If the mouse is moves to the right rotate clockwise,
            // if it moves to the left rotate counter-clockwise,
            // if it does not move, do not rotate.
            if (Mouse.current.rightButton.isPressed)
            {
                rotatingBuilding = true;
                Vector2 mouseDelta = Mouse.current.delta.ReadValue();
                float rotationAmount = mouseDelta.x * buildingRotateSpeed;
                buildingManager.spawnedBuildingInstance.transform.Rotate(Vector3.forward, rotationAmount);
            }
            else
            {
                rotatingBuilding = false;
            }
        }

        private void HandleMovement()
        {
            // Transform the pan movement so that it's inverted based on the camera's orientation
            Vector3 panMovement = new Vector3(-pan.x, 0, -pan.y) * (Time.deltaTime * panSpeed);
            target.transform.Translate(panMovement, Space.Self);

            if (!rotatingBuilding)
            {
                Vector3 rotationMovement = new Vector3(0, rotate.x, 0) * (rotateSpeed * Time.deltaTime);
                target.transform.Rotate(rotationMovement, Space.World);
            }

            // Update the target zoom level based on the input
            targetZoomLevel += zoom * zoomSpeed * Time.deltaTime;
            // Clamp the target zoom level to the valid range
            // Assuming 0f and 1f are the min and max zoom levels
            targetZoomLevel = Mathf.Clamp(targetZoomLevel, 0f, 1f);
            // Smoothly interpolate towards the target zoom level
            freeLook.m_YAxis.Value = Mathf.SmoothDamp(freeLook.m_YAxis.Value, targetZoomLevel, ref currentZoomVelocity,
                zoomSmoothTime);
        }
    }
}