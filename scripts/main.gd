extends Node3D

const CITY_SIZE := 220.0
const SAVE_PATH := "user://corrupt_state_save.json"
const MISSIONS := ["إكمال تسجيل المواطن", "فتح حساب بنكي", "استخراج رخصة القيادة", "شراء أول مركبة", "اختيار وظيفة", "زيارة مركز الشرطة", "زيارة المستشفى", "زيارة المحكمة", "التعرف على منطقة العصابات", "إكمال أول مهمة عمل", "شراء منزل أو شقة", "بناء سمعة داخل المدينة"]
const FACTIONS := ["مدني", "الشرطة", "الجيش", "الإسعاف", "الدفاع المدني", "الحكومة", "القضاء", "السجن", "عصابة المدينة", "عصابة الميناء"]
const JOBS := {"سائق شاحنة": 900, "سائق تاكسي": 650, "ميكانيكي": 800, "مسعف": 1000, "شرطي": 1200, "رجل إطفاء": 1100, "حارس أمن": 850, "تاجر": 750}

var world: RPWorldBuilder
var player: RPPlayerController
var vehicle: RPVehicleController
var camera: Camera3D
var hud: CanvasLayer
var stats_label: Label
var mission_label: Label
var notice_label: Label
var panel: Panel
var panel_title: Label
var panel_body: Label
var touch_move := Vector2.ZERO
var phone_open := false
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
var current_job := "عاطل"
var current_faction := "مدني"
var territory_progress := 0
var in_vehicle := false
var inventory := {"ماء": 3, "طعام": 2, "إسعاف": 1, "ذخيرة": 20, "هوية": 1}

func _ready() -> void:
	_load_game()
	world = RPWorldBuilder.new()
	world.name = "World"
	add_child(world)
	world.build()
	_build_player()
	_build_npcs()
	_build_vehicles()
	_build_hud()
	_show_notice("Corrupt State RP — تم تشغيل نواة العالم المفتوح الجديدة")
	_update_hud()

func _process(delta: float) -> void:
	hunger = max(0.0, hunger - delta * 0.010)
	thirst = max(0.0, thirst - delta * 0.018)
	if hunger <= 0.0 or thirst <= 0.0:
		health = max(1.0, health - delta * 0.8)
	stamina = min(100.0, stamina + delta * 8.0)
	_update_hud()

func _physics_process(delta: float) -> void:
	if not player:
		return
	var input_vec := Input.get_vector("move_left", "move_right", "move_forward", "move_back")
	if touch_move.length() > 0.1:
		input_vec = touch_move
	if in_vehicle and vehicle:
		vehicle.drive(input_vec, delta)
		camera.global_position = vehicle.global_position + Vector3(0, 6.5, 12.0)
		camera.look_at(vehicle.global_position + Vector3(0, 1.0, 0))
		return
	player.set_move_input(input_vec, false)
	stamina = max(0.0, stamina - delta * 2.2 if input_vec.length() > 0.1 else 0.0)
	player.global_position.x = clamp(player.global_position.x, -CITY_SIZE / 2.0, CITY_SIZE / 2.0)
	player.global_position.z = clamp(player.global_position.z, -CITY_SIZE / 2.0, CITY_SIZE / 2.0)
	camera.global_position = player.global_position + Vector3(0, 6.5, 10.5)
	camera.look_at(player.global_position + Vector3(0, 1.0, 0))

func _build_player() -> void:
	player = RPPlayerController.new()
	player.name = "Player"
	player.position = Vector3(0, 1.2, 28)
	world.add_child(player)
	camera = Camera3D.new()
	camera.current = true
	camera.fov = 68.0
	world.add_child(camera)

func _build_npcs() -> void:
	var data := [["شرطي", Vector3(-55, 1.0, -48), Color("#254f86")], ["مسعف", Vector3(55, 1.0, -48), Color("#d6d6d6")], ["موظف بنك", Vector3(-55, 1.0, 55), Color("#c4a65c")], ["موظف حكومة", Vector3(55, 1.0, 55), Color("#8356a0")], ["ميكانيكي", Vector3(0, 1.0, -92), Color("#8b5931")], ["تاجر", Vector3(0, 1.0, 92), Color("#3b8867")], ["زعيم عصابة", Vector3(-92, 1.0, 92), Color("#9a294d")]]
	for item in data:
		var npc := Node3D.new()
		npc.name = str(item[0])
		var mesh := MeshInstance3D.new()
		var capsule := CapsuleMesh.new()
		capsule.height = 1.9
		capsule.radius = 0.38
		mesh.mesh = capsule
		var mat := StandardMaterial3D.new()
		mat.albedo_color = item[2]
		mesh.material_override = mat
		npc.add_child(mesh)
		npc.position = item[1]
		world.add_child(npc)

