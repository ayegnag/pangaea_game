using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UIElements;

namespace KOI
{
    public class PlayerInterface : MonoBehaviour
    {
         
        
		// public static event EventHandler<OnHoverEntityArgs> OnHoverEntity;
        private Camera cameraMain;
        private Canvas canvas;
        Camera cam { get { return canvas.worldCamera; } }
        RectTransform CanvasRect;
        // private RectTransform CanvasRect;

        private Vector3[] panelCorners = new Vector3[4];
        private RectTransform EntityInfoPanel;
        private RectTransform ToolTip;
        private Transform Selector;

        // // Calculate the screen bounds
        private float screenWidth;
        private float screenHeight;
        private Vector3 screenBottomLeft = new Vector3(0, 0, 0);
        private Vector3 screenTopRight;
        private TextMeshProUGUI EntityName;
        private TextMeshProUGUI EntityAge;
        private TextMeshProUGUI EntityFeeling;

        private GameObject ClickOutsideContainer;
        private RectTransform ToolTipEntityHearts;
        private HealthHearts HeartScript;
        private TextMeshProUGUI ToolTipEntityName;
        private RectTransform ToolTipBackground;
        private float HEART_WIDTH = 16f;

        private string SelectedEntityIdentity = null;
        public static GameObject SelectedEntityObject = null;

        private bool heartsCreated = false;

        private GameObject EntityObject;
        
        public static event EventHandler<OnFollowEntityArgs> OnCloseEntityInfoPanel;

        private void Awake() {
            // Setup Events
            SetupEvents();

            cameraMain = Camera.main;
            canvas = gameObject.transform.GetChild(0).GetComponent<Canvas>();
            CanvasRect = canvas.GetComponent<RectTransform>();

            // Event container for clicking outside UI
            ClickOutsideContainer = canvas.transform.GetChild(0).gameObject;

            // Entity Information Panel
            EntityInfoPanel = canvas.transform.GetChild(1).gameObject.GetComponent<RectTransform>();
            EntityName = EntityInfoPanel.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>();
            EntityAge = EntityInfoPanel.transform.GetChild(4).gameObject.GetComponent<TextMeshProUGUI>();
            EntityFeeling = EntityInfoPanel.transform.GetChild(8).gameObject.GetComponent<TextMeshProUGUI>();

            // Entity ToolTip
            ToolTip = canvas.transform.GetChild(2).gameObject.GetComponent<RectTransform>();
            ToolTipEntityName = ToolTip.transform.GetChild(2).gameObject.GetComponent<TextMeshProUGUI>();
            ToolTipEntityHearts = ToolTip.transform.GetChild(1).gameObject.GetComponent<RectTransform>();
            ToolTipBackground = ToolTip.transform.GetChild(0).gameObject.GetComponent<RectTransform>();
            HeartScript = ToolTipEntityHearts.GetComponent<HealthHearts>();

            // Entity Selector highlighter
            Selector =  GameObject.Find("Selector").gameObject.GetComponent<Transform>();

            // Disable the click container on load.
            ClickOutsideContainer.gameObject.SetActive(false);
            // // Keep the info panel hidden
            EntityInfoPanel.gameObject.SetActive(false);
            Selector.gameObject.SetActive(false);
            ToolTip.gameObject.SetActive(false);

            // Debug.Log(ToolTip);
            // Debug.Log(EntityInfoPanel);
            // Debug.Log(HeartScript);
        }

        void SetupEvents() {
            EntityInteraction.OnMouseDownEvent += ShowInfoPanel;
            EntityInteraction.OnMouseEnterEvent += ShowToolTip;
            EntityInteraction.OnMouseExitEvent += HideToolTip;
            OuterClickContainer.OnMouseDownOutsideEvent += CloseInfoPanel;
        }

        void Start()
        {
            screenWidth = Screen.width;
            screenHeight = Screen.height;
            screenTopRight = new Vector3(screenWidth, screenHeight, 0);
        }

