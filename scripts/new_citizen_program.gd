class_name RPNewCitizenProgram
extends RefCounted

## PDF-aligned onboarding state machine.
## The main scene owns the world/player; this class owns only progression state.

const TOTAL_MISSIONS := 10
const MISSION_NAMES := [
	"استخراج الهوية الوطنية",
	"فتح حساب بنكي",
	"استخراج رخصة القيادة",
	"شراء أول مركبة",
	"الحصول على أول وظيفة",
	"زيارة المؤسسات الحكومية",
	"الالتزام بالقانون",
	"التعرف على المدينة",
	"التفاعل مع المجتمع",
	"اختيار المستقبل"
]

const MISSION_REWARDS := [1000, 500, 500, 0, 2000, 1500, 2000, 1000, 1500, 0]
const GOVERNMENT_VISITS := ["البلدية والأحوال المدنية", "مركز الشرطة", "المستشفى", "البنك المركزي", "المحكمة"]
const CITY_DISCOVERY := ["المطار", "الميناء", "الحديقة العامة", "جامعة RP", "المنطقة التجارية"]

var mission_index := 0
var identity_issued := false
var bank_opened := false
var bank_card_received := false
var atm_used := false
var theory_passed := false
var practical_passed := false
var vehicle_purchased := false
var vehicle_registered := false
var first_maintenance_discount := false
var job_selected := false
var legal_earnings := 0
var government_visits: Dictionary = {}
var law_seconds := 0.0
var city_discovery: Dictionary = {}
var greetings := 0
var first_rp_transaction := false
var future_path := ""
var completed := false
var citizen_title := ""
var rental_home_until_unix := 0
var official_jobs_unlocked := false
var starter_vehicle_awarded := false
var completion_reward_claimed := false

func mission_name() -> String:
	if completed:
		return "برنامج المواطنين الجدد — مكتمل"
	return MISSION_NAMES[clampi(mission_index, 0, TOTAL_MISSIONS - 1)]

func objective() -> String:
	if completed:
		return "تم إكمال البرنامج. اختر مسارك داخل الجمهورية."
	match mission_index:
		0:
			return "اذهب إلى البلدية/الأحوال المدنية واستخرج بطاقة الهوية."
		1:
			if not bank_opened:
				return "اذهب إلى البنك المركزي وافتح حساباً."
			if not atm_used:
				return "استلم البطاقة البنكية ثم استخدم أول جهاز صراف."
			return "اكتملت إجراءات البنك."
		2:
			if not theory_passed:
				return "مدرسة القيادة: اجتز الاختبار النظري."
			return "مدرسة القيادة: اجتز الاختبار العملي."
		3:
			if not vehicle_purchased:
				return "وكالة السيارات: اشترِ أول مركبة."
			return "وكالة السيارات: سجّل المركبة باسمك."
		4:
			if not job_selected:
				return "مركز التوظيف: اختر وظيفة مدنية."
			return "حقق 5,000$ من العمل القانوني."
		5:
			var remaining := GOVERNMENT_VISITS.size() - government_visits.size()
			return "زر المؤسسات الحكومية المطلوبة — متبقّي %d." % remaining
		6:
			return "التزم بالقانون لمدة 30 دقيقة دون مخالفة."
		7:
			var remaining_city := CITY_DISCOVERY.size() - city_discovery.size()
			return "اكتشف معالم المدينة — متبقّي %d." % remaining_city
		8:
			return "حيِّ 5 مواطنين وأنجز أول معاملة RP."
		9:
			return "اختر مسارك: الشرطة، المستشفى، القانون، الحكومة، شركة، وظائف مدنية أو الإعلام."
	return ""