func _build_vehicles() -> void:
	vehicle = RPVehicleController.new()
	vehicle.name = "PlayerVehicle"
	vehicle.position = Vector3(0, 1.0, 12)
	vehicle.rotation_degrees.y = 180.0
	vehicle.set_controlled(false)
	world.add_child(vehicle)

func _build_hud() -> void:
	hud = CanvasLayer.new()
	add_child(hud)
	var top := ColorRect.new()
	top.position = Vector2(14, 14)
	top.size = Vector2(530, 142)
	top.color = Color(0.015, 0.02, 0.03, 0.88)
	hud.add_child(top)
	stats_label = Label.new()
	stats_label.position = Vector2(28, 24)
	stats_label.add_theme_font_size_override("font_size", 18)
	hud.add_child(stats_label)
	mission_label = Label.new()
	mission_label.position = Vector2(28, 116)
	mission_label.add_theme_font_size_override("font_size", 16)
	hud.add_child(mission_label)
	notice_label = Label.new()
	notice_label.position = Vector2(180, 160)
	notice_label.size = Vector2(900, 70)
	notice_label.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	notice_label.add_theme_font_size_override("font_size", 22)
	hud.add_child(notice_label)
	_add_button("الهاتف", Vector2(1030, 22), Callable(self, "_toggle_phone"))
	_add_button("المهمة", Vector2(1030, 82), Callable(self, "_next_mission"))
	_add_button("تفاعل", Vector2(1030, 142), Callable(self, "_interact"))
	_add_button("السيارة", Vector2(1030, 202), Callable(self, "_vehicle"))
	_add_button("الجرد", Vector2(1030, 262), Callable(self, "_inventory"))
	_add_button("الفصائل", Vector2(1030, 322), Callable(self, "_factions"))
	_add_button("الوظائف", Vector2(1030, 382), Callable(self, "_jobs"))
	_add_button("حفظ", Vector2(1030, 442), Callable(self, "_save_game"))
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

func _add_button(txt: String, pos: Vector2, cb: Callable) -> void:
	var b := Button.new()
	b.text = txt
	b.position = pos
	b.size = Vector2(170, 50)
	b.add_theme_font_size_override("font_size", 17)
	b.pressed.connect(cb)
	hud.add_child(b)

func _add_touch(pos: Vector2, size: Vector2, txt: String, vec: Vector2) -> void:
	var b := Button.new()
	b.text = txt
	b.position = pos
	b.size = size
	b.modulate = Color(1, 1, 1, 0.72)
	b.add_theme_font_size_override("font_size", 22)
	b.button_down.connect(func(): touch_move = vec)
	b.button_up.connect(func(): touch_move = Vector2.ZERO)
	hud.add_child(b)

func _add_panel_button(txt: String, pos: Vector2, cb: Callable) -> void:
	var b := Button.new()
	b.text = txt
	b.position = pos
	b.size = Vector2(150, 48)
	b.pressed.connect(cb)
	panel.add_child(b)

func _show_panel(title: String, body: String) -> void:
	panel_title.text = title
	panel_body.text = body
	panel.visible = true

func _close_panel() -> void:
	panel.visible = false

func _toggle_phone() -> void:
	phone_open = not phone_open
	if phone_open:
		_show_panel("الهاتف الذكي", "Corrupt Phone\n\nالبنك: %d$\nالوظيفة: %s\nالفصيل: %s\n\nهذه الواجهة هي نقطة ربط لاحقة للبنك وGPS والرسائل والإعلانات." % [bank, current_job, current_faction])
	else:
		_close_panel()

func _next_mission() -> void:
	mission_index = (mission_index + 1) % MISSIONS.size()
	xp += 100
	money += 250
	_level_check()
	_show_notice("المهمة: %s — +250$ +100 XP" % MISSIONS[mission_index])
	_save_game()

