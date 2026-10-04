extends Node3D
const CITY_SIZE := 220.0
const ROAD := 14.0
const SAVE_PATH := "user://corrupt_state_save.json"
var player: CharacterBody3D
var camera: Camera3D
var world: Node3D
var hud: CanvasLayer
var stats_label: Label
var mission_label: Label
var notice_label: Label
var panel: Panel
var panel_title: Label
var panel_body: Label
var player_name := "مواطن جديد"
var money := 2500
var bank := 10000
var xp := 0
var level := 1
var health := 100.0
var hunger := 100.0
var thirst := 100.0
var stamina := 100.0
var wanted := 0
var mission_index := 0
var in_vehicle := false
var touch_move := Vector2.ZERO
var current_job := "عاطل"
var current_faction := "مدني"
var phone_open := false
var territory_progress := 0
var chat_lines: Array[String] = []
var inventory := {"ماء": 3, "طعام": 2, "إسعاف": 1, "ذخيرة": 20, "هوية": 1}
var buildings: Array[Node3D] = []
var npcs: Array[Node3D] = []
var vehicles: Array[Node3D] = []
const JOBS := {"سائق شاحنة": {"pay": 900}, "سائق تاكسي": {"pay": 650}, "ميكانيكي": {"pay": 800}, "مسعف": {"pay": 1000}, "شرطي": {"pay": 1200}, "رجل إطفاء": {"pay": 1100}, "حارس أمن": {"pay": 850}, "تاجر": {"pay": 750}}
const FACTIONS := ["مدني", "الشرطة", "الجيش", "الإسعاف", "الدفاع المدني", "الحكومة", "القضاء", "السجن", "عصابة المدينة", "عصابة الميناء"]
const MISSIONS := ["إكمال تسجيل المواطن", "فتح حساب بنكي", "استخراج رخصة القيادة", "شراء أول مركبة", "اختيار وظيفة", "زيارة مركز الشرطة", "زيارة المستشفى", "زيارة المحكمة", "التعرف على منطقة العصابات", "إكمال أول مهمة عمل", "شراء منزل أو شقة", "بناء سمعة داخل المدينة"]

func _ready():
    _load_game()
    _build_environment()
    _build_player()
    _build_npcs()
    _build_vehicles()
    _build_hud()
    _show_notice("مرحباً بك في Corrupt State RP — مدينة مفتوحة، اقتصاد وفصائل ومهن")
    _update_hud()

func _process(delta):
    hunger = max(0.0, hunger - delta * 0.010)
    thirst = max(0.0, thirst - delta * 0.018)
    if hunger <= 0.0 or thirst <= 0.0:
        health = max(1.0, health - delta * 0.8)
    stamina = min(100.0, stamina + delta * 8.0)
    _update_hud()

func _physics_process(delta):
    if not player:
        return
    var input_vec := Input.get_vector("move_left", "move_right", "move_forward", "move_back")
    if touch_move.length() > 0.1:
        input_vec = touch_move
    var dir := Vector3(input_vec.x, 0, input_vec.y)
    var speed := 7.0 if not in_vehicle else 16.0
    if input_vec.length() > 0.1:
        stamina = max(0.0, stamina - delta * (3.0 if not in_vehicle else 1.5))
    player.velocity.x = dir.x * speed
    player.velocity.z = dir.z * speed
    if not player.is_on_floor():
        player.velocity.y -= 20.0 * delta
    else:
        player.velocity.y = -0.2
    player.move_and_slide()
    player.global_position.x = clamp(player.global_position.x, -CITY_SIZE / 2.0, CITY_SIZE / 2.0)
    player.global_position.z = clamp(player.global_position.z, -CITY_SIZE / 2.0, CITY_SIZE / 2.0)
    if camera:
        var cam_distance := 11.0 if not in_vehicle else 14.0
        camera.global_position = player.global_position + Vector3(0, 7.0, cam_distance)
        camera.look_at(player.global_position + Vector3(0, 1.1, 0))

