using System;
using System.Collections.Generic;
using UnityEngine;

namespace CodeMonkey.MonoBehaviours
{
    public class CameraFollow : MonoBehaviour
    {
        public static CameraFollow Instance { get; private set; }

        private Camera myCamera;
        private Func<Vector3> GetCameraFollowPositionFunc;
        private Func<float> GetCameraZoomFunc;
        private Vector3 shakeOffset = Vector3.zero;

        // zoom out if the player has his mouse in the edges of the screen  
        private float originalZoom;
        [SerializeField] private float firstMaxZoomOut = 7f; // Adjust this to your preference
        [SerializeField] private float firstMouseEdgeThreshold = 0.9f; // This means the outer 10% of the viewport
        [SerializeField] private float secondMaxZoomOut = 6f; // Adjust this to your preference
        [SerializeField] private float secondMouseEdgeThreshold = 0.75f; // This means the outer 25% of the viewport

        // if the mouse is at the top or bottom offset the camera so that the player can see there
        [SerializeField] private float bottomScreenOffsetY = -2f;
        [SerializeField] private float topScreenOffsetY = 2f;


        public void Setup(Func<Vector3> GetCameraFollowPositionFunc, Func<float> GetCameraZoomFunc, bool teleportToFollowPosition, bool instantZoom)
        {
            this.GetCameraFollowPositionFunc = GetCameraFollowPositionFunc;
            this.GetCameraZoomFunc = GetCameraZoomFunc;

            if (teleportToFollowPosition)
            {
                Vector3 cameraFollowPosition = GetCameraFollowPositionFunc();
                cameraFollowPosition.z = transform.position.z;
                transform.position = cameraFollowPosition;
            }

            if (instantZoom)
            {
                myCamera.orthographicSize = GetCameraZoomFunc();
            }
        }

        private void Awake()
        {
            Instance = this;
            myCamera = transform.GetComponent<Camera>();
            originalZoom = myCamera.orthographicSize;
        }

        public void SetCameraFollowPosition(Vector3 cameraFollowPosition)
        {
            SetGetCameraFollowPositionFunc(() => cameraFollowPosition);
        }

        public void SetGetCameraFollowPositionFunc(Func<Vector3> GetCameraFollowPositionFunc)
        {
            this.GetCameraFollowPositionFunc = GetCameraFollowPositionFunc;
        }

        public void SetCameraZoom(float cameraZoom)
        {
            SetGetCameraZoomFunc(() => cameraZoom);
        }

        public void SetGetCameraZoomFunc(Func<float> GetCameraZoomFunc)
        {
            this.GetCameraZoomFunc = GetCameraZoomFunc;
        }


        private void Update()
        {
            HandleMovement();
            HandleZoom();
        }

        private void HandleMovement()
        {
            if (GetCameraFollowPositionFunc == null) return;

            Vector3 cameraFollowPosition = GetCameraFollowPositionFunc();
            cameraFollowPosition = AdjustPositionBasedOnMousePosition(cameraFollowPosition);
            cameraFollowPosition.z = transform.position.z;

            Vector3 cameraMoveDir = (cameraFollowPosition - transform.position).normalized;
            float distance = Vector3.Distance(cameraFollowPosition, transform.position);
            float cameraMoveSpeed = 3f;

            if (distance > 0)
            {
                Vector3 newCameraPosition = transform.position + cameraMoveDir * distance * cameraMoveSpeed * Time.deltaTime;

                float distanceAfterMoving = Vector3.Distance(newCameraPosition, cameraFollowPosition);

                if (distanceAfterMoving > distance)
                {
                    // Overshot the target
                    newCameraPosition = cameraFollowPosition;
                }
                
                newCameraPosition += shakeOffset;

                transform.position = newCameraPosition;
            }
        }

        private void HandleZoom()
        {
            if (GetCameraZoomFunc == null) return;
            float cameraZoom = GetCameraZoomFunc();

            float cameraZoomDifference = cameraZoom - myCamera.orthographicSize;
            float cameraZoomSpeed = 1f;

            myCamera.orthographicSize += cameraZoomDifference * cameraZoomSpeed * Time.deltaTime;

            if (cameraZoomDifference > 0)
            {
                if (myCamera.orthographicSize > cameraZoom)
                {
                    myCamera.orthographicSize = cameraZoom;
                }
            }
            else
            {
                if (myCamera.orthographicSize < cameraZoom)
                {
                    myCamera.orthographicSize = cameraZoom;
                }
            }

            Vector3 mousePosition = myCamera.ScreenToViewportPoint(Input.mousePosition);

            // Check if the mouse is near the very edge of the viewport (first threshold)
            if (mousePosition.x < 1 - firstMouseEdgeThreshold || mousePosition.x > firstMouseEdgeThreshold ||
                mousePosition.y < 1 - firstMouseEdgeThreshold || mousePosition.y > firstMouseEdgeThreshold)
            {
                SetCameraZoom(firstMaxZoomOut);
            }
            // Check if the mouse is a bit closer to the center, but still near the edge (second threshold)
            else if (mousePosition.x < 1 - secondMouseEdgeThreshold || mousePosition.x > secondMouseEdgeThreshold ||
                     mousePosition.y < 1 - secondMouseEdgeThreshold || mousePosition.y > secondMouseEdgeThreshold)
            {
                SetCameraZoom(secondMaxZoomOut);
            }
            else
            {
                SetCameraZoom(originalZoom);
            }
        }

        public void ApplyShake(Vector3 offset)
        {
            shakeOffset = offset;
        }

        private Vector3 AdjustPositionBasedOnMousePosition(Vector3 targetPosition)
        {
            float mousePosY = Input.mousePosition.y / Screen.height;

            if (mousePosY < 0.1f)
            {
                targetPosition.y += bottomScreenOffsetY;
            }
            else if (mousePosY > 0.9f)
            {
                targetPosition.y += topScreenOffsetY;
            }

            return targetPosition;
        }

    }

}