extends Node3D

const CITY_SIZE := 600.0
const BATCH_ONE := "Real World Foundation"
const SAVE_PATH := "user://corrupt_state_save.json"
const MISSIONS := RPNewCitizenProgram.MISSION_NAMES
const FACTIONS := ["مدني", "الشرطة", "الجيش", "الإسعاف", "الدفاع المدني", "الحكومة", "القضاء", "السجن", "عصابة المدينة", "عصابة الميناء"]
const JOBS := {"سائق شاحنة": 900, "سائق تاكسي": 650, "ميكانيكي": 800, "مسعف": 1000, "شرطي": 1200, "رجل إطفاء": 1100, "حارس أمن": 850, "تاجر": 750}

var world: RPWorldBuilder
var player: RPPlayerController
var vehicle: RPVehicleController
var traffic: RPTrafficManager
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
var citizen_program: RPNewCitizenProgram
var rental_home_until_unix := 0
var future_buttons: Array[Button] = []
var inventory := {"ماء": 3, "طعام": 2, "إسعاف": 1, "ذخيرة": 20, "هوية": 1}

func _ready() -> void:
	citizen_program = RPNewCitizenProgram.new()
	_load_game()
	world = RPWorldBuilder.new()
	world.name = "World"
	add_child(world)
	world.build()
	var city := RPCityCore.new()
	city.build(world)
	var regions := RPWorldRegions.new()
	regions.build(world)
	_build_player()
	_build_npcs()
	_build_vehicles()
	_build_traffic()
	_build_hud()
	_show_notice("Corrupt State RP — تم تشغيل نواة العالم المفتوح الجديدة")
	_update_hud()

func _process(delta: float) -> void:
	hunger = max(0.0, hunger - delta * 0.010)
	thirst = max(0.0, thirst - delta * 0.018)
	if hunger <= 0.0 or thirst <= 0.0:
		health = max(1.0, health - delta * 0.8)
	stamina = min(100.0, stamina + delta * 8.0)
	if citizen_program:
		citizen_program.tick_lawful(delta, wanted, false)
		if citizen_program.mission_index == 6 and citizen_program.law_minutes() >= 30.0:
			var law_result := citizen_program.complete_lawful()
			_apply_program_result(law_result)
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
	if input_vec.length() > 0.1:
		stamina = max(0.0, stamina - delta * 2.2)
	player.global_position.x = clamp(player.global_position.x, -CITY_SIZE / 2.0, CITY_SIZE / 2.0)
	player.global_position.z = clamp(player.global_position.z, -CITY_SIZE / 2.0, CITY_SIZE / 2.0)
	camera.global_position = player.global_position + Vector3(0, 6.5, 10.5)
	camera.look_at(player.global_position + Vector3(0, 1.0, 0))

func _build_player() -> void:
	player = RPPlayerController.new()
	player.name = "Player"
	player.position = Vector3(-42, 1.2, -68)
	world.add_child(player)
	camera = Camera3D.new()
	camera.fov = 68.0
	camera.near = 0.05
	camera.far = 1200.0
	camera.position = player.position + Vector3(0, 6.5, 10.5)
	world.add_child(camera)
	camera.make_current()
	camera.current = true