func _build_environment():
    world = Node3D.new()
    world.name = "World"
    add_child(world)
    var env := WorldEnvironment.new()
    var e := Environment.new()
    e.background_mode = Environment.BG_COLOR
    e.background_color = Color("#09101a")
    e.ambient_light_source = Environment.AMBIENT_SOURCE_COLOR
    e.ambient_light_color = Color("#9fb5d0")
    e.ambient_light_energy = 0.8
    e.tonemap_mode = Environment.TONE_MAPPER_FILMIC
    env.environment = e
    world.add_child(env)
    var sun := DirectionalLight3D.new()
    sun.rotation_degrees = Vector3(-52, -30, 0)
    sun.light_energy = 1.35
    sun.shadow_enabled = true
    world.add_child(sun)
    _make_box("Ground", Vector3(0, -1, 0), Vector3(CITY_SIZE, 1, CITY_SIZE), Color("#273039"))
    _make_box("MainRoadNS", Vector3(0, 0, 0), Vector3(ROAD, 0.15, CITY_SIZE), Color("#12161b"))
    _make_box("MainRoadEW", Vector3(0, 0, 0), Vector3(CITY_SIZE, 0.15, ROAD), Color("#12161b"))
    for p in [-75.0, -38.0, 38.0, 75.0]:
        _make_box("RoadNS", Vector3(p, 0, 0), Vector3(8, 0.15, CITY_SIZE), Color("#1b2026"))
        _make_box("RoadEW", Vector3(0, 0, p), Vector3(CITY_SIZE, 0.15, 8), Color("#1b2026"))
    for x in range(-90, 91, 30):
        for z in range(-90, 91, 30):
            if abs(x) < 16 or abs(z) < 16:
                continue
            var h := 8.0 + float(abs((x * 13 + z * 7) % 22))
            var c := Color("#3c4854") if (x + z) % 2 == 0 else Color("#4d4650")
            _make_box("Building", Vector3(x, h / 2.0, z), Vector3(18, h, 18), c)
    _make_landmark("مركز الشرطة", Vector3(-65, 4, -65), Vector3(22, 8, 18), Color("#214e83"))
    _make_landmark("المستشفى", Vector3(65, 4, -65), Vector3(22, 8, 18), Color("#812d3b"))
    _make_landmark("البنك المركزي", Vector3(-65, 4, 65), Vector3(22, 8, 18), Color("#75602b"))
    _make_landmark("دار الحكومة", Vector3(65, 4, 65), Vector3(22, 8, 18), Color("#62407b"))
    _make_landmark("المحكمة", Vector3(-105, 4, 0), Vector3(18, 8, 18), Color("#5d5d62"))
    _make_landmark("السجن", Vector3(105, 4, 0), Vector3(18, 8, 18), Color("#3b3d43"))
    _make_landmark("الكراج", Vector3(0, 4, -105), Vector3(20, 8, 16), Color("#7a4e2d"))
    _make_landmark("السوق", Vector3(0, 4, 105), Vector3(20, 8, 16), Color("#2e7057"))
    _make_landmark("منطقة العصابات", Vector3(-105, 4, 105), Vector3(24, 8, 24), Color("#6e273f"))
    _make_landmark("الميناء", Vector3(105, 4, 105), Vector3(24, 8, 24), Color("#285f78"))

func _make_box(n: String, pos: Vector3, size: Vector3, color: Color):
    var body := StaticBody3D.new()
    body.name = n
    body.position = pos
    var mesh := MeshInstance3D.new()
    var box := BoxMesh.new()
    box.size = size
    mesh.mesh = box
    var mat := StandardMaterial3D.new()
    mat.albedo_color = color
    mat.roughness = 0.82
    mesh.material_override = mat
    body.add_child(mesh)
    var shape := CollisionShape3D.new()
    var cs := BoxShape3D.new()
    cs.size = size
    shape.shape = cs
    body.add_child(shape)
    world.add_child(body)
    buildings.append(body)

func _make_landmark(title: String, pos: Vector3, size: Vector3, color: Color):
    _make_box(title, pos, size, color)

func _build_player():
    player = CharacterBody3D.new()
    player.name = "Player"
    player.position = Vector3(0, 1.2, 28)
    world.add_child(player)
    var mesh := MeshInstance3D.new()
    var capsule := CapsuleMesh.new()
    capsule.height = 2.0
    capsule.radius = 0.42
    mesh.mesh = capsule
    var mat := StandardMaterial3D.new()
    mat.albedo_color = Color("#d9b08c")
    mesh.material_override = mat
    player.add_child(mesh)
    var col := CollisionShape3D.new()
    var shape := CapsuleShape3D.new()
    shape.height = 2.0
    shape.radius = 0.42
    col.shape = shape
    player.add_child(col)
    camera = Camera3D.new()
    camera.current = true
    camera.fov = 68
    world.add_child(camera)

