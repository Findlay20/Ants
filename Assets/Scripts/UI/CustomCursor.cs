using UnityEngine;
using UnityEngine.InputSystem;

public class CustomCursor : MonoBehaviour
{

    private RectTransform rectTransform;
    public InputActionAsset inputActions;
    private InputActionMap inputActionMap;
    public InputAction cursorMove;

    private Vector2 cursorPositionOffset;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = gameObject.GetComponent<RectTransform>();
        inputActionMap = inputActions.FindActionMap("TopView").Clone();
        cursorMove = inputActionMap.FindAction("cursorMove");
    }

    // Update is called once per frame
    void Update()
    {
        if (cursorMove.IsPressed()) {
            cursorPositionOffset = cursorMove.ReadValue<Vector2>();
            Debug.Log(cursorPositionOffset);

            Vector3 newPos = new Vector3(cursorPositionOffset.x, cursorPositionOffset.y, 0);
            //rectTransform.SetPositionAndRotation(newPos, transform.rotation);
            rectTransform.rect.Set(cursorPositionOffset.x, cursorPositionOffset.y, rectTransform.rect.width, rectTransform.rect.height);     
        }

    }




}
