class_name RPWorldRegions
extends RefCounted

const ASSET_CITY := "res://assets/external/city/"
const ASSET_CARS := "res://assets/external/vehicles/"

var rng := RandomNumberGenerator.new()

func build(world: Node3D) -> void:
    rng.seed = 20261006
    _add_coast(world)
    _add_forest(world)
    _add_mountains(world)
    _add_desert_prison(world)
    _add_military_base(world)
    _add_port(world)
    _add_district_signs(world)

func _add_coast(world: Node3D) -> void:
    var water := MeshInstance3D.new()
    water.name = "CoastalSea"
    var mesh := BoxMesh.new()
    mesh.size = Vector3(580, 0.6, 150)
    water.mesh = mesh
    water.position = Vector3(0, -0.15, 360)
    water.material_override = _mat(Color("#1b5d78"), 0.25, 0.05)
    world.add_child(water)

    var beach := MeshInstance3D.new()
    beach.name = "CoastalBeach"
    var beach_mesh := BoxMesh.new()
    beach_mesh.size = Vector3(580, 0.35, 55)
    beach.mesh = beach_mesh
    beach.position = Vector3(0, 0.05, 285)
    beach.material_override = _mat(Color("#c8ad78"), 0.98, 0.0)
    world.add_child(beach)

    for i in range(10):
        var x := -220.0 + i * 48.0
        _add_asset(world, ASSET_CITY + ("suburban_a.glb" if i % 2 == 0 else "suburban_b.glb"), Vector3(x, 0, 268), 0.9, "CoastalVilla")
    for i in range(6):
        _add_asset(world, ASSET_CARS + "suv.glb", Vector3(-180 + i * 70, 0.2, 286), 0.85, "BeachVehicle")

func _add_forest(world: Node3D) -> void:
    var forest_center := Vector3(0, 0, -360)
    for i in range(150):
        var p := forest_center + Vector3(rng.randf_range(-270, 270), 0, rng.randf_range(-80, 80))
        _add_tree(world, p, rng.randf_range(0.8, 1.5))
    for p in [Vector3(-180, 0, -345), Vector3(150, 0, -360), Vector3(40, 0, -400)]:
        _add_asset(world, ASSET_CITY + "suburban_c.glb", p, 1.15, "ForestHouse")

func _add_mountains(world: Node3D) -> void:
    for i in range(18):
        var x := -260.0 + i * 30.0
        var h := 35.0 + float((i * 17) % 45)
        _add_hill(world, Vector3(x, h * 0.5, -455), Vector3(28, h, 55))
    for i in range(7):
        _add_asset(world, ASSET_CITY + "suburban_b.glb", Vector3(-190 + i * 62, 12, -405), 0.75, "MountainStructure")

func _add_desert_prison(world: Node3D) -> void:
    var base := Vector3(235, 0, -255)
    var ground := MeshInstance3D.new()
    var gm := BoxMesh.new()
    gm.size = Vector3(120, 0.3, 110)
    ground.mesh = gm
    ground.position = base
    ground.material_override = _mat(Color("#b59663"), 1.0, 0.0)
    world.add_child(ground)

    _add_asset(world, ASSET_CITY + "commercial_b.glb", base, 1.4, "PrisonMainBlock")
    for x in [-48, 48]:
        _add_watchtower(world, base + Vector3(x, 0, -42))
        _add_watchtower(world, base + Vector3(x, 0, 42))
    for z in [-24, 0, 24]:
        _add_prison_block(world, base + Vector3(0, 0, z))

func _add_military_base(world: Node3D) -> void:
    var base := Vector3(-225, 0, -260)
    var yard := MeshInstance3D.new()
    var ym := BoxMesh.new()
    ym.size = Vector3(125, 0.5, 105)
    yard.mesh = ym
    yard.position = base
    yard.material_override = _mat(Color("#53605a"), 0.95, 0.0)
    world.add_child(yard)

    _add_gate(world, base + Vector3(-58, 0, 0))
    _add_gate(world, base + Vector3(58, 0, 0))
    _add_asset(world, ASSET_CITY + "commercial_a.glb", base + Vector3(0, 0, -25), 1.2, "MilitaryHQ")
    _add_asset(world, ASSET_CITY + "suburban_a.glb", base + Vector3(-35, 0, 25), 0.9, "MilitaryHospital")
    _add_asset(world, ASSET_CITY + "suburban_b.glb", base + Vector3(35, 0, 25), 0.9, "MilitaryArmory")
    for x in [-40, -20, 0, 20, 40]:
        _add_training_marker(world, base + Vector3(x, 0.2, 48))
    for i in range(5):
        _add_asset(world, ASSET_CARS + ("suv.glb" if i % 2 == 0 else "police.glb"), base + Vector3(-42 + i * 21, 0.2, 12), 0.8, "MilitaryVehicle")

func _add_port(world: Node3D) -> void:
    var base := Vector3(225, 0, 205)
    for x in [-50, -25, 0, 25, 50]:
        var pier := MeshInstance3D.new()
        var pm := BoxMesh.new()
        pm.size = Vector3(18, 1.2, 75)
        pier.mesh = pm
        pier.position = base + Vector3(x, 0.3, 28)
        pier.material_override = _mat(Color("#5d6264"), 0.9, 0.0)
        world.add_child(pier)
        _add_crane(world, base + Vector3(x, 0, -12))
    for i in range(8):
        _add_container(world, base + Vector3(-54 + (i % 4) * 18, 1.5, -45 + int(i / 4) * 14))