func _build_npcs() -> void:
	var data := [["شرطي", Vector3(-55, 1.0, -48), Color("#254f86")], ["مسعف", Vector3(55, 1.0, -48), Color("#d6d6d6")], ["موظف بنك", Vector3(-55, 1.0, 55), Color("#c4a65c")], ["موظف حكومة", Vector3(55, 1.0, 55), Color("#8356a0")], ["ميكانيكي", Vector3(0, 1.0, -92), Color("#8b5931")], ["تاجر", Vector3(0, 1.0, 92), Color("#3b8867")], ["زعيم عصابة", Vector3(-92, 1.0, 92), Color("#9a294d")]]
	for item in data:
		var npc := Node3D.new()
		npc.name = str(item[0])
		npc.position = item[1]
		var asset_path := "res://assets/external/character/citizen.glb"
		if ResourceLoader.exists(asset_path):
			var scene := load(asset_path) as PackedScene
			if scene:
				var citizen := scene.instantiate()
				citizen.name = "Citizen_" + str(item[0])
				npc.add_child(citizen)
				world.add_child(npc)
				continue
		# Fallback only if the licensed citizen asset is unavailable.
		var mesh := MeshInstance3D.new()
		var capsule := CapsuleMesh.new()
		capsule.height = 1.9
		capsule.radius = 0.38
		mesh.mesh = capsule
		var mat := StandardMaterial3D.new()
		mat.albedo_color = item[2]
		mesh.material_override = mat
		npc.add_child(mesh)
		world.add_child(npc)

func _build_vehicles() -> void:
	vehicle = RPVehicleController.new()
	vehicle.name = "PlayerVehicle"
	vehicle.position = Vector3(-42, 1.0, 145)
	vehicle.rotation_degrees.y = 180.0
	vehicle.set_controlled(false)
	world.add_child(vehicle)

func _build_traffic() -> void:
	traffic = RPTrafficManager.new()
	traffic.name = "Traffic"
	traffic.setup(CITY_SIZE)
	world.add_child(traffic)

func _build_hud() -> void:
	hud = CanvasLayer.new()
	add_child(hud)
	var root := Control.new()
	root.set_anchors_and_offsets_preset(Control.PRESET_FULL_RECT)
	var ui_theme := Theme.new()
	var arabic_font_path := "res://assets/fonts/NotoSansArabic-Regular.ttf"
	if ResourceLoader.exists(arabic_font_path):
		var arabic_font := load(arabic_font_path) as Font
		if arabic_font:
			ui_theme.default_font = arabic_font
	root.theme = ui_theme
	hud.add_child(root)

	var top := Panel.new()
	top.position = Vector2(18, 18)
	top.size = Vector2(420, 142)
	top.modulate = Color(1, 1, 1, 0.94)
	root.add_child(top)
	stats_label = Label.new()
	stats_label.position = Vector2(18, 14)
	stats_label.size = Vector2(384, 92)
	stats_label.add_theme_font_size_override("font_size", 17)
	top.add_child(stats_label)
	mission_label = Label.new()
	mission_label.layout_direction = Control.LAYOUT_DIRECTION_RTL
	mission_label.position = Vector2(18, 108)
	mission_label.size = Vector2(384, 28)
	mission_label.add_theme_font_size_override("font_size", 14)
	top.add_child(mission_label)

	notice_label = Label.new()
	notice_label.layout_direction = Control.LAYOUT_DIRECTION_RTL
	notice_label.set_anchors_preset(Control.PRESET_CENTER_TOP)
	notice_label.position = Vector2(-360, 22)
	notice_label.size = Vector2(720, 48)
	notice_label.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
	notice_label.add_theme_font_size_override("font_size", 18)
	root.add_child(notice_label)

	var menu := VBoxContainer.new()
	menu.set_anchors_preset(Control.PRESET_TOP_RIGHT)
	menu.position = Vector2(-190, 22)
	menu.size = Vector2(172, 420)
	menu.add_theme_constant_override("separation", 8)
	root.add_child(menu)
	_add_button_to(menu, "الهاتف", Callable(self, "_toggle_phone"))
	_add_button_to(menu, "المهمة", Callable(self, "_next_mission"))
	_add_button_to(menu, "تفاعل", Callable(self, "_interact"))
	_add_button_to(menu, "السيارة", Callable(self, "_vehicle"))
	_add_button_to(menu, "محرك / أنوار", Callable(self, "_vehicle_controls"))
	_add_button_to(menu, "الجرد", Callable(self, "_inventory"))
	_add_button_to(menu, "الفصائل", Callable(self, "_factions"))
	_add_button_to(menu, "الوظائف", Callable(self, "_jobs"))
	_add_button_to(menu, "العمل", Callable(self, "_work_shift"))
	_add_button_to(menu, "حفظ", Callable(self, "_save_game"))

	var dpad := Control.new()
	dpad.set_anchors_preset(Control.PRESET_BOTTOM_LEFT)
	dpad.position = Vector2(18, -190)
	dpad.size = Vector2(210, 170)
	root.add_child(dpad)
	_add_touch(dpad, Vector2(70, 0), Vector2(70, 55), "▲", Vector2(0, -1))
	_add_touch(dpad, Vector2(70, 112), Vector2(70, 55), "▼", Vector2(0, 1))
	_add_touch(dpad, Vector2(0, 56), Vector2(70, 55), "◀", Vector2(-1, 0))
	_add_touch(dpad, Vector2(140, 56), Vector2(70, 55), "▶", Vector2(1, 0))

	panel = Panel.new()
	panel.set_anchors_preset(Control.PRESET_CENTER)
	panel.position = Vector2(-330, -230)
	panel.size = Vector2(660, 460)
	panel.visible = false
	root.add_child(panel)
	panel_title = Label.new()
	panel_title.position = Vector2(24, 18)
	panel_title.size = Vector2(612, 44)
	panel_title.add_theme_font_size_override("font_size", 24)
	panel.add_child(panel_title)
	panel_body = Label.new()
	panel_body.position = Vector2(24, 72)
	panel_body.size = Vector2(612, 320)
	panel_body.autowrap_mode = TextServer.AUTOWRAP_WORD_SMART
	panel_body.add_theme_font_size_override("font_size", 17)
	panel.add_child(panel_body)
	_add_panel_button("إغلاق", Vector2(500, 410), Callable(self, "_close_panel"))

