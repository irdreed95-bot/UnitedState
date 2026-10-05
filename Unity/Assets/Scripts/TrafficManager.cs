using System.Collections.Generic;using UnityEngine;
namespace CorruptStateRP {
public sealed class TrafficManager:MonoBehaviour{
 public int maxCars=18;readonly List<GameObject> cars=new();readonly string[] assets={"External/vehicles/sedan","External/vehicles/suv","External/vehicles/taxi","External/vehicles/police"};float timer;
 void Update(){timer+=Time.deltaTime;if(timer>1.8f&&cars.Count<maxCars){timer=0;Spawn();}for(int i=cars.Count-1;i>=0;i--){if(!cars[i]){cars.RemoveAt(i);continue;}cars[i].transform.Translate(Vector3.forward*7*Time.deltaTime,Space.Self);if(Mathf.Abs(cars[i].transform.position.x)>330||Mathf.Abs(cars[i].transform.position.z)>330){Destroy(cars[i]);cars.RemoveAt(i);}}}
 void Spawn(){float lane=new[]{-120f,-80f,-40f,0f,40f,80f,120f}[Random.Range(0,7)];bool h=Random.value>.5f;var p=h?new Vector3(-270,.3f,lane):new Vector3(lane,.3f,-270);var prefab=Resources.Load<GameObject>(assets[Random.Range(0,assets.Length)]);var go=prefab?Instantiate(prefab,p,Quaternion.identity):GameObject.CreatePrimitive(PrimitiveType.Cube);go.name="TrafficVehicle";if(!prefab)go.transform.localScale=new(1.8f,.7f,3.6f);if(h)go.transform.rotation=Quaternion.Euler(0,90,0);cars.Add(go);}
}}