func _add_district_signs(world: Node3D) -> void:
    var signs := [
        ["المنطقة التجارية", Vector3(0, 5, 0)],
        ["حي الأغنياء", Vector3(210, 5, -120)],
        ["حي الفقراء", Vector3(-205, 5, 120)],
        ["حي العصابات", Vector3(-110, 5, 110)],
        ["الواجهة الساحلية", Vector3(0, 5, 265)],
        ["الغابة", Vector3(0, 5, -285)],
        ["القاعدة العسكرية", Vector3(-225, 5, -210)],
        ["السجن الصحراوي", Vector3(235, 5, -205)]
    ]
    for item in signs:
        var label := Label3D.new()
        label.text = str(item[0])
        label.font_size = 52
        label.outline_size = 14
        label.modulate = Color("#f4f0df")
        label.position = item[1]
        world.add_child(label)

func _add_tree(world: Node3D, p: Vector3, s: float) -> void:
    var root := Node3D.new()
    root.name = "ForestTree"
    root.position = p
    root.scale = Vector3.ONE * s
    var trunk := MeshInstance3D.new()
    var tm := CylinderMesh.new()
    tm.top_radius = 0.18
    tm.bottom_radius = 0.28
    tm.height = 3.2
    trunk.mesh = tm
    trunk.position.y = 1.6
    trunk.material_override = _mat(Color("#5b3b26"), 0.95, 0.0)
    root.add_child(trunk)
    var crown := MeshInstance3D.new()
    var cm := SphereMesh.new()
    cm.radius = 1.5
    cm.height = 3.0
    crown.mesh = cm
    crown.position.y = 3.8
    crown.material_override = _mat(Color("#2f6b3b"), 0.9, 0.0)
    root.add_child(crown)
    world.add_child(root)

func _add_hill(world: Node3D, p: Vector3, size: Vector3) -> void:
    var hill := MeshInstance3D.new()
    var hm := CylinderMesh.new()
    hm.top_radius = 2.0
    hm.bottom_radius = 0.9
    hm.height = size.y
    hill.mesh = hm
    hill.position = p
    hill.scale = Vector3(size.x / 4.0, 1.0, size.z / 4.0)
    hill.material_override = _mat(Color("#687060"), 1.0, 0.0)
    world.add_child(hill)

func _add_watchtower(world: Node3D, p: Vector3) -> void:
    var tower := MeshInstance3D.new()
    var tm := BoxMesh.new()
    tm.size = Vector3(5, 14, 5)
    tower.mesh = tm
    tower.position = p + Vector3(0, 7, 0)
    tower.material_override = _mat(Color("#57524b"), 0.95, 0.0)
    world.add_child(tower)

func _add_prison_block(world: Node3D, p: Vector3) -> void:
    _add_asset(world, ASSET_CITY + "commercial_a.glb", p, 0.75, "PrisonBlock")

func _add_gate(world: Node3D, p: Vector3) -> void:
    var gate := MeshInstance3D.new()
    var gm := BoxMesh.new()
    gm.size = Vector3(5, 9, 18)
    gate.mesh = gm
    gate.position = p + Vector3(0, 4.5, 0)
    gate.material_override = _mat(Color("#4a4f52"), 0.9, 0.0)
    world.add_child(gate)

func _add_training_marker(world: Node3D, p: Vector3) -> void:
    var m := MeshInstance3D.new()
    var mm := BoxMesh.new()
    mm.size = Vector3(8, 0.2, 3)
    m.mesh = mm
    m.position = p
    m.material_override = _mat(Color("#d8d1a0"), 0.8, 0.0)
    world.add_child(m)

func _add_crane(world: Node3D, p: Vector3) -> void:
    var crane := MeshInstance3D.new()
    var cm := BoxMesh.new()
    cm.size = Vector3(2.5, 20, 2.5)
    crane.mesh = cm
    crane.position = p + Vector3(0, 10, 0)
    crane.material_override = _mat(Color("#c88b32"), 0.75, 0.0)
    world.add_child(crane)
    var arm := MeshInstance3D.new()
    var am := BoxMesh.new()
    am.size = Vector3(18, 1.2, 1.2)
    arm.mesh = am
    arm.position = p + Vector3(8, 19, 0)
    arm.material_override = _mat(Color("#c88b32"), 0.75, 0.0)
    world.add_child(arm)

func _add_container(world: Node3D, p: Vector3) -> void:
    var c := MeshInstance3D.new()
    var cm := BoxMesh.new()
    cm.size = Vector3(15, 3, 6)
    c.mesh = cm
    c.position = p
    c.material_override = _mat(Color("#7d4b39"), 0.9, 0.0)
    world.add_child(c)

func _add_asset(world: Node3D, path: String, p: Vector3, scale_value: float, node_name: String) -> void:
    if not ResourceLoader.exists(path):
        return
    var scene := load(path) as PackedScene
    if scene == null:
        return
    var node := Node3D.new()
    node.name = node_name
    node.position = p
    node.scale = Vector3.ONE * scale_value
    node.add_child(scene.instantiate())
    world.add_child(node)

func _mat(color: Color, roughness: float, metallic: float) -> StandardMaterial3D:
    var mat := StandardMaterial3D.new()
    mat.albedo_color = color
    mat.roughness = roughness
    mat.metallic = metallic
    return mat