func _interact() -> void:
	var nearest := _nearest_landmark()
	match nearest:
		"مركز الشرطة":
			wanted = max(0, wanted - 1)
			_show_notice("الشرطة: تمت معالجة ملفك — المطلوب %d" % wanted)
		"المستشفى":
			health = 100.0
			hunger = min(100.0, hunger + 10)
			thirst = min(100.0, thirst + 10)
			_show_notice("المستشفى: تم العلاج")
		"البنك المركزي":
			bank += money
			money = 0
			_show_notice("البنك: تم إيداع النقود")
		"دار الحكومة":
			current_faction = "الحكومة"
			_show_notice("الحكومة: تم تسجيل المسار الحكومي")
		"المحكمة":
			wanted = max(0, wanted - 2)
			_show_notice("المحكمة: تمت تسوية القضية")
		"السجن":
			if wanted >= 3:
				player.global_position = Vector3(92, 1.2, 6)
				wanted = 0
				_show_notice("تم تنفيذ الحكم")
			else:
				_show_notice("لا توجد قضية تستوجب الاحتجاز")
		"الكراج":
			_vehicle()
		"السوق":
			_buy_food()
		"منطقة العصابات", "الميناء":
			wanted = min(5, wanted + 1)
			territory_progress = min(100, territory_progress + 10)
			_show_notice("منطقة نفوذ: +10% سيطرة — المطلوب %d" % wanted)
		_:
			money += 100
			xp += 25
			_level_check()
			_show_notice("تفاعل مدني ناجح +100$ +25 XP")

func _vehicle() -> void:
	in_vehicle = not in_vehicle
	vehicle.set_controlled(in_vehicle)
	if in_vehicle:
		player.visible = false
		player.set_physics_process(false)
		_show_notice("ركبت السيارة — تحكم في التوجيه والتسارع والفرامل")
	else:
		player.global_position = vehicle.global_position + Vector3(2.2, 1.0, 0)
		player.visible = true
		player.set_physics_process(true)
		_show_notice("نزلت من السيارة")

func _inventory() -> void:
	_show_panel("الجرد", "ماء ×%d\nطعام ×%d\nإسعاف ×%d\nذخيرة ×%d\nهوية ×%d\n\nالصحة %.0f%% | الجوع %.0f%% | العطش %.0f%%" % [inventory["ماء"], inventory["طعام"], inventory["إسعاف"], inventory["ذخيرة"], inventory["هوية"], health, hunger, thirst])

func _factions() -> void:
	_show_panel("الفصائل", "الفصيل الحالي: %s\n\n%s\n\nالرتب والصلاحيات ستنتقل لاحقاً إلى نظام Online authoritative." % [current_faction, "\n".join(FACTIONS)])

func _jobs() -> void:
	var lines := ["الوظيفة الحالية: %s" % current_job, ""]
	for job in JOBS:
		lines.append("• %s — %d$" % [job, JOBS[job]])
	_show_panel("الوظائف", "\n".join(lines))

func _buy_food() -> void:
	if money >= 100:
		money -= 100
		inventory["طعام"] += 1
		hunger = min(100.0, hunger + 20)
		_show_notice("السوق: اشتريت طعاماً")
	else:
		_show_notice("السوق: الرصيد غير كافٍ")

func _level_check() -> void:
	if xp >= level * 500:
		xp -= level * 500
		level += 1
		money += 1000
		_show_notice("ترقية! المستوى %d" % level)

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

func _save_game() -> void:
	if not player:
		return
	var data := {"money": money, "bank": bank, "xp": xp, "level": level, "health": health, "hunger": hunger, "thirst": thirst, "stamina": stamina, "wanted": wanted, "mission_index": mission_index, "current_job": current_job, "current_faction": current_faction, "territory_progress": territory_progress, "inventory": inventory, "position": [player.global_position.x, player.global_position.y, player.global_position.z]}
	var file := FileAccess.open(SAVE_PATH, FileAccess.WRITE)
	if file:
		file.store_string(JSON.stringify(data))
		file.flush()
	_show_notice("تم حفظ تقدمك محلياً")

func _load_game() -> void:
	if not FileAccess.file_exists(SAVE_PATH):
		return
	var data = JSON.parse_string(FileAccess.get_file_as_string(SAVE_PATH))
	if typeof(data) != TYPE_DICTIONARY:
		return
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

func _update_hud() -> void:
	if stats_label:
		stats_label.text = "CORRUPT STATE RP\n$ %d | بنك %d$ | LV %d | XP %d\n❤️ %.0f%%  🍖 %.0f%%  💧 %.0f%%  ⭐ %d\n%s • %s" % [money, bank, level, xp, health, hunger, thirst, wanted, current_job, current_faction]
	if mission_label:
		mission_label.text = "المهمة: %s" % MISSIONS[mission_index]

func _show_notice(message: String) -> void:
	if notice_label:
		notice_label.text = message
		var timer := get_tree().create_timer(3.0)
		timer.timeout.connect(func(): if notice_label: notice_label.text = "")
