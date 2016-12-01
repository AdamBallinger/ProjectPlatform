using Assets.Scripts.General.UnityLayer.UI.LevelEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts.General.UnityLayer
{
    public enum SelectionMode
    {
        BuildMode,
        ClearMode,
        PlayerSpawnSet,
        CoinPickup
    }

    public class MouseController : MonoBehaviour
    {
        public WorldController worldController;
        public LevelEditorUIController editorUIController;

        public GameObject mouseSelectCursor;
        public GameObject playerSpawnObject;

        // The behaviour of the mouse when selecting/dragging tiles.
        public SelectionMode SelectMode { get; set; }

        // If build mode what type of tile should be built.
        public TileType TileBuildType { get; set; }

        public float zoomSpeed = 1f;
        public float maxZoomOut = 2f;
        public float maxZoomIn = 0.25f;

        private Vector2 currentMousePosition;
        private Vector2 lastMousePosition;

        private Vector2 mouseDragStartPosition;

        private bool mouseDragging = false;

        public void Start()
        {
            SelectMode = SelectionMode.ClearMode;
        }

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
            if (mouseDragging)
            {
                // Don't handle position cursor if the mouse is dragging because of different behaviour.
                return;
            }

            var tileHovered = World.Current.GetTileAtWorldCoord(currentMousePosition);
            if (tileHovered != null)
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
            // Check if the mouse is currently hovering over a EventSystem game object (UI object), to prevent tiles being changed when clicking UI buttons etc.
            var mouseOnUI = EventSystem.current.IsPointerOverGameObject();

            // If Left mouse button is down start mouse drag..
            if (Input.GetMouseButtonDown(0) && !mouseOnUI)
            {
                mouseDragging = true;
                mouseDragStartPosition = currentMousePosition;

            }

            if (mouseDragging)
            {
                // Add 0.5f to compensate for Unity gameobjects pivot points being the center of the object.
                var startX = Mathf.FloorToInt(mouseDragStartPosition.x + 0.5f);
                var endX = Mathf.FloorToInt(currentMousePosition.x + 0.5f);
                var startY = Mathf.FloorToInt(mouseDragStartPosition.y + 0.5f);
                var endY = Mathf.FloorToInt(currentMousePosition.y + 0.5f);

                // If the mode is to change player spawn, then position the spawn at the start of the mouse drag (if dragged) and braek out the loop.
                if (SelectMode == SelectionMode.PlayerSpawnSet)
                {
                    if(startX >= 0 || startX < World.Current.Width || startY >= 0 || startY < World.Current.Height)
                    {
                        if(World.Current.GetTileAt(startX, startY) != null && World.Current.GetTileAt(startX, startY).Type != TileType.Platform)
                        {
                            playerSpawnObject.transform.position = new Vector2(startX, startY);
                        }
                    }
                }

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
                if (Input.GetMouseButton(0) && !mouseOnUI)
                {
                    // Because the world/tile map start at 0,0 we need to add 1 to the drag dimensions so that the cursor is the correct size.
                    var dragWidth = endX - startX + 1f;
                    var dragHeight = endY - startY + 1f;

                    mouseSelectCursor.transform.localScale = new Vector2(dragWidth, dragHeight);

                    var newCursorPosition = new Vector2(startX + dragWidth / 2 - 0.5f, startY + dragHeight / 2 - 0.5f);
                    mouseSelectCursor.transform.position = newCursorPosition;
                }

                // End mouse drag.
                if (Input.GetMouseButtonUp(0) && !mouseOnUI)
                {
                    mouseDragging = false;
                    // Reset size to 1 tile.
                    mouseSelectCursor.transform.localScale = Vector2.one;


                    for (var x = startX; x <= endX; x++)
                    {
                        for (var y = startY; y <= endY; y++)
                        {
                            var tile = World.Current.GetTileAt(x, y);


                            if (tile != null)
                            {
                                ProcessTileSelected(tile);
                            }
                        }
                    }

                    World.Current.OnWorldModifyFinishCallback();
                }
            }
        }

        private void HandleCameraMovement()
        {
            // If the middle or right mouse buttons are being held down the move camera with the mouse.
            if (Input.GetMouseButton(1) || Input.GetMouseButton(2))
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

        /// <summary>
        /// When a tile is selected / dragged with the mouse handle what to do with it here.
        /// </summary>
        /// <param name="_tile"></param>
        private void ProcessTileSelected(Tile _tile)
        {
            switch (SelectMode)
            {
                case SelectionMode.BuildMode:
                    _tile.Type = TileBuildType;
                    worldController.SetTileMaterial(_tile.X, _tile.Y);
                    break;
                case SelectionMode.ClearMode:
                    _tile.Type = TileType.Empty;
                    break;
                case SelectionMode.CoinPickup:
                    worldController.AddCoinPickup(new Vector2(_tile.X, _tile.Y));
                    break;
            }
        }
    }
}