func _build_npcs():
    var data := [["شرطي", Vector3(-55, 1.0, -48), Color("#254f86")], ["مسعف", Vector3(55, 1.0, -48), Color("#d6d6d6")], ["موظف بنك", Vector3(-55, 1.0, 55), Color("#c4a65c")], ["موظف حكومة", Vector3(55, 1.0, 55), Color("#8356a0")], ["ميكانيكي", Vector3(0, 1.0, -92), Color("#8b5931")], ["تاجر", Vector3(0, 1.0, 92), Color("#3b8867")], ["زعيم عصابة", Vector3(-92, 1.0, 92), Color("#9a294d")], ["حارس السجن", Vector3(92, 1.0, 0), Color("#4a4c54")]]
    for item in data:
        var npc := MeshInstance3D.new()
        npc.name = str(item[0])
        var capsule := CapsuleMesh.new()
        capsule.height = 1.9
        capsule.radius = 0.38
        npc.mesh = capsule
        var mat := StandardMaterial3D.new()
        mat.albedo_color = item[2]
        npc.material_override = mat
        npc.position = item[1]
        world.add_child(npc)
        npcs.append(npc)

func _build_vehicles():
    var positions := [Vector3(0, 0.9, 12), Vector3(-28, 0.9, 0), Vector3(28, 0.9, 0), Vector3(0, 0.9, -18)]
    for i in positions.size():
        var car := Node3D.new()
        car.name = "Vehicle_%d" % i
        car.position = positions[i]
        var body := MeshInstance3D.new()
        var box := BoxMesh.new()
        box.size = Vector3(2.1, 0.7, 4.2)
        body.mesh = box
        var mat := StandardMaterial3D.new()
        mat.albedo_color = [Color("#11151a"), Color("#b42b37"), Color("#e0e0e0"), Color("#2e5578")][i]
        body.material_override = mat
        car.add_child(body)
        world.add_child(car)
        vehicles.append(car)

func _build_hud():
    hud = CanvasLayer.new()
    add_child(hud)
    var top := ColorRect.new()
    top.position = Vector2(14, 14)
    top.size = Vector2(510, 138)
    top.color = Color(0.015, 0.02, 0.03, 0.88)
    hud.add_child(top)
    stats_label = Label.new()
    stats_label.position = Vector2(28, 24)
    stats_label.add_theme_font_size_override("font_size", 18)
    hud.add_child(stats_label)
    mission_label = Label.new()
    mission_label.position = Vector2(28, 112)
    mission_label.add_theme_font_size_override("font_size", 16)
    hud.add_child(mission_label)
    notice_label = Label.new()
    notice_label.position = Vector2(180, 160)
    notice_label.size = Vector2(920, 70)
    notice_label.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
    notice_label.add_theme_font_size_override("font_size", 24)
    hud.add_child(notice_label)
    _add_button("الهاتف", Vector2(1030, 22), Callable(self, "_toggle_phone"), 170)
    _add_button("المهام", Vector2(1030, 88), Callable(self, "_next_mission"), 170)
    _add_button("تفاعل", Vector2(1030, 154), Callable(self, "_interact"), 170)
    _add_button("المركبة", Vector2(1030, 220), Callable(self, "_vehicle"), 170)
    _add_button("الجرد", Vector2(1030, 286), Callable(self, "_inventory"), 170)
    _add_button("الفصيلة", Vector2(1030, 352), Callable(self, "_factions"), 170)
    _add_button("وظيفة", Vector2(1030, 418), Callable(self, "_jobs"), 170)
    _add_button("حفظ", Vector2(1030, 484), Callable(self, "_save_game"), 170)
    _add_touch(Vector2(72, 548), Vector2(150, 78), "↑", Vector2(0, -1))
    _add_touch(Vector2(72, 626), Vector2(150, 78), "↓", Vector2(0, 1))
    _add_touch(Vector2(0, 626), Vector2(72, 78), "←", Vector2(-1, 0))
    _add_touch(Vector2(222, 626), Vector2(72, 78), "→", Vector2(1, 0))
    panel = Panel.new()
    panel.position = Vector2(250, 105)
    panel.size = Vector2(780, 500)
    panel.visible = false
    hud.add_child(panel)
    panel_title = Label.new()
    panel_title.position = Vector2(24, 18)
    panel_title.add_theme_font_size_override("font_size", 26)
    panel.add_child(panel_title)
    panel_body = Label.new()
    panel_body.position = Vector2(24, 70)
    panel_body.size = Vector2(730, 390)
    panel_body.add_theme_font_size_override("font_size", 18)
    panel.add_child(panel_body)
    _add_panel_button("إغلاق", Vector2(590, 430), Callable(self, "_close_panel"))

