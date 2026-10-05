class_name RPVehicleController
extends VehicleBody3D

@export var engine_power: float = 38.0
@export var brake_power: float = 28.0
@export var steering_limit: float = 0.48
var player_controlled: bool = false
var engine_on: bool = false
var headlights_on: bool = false
var headlight_left: OmniLight3D
var headlight_right: OmniLight3D

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
	if not player_controlled or not engine_on:
		return
	var throttle: float = clampf(-input.y, -1.0, 1.0)
	var steer: float = clampf(input.x, -1.0, 1.0)
	engine_force = throttle * engine_power
	brake = brake_power if absf(throttle) < 0.05 else 0.0
	var steering_target: float = steer * steering_limit
	for child in get_children():
		if child is VehicleWheel3D and child.use_as_steering:
			child.steering = move_toward(child.steering, steering_target, delta * 3.5)

func toggle_engine() -> void:
	engine_on = not engine_on
	if not engine_on:
		engine_force = 0.0
		brake = brake_power

func toggle_lights() -> void:
	headlights_on = not headlights_on
	if headlight_left:
		headlight_left.visible = headlights_on
	if headlight_right:
		headlight_right.visible = headlights_on

func _build_chassis() -> void:
	var collision := CollisionShape3D.new()
	var shape := BoxShape3D.new()
	shape.size = Vector3(1.9, 0.62, 4.0)
	collision.shape = shape
	collision.position.y = 0.62
	add_child(collision)

	var asset_path := "res://assets/external/vehicles/sedan.glb"
	if ResourceLoader.exists(asset_path):
		var scene := load(asset_path) as PackedScene
		if scene:
			var car := scene.instantiate()
			car.name = "KenneySedan"
			car.position.y = 0.20
			car.scale = Vector3.ONE
			add_child(car)
			headlight_left = _add_light(Vector3(-0.58, 0.72, -2.02))
			headlight_right = _add_light(Vector3(0.58, 0.72, -2.02))
			return

	# Fallback only when the external vehicle asset is unavailable.
	var body := MeshInstance3D.new()
	var mesh := BoxMesh.new()
	mesh.size = Vector3(1.9, 0.58, 4.0)
	body.mesh = mesh
	body.position.y = 0.62
	var mat := StandardMaterial3D.new()
	mat.albedo_color = Color("#8c2632")
	mat.metallic = 0.58
	mat.roughness = 0.25
	body.material_override = mat
	add_child(body)
	headlight_left = _add_light(Vector3(-0.58, 0.72, -2.02))
	headlight_right = _add_light(Vector3(0.58, 0.72, -2.02))

func _add_light(p: Vector3) -> OmniLight3D:
	var light := OmniLight3D.new()
	light.light_color = Color("#fff0c5")
	light.light_energy = 1.8
	light.omni_range = 8.0
	light.position = p
	light.visible = false
	add_child(light)
	return light

func _build_wheels() -> void:
	var positions: Array[Vector3] = [Vector3(-0.92, 0.45, -1.35), Vector3(0.92, 0.45, -1.35), Vector3(-0.92, 0.45, 1.35), Vector3(0.92, 0.45, 1.35)]
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