        private void Update() {
            if (SelectedEntityIdentity != null) {
                Selector.transform.position = SelectedEntityObject.transform.position;
            }
        }

        // void FixedUpdate()
        // {
        //     // ToolTip.anchoredPosition = Input.mousePosition / canvas.scaleFactor;    // This #@$king logic took ages to find!
        //     Vector2 mousePos =  Input.mousePosition;
        //     Vector3 worldPos = cameraMain.ScreenToWorldPoint(mousePos);
        //     Vector2 pos = new Vector2(worldPos.x, worldPos.y);
        //     Vector3 anchoredPos = Vector2.zero;
        //     // Check what's under the mouse pointer using RayCasting and Colliders of the Entities
        //     RaycastHit2D hit2D = Physics2D.Raycast(pos, Vector2.zero);
        //     if (hit2D.collider != null) {
        //     //     Debug.Log ("Name = " + hit2D.collider.name);
        //     //     Debug.Log ("Tag = " + hit2D.collider.tag);
        //     //     Debug.Log ("Hit Point = " + hit2D.point);
        //     //     Debug.Log ("Object position = " + hit2D.collider.gameObject.transform.position);
        //     //     Debug.Log ("--------------");
        //         EntityId entity = Utils.GetEntityTypeIdFromName(hit2D.collider.name);
        //         DogAttributes attrs = GameStateManager.Instance.EntitySystem.GetDogAttributes(entity.Id);
        //         if (Input.GetMouseButtonDown(0))
        //         {
        //             // Select the Entity, show a selector around it.
        //             SelectedEntityIdentity = hit2D.collider.name;
        //             Selector.transform.position = hit2D.collider.gameObject.transform.position;
        //             Selector.gameObject.SetActive(true);
        //             EntityObject = hit2D.collider.gameObject;

        //             // Show the Entity Info Panel

        //         } else {
        //             ToolTipHandler(attrs, mousePos);
        //         }
        //     } else {
        //         if (Input.GetMouseButtonDown(0) && SelectedEntityIdentity != null)
        //         {
        //             Selector.gameObject.SetActive(false);
        //             SelectedEntityIdentity = null;
        //             EntityObject = null;
        //         }
        //         // Hide the info panel if mouse not hovering 
        //         ToolTipReset();
        //     }
        // }

        private void ShowInfoPanel(object sender, OnDogMouseEventArgs eventArgs) {
            // Debug.Log("Received Event for Panel");
            // Select the Entity, show a selector around it.
            SelectedEntityIdentity = eventArgs.Name;
            SelectedEntityObject = GameObject.Find(SelectedEntityIdentity);
            Selector.transform.position = SelectedEntityObject.transform.position;

            // Selector.transform.position = hit2D.collider.gameObject.transform.position;
            Selector.gameObject.SetActive(true);

            // Show the Entity Info Panel
            EntityId entity = Utils.GetEntityTypeIdFromName(SelectedEntityIdentity);
            DogAttributes attrs = GameStateManager.Instance.EntitySystem.GetDogAttributes(entity.Id);
            EntityName.text = attrs.Name.ToString();
            EntityAge.text = "10"; // [TODO:] Add age attribute to the Entities with some offset on load.
            EntityFeeling.text = attrs.Feeling.ToString();
            // Hide tooltip that appears.
            ToolTipReset();
            EntityInfoPanel.gameObject.SetActive(true);
            ClickOutsideContainer.gameObject.SetActive(true);
        }

        private void CloseInfoPanel(object sender, EventArgs eventArgs) {
            if (SelectedEntityIdentity != null) {
                EntityInfoPanel.gameObject.SetActive(false);
                ClickOutsideContainer.gameObject.SetActive(false);
                Selector.gameObject.SetActive(false);
                SelectedEntityIdentity = null;
                SelectedEntityObject = null;
                OnCloseEntityInfoPanel?.Invoke(this, null);
            }
        }