func _add_button(txt: String, pos: Vector2, cb: Callable, width := 190):
    var b := Button.new()
    b.text = txt
    b.position = pos
    b.size = Vector2(width, 54)
    b.add_theme_font_size_override("font_size", 18)
    b.pressed.connect(cb)
    hud.add_child(b)

func _add_touch(pos: Vector2, size: Vector2, txt: String, vec: Vector2):
    var b := Button.new()
    b.text = txt
    b.position = pos
    b.size = size
    b.modulate = Color(1, 1, 1, 0.72)
    b.add_theme_font_size_override("font_size", 22)
    b.button_down.connect(func(): touch_move = vec)
    b.button_up.connect(func(): touch_move = Vector2.ZERO)
    hud.add_child(b)

func _add_panel_button(txt: String, pos: Vector2, cb: Callable):
    var b := Button.new()
    b.text = txt
    b.position = pos
    b.size = Vector2(150, 48)
    b.pressed.connect(cb)
    panel.add_child(b)

func _show_panel(title: String, body: String):
    panel_title.text = title
    panel_body.text = body
    panel.visible = true

func _close_panel():
    panel.visible = false

func _toggle_phone():
    phone_open = not phone_open
    if phone_open:
        _show_panel("الهاتف الذكي", "📱 Corrupt Phone\n\nالبنك: رصيد %d$\nGPS: اختر مؤسسة من قائمة المهام\nالوظائف: %s\nالرسائل: %d\nالإعلانات: لا توجد إعلانات جديدة\n\nالهاتف الآن نواة قابلة للتوسع إلى تطبيقات حقيقية." % [bank, current_job, chat_lines.size()])
    else:
        _close_panel()

func _next_mission():
    mission_index = (mission_index + 1) % MISSIONS.size()
    xp += 100
    money += 250
    _level_check()
    _show_notice("تم تحديث المهمة: %s — +250$ +100 XP" % MISSIONS[mission_index])
    _save_game()

func _interact():
    var nearest := _nearest_landmark()
    if nearest == "مركز الشرطة":
        wanted = max(0, wanted - 1)
        _show_notice("مركز الشرطة: تمت معالجة ملفك — مستوى المطاردة %d" % wanted)
    elif nearest == "المستشفى":
        health = 100.0
        hunger = min(100.0, hunger + 10)
        thirst = min(100.0, thirst + 10)
        _show_notice("المستشفى: تم علاج الشخصية بالكامل")
    elif nearest == "البنك المركزي":
        bank += money
        money = 0
        _show_notice("البنك: تم إيداع النقود في الحساب")
    elif nearest == "دار الحكومة":
        current_faction = "الحكومة"
        _show_notice("تم تسجيلك في المسار الحكومي")
    elif nearest == "المحكمة":
        wanted = max(0, wanted - 2)
        _show_notice("المحكمة: تمت تسوية القضية — المطلوب %d" % wanted)
    elif nearest == "السجن":
        if wanted >= 3:
            player.global_position = Vector3(92, 1.2, 6)
            wanted = 0
            _show_notice("تم تنفيذ الحكم — وصلت إلى السجن")
        else:
            _show_notice("السجن: لا توجد قضية تستوجب الاحتجاز")
    elif nearest == "الكراج":
        _vehicle()
    elif nearest == "السوق":
        _buy_food()
    elif nearest == "منطقة العصابات" or nearest == "الميناء":
        wanted = min(5, wanted + 1)
        territory_progress = min(100, territory_progress + 10)
        _show_notice("منطقة نفوذ: +10% سيطرة — المطلوب %d" % wanted)
    else:
        money += 100
        xp += 25
        _level_check()
        _show_notice("تفاعل مدني ناجح +100$ +25 XP")

func _vehicle():
    in_vehicle = not in_vehicle
    _show_notice("دخلت المركبة — السرعة أصبحت 16" if in_vehicle else "تركت المركبة — عدت إلى وضع المشي")

func _inventory():
    _show_panel("الجرد", "الحقيبة الشخصية\n\nماء ×%d\nطعام ×%d\nإسعاف ×%d\nذخيرة ×%d\nهوية ×%d\n\nالصحة %.0f%% | الجوع %.0f%% | العطش %.0f%%" % [inventory["ماء"], inventory["طعام"], inventory["إسعاف"], inventory["ذخيرة"], inventory["هوية"], health, hunger, thirst])

