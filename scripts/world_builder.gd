class_name RPWorldBuilder
extends Node3D

const CITY_SIZE := 600.0
const CELL := 10.0
var grid: GridMap

func build() -> void:
	_build_environment()
	_build_grid_city()
	_build_procedural_city()
	_build_collision_landmarks()
	build_street_details()

func _build_environment() -> void:
	var env_node := WorldEnvironment.new()
	var env := Environment.new()
	env.background_mode = Environment.BG_SKY
	var sky := Sky.new()
	var sky_mat := ProceduralSkyMaterial.new()
	sky_mat.sky_top_color = Color("#07111f")
	sky_mat.sky_horizon_color = Color("#5e7184")
	sky_mat.ground_bottom_color = Color("#0a1118")
	sky_mat.ground_horizon_color = Color("#334250")
	sky_mat.sun_angle_max = 18.0
	sky.material = sky_mat
	env.sky = sky
	env.ambient_light_source = Environment.AMBIENT_SOURCE_COLOR
	env.ambient_light_color = Color("#a9bdd0")
	env.ambient_light_energy = 0.82
	env.tonemap_mode = Environment.TONE_MAPPER_FILMIC
	env.glow_enabled = false
	env.ssao_enabled = false
	env.sdfgi_enabled = false
	env.background_energy_multiplier = 0.9
	env_node.environment = env
	add_child(env_node)

	var sun := DirectionalLight3D.new()
	sun.rotation_degrees = Vector3(-48, -32, 0)
	sun.light_energy = 1.15
	sun.shadow_enabled = true
	sun.directional_shadow_max_distance = 220.0
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
	library.set_item_mesh(0, _box_mesh(Vector3(CELL, 0.12, CELL), Color("#11161b")))
	library.set_item_shapes(0, [_shape_box(Vector3(CELL, 0.12, CELL)), Transform3D.IDENTITY])
	grid.mesh_library = library
	add_child(grid)

	for x in range(-30, 31):
		for z in range(-30, 31):
			var road: bool = abs(x) <= 1 or abs(z) <= 1 or abs(x) % 4 == 0 or abs(z) % 4 == 0
			if road:
				grid.set_cell_item(Vector3i(x, 0, z), 0)
				if (x % 4 == 0 and z % 4 == 0) or abs(x) <= 1 and z % 4 == 0 or abs(z) <= 1 and x % 4 == 0:
					_add_real_road(Vector3(x * CELL, 0.08, z * CELL), true)
				elif x % 4 == 0:
					_add_real_road(Vector3(x * CELL, 0.08, z * CELL), true)
				elif z % 4 == 0:
					_add_real_road(Vector3(x * CELL, 0.08, z * CELL), false)

	var ground := StaticBody3D.new()
	ground.name = "CityGround"
	var mesh := MeshInstance3D.new()
	var plane := BoxMesh.new()
	plane.size = Vector3(CITY_SIZE, 1.0, CITY_SIZE)
	mesh.mesh = plane
	var mat := StandardMaterial3D.new()
	mat.albedo_color = Color("#26313b")
	mat.roughness = 0.94
	mesh.material_override = mat
	mesh.position.y = -0.55
	ground.add_child(mesh)
	var col := CollisionShape3D.new()
	var cs := BoxShape3D.new()
	cs.size = Vector3(CITY_SIZE, 1.0, CITY_SIZE)
	col.shape = cs
	col.position.y = -0.55
	ground.add_child(col)
	add_child(ground)

func _add_real_road(p: Vector3, vertical: bool) -> void:
	var path := "res://assets/external/roads/road-straight.glb"
	if not ResourceLoader.exists(path):
		return
	var scene := load(path) as PackedScene
	if scene == null:
		return
	var road := scene.instantiate()
	road.position = p
	if vertical:
		road.rotation_degrees.y = 90.0
	add_child(road)

func _build_procedural_city() -> void:
	var rng := RandomNumberGenerator.new()
	rng.seed = 9527
	var assets := [
		["res://assets/external/city/commercial_a.glb", Vector3(10, 8, 10)],
		["res://assets/external/city/commercial_b.glb", Vector3(10, 8, 10)],
		["res://assets/external/city/skyscraper_a.glb", Vector3(14, 30, 14)],
		["res://assets/external/city/skyscraper_b.glb", Vector3(14, 36, 14)],
		["res://assets/external/city/skyscraper_c.glb", Vector3(14, 42, 14)],
		["res://assets/external/city/suburban_a.glb", Vector3(11, 7, 11)],
		["res://assets/external/city/suburban_b.glb", Vector3(11, 9, 11)],
		["res://assets/external/city/suburban_c.glb", Vector3(11, 10, 11)]
	]
	var available: Array = []
	for item in assets:
		if ResourceLoader.exists(str(item[0])):
			available.append(item)
	if available.is_empty():
		return

	for x in range(-28, 29, 2):
		for z in range(-28, 29, 2):
			if abs(x) <= 1 or abs(z) <= 1 or abs(x) % 4 == 0 or abs(z) % 4 == 0:
				continue
			var p := Vector3(x * CELL, 0.0, z * CELL)
			var central: bool = abs(x) < 10 and abs(z) < 10
			var index: int
			if central:
				index = rng.randi_range(2, min(4, available.size() - 1))
			else:
				index = rng.randi_range(0, available.size() - 1)
			var item: Array = available[index]
			_add_real_building(p, str(item[0]), item[1], rng)

