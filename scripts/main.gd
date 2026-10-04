extends Node3D

var player: CharacterBody3D
var camera: Camera3D
var world: Node3D
var money := 1000
var xp := 0
var level := 1
var hunger := 100.0
var thirst := 100.0
var mission_index := 0
var in_vehicle := false
var stats_label: Label
var mission_label: Label
var notice_label: Label
var touch_move := Vector2.ZERO
var buildings: Array[Node3D] = []

const CITY_SIZE := 180.0
const ROAD := 14.0

func _ready():
    _build_environment()
    _build_player()
    _build_hud()
    _show_notice("مرحباً بك في Corrupt State — ابدأ رحلتك داخل الجمهورية")
    _update_hud()

func _process(delta):
    hunger = max(0.0, hunger - delta * 0.015)
    thirst = max(0.0, thirst - delta * 0.025)
    _update_hud()

func _physics_process(delta):
    if not player: return
    var input_vec = Input.get_vector("move_left", "move_right", "move_forward", "move_back")
    if touch_move.length() > 0.1: input_vec = touch_move
    var dir = Vector3(input_vec.x, 0, input_vec.y)
    var speed = 7.0 if not in_vehicle else 15.0
    player.velocity.x = dir.x * speed
    player.velocity.z = dir.z * speed
    if not player.is_on_floor(): player.velocity.y -= 20.0 * delta
    else: player.velocity.y = -0.2
    player.move_and_slide()
    player.global_position.x = clamp(player.global_position.x, -CITY_SIZE/2, CITY_SIZE/2)
    player.global_position.z = clamp(player.global_position.z, -CITY_SIZE/2, CITY_SIZE/2)
    if camera:
        camera.global_position = player.global_position + Vector3(0, 7.5, 10)
        camera.look_at(player.global_position + Vector3(0, 1.2, 0))

func _build_environment():
    world = Node3D.new()
    world.name = "World"
    add_child(world)
    var env = WorldEnvironment.new()
    var e = Environment.new()
    e.background_mode = Environment.BG_COLOR
    e.background_color = Color("#101722")
    e.ambient_light_source = Environment.AMBIENT_SOURCE_COLOR
    e.ambient_light_color = Color("#a8b6d1")
    e.ambient_light_energy = 0.7
    e.tonemap_mode = Environment.TONE_MAPPER_FILMIC
    env.environment = e
    world.add_child(env)
    var sun = DirectionalLight3D.new()
    sun.rotation_degrees = Vector3(-52, -30, 0)
    sun.light_energy = 1.2
    sun.shadow_enabled = true
    world.add_child(sun)
    _make_box("Ground", Vector3(0,-1,0), Vector3(CITY_SIZE,1,CITY_SIZE), Color("#303840"))
    _make_box("RoadNS", Vector3(0,0,0), Vector3(ROAD,0.15,CITY_SIZE), Color("#171a1f"))
    _make_box("RoadEW", Vector3(0,0,0), Vector3(CITY_SIZE,0.15,ROAD), Color("#171a1f"))
    for p in [-60.0,-30.0,30.0,60.0]:
        _make_box("RoadNS"+str(p), Vector3(p,0,0), Vector3(8,0.15,CITY_SIZE), Color("#20242a"))
        _make_box("RoadEW"+str(p), Vector3(0,0,p), Vector3(CITY_SIZE,0.15,8), Color("#20242a"))
    for x in range(-75, 76, 30):
        for z in range(-75, 76, 30):
            if abs(x) < 12 or abs(z) < 12: continue
            var h = 7.0 + float(abs((x*13+z*7)%19))
            var c = Color("#39434f") if (x+z)%2 == 0 else Color("#4a4650")
            _make_box("Building", Vector3(x,h/2,z), Vector3(18,h,18), c)
    _make_landmark("Police HQ", Vector3(-45,4,-45), Color("#234c7d"))
    _make_landmark("Hospital", Vector3(45,4,-45), Color("#7d2935"))
    _make_landmark("Bank", Vector3(-45,4,45), Color("#75602a"))
    _make_landmark("City Hall", Vector3(45,4,45), Color("#5a3e6d"))

func _make_box(n:String, pos:Vector3, size:Vector3, color:Color):
    var body = StaticBody3D.new()
    body.name = n
    var mesh = MeshInstance3D.new()
    var box = BoxMesh.new()
    box.size = size
    mesh.mesh = box
    var mat = StandardMaterial3D.new()
    mat.albedo_color = color
    mat.roughness = 0.82
    mesh.material_override = mat
    body.position = pos
    body.add_child(mesh)
    var shape = CollisionShape3D.new()
    var cs = BoxShape3D.new()
    cs.size = size
    shape.shape = cs
    body.add_child(shape)
    world.add_child(body)
    buildings.append(body)

func _make_landmark(title:String, pos:Vector3, color:Color):
    _make_box(title, pos, Vector3(20,8,16), color)

