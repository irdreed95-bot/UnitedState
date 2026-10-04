class_name RPVehicleController
extends VehicleBody3D

@export var engine_power := 38.0
@export var brake_power := 28.0
@export var steering_limit := 0.48
var player_controlled := false

func _ready() -> void:
	mass = 1100.0
	_build_chassis()
	_build_wheels()

func set_controlled(value: bool) -> void:
	player_controlled = value
	if not value:
		engine_force = 0.0
		brake = brake_power

func drive(input: Vector2, delta: float) -> void:
	if not player_controlled:
		return
	var throttle := clamp(-input.y, -1.0, 1.0)
	var steer := clamp(input.x, -1.0, 1.0)
	engine_force = throttle * engine_power
	brake = brake_power if abs(throttle) < 0.05 else 0.0
	var steering_target := steer * steering_limit
	for child in get_children():
		if child is VehicleWheel3D and child.use_as_steering:
			child.steering = move_toward(child.steering, steering_target, delta * 3.5)

func _build_chassis() -> void:
	var collision := CollisionShape3D.new()
	var shape := BoxShape3D.new()
	shape.size = Vector3(1.9, 0.55, 4.0)
	collision.shape = shape
	collision.position.y = 0.62
	add_child(collision)
	var body := MeshInstance3D.new()
	var mesh := BoxMesh.new()
	mesh.size = Vector3(1.9, 0.55, 4.0)
	body.mesh = mesh
	body.position.y = 0.62
	var mat := StandardMaterial3D.new()
	mat.albedo_color = Color("#18212b")
	mat.metallic = 0.55
	mat.roughness = 0.28
	body.material_override = mat
	add_child(body)
	var glass := MeshInstance3D.new()
	var glass_mesh := BoxMesh.new()
	glass_mesh.size = Vector3(1.35, 0.32, 1.45)
	glass.mesh = glass_mesh
	glass.position = Vector3(0, 1.02, -0.05)
	var glass_mat := StandardMaterial3D.new()
	glass_mat.albedo_color = Color("#243b4b")
	glass_mat.metallic = 0.2
	glass_mat.roughness = 0.12
	glass.material_override = glass_mat
	add_child(glass)

func _build_wheels() -> void:
	var positions := [Vector3(-0.92, 0.45, -1.35), Vector3(0.92, 0.45, -1.35), Vector3(-0.92, 0.45, 1.35), Vector3(0.92, 0.45, 1.35)]
	for i in positions.size():
		var wheel := VehicleWheel3D.new()
		wheel.name = "Wheel_%d" % i
		wheel.position = positions[i]
		wheel.wheel_radius = 0.34
		wheel.wheel_rest_length = 0.16
		wheel.suspension_travel = 0.22
		wheel.suspension_stiffness = 6.0
		wheel.damping_compression = 0.9
		wheel.damping_relaxation = 0.92
		wheel.wheel_friction_slip = 1.15
		wheel.use_as_steering = i < 2
		wheel.use_as_traction = i >= 2
		add_child(wheel)
