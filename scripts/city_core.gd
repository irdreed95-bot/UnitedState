class_name RPCityCore
extends RefCounted

const CITY := "res://assets/external/city/"
const ROAD := "res://assets/external/roads/road-straight.glb"
const CAR := "res://assets/external/vehicles/"

func build(world: Node3D) -> void:
    _build_main_avenues(world)
    _build_central_district(world)
    _build_government_block(world)
    _build_service_block(world)
    _build_commercial_block(world)
    _build_residential_districts(world)
    _build_landmark_labels(world)

func _build_main_avenues(world: Node3D) -> void:
    for x in [-180.0, -90.0, 0.0, 90.0, 180.0]:
        _road(world, Vector3(x, 0.03, 0), Vector3(1, 1, 7), 90.0)
    for z in [-180.0, -90.0, 0.0, 90.0, 180.0]:
        _road(world, Vector3(0, 0.04, z), Vector3(7, 1, 1), 0.0)
    _add_street_name(world, "شارع بغداد", Vector3(-90, 2.4, -8), 0.0)
    _add_street_name(world, "شارع الرياض", Vector3(0, 2.4, -8), 0.0)
    _add_street_name(world, "شارع أبوظبي", Vector3(90, 2.4, -8), 0.0)
    _add_street_name(world, "شارع الكويت", Vector3(-8, 2.4, -90), 90.0)
    _add_street_name(world, "شارع الدوحة", Vector3(-8, 2.4, 90), 90.0)

func _build_central_district(world: Node3D) -> void:
    _building(world, "البلدية والأحوال المدنية", Vector3(-42, 0, -42), "commercial_a.glb", 1.5)
    _building(world, "البنك المركزي", Vector3(42, 0, -42), "commercial_b.glb", 1.55)
    _building(world, "المحكمة", Vector3(-42, 0, 42), "skyscraper_a.glb", 1.1)
    _building(world, "الحكومة", Vector3(42, 0, 42), "skyscraper_b.glb", 1.15)
    _building(world, "جامعة RP", Vector3(0, 0, -135), "commercial_a.glb", 1.7)
    _building(world, "مركز الشرطة", Vector3(-125, 0, -42), "commercial_b.glb", 1.35)
    _building(world, "المستشفى", Vector3(125, 0, -42), "commercial_a.glb", 1.35)

func _build_government_block(world: Node3D) -> void:
    _building(world, "مركز التدريب الحكومي", Vector3(-120, 0, 48), "suburban_a.glb", 1.25)
    _building(world, "مركز التوظيف", Vector3(-78, 0, 48), "suburban_b.glb", 1.2)
    _building(world, "مركز الإعلام والإذاعة", Vector3(78, 0, 48), "commercial_a.glb", 1.2)
    _building(world, "مدرسة تعليم RP", Vector3(120, 0, 48), "suburban_c.glb", 1.25)

func _build_service_block(world: Node3D) -> void:
    _building(world, "مدرسة القيادة", Vector3(-125, 0, 115), "suburban_a.glb", 1.15)
    _building(world, "وكالة السيارات", Vector3(-42, 0, 135), "commercial_b.glb", 1.25)
    _building(world, "الكراج والميكانيكي", Vector3(42, 0, 135), "commercial_a.glb", 1.15)
    _building(world, "محل الهواتف", Vector3(125, 0, 115), "suburban_b.glb", 1.1)
    _building(world, "المطعم", Vector3(0, 0, 135), "suburban_c.glb", 1.0)
    _add_vehicle_row(world, Vector3(-42, 0.5, 151), ["sedan.glb", "suv.glb", "taxi.glb"])

func _build_commercial_block(world: Node3D) -> void:
    for i in range(6):
        var x := -180.0 + float(i) * 72.0
        _building(world, "تجاري_%02d" % i, Vector3(x, 0, 185), "commercial_a.glb", 1.05 + float(i % 2) * 0.15)
    for i in range(4):
        _building(world, "برج_%02d" % i, Vector3(-150.0 + float(i) * 100.0, 0, -135), "skyscraper_%s.glb" % char(97 + (i % 3)), 0.8)

