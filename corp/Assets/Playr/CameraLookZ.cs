using UnityEngine;
using UnityEngine.EventSystems;

// قمت بتغيير اسم الكلاس هنا ليتطابق مع طلبك (تأكد أن اسم الملف هو CameraLookZ.cs)
public class CameraLookZ : MonoBehaviour 
{
    public Transform playerBody; 
    
    [Header("Sensitivity Settings")]
    public float mouseSensitivity = 100f;
    public float mobileSensitivity = 0.15f;

    private float xRotation = 0f; 
    private float yRotation = 0f; 

    void Start()
    {
        Vector3 rot = transform.localRotation.eulerAngles;
        yRotation = rot.y;
        xRotation = rot.x;
    }

    void Update()
    {
        float mouseX = 0;
        float mouseY = 0;

        // التحقق من وجود نظام الأحداث لتجنب الـ NullReferenceException
        bool isOverUI = false;
        if (EventSystem.current != null)
        {
            isOverUI = EventSystem.current.IsPointerOverGameObject();
        }

        #if UNITY_EDITOR || UNITY_STANDALONE
        if (Input.GetMouseButton(0) && !isOverUI)
        {
            mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
            mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        }
        #endif

        if (Input.touchCount > 0)
        {
            foreach (Touch t in Input.touches)
            {
                // حماية إضافية للموبايل
                bool isOverUITouch = false;
                if (EventSystem.current != null)
                {
                    isOverUITouch = EventSystem.current.IsPointerOverGameObject(t.fingerId);
                }

                if (t.position.x > Screen.width / 2f && !isOverUITouch)
                {
                    if (t.phase == TouchPhase.Moved)
                    {
                        mouseX = t.deltaPosition.x * mobileSensitivity;
                        mouseY = t.deltaPosition.y * mobileSensitivity;
                    }
                }
            }
        }

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -80f, 80f); 
        yRotation += mouseX; 

        transform.localRotation = Quaternion.Euler(xRotation, yRotation, 0f);

        if (playerBody != null)
        {
            playerBody.Rotate(Vector3.up * mouseX);
        }
    }
}