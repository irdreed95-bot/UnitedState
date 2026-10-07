using UnityEngine;
namespace CorruptStateRP{
public sealed class ThirdPersonCameraFollow:MonoBehaviour{
 [SerializeField] Transform target;
 [SerializeField] Vector3 offset=new Vector3(0f,5.5f,9f);
 [SerializeField] float smooth=10f;
 public void SetTarget(Transform t){target=t;}
 void LateUpdate(){if(target==null)return;var desired=target.position+target.rotation*offset;transform.position=Vector3.Lerp(transform.position,desired,1f-Mathf.Exp(-smooth*Time.deltaTime));transform.LookAt(target.position+Vector3.up*1.15f);}
}}
