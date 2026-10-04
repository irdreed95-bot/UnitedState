class_name RPWorldBuilder
extends Node3D

const CITY_SIZE := 220.0
const CELL := 10.0
var grid: GridMap

func build() -> void:
	_build_environment()
	_build_grid_city()
	_build_collision_landmarks()

func _build_environment() -> void:
	var env_node := WorldEnvironment.new()
	var env := Environment.new()
	env.background_mode = Environment.BG_COLOR
	env.background_color = Color("#08111d")
	env.ambient_light_source = Environment.AMBIENT_SOURCE_COLOR
	env.ambient_light_color = Color("#9fb7d0")
	env.ambient_light_energy = 0.72
	env.tonemap_mode = Environment.TONE_MAPPER_FILMIC
	env.glow_enabled = true
	env.glow_intensity = 0.75
	env.glow_bloom = 0.12
	env.ssao_enabled = true
	env.ssao_radius = 2.5
	env.ssao_intensity = 1.5
	env.sdfgi_enabled = false
	env_node.environment = env
	add_child(env_node)
	var sun := DirectionalLight3D.new()
	sun.rotation_degrees = Vector3(-52, -30, 0)
	sun.light_energy = 1.35
	sun.shadow_enabled = true
	sun.directional_shadow_max_distance = 180.0
	add_child(sun)

func _build_grid_city() -> void:
	grid = GridMap.new()
	grid.name = "CityGridMap"
	grid.cell_size = Vector3(CELL, CELL, CELL)
	grid.cell_octant_size = 8
	grid.collision_layer = 1
	grid.collision_mask = 1
	var library := MeshLibrary.new()
	library.create_item(0)
	library.set_item_name(0, "Road")
	library.set_item_mesh(0, _box_mesh(Vector3(CELL, 0.2, CELL), Color("#12171d")))
	library.set_item_shapes(0, [_shape_box(Vector3(CELL, 0.2, CELL)), Transform3D.IDENTITY])
	library.create_item(1)
	library.set_item_name(1, "Building")
	library.set_item_mesh(1, _box_mesh(Vector3(CELL * 0.86, CELL * 2.0, CELL * 0.86), Color("#3c4854")))
	library.set_item_shapes(1, [_shape_box(Vector3(CELL * 0.86, CELL * 2.0, CELL * 0.86)), Transform3D.IDENTITY])
	library.create_item(2)
	library.set_item_name(2, "BuildingDark")
	library.set_item_mesh(2, _box_mesh(Vector3(CELL * 0.86, CELL * 3.0, CELL * 0.86), Color("#4a4650")))
	library.set_item_shapes(2, [_shape_box(Vector3(CELL * 0.86, CELL * 3.0, CELL * 0.86)), Transform3D.IDENTITY])
	grid.mesh_library = library
	add_child(grid)
	for x in range(-11, 12):
		for z in range(-11, 12):
			var road := abs(x) <= 1 or abs(z) <= 1 or abs(x) % 4 == 0 or abs(z) % 4 == 0
			if road:
				grid.set_cell_item(Vector3i(x, 0, z), 0)
			else:
				var id := 1 if (x + z) % 2 == 0 else 2
				grid.set_cell_item(Vector3i(x, 1, z), id)
	var ground := StaticBody3D.new()
	ground.name = "CityGround"
	var mesh := MeshInstance3D.new()
	var plane := BoxMesh.new()
	plane.size = Vector3(CITY_SIZE, 1.0, CITY_SIZE)
	mesh.mesh = plane
	var mat := StandardMaterial3D.new()
	mat.albedo_color = Color("#263039")
	mat.roughness = 0.9
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

func _shape_box(size: Vector3) -> BoxShape3D:
	var shape := BoxShape3D.new()
	shape.size = size
	return shape

func _box_mesh(size: Vector3, color: Color) -> BoxMesh:
	var mesh := BoxMesh.new()
	mesh.size = size
	var mat := StandardMaterial3D.new()
	mat.albedo_color = color
	mat.roughness = 0.78
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
		var col := CollisionShape3D.new()
		var shape := BoxShape3D.new()
		shape.size = item[2]
		col.shape = shape
		body.add_child(col)
		add_child(body)