func _add_button_to(parent: Control, txt: String, cb: Callable) -> void:
	var b := Button.new()
	b.text = txt
	b.custom_minimum_size = Vector2(172, 44)
	b.add_theme_font_size_override("font_size", 15)
	b.pressed.connect(cb)
	parent.add_child(b)

func _add_touch(parent: Control, pos: Vector2, size: Vector2, txt: String, vec: Vector2) -> void:
	var b := Button.new()
	b.text = txt
	b.position = pos
	b.size = size
	b.modulate = Color(1, 1, 1, 0.78)
	b.add_theme_font_size_override("font_size", 22)
	b.button_down.connect(func(): touch_move = vec)
	b.button_up.connect(func(): touch_move = Vector2.ZERO)
	parent.add_child(b)

func _add_panel_button(txt: String, pos: Vector2, cb: Callable) -> void:
	var b := Button.new()
	b.text = txt
	b.position = pos
	b.size = Vector2(140, 44)
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
	_show_panel("برنامج المواطنين الجدد", _program_summary())

func _interact() -> void:
	var nearest := _nearest_landmark()
	if nearest == "":
		_show_notice("اقترب من نقطة تفاعل أو مؤسسة واضحة ثم اضغط «تفاعل».")
		return

	if citizen_program and citizen_program.mission_index == 9 and nearest == "دار الحكومة":
		_show_future_paths()
		return

	if citizen_program and citizen_program.mission_index == 8 and nearest == "مواطن":
		var social_result := citizen_program.interact("مواطن", money)
		_apply_program_result(social_result)
		_save_game()
		return

	if citizen_program:
		var program_result := citizen_program.interact(nearest, money)
		if program_result.advanced or program_result.reward_money > 0 or program_result.reward_xp > 0 or program_result.message != "":
			if program_result.message != "":
				_apply_program_result(program_result)
				_save_game()
				return

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

