class_name RPWorldBuilder
extends Node3D

const CITY_SIZE := 600.0
const CELL := 10.0
const ROAD_SPACING := 40.0
var grid: GridMap
var rng := RandomNumberGenerator.new()

func build() -> void:
	rng.seed = 20261005
	_build_environment()
	_build_grid_city()
	_build_real_city()
	_build_landmarks()
	_build_city_life()
	build_street_details()

func _build_environment() -> void:
	var env_node := WorldEnvironment.new()
	env_node.name = "CityEnvironment"
	var env := Environment.new()
	env.background_mode = Environment.BG_COLOR
	env.background_color = Color("#7890a3")
	env.background_energy_multiplier = 1.0
	env.ambient_light_source = Environment.AMBIENT_SOURCE_COLOR
	env.ambient_light_color = Color("#d7e1e8")
	env.ambient_light_energy = 1.1
	env.tonemap_mode = Environment.TONE_MAPPER_FILMIC
	env.glow_enabled = false
	env.ssao_enabled = false
	env.sdfgi_enabled = false
	env.fog_enabled = true
	env.fog_light_color = Color("#8da1b0")
	env.fog_light_energy = 0.35
	env.fog_density = 0.004
	env.fog_sky_affect = 0.35
	env_node.environment = env
	add_child(env_node)

	var sun := DirectionalLight3D.new()
	sun.name = "Sun"
	sun.rotation_degrees = Vector3(-52, -28, 0)
	sun.light_energy = 1.7
	sun.shadow_enabled = true
	sun.directional_shadow_max_distance = 280.0
	add_child(sun)

func _build_grid_city() -> void:
	grid = GridMap.new()
	grid.name = "CityRoadBase"
	grid.cell_size = Vector3(CELL, CELL, CELL)
	grid.cell_octant_size = 8
	grid.collision_layer = 1
	grid.collision_mask = 1
	var library := MeshLibrary.new()
	library.create_item(0)
	library.set_item_name(0, "RoadBase")
	library.set_item_mesh(0, _box_mesh(Vector3(CELL, 0.12, CELL), Color("#202830")))
	library.set_item_shapes(0, [_shape_box(Vector3(CELL, 0.12, CELL)), Transform3D.IDENTITY])
	grid.mesh_library = library
	add_child(grid)

	for x in range(-30, 31):
		for z in range(-30, 31):
			var road := abs(x) <= 1 or abs(z) <= 1 or abs(x) % 4 == 0 or abs(z) % 4 == 0
			if road:
				grid.set_cell_item(Vector3i(x, 0, z), 0)
				if (x % 4 == 0 and z % 4 == 0) or (abs(x) <= 1 and z % 4 == 0) or (abs(z) <= 1 and x % 4 == 0):
					_add_real_road(Vector3(x * CELL, 0.08, z * CELL), x % 4 == 0)

	var ground := StaticBody3D.new()
	ground.name = "CityGround"
	var mesh := MeshInstance3D.new()
	var plane := BoxMesh.new()
	plane.size = Vector3(CITY_SIZE, 1.0, CITY_SIZE)
	mesh.mesh = plane
	var mat := StandardMaterial3D.new()
	mat.albedo_color = Color("#35414c")
	mat.roughness = 0.96
	mesh.material_override = mat
	mesh.position.y = -0.55
	ground.add_child(mesh)
	var col := CollisionShape3D.new()
	col.shape = _shape_box(Vector3(CITY_SIZE, 1.0, CITY_SIZE))
	col.position.y = -0.55
	ground.add_child(col)
	add_child(ground)

func _add_real_road(p: Vector3, vertical: bool) -> void:
	var scene := _load_scene("res://assets/external/roads/road-straight.glb")
	if scene == null:
		return
	var road := scene.instantiate()
	road.position = p
	road.scale = Vector3.ONE * 1.0
	if vertical:
		road.rotation_degrees.y = 90.0
	add_child(road)

