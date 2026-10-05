class_name RPTrafficManager
extends Node3D

var lanes: Array[Vector3] = []
var cars: Array[VehicleBody3D] = []
var spawn_timer := 0.0
var max_cars := 10

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
    if spawn_timer > 3.0 and cars.size() < max_cars:
        spawn_timer = 0.0
        _spawn_car()
    for car in cars:
        if not is_instance_valid(car):
            continue
        var speed := float(car.get_meta("traffic_speed", 5.0))
        car.position += -car.transform.basis.z * speed * delta
        if abs(car.position.x) > 330.0 or abs(car.position.z) > 330.0:
            car.queue_free()
    cars = cars.filter(func(c): return is_instance_valid(c))

func _spawn_car() -> void:
    if lanes.is_empty():
        return
    var car := VehicleBody3D.new()
    car.mass = 950.0
    car.position = lanes[randi() % lanes.size()]
    var asset_path := "res://assets/external/vehicles/sedan.glb"
    if ResourceLoader.exists(asset_path):
        var scene := load(asset_path) as PackedScene
        if scene:
            var visual := scene.instantiate()
            visual.name = "TrafficSedan"
            visual.position.y = 0.20
            car.add_child(visual)
        else:
            _add_fallback_car_body(car)
    else:
        _add_fallback_car_body(car)
    var collision := CollisionShape3D.new()
    var shape := BoxShape3D.new()
    shape.size = Vector3(1.7, 0.55, 3.6)
    collision.shape = shape
    collision.position.y = 0.75
    car.add_child(collision)
    car.set_meta("traffic_speed", randf_range(4.0, 8.0))
    add_child(car)
    cars.append(car)

func _add_fallback_car_body(car: VehicleBody3D) -> void:
    var body := MeshInstance3D.new()
    var mesh := BoxMesh.new()
    mesh.size = Vector3(1.7, 0.55, 3.6)
    body.mesh = mesh
    var mat := StandardMaterial3D.new()
    mat.albedo_color = Color.from_hsv(randf(), 0.55, 0.75)
    mat.roughness = 0.55
    body.material_override = mat
    body.position.y = 0.75
    car.add_child(body)
