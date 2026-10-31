using UnityEngine;

public class PlayerMovementZ : MonoBehaviour
{
    public float speed = 15.9f; 
    public Transform cameraTransform; 
    private CharacterController controller;
    private Vector3 velocity;
    private float gravity = -20f;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        
        // البحث عن الكاميرا بأكثر من طريقة لضمان عدم حدوث NullReferenceException
        if (cameraTransform == null)
        {
            // 1. البحث عن الكاميرا التي تحمل السكربت الجديد CameraLookZ
            CameraLookZ camZ = Object.FindAnyObjectByType<CameraLookZ>();
            if (camZ != null)
            {
                cameraTransform = camZ.transform;
                Debug.Log("تم العثور على الكاميرا عبر سكربت CameraLookZ");
            }
            // 2. إذا لم يجدها، يبحث عن الكاميرا الأساسية في المشهد
            else if (Camera.main != null)
            {
                cameraTransform = Camera.main.transform;
            }
        }
    }

    void Update()
    {
        // حماية: إذا لم يجد الكاميرا بعد كل محاولات البحث، يتوقف لتجنب الأخطاء الحمراء
        if (cameraTransform == null) return;

        float x = Input.GetAxis("Horizontal"); 
        float z = Input.GetAxis("Vertical");

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        Vector3 move = forward * z + right * x;

        if (move.magnitude > 0.1f)
        {
            controller.Move(move * speed * Time.deltaTime);
        }

        if (controller.isGrounded && velocity.y < 0) 
        {
            velocity.y = -2f;
        }
        
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
}