class_name RPPlayerController
extends CharacterBody3D

@export var walk_speed := 5.5
@export var run_speed := 8.5
@export var acceleration := 18.0
@export var gravity := 22.0

var move_input := Vector2.ZERO
var is_running := false
var animation_player: AnimationPlayer
var animation_tree: AnimationTree
var body_visual: Node3D

func _ready() -> void:
	_build_humanoid()
	_build_animation_system()
	var shape := CollisionShape3D.new()
	var capsule := CapsuleShape3D.new()
	capsule.height = 1.8
	capsule.radius = 0.38
	shape.shape = capsule
	shape.position.y = 0.9
	add_child(shape)

func set_move_input(value: Vector2, running: bool = false) -> void:
	move_input = value
	is_running = running

func _physics_process(delta: float) -> void:
	var direction := Vector3(move_input.x, 0.0, move_input.y)
	if direction.length() > 1.0:
		direction = direction.normalized()
	var target_speed := run_speed if is_running else walk_speed
	var target := direction * target_speed
	velocity.x = move_toward(velocity.x, target.x, acceleration * delta)
	velocity.z = move_toward(velocity.z, target.z, acceleration * delta)
	if not is_on_floor():
		velocity.y -= gravity * delta
	else:
		velocity.y = -0.2
	move_and_slide()
	if direction.length() > 0.1:
		rotation.y = lerp_angle(rotation.y, atan2(direction.x, direction.z), delta * 9.0)
	_update_animation(direction.length())

func _build_humanoid() -> void:
	body_visual = Node3D.new()
	body_visual.name = "CitizenCharacter"
	add_child(body_visual)
	var asset_path := "res://assets/external/character/citizen.glb"
	if ResourceLoader.exists(asset_path):
		var scene := load(asset_path) as PackedScene
		if scene:
			var citizen := scene.instantiate()
			citizen.name = "QuaterniusCitizen"
			citizen.scale = Vector3.ONE
			citizen.position = Vector3(0, 0, 0)
			body_visual.add_child(citizen)
			return
	# Fallback only when the external asset is unavailable.
	_add_capsule("FallbackBody", Vector3(0, 1.02, 0), 0.9, 0.34, Color("#26394c"))
	_add_sphere("FallbackHead", Vector3(0, 1.82, 0), 0.28, Color("#c98f68"))

func _build_animation_system() -> void:
	animation_player = AnimationPlayer.new()
	add_child(animation_player)
	var library := AnimationLibrary.new()
	var idle := Animation.new()
	idle.length = 2.0
	idle.loop_mode = Animation.LOOP_LINEAR
	var walk := Animation.new()
	walk.length = 0.8
	walk.loop_mode = Animation.LOOP_LINEAR
	library.add_animation("idle", idle)
	library.add_animation("walk", walk)
	animation_player.add_animation_library("", library)
	animation_tree = AnimationTree.new()
	animation_tree.anim_player = NodePath("../AnimationPlayer")
	var state_machine := AnimationNodeStateMachine.new()
	var idle_node := AnimationNodeAnimation.new()
	idle_node.animation = "idle"
	var walk_node := AnimationNodeAnimation.new()
	walk_node.animation = "walk"
	state_machine.add_node("Idle", idle_node, Vector2(100, 100))
	state_machine.add_node("Walk", walk_node, Vector2(300, 100))
	state_machine.add_transition("Idle", "Walk", AnimationNodeStateMachineTransition.new())
	state_machine.add_transition("Walk", "Idle", AnimationNodeStateMachineTransition.new())
	animation_tree.tree_root = state_machine
	animation_tree.active = true
	add_child(animation_tree)

func _update_animation(amount: float) -> void:
	if animation_tree == null:
		return
	var playback = animation_tree.get("parameters/playback")
	if playback:
		playback.travel("Walk" if amount > 0.1 else "Idle")