func _build_real_city() -> void:
	var assets := [
		"res://assets/external/city/commercial_a.glb",
		"res://assets/external/city/commercial_b.glb",
		"res://assets/external/city/skyscraper_a.glb",
		"res://assets/external/city/skyscraper_b.glb",
		"res://assets/external/city/skyscraper_c.glb",
		"res://assets/external/city/suburban_a.glb",
		"res://assets/external/city/suburban_b.glb",
		"res://assets/external/city/suburban_c.glb",
		"res://assets/external/city/suburban_d.glb",
		"res://assets/external/city/suburban_e.glb",
		"res://assets/external/city/suburban_c.glb"
	]
	var available: Array[String] = []
	for path in assets:
		if ResourceLoader.exists(path):
			available.append(path)
	if available.is_empty():
		return

	for x in range(-28, 29, 2):
		for z in range(-28, 29, 2):
			if abs(x) <= 1 or abs(z) <= 1 or abs(x) % 4 == 0 or abs(z) % 4 == 0:
				continue
			var p := Vector3(x * CELL, 0.0, z * CELL)
			var central := abs(x) < 12 and abs(z) < 12
			var path := _pick_building(available, central)
			_add_real_building(p, path, central)

	# Extra skyline ring so the city reads as a real city from the spawn point.
	for p in [Vector3(-150,0,-170), Vector3(-100,0,-170), Vector3(-50,0,-170), Vector3(50,0,-170), Vector3(100,0,-170), Vector3(150,0,-170),
		Vector3(-170,0,-120), Vector3(170,0,-120), Vector3(-170,0,-60), Vector3(170,0,-60)]:
		_add_real_building(p, _pick_building(available, true), true)

func _pick_building(available: Array[String], central: bool) -> String:
	var skyscrapers: Array[String] = []
	for p in available:
		if p.contains("skyscraper"):
			skyscrapers.append(p)
	if central and not skyscrapers.is_empty():
		return skyscrapers[rng.randi_range(0, skyscrapers.size() - 1)]
	return available[rng.randi_range(0, available.size() - 1)]

func _add_real_building(p: Vector3, asset_path: String, central: bool) -> void:
	var scene := _load_scene(asset_path)
	if scene == null:
		return
	var body := StaticBody3D.new()
	body.name = "RealBuilding"
	body.position = p
	var visual := scene.instantiate()
	visual.name = "BuildingModel"
	visual.scale = Vector3.ONE * (1.25 if central else 1.05)
	body.add_child(visual)

	var col := CollisionShape3D.new()
	var size := Vector3(16, 32, 16) if central else Vector3(14, 16, 14)
	if not central and asset_path.contains("suburban"):
		size = Vector3(14, 11, 14)
	col.shape = _shape_box(size)
	col.position.y = size.y * 0.5
	body.add_child(col)
	add_child(body)

func _build_landmarks() -> void:
	var landmark_data := [
		["مركز الشرطة", Vector3(-65,0,-65), "res://assets/external/city/commercial_a.glb"],
		["المستشفى", Vector3(65,0,-65), "res://assets/external/city/commercial_b.glb"],
		["البنك المركزي", Vector3(-65,0,65), "res://assets/external/city/skyscraper_a.glb"],
		["دار الحكومة", Vector3(65,0,65), "res://assets/external/city/skyscraper_b.glb"],
		["المحكمة", Vector3(-105,0,0), "res://assets/external/city/suburban_a.glb"],
		["السجن", Vector3(105,0,0), "res://assets/external/city/suburban_b.glb"],
		["الكراج", Vector3(0,0,-105), "res://assets/external/city/commercial_a.glb"],
		["السوق", Vector3(0,0,105), "res://assets/external/city/commercial_b.glb"],
		["منطقة العصابات", Vector3(-105,0,105), "res://assets/external/city/suburban_c.glb"],
		["الميناء", Vector3(105,0,105), "res://assets/external/city/commercial_a.glb"]
	]
	for item in landmark_data:
		var body := StaticBody3D.new()
		body.name = str(item[0])
		body.position = item[1]
		var scene := _load_scene(str(item[2]))
		if scene:
			var visual := scene.instantiate()
			visual.scale = Vector3.ONE * 1.35
			body.add_child(visual)
		else:
			var mesh := MeshInstance3D.new()
			mesh.mesh = _box_mesh(Vector3(18, 8, 18), Color("#48515a"))
			mesh.position.y = 4
			body.add_child(mesh)
		var col := CollisionShape3D.new()
		col.shape = _shape_box(Vector3(18, 18, 18))
		col.position.y = 9
		body.add_child(col)
		_add_landmark_sign(body, str(item[0]))
		add_child(body)

func _add_landmark_sign(parent: Node3D, title: String) -> void:
	var label := Label3D.new()
	label.text = title
	label.font_size = 48
	label.outline_size = 12
	label.modulate = Color("#f1f4f6")
	label.position = Vector3(0, 10, 0)
	parent.add_child(label)

func _build_city_life() -> void:
	for i in range(80):
	var x := rng.randf_range(-285.0, 285.0)
	var z := rng.randf_range(-285.0, 285.0)
	if abs(x) < 125.0 and abs(z) < 125.0:
		continue
	if abs(fmod(x, ROAD_SPACING)) < 8.0 or abs(fmod(z, ROAD_SPACING)) < 8.0:
		continue
	_add_procedural_tree(Vector3(x, 0, z), rng.randf_range(0.8, 1.3))