        private void ShowToolTip(object sender, OnDogMouseEventArgs eventArgs) {
            // Debug.Log("Received Show Event for ToolTip");
            // [TODO:] Show tooltip after a delay.
            if (SelectedEntityIdentity == null) {
                EntityId entity = Utils.GetEntityTypeIdFromName(eventArgs.Name);
                DogAttributes attrs = GameStateManager.Instance.EntitySystem.GetDogAttributes(entity.Id);
                ToolTipHandler(attrs, Input.mousePosition);
            }
        }

        private void HideToolTip(object sender, OnDogMouseEventArgs eventArgs) {
            ToolTipReset();
        }

        void ToolTipHandler(DogAttributes attrs, Vector2 mousePos) {
            // [TODO:] See if optimizations can be done here by caching the values of the Entity
            ToolTip.gameObject.SetActive(true);
            ToolTipEntityName.text = attrs.Name.ToString();
            if (!heartsCreated) {
                ToolTip.sizeDelta = new Vector2(ToolTip.sizeDelta.x + HEART_WIDTH * (float)attrs.Health, ToolTip.sizeDelta.y);
                ToolTipBackground.sizeDelta = new Vector2(ToolTip.sizeDelta.x, ToolTip.sizeDelta.y);
                HeartScript.ShowHearts(attrs.MaxHealth, attrs.Health);
                heartsCreated = true;
            }                
            // RectTransformUtility.ScreenPointToWorldPointInRectangle(canvas.GetComponent<RectTransform>(), Input.mousePosition, cam, out anchoredPos);
            // Now that ToolTip has been initialized, move it to cursor position.
            ToolTip.anchoredPosition = KeepWithinScreen(mousePos);
        }

        void ToolTipReset() {
            ToolTip.sizeDelta = new Vector2(108f, 32f);
            ToolTip.gameObject.SetActive(false);      
            heartsCreated = false;
        }

        // private void OnMouseDown() {
        //     if (SelectedEntityIdentity != null) {
        //         InfoPanelReset();
        //     }
        // }

        Vector2 KeepWithinScreen(Vector2 newPos) {
            float rectWidth = ToolTip.rect.width * ToolTip.pivot.x;
            // Since the tooltip is on the left of the cursor, check if it has crossed the canvas
            if (newPos.x - rectWidth < 0) {
                newPos.x = rectWidth;
                // if tooltip is too close to the top, bring it below the cursor
                if (newPos.y > CanvasRect.rect.height - ToolTip.rect.height * 2) {
                    newPos.y -= ToolTip.rect.height;
                } else {
                    // else move it over the cursor
                    newPos.y += ToolTip.rect.height;
                }
            }
            return newPos / canvas.scaleFactor;
        }

        // void LateUpdate()
        // {
        //     Vector3 Worldpos = cameraMain.ScreenToWorldPoint(Input.mousePosition);
        //     Vector2 pos = new Vector2(Worldpos.x, Worldpos.y);
        //     RaycastHit2D hit2D = Physics2D.Raycast(pos, Vector2.zero);
        //     if (hit2D.collider != null) {
        //     //     Debug.Log ("Name = " + hit2D.collider.name);
        //     //     Debug.Log ("Tag = " + hit2D.collider.tag);
        //     //     Debug.Log ("Hit Point = " + hit2D.point);
        //     //     Debug.Log ("Object position = " + hit2D.collider.gameObject.transform.position);
        //     //     Debug.Log ("--------------");
        //         moveEntityInfoPanel(cameraMain.WorldToScreenPoint(hit2D.collider.gameObject.transform.position));
        //     } else {
        //         EntityInfoPanel.gameObject.SetActive(false);
        //     }
        // }
        // public void OnUpdateDisplayPanel()
        // {
          