func _build_player():
    player = CharacterBody3D.new()
    player.name = "Player"
    player.position = Vector3(0,1.1,28)
    world.add_child(player)
    var mesh = MeshInstance3D.new()
    var capsule = CapsuleMesh.new()
    capsule.height = 2.0
    capsule.radius = 0.42
    mesh.mesh = capsule
    var mat = StandardMaterial3D.new()
    mat.albedo_color = Color("#d9b08c")
    mesh.material_override = mat
    player.add_child(mesh)
    var col = CollisionShape3D.new()
    var shape = CapsuleShape3D.new()
    shape.height = 2.0
    shape.radius = 0.42
    col.shape = shape
    player.add_child(col)
    camera = Camera3D.new()
    camera.current = true
    camera.fov = 68
    world.add_child(camera)

func _build_hud():
    var layer = CanvasLayer.new()
    add_child(layer)
    var panel = ColorRect.new()
    panel.position = Vector2(18,18)
    panel.size = Vector2(380,126)
    panel.color = Color(0.02,0.025,0.035,0.82)
    layer.add_child(panel)
    stats_label = Label.new()
    stats_label.position = Vector2(32,30)
    stats_label.add_theme_font_size_override("font_size",22)
    layer.add_child(stats_label)
    mission_label = Label.new()
    mission_label.position = Vector2(32,88)
    mission_label.add_theme_font_size_override("font_size",18)
    layer.add_child(mission_label)
    notice_label = Label.new()
    notice_label.position = Vector2(0,150)
    notice_label.size = Vector2(1280,70)
    notice_label.horizontal_alignment = HORIZONTAL_ALIGNMENT_CENTER
    notice_label.add_theme_font_size_override("font_size",28)
    layer.add_child(notice_label)
    _add_button(layer,"المهام",Vector2(1010,35),Callable(self,"_next_mission"))
    _add_button(layer,"تفاعل",Vector2(1010,105),Callable(self,"_interact"))
    _add_button(layer,"سيارة",Vector2(1010,175),Callable(self,"_vehicle"))
    _add_button(layer,"الجرد",Vector2(1010,245),Callable(self,"_inventory"))
    _add_button(layer,"الفصائل",Vector2(1010,315),Callable(self,"_factions"))
    _add_button(layer,"استراحة",Vector2(1010,385),Callable(self,"_rest"))
    _add_touch(layer,Vector2(80,580),Vector2(210,90),"↑",Vector2(0,-1))
    _add_touch(layer,Vector2(80,670),Vector2(210,90),"↓",Vector2(0,1))
    _add_touch(layer,Vector2(0,625),Vector2(140,90),"←",Vector2(-1,0))
    _add_touch(layer,Vector2(220,625),Vector2(140,90),"→",Vector2(1,0))

func _add_button(layer, txt, pos, cb):
    var b=Button.new()
    b.text=txt
    b.position=pos
    b.size=Vector2(190,58)
    b.add_theme_font_size_override("font_size",20)
    b.pressed.connect(cb)
    layer.add_child(b)

func _add_touch(layer,pos,size,txt,vec):
    var b=Button.new()
    b.text=txt
    b.position=pos
    b.size=size
    b.modulate=Color(1,1,1,0.72)
    b.button_down.connect(func(): touch_move=vec)
    b.button_up.connect(func(): touch_move=Vector2.ZERO)
    layer.add_child(b)

func _next_mission():
    mission_index += 1
    if mission_index > 10: mission_index=1
    xp += 100
    if xp >= level*500:
        xp=0; level+=1; money+=1000
        _show_notice("ترقية مستوى! حصلت على 1000$")
    else:
        money += 250
    _show_notice("تم تحديث المهمة رقم %d" % mission_index)
    _update_hud()

func _interact():
    money += 100
    xp += 25
    _show_notice("تفاعل RP ناجح +100$ +25 XP")

func _vehicle():
    in_vehicle = !in_vehicle
    _show_notice("حالة المركبة: " + ("قيادة" if in_vehicle else "مشاة"))

func _inventory():
    _show_notice("الحقيبة: ماء ×3 | طعام ×2 | إسعاف ×1 | ذخيرة ×20")

func _factions():
    _show_notice("الفصائل: الشرطة | الجيش | الطب | الدفاع المدني | الحكومة | القضاء | العصابات")

func _rest():
    hunger=min(100.0,hunger+25)
    thirst=min(100.0,thirst+25)
    _show_notice("استراحة — تم استعادة احتياجات الشخصية")

func _show_notice(t):
    if notice_label:
        notice_label.text=t
        var timer=get_tree().create_timer(3.0)
        timer.timeout.connect(func(): if notice_label: notice_label.text="")

func _update_hud():
    if stats_label:
        stats_label.text="Corrupt State RP\n$ %d   |   LV %d   |   XP %d\n🍖 %.0f%%   💧 %.0f%%" % [money,level,xp,hunger,thirst]
    if mission_label:
        mission_label.text="المهمة الحالية: %s" % ["استخراج الهوية الوطنية","فتح حساب بنكي","رخصة القيادة","شراء أول مركبة","الحصول على وظيفة","زيارة المؤسسات","الالتزام بالقانون","التعرف على المدينة","التفاعل مع المجتمع","اختيار المستقبل"][mission_index]
