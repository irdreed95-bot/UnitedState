class_name RPTrafficManager
extends Node3D

var lanes: Array[Vector3] = []
var cars: Array[VehicleBody3D] = []
var spawn_timer := 0.0
var max_cars := 18
var vehicle_assets := [
	"res://assets/external/vehicles/sedan.glb",
	"res://assets/external/vehicles/suv.glb",
	"res://assets/external/vehicles/taxi.glb",
	"res://assets/external/vehicles/sedan_sports.glb",
	"res://assets/external/vehicles/van.glb",
	"res://assets/external/vehicles/truck.glb",
	"res://assets/external/vehicles/police.glb",
	"res://assets/external/vehicles/ambulance.glb"
]

func setup(world_size: float) -> void:
	lanes.clear()
	var half := world_size * 0.45
	for v in [-120.0, -80.0, -40.0, 0.0, 40.0, 80.0, 120.0]:
		if abs(v) <= half:
			lanes.append(Vector3(v, 0.4, -half))
			lanes.append(Vector3(v, 0.4, half))
			lanes.append(Vector3(-half, 0.4, v))
			lanes.append(Vector3(half, 0.4, v))

func _process(delta: float) -> void:
	spawn_timer += delta
	if spawn_timer > 1.8 and cars.size() < max_cars:
		spawn_timer = 0.0
		_spawn_car()
	for car in cars:
		if not is_instance_valid(car):
			continue
		var speed := float(car.get_meta("traffic_speed", 7.0))
		car.position += -car.transform.basis.z * speed * delta
		if abs(car.position.x) > 330.0 or abs(car.position.z) > 330.0:
			car.queue_free()
	cars = cars.filter(func(c): return is_instance_valid(c))

func _spawn_car() -> void:
	if lanes.is_empty():
		return
	var lane := lanes[randi() % lanes.size()]
	var car := VehicleBody3D.new()
	car.name = "TrafficVehicle"
	car.mass = 950.0
	car.position = lane
	var visual_path := vehicle_assets[randi() % vehicle_assets.size()]
	var scene := _load_scene(visual_path)
	if scene:
		var visual := scene.instantiate()
		visual.name = "RealTrafficVehicle"
		visual.position.y = 0.2
		car.add_child(visual)
	else:
		_add_fallback_car_body(car)

	var collision := CollisionShape3D.new()
	collision.shape = _box_shape(Vector3(1.9, 0.9, 4.0))
	collision.position.y = 0.75
	car.add_child(collision)
	car.set_meta("traffic_speed", randf_range(5.0, 10.0))
	add_child(car)
	cars.append(car)

func _load_scene(path: String) -> PackedScene:
	if not ResourceLoader.exists(path):
		return null
	return load(path) as PackedScene

func _box_shape(size: Vector3) -> BoxShape3D:
	var shape := BoxShape3D.new()
	shape.size = size
	return shape

func _add_fallback_car_body(car: VehicleBody3D) -> void:
	var body := MeshInstance3D.new()
	var mesh := BoxMesh.new()
	mesh.size = Vector3(1.7, 0.55, 3.6)
	body.mesh = mesh
	body.position.y = 0.75
	car.add_child(body)