func _add_real_building(p: Vector3, asset_path: String, collision_size: Vector3, rng: RandomNumberGenerator) -> void:
	var scene := load(asset_path) as PackedScene
	if scene == null:
		return
	var body := StaticBody3D.new()
	body.name = "RealBuilding"
	body.position = p
	var visual := scene.instantiate()
	visual.position.y = 0.0
	var scale_factor: float = 1.0
	if collision_size.y > 25.0:
		scale_factor = 1.05
	visual.scale = Vector3.ONE * scale_factor
	body.add_child(visual)
	var col := CollisionShape3D.new()
	var shape := BoxShape3D.new()
	shape.size = collision_size
	col.shape = shape
	col.position.y = collision_size.y * 0.5
	body.add_child(col)
	if rng.randf() > 0.72:
		var rooftop := MeshInstance3D.new()
		var marker := BoxMesh.new()
		marker.size = Vector3(min(2.5, collision_size.x * 0.3), 0.25, min(2.5, collision_size.z * 0.3))
		rooftop.mesh = marker
		rooftop.position.y = collision_size.y + 0.15
		var m := StandardMaterial3D.new()
		m.albedo_color = Color("#b22e3b")
		m.emission_enabled = true
		m.emission = Color("#4a1018")
		m.emission_energy_multiplier = 0.5
		rooftop.material_override = m
		body.add_child(rooftop)
	add_child(body)

func _shape_box(size: Vector3) -> BoxShape3D:
	var shape := BoxShape3D.new()
	shape.size = size
	return shape

func _box_mesh(size: Vector3, color: Color) -> BoxMesh:
	var mesh := BoxMesh.new()
	mesh.size = size
	var mat := StandardMaterial3D.new()
	mat.albedo_color = color
	mat.roughness = 0.82
	mesh.material = mat
	return mesh

func _build_collision_landmarks() -> void:
	var landmarks := [
		["مركز الشرطة", Vector3(-65, 4, -65), Vector3(22, 8, 18), Color("#214e83")],
		["المستشفى", Vector3(65, 4, -65), Vector3(22, 8, 18), Color("#812d3b")],
		["البنك المركزي", Vector3(-65, 4, 65), Vector3(22, 8, 18), Color("#75602b")],
		["دار الحكومة", Vector3(65, 4, 65), Vector3(22, 8, 18), Color("#62407b")],
		["المحكمة", Vector3(-105, 4, 0), Vector3(18, 8, 18), Color("#5d5d62")],
		["السجن", Vector3(105, 4, 0), Vector3(18, 8, 18), Color("#3b3d43")],
		["الكراج", Vector3(0, 4, -105), Vector3(20, 8, 16), Color("#7a4e2d")],
		["السوق", Vector3(0, 4, 105), Vector3(20, 8, 16), Color("#2e7057")],
		["منطقة العصابات", Vector3(-105, 4, 105), Vector3(24, 8, 24), Color("#6e273f")],
		["الميناء", Vector3(105, 4, 105), Vector3(24, 8, 24), Color("#285f78")]
	]
	for item in landmarks:
		var body := StaticBody3D.new()
		body.name = str(item[0])
		body.position = item[1]
		var mesh := MeshInstance3D.new()
		mesh.mesh = _box_mesh(item[2], item[3])
		body.add_child(mesh)
		var roof := MeshInstance3D.new()
		var roof_mesh := BoxMesh.new()
		roof_mesh.size = Vector3(item[2].x * 0.82, 0.35, item[2].z * 0.82)
		roof.mesh = roof_mesh
		roof.position.y = item[2].y * 0.5 + 0.2
		var roof_mat := StandardMaterial3D.new()
		roof_mat.albedo_color = Color("#151a20")
		roof.material_override = roof_mat
		body.add_child(roof)
		var col := CollisionShape3D.new()
		var shape := BoxShape3D.new()
		shape.size = item[2]
		col.shape = shape
		body.add_child(col)
		add_child(body)

func build_street_details() -> void:
	var half := CITY_SIZE * 0.48
	for x in range(-28, 29, 4):
		_add_street_light(Vector3(x * CELL, 0, -half))
		_add_street_light(Vector3(x * CELL, 0, half))
	for z in range(-28, 29, 4):
		_add_street_light(Vector3(-half, 0, z * CELL))
		_add_street_light(Vector3(half, 0, z * CELL))
	for x in range(-280, 281, 40):
		_add_lane_mark(Vector3(x, 0.12, 0), Vector3(3.0, 0.035, 0.16))
		_add_lane_mark(Vector3(0, 0.12, x), Vector3(0.16, 0.035, 3.0))

func _add_lane_mark(p: Vector3, size: Vector3) -> void:
	var mark := MeshInstance3D.new()
	var mesh := BoxMesh.new()
	mesh.size = size
	mark.mesh = mesh
	mark.position = p
	var mat := StandardMaterial3D.new()
	mat.albedo_color = Color("#d9c88d")
	mat.roughness = 0.7
	mark.material_override = mat
	add_child(mark)

func _add_street_light(p: Vector3) -> void:
	var pole := MeshInstance3D.new()
	var mesh := CylinderMesh.new()
	mesh.top_radius = 0.045
	mesh.bottom_radius = 0.07
	mesh.height = 5.0
	pole.mesh = mesh
	pole.position = p + Vector3(0, 2.5, 0)
	var mat := StandardMaterial3D.new()
	mat.albedo_color = Color("#34383d")
	mat.roughness = 0.7
	pole.material_override = mat
	add_child(pole)
	var lamp := OmniLight3D.new()
	lamp.light_color = Color("#ffdca0")
	lamp.light_energy = 0.65
	lamp.omni_range = 7.0
	lamp.position = p + Vector3(0, 4.7, 0)
	add_child(lamp)