func _apply_program_result(result: Dictionary) -> void:
	if result.has("reward_money"):
		money += int(result.reward_money)
	if result.has("reward_xp"):
		xp += int(result.reward_xp)
	_level_check()
	if result.get("message", "") != "":
		_show_notice(str(result.message))
	mission_index = citizen_program.mission_index
	if citizen_program.job_selected and current_job == "عاطل":
		current_job = "سائق تاكسي"
	if citizen_program.identity_issued:
		inventory["هوية"] = 1
	if citizen_program.bank_card_received:
		inventory["بطاقة بنكية"] = 1
	if citizen_program.practical_passed:
		inventory["رخصة قيادة"] = 1
	if citizen_program.vehicle_registered:
		inventory["مفتاح المركبة"] = 1
	if citizen_program.completed:
		_show_notice("🎉 برنامج المواطنين الجدد مكتمل — أنت الآن مواطن الجمهورية.")
	_update_hud()

func _program_summary() -> String:
	if citizen_program == null:
		return "برنامج المواطنين الجدد غير مهيأ."
	var progress := "المهمة %d / %d — %s" % [citizen_program.mission_index + 1, citizen_program.TOTAL_MISSIONS, citizen_program.mission_name()]
	var objective := citizen_program.objective()
	var law := ""
	if citizen_program.mission_index == 6:
		law = "\n\nمدة الالتزام: %.1f / 30 دقيقة" % citizen_program.law_minutes()
	var extras := "\n\nالهوية: %s | البنك: %s | الرخصة: %s | المركبة: %s | الوظيفة: %s" % [
		"✓" if citizen_program.identity_issued else "—",
		"✓" if citizen_program.bank_opened and citizen_program.atm_used else "—",
		"✓" if citizen_program.practical_passed else "—",
		"✓" if citizen_program.vehicle_registered else "—",
		current_job if citizen_program.job_selected else "—",
	]
	return "%s\n\nالمطلوب:\n%s%s%s" % [progress, objective, law, extras]

func _work_shift() -> void:
	if citizen_program == null or not citizen_program.job_selected:
		_show_notice("اختر وظيفتك أولاً من مركز التوظيف.")
		return
	var pay := 500
	money += pay
	citizen_program.add_legal_earnings(pay)
	xp += 75
	_show_notice("أنهيت وردية قانونية كـ%s +%d$ — إجمالي دخل المهمة %d$/5000$" % [current_job, pay, citizen_program.legal_earnings])
	_save_game()

func _show_future_paths() -> void:
	_close_panel()
	_show_panel("اختيار المستقبل", "اختر المسار الذي يناسب شخصيتك داخل الجمهورية.\n\nالاختيار يفتح جميع الوظائف الرسمية ويُنهي برنامج المواطنين الجدد.")
	for b in future_buttons:
		if is_instance_valid(b):
			b.queue_free()
	future_buttons.clear()
	var paths := ["الشرطة", "المستشفى", "القانون", "الحكومة", "تأسيس شركة", "الوظائف المدنية", "الإعلام"]
	for i in range(paths.size()):
		var b := Button.new()
		b.text = paths[i]
		b.position = Vector2(24 + (i % 3) * 205, 270 + (i / 3) * 48)
		b.size = Vector2(185, 40)
		b.pressed.connect(_select_future_path.bind(paths[i]))
		panel.add_child(b)
		future_buttons.append(b)

func _select_future_path(path: String) -> void:
	var result := citizen_program.choose_future(path)
	if not result.success:
		_show_notice(result.message)
		return
	money += int(result.reward_money)
	xp += int(result.reward_xp)
	current_faction = "مدني"
	citizen_program.starter_vehicle_awarded = true
	vehicle.global_position = player.global_position + Vector3(3.0, 0.8, 0)
	vehicle.set_controlled(false)
	vehicle.engine_on = false
	rental_home_until_unix = Time.get_unix_time_from_system() + 7 * 24 * 60 * 60
	_show_notice("مبروك! +10,000$ | سيارة بداية مجانية | منزل إيجار 7 أيام | لقب مواطن الجمهورية")
	_show_panel("مواطن الجمهورية", "المسار المختار: %s\n\n✓ 10,000$\n✓ سيارة بداية اقتصادية\n✓ منزل إيجار مجاني لمدة 7 أيام\n✓ لقب مواطن الجمهورية\n✓ جميع الوظائف الرسمية مفتوحة\n\nهذه هي بداية رحلتك داخل الجمهورية." % path)
	_save_game()