        //     Vector3 Worldpos = cameraMain.ScreenToWorldPoint(Input.mousePosition);
        //     Vector2 pos = new Vector2(Worldpos.x, Worldpos.y);
        //     // Check what's under the mouse pointer using RayCasting and Colliders of the Entities
        //     RaycastHit2D hit2D = Physics2D.Raycast(pos, Vector2.zero);
        //     if (hit2D.collider != null) {
        //     //     Debug.Log ("Name = " + hit2D.collider.name);
        //     //     Debug.Log ("Tag = " + hit2D.collider.tag);
        //     //     Debug.Log ("Hit Point = " + hit2D.point);
        //     //     Debug.Log ("Object position = " + hit2D.collider.gameObject.transform.position);
        //     //     Debug.Log ("--------------");
        //         EntityName.text = hit2D.collider.name;
        //         moveEntityInfoPanel(hit2D.collider.gameObject.transform.position);
        //         EntityInfoPanel.gameObject.SetActive(true);
        //         // Trigger event to the Entity System to get
        //         // EntityId entityInfo = Utils.GetEntityTypeIdFromName(hit2D.collider.name);
        //         // OnHoverEntity?.Invoke(this, new OnHoverEntityArgs(entityInfo.Type, entityInfo.Id));
        //     } else {
        //         // Hide the info panel if mouse not hovering 
        //         EntityInfoPanel.gameObject.SetActive(false);
        //     }
        // }

        // void moveEntityInfoPanel(Vector3 entityWorldPosition)
        // {
        //     // Check if any of the panel corners are outside the screen bounds
        //     ToolTip.GetWorldCorners(panelCorners);
        //     bool panelOutsideScreen = false;
        //     for (int i = 0; i < 4; i++)
        //     {
        //         if (!RectTransformUtility.RectangleContainsScreenPoint(ToolTip, panelCorners[i]))
        //         {
        //             panelOutsideScreen = true;
        //             break;
        //         }
        //     }

        //     // If the panel is outside the screen bounds, move it back inside
        //     if (panelOutsideScreen)
        //     {
        //         Vector3 panelPosition = ToolTip.position;

        //         if (panelCorners[0].x < screenBottomLeft.x)
        //         {   
        //             Debug.Log(panelCorners[0].x + " " + screenBottomLeft.x);
        //             panelPosition.x += screenBottomLeft.x - panelCorners[0].x;
        //         }
        //         else if (panelCorners[2].x > screenTopRight.x)
        //         {
        //             Debug.Log(panelCorners[2].x + " - " + screenTopRight.x);
        //             panelPosition.x -= panelCorners[2].x - screenTopRight.x;
        //         }

        //         if (panelCorners[0].y < screenBottomLeft.y)
        //         {
        //             Debug.Log(panelCorners[0].y + " " + screenBottomLeft.y);
        //             panelPosition.y += screenBottomLeft.y - panelCorners[0].y;
        //         }
        //         else if (panelCorners[2].y > screenTopRight.y)
        //         {
        //             Debug.Log(panelCorners[0].y + " - " + screenTopRight.y);
        //             panelPosition.y -= panelCorners[2].y - screenTopRight.y;
        //         }

        //         ToolTip.position = panelPosition;
        //     } else {
        //         // Vector2 panelPos;
        //         Vector2 panelCamPos = cameraMain.WorldToScreenPoint(entityWorldPosition);
        //         // // RectTransformUtility.ScreenPointToLocalPointInRectangle(EntityInfoPanel, entityPosition, cameraMain, out anchoredPos);

        //         // panelPos = new Vector2(panelCamPos.x - ToolTip.rect.width / 2 - Screen.width / 2 - Gap,
        //         // panelCamPos.y - Screen.height / 2);
        //         // EntityInfoPanel.anchoredPosition = KeepFullyOnScreen(EntityInfoPanel.gameObject, panelPos);
        //         // EntityInfoPanel.transform.localPosition = new Vector2(0, 0);
                
        //         // Get the corners of the panel
        //         // ToolTip.anchoredPosition = panelPos;
        //         ToolTip.anchoredPosition = entityWorldPosition / CanvasRect.localScale.x;
        //     }
        // }

        private void OnDisable() {
            EntityInteraction.OnMouseDownEvent -= ShowInfoPanel;
            EntityInteraction.OnMouseEnterEvent -= ShowToolTip;
            EntityInteraction.OnMouseExitEvent -= HideToolTip;
        }
    }
}