func interact(location: String, player_money: int) -> Dictionary:
	var result := {"advanced": false, "reward_money": 0, "reward_xp": 0, "message": ""}
	if completed:
		result.message = "برنامج المواطنين الجدد مكتمل."
		return result

	match mission_index:
		0:
			if location == "البلدية والأحوال المدنية":
				if not identity_issued:
					identity_issued = true
					result.reward_money = 1000
					result.reward_xp = 250
					result.message = "تم إصدار بطاقة الهوية الوطنية."
					_advance(result)
				else:
					result.message = "هويتك الوطنية مفعّلة."
		1:
			if location == "البنك المركزي":
				if not bank_opened:
					bank_opened = true
					bank_card_received = true
					result.message = "تم فتح الحساب واستلام البطاقة البنكية. استخدم الصراف الآن."
				elif not atm_used:
					atm_used = true
					result.reward_money = 500
					result.reward_xp = 250
					result.message = "تم استخدام أول جهاز صراف واستلام 500$ في الحساب."
					_advance(result)
				else:
					result.message = "حسابك البنكي والبطاقة مفعّلان."
		2:
			if location == "مدرسة القيادة":
				if not theory_passed:
					theory_passed = true
					result.message = "نجحت في الاختبار النظري. ابدأ الاختبار العملي."
				elif not practical_passed:
					practical_passed = true
					result.reward_xp = 500
					result.message = "نجحت في الاختبار العملي. رخصة القيادة صالحة."
					_advance(result)
		3:
			if location == "وكالة السيارات":
				if not vehicle_purchased:
					if player_money < 1500:
						result.message = "سعر مركبة البداية 1,500$. لا يوجد رصيد كافٍ."
					else:
						vehicle_purchased = true
						result.reward_money = -1500
						result.reward_xp = 150
						result.message = "تم شراء أول مركبة مقابل 1,500$. سجّلها الآن باسمك."
				elif not vehicle_registered:
					vehicle_registered = true
					first_maintenance_discount = true
					result.message = "تم تسجيل المركبة باسمك. حصلت على خصم 10% لأول صيانة."
					_advance(result)
		4:
			if location == "مركز التوظيف":
				if not job_selected:
					job_selected = true
					result.message = "تم تسجيلك في الوظيفة المدنية الأولى. ابدأ بجمع دخل قانوني."
				elif legal_earnings >= 5000:
					result.reward_money = 2000
					result.reward_xp = 500
					result.message = "أكملت شرط الدخل القانوني 5,000$. لقب المواطن المجتهد."
					_advance(result)
				else:
					result.message = "الدخل القانوني الحالي: %d$ / 5,000$." % legal_earnings
		5:
			if location in GOVERNMENT_VISITS:
				if not government_visits.has(location):
					government_visits[location] = true
					if government_visits.size() >= GOVERNMENT_VISITS.size():
						result.reward_money = 1500
						result.reward_xp = 1000
						result.message = "زرت البلدية والشرطة والمستشفى والبنك والمحكمة."
						_advance(result)
					else:
						result.message = "تم تسجيل زيارة %s." % location
		7:
			if location in CITY_DISCOVERY:
				if not city_discovery.has(location):
					city_discovery[location] = true
					if city_discovery.size() >= CITY_DISCOVERY.size():
						result.reward_money = 1000
						result.reward_xp = 500
						result.message = "اكتملت جولة التعرف على المدينة — خريطة المدينة أصبحت متاحة."
						_advance(result)
					else:
						result.message = "تم اكتشاف %s." % location
		8:
			if location == "مواطن":
				greetings = mini(greetings + 1, 5)
				if greetings >= 5 and first_rp_transaction:
					result.reward_money = 1500
					result.reward_xp = 500
					result.message = "أكملت التفاعل المجتمعي وحصلت على وسام المواطن الاجتماعي."
					_advance(result)
				else:
					result.message = "تحية مسجلة: %d / 5." % greetings
			elif location == "معاملة RP":
				first_rp_transaction = true
				if greetings >= 5:
					result.reward_money = 1500
					result.reward_xp = 500
					result.message = "أكملت أول معاملة RP وحصلت على وسام المواطن الاجتماعي."
					_advance(result)
				else:
					result.message = "المعاملة مسجلة. أكمل تحية 5 مواطنين."
		9:
			if location == "اختيار المستقبل":
				result.message = "اختر مسارك من واجهة المسار."
	return result

func add_legal_earnings(amount: int) -> void:
	if amount > 0:
		legal_earnings += amount

func tick_lawful(delta: float, wanted: int, traffic_violation: bool = false) -> void:
	if mission_index != 6 or completed:
		return
	if wanted > 0 or traffic_violation:
		law_seconds = 0.0
		return
	law_seconds += delta

func law_minutes() -> float:
	return law_seconds / 60.0

func complete_lawful() -> Dictionary:
	if mission_index != 6 or law_seconds < 1800.0 or completed:
		return {"success": false, "message": "شرط الالتزام بالقانون لم يكتمل بعد."}
	law_seconds = 1800.0
	var result := {"advanced": true, "reward_money": 2000, "reward_xp": 500, "message": "أكملت 30 دقيقة دون مخالفة. حصلت على شهادة المواطن الملتزم."}
	_advance(result)
	return result

func choose_future(path: String) -> Dictionary:
	var allowed := ["الشرطة", "المستشفى", "القانون", "الحكومة", "تأسيس شركة", "الوظائف المدنية", "الإعلام"]
	if mission_index != 9 or not allowed.has(path):
		return {"success": false, "message": "هذا المسار غير متاح الآن."}
	future_path = path
	citizen_title = "مواطن الجمهورية"
	official_jobs_unlocked = true
	completed = true
	completion_reward_claimed = true
	return {
		"success": true,
		"message": "اكتمل برنامج المواطنين الجدد. حصلت على لقب مواطن الجمهورية وفتحت جميع الوظائف الرسمية.",
		"reward_money": 10000,
		"reward_xp": 1500
	}

func _advance(result: Dictionary) -> void:
	result.advanced = true
	mission_index += 1
	if mission_index >= TOTAL_MISSIONS:
		mission_index = TOTAL_MISSIONS - 1