func _vehicle_controls() -> void:
	if in_vehicle and vehicle:
		vehicle.toggle_engine()
		vehicle.toggle_lights()
		_show_notice("السيارة: محرك %s | الأنوار %s" % ["ON" if vehicle.engine_on else "OFF", "ON" if vehicle.headlights_on else "OFF"])

func _vehicle() -> void:
	in_vehicle = not in_vehicle
	vehicle.set_controlled(in_vehicle)
	if in_vehicle:
		vehicle.engine_on = true
		player.visible = false
		player.set_physics_process(false)
		_show_notice("ركبت السيارة — تحكم في التوجيه والتسارع والفرامل")
	else:
		vehicle.engine_on = false
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
	if citizen_program and citizen_program.mission_index < 4:
		lines.append("الوظائف المدنية ستُفتح ضمن برنامج المواطنين الجدد.")
	else:
		for job in JOBS:
			lines.append("• %s — %d$" % [job, JOBS[job]])
		if citizen_program and citizen_program.mission_index == 4:
			lines.append("")
			lines.append("الدخل القانوني: %d$ / 5,000$" % citizen_program.legal_earnings)
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
	var landmarks := {
		"البلدية والأحوال المدنية": Vector3(-42, 0, -42),
		"البنك المركزي": Vector3(42, 0, -42),
		"المحكمة": Vector3(-42, 0, 42),
		"دار الحكومة": Vector3(42, 0, 42),
		"مركز الشرطة": Vector3(-125, 0, -42),
		"المستشفى": Vector3(125, 0, -42),
		"جامعة RP": Vector3(0, 0, -135),
		"مدرسة القيادة": Vector3(-125, 0, 115),
		"وكالة السيارات": Vector3(-42, 0, 135),
		"الكراج": Vector3(42, 0, 135),
		"مركز التوظيف": Vector3(-78, 0, 48),
		"الميناء": Vector3(225, 0, 205),
		"السجن": Vector3(235, 0, -255),
		"المطار": Vector3(245, 0, 0),
		"الحديقة العامة": Vector3(90, 0, 90),
		"المنطقة التجارية": Vector3(0, 0, 185),
		"منطقة العصابات": Vector3(-125, 0, 145),
		"السوق": Vector3(0, 0, 135),
		"معاملة RP": Vector3(0, 0, 135),
	}
	var nearest := ""
	var distance := 99999.0
	for name in landmarks:
		var d: float = p.distance_to(landmarks[name])
		if d < distance:
			distance = d
			nearest = name
	if citizen_program and citizen_program.mission_index == 8:
		var npc_distance := 99999.0
		for child in world.get_children():
			if child is Node3D and (str(child.name).begins_with("شرطي") or str(child.name).begins_with("مسعف") or str(child.name).begins_with("موظف")):
				var nd: float = p.distance_to(child.global_position)
				if nd < npc_distance:
					npc_distance = nd
		if npc_distance < min(distance, 10.0):
			return "مواطن"
	return nearest if distance < 28.0 else ""