func _build_residential_districts(world: Node3D) -> void:
    _district(world, "حي الأغنياء", Vector3(205, 0, -115), "suburban_a.glb", 1.25, 8)
    _district(world, "حي الفقراء", Vector3(-205, 0, 115), "suburban_b.glb", 0.95, 10)
    _district(world, "حي العصابات", Vector3(-125, 0, 145), "suburban_c.glb", 1.0, 8)

func _district(world: Node3D, title: String, center: Vector3, asset: String, scale_value: float, count: int) -> void:
    _add_landmark_label(world, title, center + Vector3(0, 5.5, -22))
    for i in range(count):
        var col := float(i % 4)
        var row := float(i / 4)
        var p := center + Vector3((col - 1.5) * 25.0, 0, row * 30.0)
        _building(world, title + "_house_%02d" % i, p, asset, scale_value)

func _road(world: Node3D, p: Vector3, scale_value: Vector3, rotation_y: float) -> void:
    if ResourceLoader.exists(ROAD):
        var scene := load(ROAD) as PackedScene
        if scene:
            var road := Node3D.new()
            road.name = "Road"
            road.position = p
            road.rotation_degrees.y = rotation_y
            road.scale = scale_value
            road.add_child(scene.instantiate())
            world.add_child(road)
            return
    var fallback := MeshInstance3D.new()
    var mesh := BoxMesh.new()
    mesh.size = Vector3(12, 0.12, 80)
    fallback.mesh = mesh
    fallback.position = p
    fallback.rotation_degrees.y = rotation_y
    fallback.material_override = _mat(Color("#303236"), 0.95)
    world.add_child(fallback)

func _building(world: Node3D, title: String, p: Vector3, asset: String, scale_value: float) -> void:
    var path := CITY + asset
    if not ResourceLoader.exists(path):
        return
    var scene := load(path) as PackedScene
    if scene == null:
        return
    var root := Node3D.new()
    root.name = title
    root.position = p
    root.add_child(scene.instantiate())
    root.scale = Vector3.ONE * scale_value
    world.add_child(root)
    _add_landmark_label(world, title, p + Vector3(0, 6.0, 0))

func _add_vehicle_row(world: Node3D, p: Vector3, assets: Array[String]) -> void:
    for i in range(assets.size()):
        var path := CAR + assets[i]
        if not ResourceLoader.exists(path):
            continue
        var scene := load(path) as PackedScene
        if scene == null:
            continue
        var car := Node3D.new()
        car.name = "DealerVehicle_%02d" % i
        car.position = p + Vector3((float(i) - 1.0) * 9.0, 0, 0)
        car.scale = Vector3.ONE * 0.7
        car.add_child(scene.instantiate())
        world.add_child(car)

func _add_landmark_label(world: Node3D, title: String, p: Vector3) -> void:
    var label := Label3D.new()
    label.name = "Sign_" + title
    label.text = title
    label.font_size = 40
    label.outline_size = 10
    label.modulate = Color("#f4f0df")
    label.position = p
    world.add_child(label)

func _add_street_name(world: Node3D, title: String, p: Vector3, rotation_y: float) -> void:
    var label := Label3D.new()
    label.text = title
    label.font_size = 32
    label.outline_size = 8
    label.rotation_degrees.y = rotation_y
    label.position = p
    label.modulate = Color("#e9d8a6")
    world.add_child(label)

func _build_landmark_labels(world: Node3D) -> void:
    _add_landmark_label(world, "وسط المدينة", Vector3(0, 8, 0))
    _add_landmark_label(world, "المنطقة التجارية", Vector3(0, 8, 185))

func _mat(color: Color, roughness: float) -> StandardMaterial3D:
    var mat := StandardMaterial3D.new()
    mat.albedo_color = color
    mat.roughness = roughness
    return mat