func _factions():
    _show_panel("الفصائل", "الفصيلة الحالية: %s\n\n%s\n\nالرتب والصلاحيات والمهام الخاصة بكل فصيل ستصبح مرتبطة بنظام صلاحيات مستقل عند الانتقال إلى Online." % [current_faction, "\n".join(FACTIONS)])

func _jobs():
    var lines := ["الوظيفة الحالية: %s" % current_job, ""]
    for job in JOBS.keys():
        lines.append("• %s — دخل المهمة %d$" % [job, JOBS[job]["pay"]])
    lines.append("")
    lines.append("يمكن ربط اختيار الوظيفة بالمؤسسات ونظام التوظيف في المرحلة التالية.")
    _show_panel("الوظائف", "\n".join(lines))

func _buy_food():
    if money >= 100:
        money -= 100
        inventory["طعام"] += 1
        hunger = min(100.0, hunger + 20)
        _show_notice("السوق: اشتريت طعاماً — الجوع +20")
    else:
        _show_notice("السوق: لا يوجد رصيد كافٍ")

func _level_check():
    if xp >= level * 500:
        xp -= level * 500
        level += 1
        money += 1000
        _show_notice("ترقية مستوى! وصلت إلى المستوى %d وحصلت على 1000$" % level)

func _nearest_landmark() -> String:
    var p := player.global_position
    var landmarks := {"مركز الشرطة": Vector3(-65, 0, -65), "المستشفى": Vector3(65, 0, -65), "البنك المركزي": Vector3(-65, 0, 65), "دار الحكومة": Vector3(65, 0, 65), "المحكمة": Vector3(-105, 0, 0), "السجن": Vector3(105, 0, 0), "الكراج": Vector3(0, 0, -105), "السوق": Vector3(0, 0, 105), "منطقة العصابات": Vector3(-105, 0, 105), "الميناء": Vector3(105, 0, 105)}
    var nearest := ""
    var distance := 99999.0
    for name in landmarks:
        var d: float = p.distance_to(landmarks[name])
        if d < distance:
            distance = d
            nearest = name
    return nearest if distance < 28.0 else ""

func _save_game():
    if not player:
        return
    var data := {"player_name": player_name, "money": money, "bank": bank, "xp": xp, "level": level, "health": health, "hunger": hunger, "thirst": thirst, "stamina": stamina, "wanted": wanted, "mission_index": mission_index, "current_job": current_job, "current_faction": current_faction, "territory_progress": territory_progress, "inventory": inventory, "position": [player.global_position.x, player.global_position.y, player.global_position.z]}
    var file := FileAccess.open(SAVE_PATH, FileAccess.WRITE)
    if file:
        file.store_string(JSON.stringify(data))
        file.flush()
    _show_notice("تم حفظ تقدمك محلياً على الجهاز")

func _load_game():
    if not FileAccess.file_exists(SAVE_PATH):
        return
    var data = JSON.parse_string(FileAccess.get_file_as_string(SAVE_PATH))
    if typeof(data) != TYPE_DICTIONARY:
        return
    player_name = str(data.get("player_name", player_name))
    money = int(data.get("money", money))
    bank = int(data.get("bank", bank))
    xp = int(data.get("xp", xp))
    level = int(data.get("level", level))
    health = float(data.get("health", health))
    hunger = float(data.get("hunger", hunger))
    thirst = float(data.get("thirst", thirst))
    stamina = float(data.get("stamina", stamina))
    wanted = int(data.get("wanted", wanted))
    mission_index = int(data.get("mission_index", mission_index))
    current_job = str(data.get("current_job", current_job))
    current_faction = str(data.get("current_faction", current_faction))
    territory_progress = int(data.get("territory_progress", territory_progress))
    if data.has("inventory"):
        inventory = data["inventory"]

func _update_hud():
    if stats_label:
        stats_label.text = "CORRUPT STATE RP\n$ %d | بنك %d$ | LV %d | XP %d\n❤️ %.0f%%  🍖 %.0f%%  💧 %.0f%%  ⭐ %d\n%s • %s" % [money, bank, level, xp, health, hunger, thirst, wanted, current_job, current_faction]
    if mission_label:
        mission_label.text = "المهمة: %s" % MISSIONS[mission_index]

func _show_notice(t: String):
    if notice_label:
        notice_label.text = t
        var timer := get_tree().create_timer(3.2)
        timer.timeout.connect(func():
            if notice_label:
                notice_label.text = ""
        )