# Park clusters around the city.
for p in [Vector3(-210,0,-80), Vector3(210,0,80), Vector3(-205,0,120), Vector3(205,0,-120)]:
	for j in range(9):
		var q := p + Vector3(rng.randf_range(-22,22),0,rng.randf_range(-18,18))
		_add_procedural_tree(q, rng.randf_range(0.8, 1.15))

# Parked vehicles make the streets read as inhabited even before AI traffic moves.
var cars := [
	"res://assets/external/vehicles/sedan.glb",
	"res://assets/external/vehicles/suv.glb",
	"res://assets/external/vehicles/taxi.glb",
		]
for i in range(34):
	var p := Vector3(rng.randf_range(-250,250), 0.2, rng.randf_range(-250,250))
	if abs(fmod(p.x, ROAD_SPACING)) > 10.0 and abs(fmod(p.z, ROAD_SPACING)) > 10.0:
		continue
	var path := cars[rng.randi_range(0,cars.size()-1)]
	_add_asset(path, p, Vector3.ONE * 1.0, "ParkedCar")

func _add_procedural_tree(p: Vector3, scale_value: float) -> void:
	var root := Node3D.new()
	root.name = "StreetTree"
	root.position = p
	root.scale = Vector3.ONE * scale_value
	var trunk := MeshInstance3D.new()
	var trunk_mesh := CylinderMesh.new()
	trunk_mesh.top_radius = 0.16
	trunk_mesh.bottom_radius = 0.22
	trunk_mesh.height = 2.4
	trunk.mesh = trunk_mesh
	trunk.position.y = 1.2
	trunk.material_override = _material(Color("#5b3b26"))
	root.add_child(trunk)
	var crown := MeshInstance3D.new()
	var crown_mesh := SphereMesh.new()
	crown_mesh.radius = 1.15
	crown_mesh.height = 2.3
	crown.mesh = crown_mesh
	crown.position.y = 3.0
	crown.material_override = _material(Color("#2f6b3b"))
	root.add_child(crown)
	add_child(root)

func _add_asset(path: String, p: Vector3, scale_value: Vector3, node_name: String) -> void:
	var scene := _load_scene(path)
	if scene == null:
		return
	var node := Node3D.new()
	node.name = node_name
	node.position = p
	node.scale = scale_value
	node.add_child(scene.instantiate())
	add_child(node)

func build_street_details() -> void:
	var half := CITY_SIZE * 0.48
	for x in range(-28, 29, 4):
		_add_street_light(Vector3(x * CELL, 0, -half))
		_add_street_light(Vector3(x * CELL, 0, half))
	for z in range(-28, 29, 4):
		_add_street_light(Vector3(-half, 0, z * CELL))
		_add_street_light(Vector3(half, 0, z * CELL))
	for x in range(-280, 281, 20):
		_add_lane_mark(Vector3(x, 0.12, 0), Vector3(5.0, 0.035, 0.16))
		_add_lane_mark(Vector3(0, 0.12, x), Vector3(0.16, 0.035, 5.0))

func _add_lane_mark(p: Vector3, size: Vector3) -> void:
	var mark := MeshInstance3D.new()
	mark.mesh = _box_mesh(size, Color("#e7d6a0"))
	mark.position = p
	add_child(mark)

func _add_street_light(p: Vector3) -> void:
	var pole := MeshInstance3D.new()
	var mesh := CylinderMesh.new()
	mesh.top_radius = 0.045
	mesh.bottom_radius = 0.07
	mesh.height = 5.0
	pole.mesh = mesh
	pole.position = p + Vector3(0, 2.5, 0)
	pole.material_override = _material(Color("#34383d"))
	add_child(pole)
	var lamp := OmniLight3D.new()
	lamp.light_color = Color("#ffdca0")
	lamp.light_energy = 0.7
	lamp.omni_range = 7.0
	lamp.position = p + Vector3(0, 4.7, 0)
	add_child(lamp)

func _load_scene(path: String) -> PackedScene:
	if not ResourceLoader.exists(path):
		return null
	return load(path) as PackedScene

func _shape_box(size: Vector3) -> BoxShape3D:
	var shape := BoxShape3D.new()
	shape.size = size
	return shape

func _box_mesh(size: Vector3, color: Color) -> BoxMesh:
	var mesh := BoxMesh.new()
	mesh.size = size
	mesh.material = _material(color)
	return mesh

func _material(color: Color) -> StandardMaterial3D:
	var mat := StandardMaterial3D.new()
	mat.albedo_color = color
	mat.roughness = 0.85
	return mat
