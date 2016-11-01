using UnityEngine;

namespace Assets.Scripts.General.UnityLayer
{
    public class MouseController : MonoBehaviour
    {

        public GameObject mouseSelectCursor;

        public float zoomSpeed = 1f;
        public float maxZoomOut = 2f;
        public float maxZoomIn = 0.25f;

        private Vector2 currentMousePosition;
        private Vector2 lastMousePosition;

        private Vector2 mouseDragStartPosition;

        private bool mouseDragging = false;

        public void Update()
        {
            currentMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            HandleCursor();
            HandleMouseDrag();
            HandleCameraMovement();
            HandleZooming();

            lastMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }

        private void HandleCursor()
        {
            if(mouseDragging)
            {
                // Don't handle position cursor if the mouse is dragging because of different behaviour.
                return;
            }

            var tileHovered = World.Current.GetTileAtWorldCoord(currentMousePosition);
            if(tileHovered != null)
            {
                mouseSelectCursor.SetActive(true);
                mouseSelectCursor.transform.position = new Vector2(tileHovered.X, tileHovered.Y);
            }
            else
            {
                mouseSelectCursor.SetActive(false);
            }
        }

        private void HandleMouseDrag()
        {
            // If Left mouse button is down start mouse drag..
            if(Input.GetMouseButtonDown(0))
            {
                mouseDragging = true;
                mouseDragStartPosition = currentMousePosition;
            }

            // Add 0.5f to compensate for Unity gameobjects pivot points being the center of the object.
            var startX = Mathf.FloorToInt(mouseDragStartPosition.x + 0.5f);
            var endX = Mathf.FloorToInt(currentMousePosition.x + 0.5f);
            var startY = Mathf.FloorToInt(mouseDragStartPosition.y + 0.5f);
            var endY = Mathf.FloorToInt(currentMousePosition.y + 0.5f);

            // Flip if dragging mouse left because endX would be less than startX and the for loop wouldnt loop
            if (endX < startX)
            {
                var tmp = endX;
                endX = startX;
                startX = tmp;
            }

            // Same for Y if the mouse is dragged down.
            if (endY < startY)
            {
                var tmp = endY;
                endY = startY;
                startY = tmp;
            }

            // If left mouse button is being held down display the drag area by resizing the select cursor over dragged area.
            if (Input.GetMouseButton(0))
            {
                // Because the world/tile map start at 0,0 we need to add 1 to the drag dimensions so that the cursor is the correct size.
                var dragWidth = endX - startX + 1f;
                var dragHeight = endY - startY + 1f;

                mouseSelectCursor.transform.localScale = new Vector2(dragWidth, dragHeight);

                var newCursorPosition = new Vector2(startX + dragWidth / 2 - 0.5f, startY + dragHeight / 2 - 0.5f);
                mouseSelectCursor.transform.position = newCursorPosition;
            }

            // End mouse drag.
            if(Input.GetMouseButtonUp(0))
            {
                mouseDragging = false;
                // Reset size to 1 tile.
                mouseSelectCursor.transform.localScale = Vector2.one;

                for(var x = startX; x <= endX; x++)
                {
                    for(var y = startY; y <= endY; y++)
                    {
                        var tile = World.Current.GetTileAt(x, y);

                        if(tile != null)
                        {
                            // TODO: Here can change the tiles that were selected during drag.
                        }
                    }
                }
            }
        }

        private void HandleCameraMovement()
        {
            // If the middle or right mouse buttons are being held down the move camera with the mouse.
            if(Input.GetMouseButton(1) || Input.GetMouseButton(2))
            {
                var difference = lastMousePosition - currentMousePosition;
                Camera.main.transform.Translate(difference);
            }
        }

        private void HandleZooming()
        {
            Camera.main.GetComponent<tk2dCamera>().CameraSettings.orthographicSize -=
                Camera.main.GetComponent<tk2dCamera>().CameraSettings.orthographicSize * Input.GetAxis("Mouse ScrollWheel") * zoomSpeed;

            Camera.main.GetComponent<tk2dCamera>().CameraSettings.orthographicSize =
                Mathf.Clamp(Camera.main.GetComponent<tk2dCamera>().CameraSettings.orthographicSize, maxZoomIn, maxZoomOut);
        }
    }
}