func _save_game() -> void:
	if not player:
		return
	var data := {
		"money": money, "bank": bank, "xp": xp, "level": level, "health": health,
		"hunger": hunger, "thirst": thirst, "stamina": stamina, "wanted": wanted,
		"mission_index": mission_index, "current_job": current_job, "current_faction": current_faction,
		"territory_progress": territory_progress, "inventory": inventory,
		"position": [player.global_position.x, player.global_position.y, player.global_position.z],
		"citizen_program": {
			"mission_index": citizen_program.mission_index,
			"identity_issued": citizen_program.identity_issued,
			"bank_opened": citizen_program.bank_opened,
			"bank_card_received": citizen_program.bank_card_received,
			"atm_used": citizen_program.atm_used,
			"theory_passed": citizen_program.theory_passed,
			"practical_passed": citizen_program.practical_passed,
			"vehicle_purchased": citizen_program.vehicle_purchased,
			"vehicle_registered": citizen_program.vehicle_registered,
			"first_maintenance_discount": citizen_program.first_maintenance_discount,
			"job_selected": citizen_program.job_selected,
			"legal_earnings": citizen_program.legal_earnings,
			"government_visits": citizen_program.government_visits,
			"law_seconds": citizen_program.law_seconds,
			"city_discovery": citizen_program.city_discovery,
			"greetings": citizen_program.greetings,
			"first_rp_transaction": citizen_program.first_rp_transaction,
			"future_path": citizen_program.future_path,
			"completed": citizen_program.completed,
			"citizen_title": citizen_program.citizen_title,
			"rental_home_until_unix": rental_home_until_unix,
			"official_jobs_unlocked": citizen_program.official_jobs_unlocked,
			"starter_vehicle_awarded": citizen_program.starter_vehicle_awarded
		}
	}
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
	if data.has("citizen_program") and citizen_program:
		var cp: Dictionary = data["citizen_program"]
		citizen_program.mission_index = int(cp.get("mission_index", citizen_program.mission_index))
		citizen_program.identity_issued = bool(cp.get("identity_issued", false))
		citizen_program.bank_opened = bool(cp.get("bank_opened", false))
		citizen_program.bank_card_received = bool(cp.get("bank_card_received", false))
		citizen_program.atm_used = bool(cp.get("atm_used", false))
		citizen_program.theory_passed = bool(cp.get("theory_passed", false))
		citizen_program.practical_passed = bool(cp.get("practical_passed", false))
		citizen_program.vehicle_purchased = bool(cp.get("vehicle_purchased", false))
		citizen_program.vehicle_registered = bool(cp.get("vehicle_registered", false))
		citizen_program.first_maintenance_discount = bool(cp.get("first_maintenance_discount", false))
		citizen_program.job_selected = bool(cp.get("job_selected", false))
		citizen_program.legal_earnings = int(cp.get("legal_earnings", 0))
		citizen_program.government_visits = cp.get("government_visits", {})
		citizen_program.law_seconds = float(cp.get("law_seconds", 0.0))
		citizen_program.city_discovery = cp.get("city_discovery", {})
		citizen_program.greetings = int(cp.get("greetings", 0))
		citizen_program.first_rp_transaction = bool(cp.get("first_rp_transaction", false))
		citizen_program.future_path = str(cp.get("future_path", ""))
		citizen_program.completed = bool(cp.get("completed", false))
		citizen_program.citizen_title = str(cp.get("citizen_title", ""))
		citizen_program.official_jobs_unlocked = bool(cp.get("official_jobs_unlocked", false))
		citizen_program.starter_vehicle_awarded = bool(cp.get("starter_vehicle_awarded", false))
		rental_home_until_unix = int(cp.get("rental_home_until_unix", 0))
		mission_index = citizen_program.mission_index

func _update_hud() -> void:
	if stats_label:
		stats_label.text = "CORRUPT STATE RP\n$ %d | بنك %d$ | LV %d | XP %d\n❤️ %.0f%%  🍖 %.0f%%  💧 %.0f%%  ⭐ %d\n%s • %s" % [money, bank, level, xp, health, hunger, thirst, wanted, current_job, current_faction]
	if mission_label:
		mission_label.text = "المهمة: %s\n%s" % [citizen_program.mission_name() if citizen_program else MISSIONS[mission_index], citizen_program.objective() if citizen_program else ""]

func _show_notice(message: String) -> void:
	if notice_label:
		notice_label.text = message
		var timer := get_tree().create_timer(3.0)
		timer.timeout.connect(_clear_notice)

func _clear_notice() -> void:
	if notice_label:
		notice_label.text = ""
