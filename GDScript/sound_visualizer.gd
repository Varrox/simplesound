extends Control

@export var bar_count:int = 6
@export var max_height = 4*7
@export var min_height = 4

@export var bars:Array[Control]

const MAX_FREQ:int = 500

static var spectrum_instance:AudioEffectSpectrumAnalyzerInstance

func _ready() -> void:
	if !spectrum_instance:
		spectrum_instance = AudioServer.get_bus_effect_instance(0, 0)

func _process(delta: float) -> void:
	if !spectrum_instance || !visible:
		return
	
	for i in range(bar_count):
		var freq_start = (i * MAX_FREQ) / bar_count
		var freq_end = ((i + 1) * MAX_FREQ) / bar_count
		var magnitude_stereo = spectrum_instance.get_magnitude_for_frequency_range(freq_start, freq_end)
		var magnitude = lerp(magnitude_stereo.x, magnitude_stereo.y, 0.5)
		var height = (magnitude * 75) / db_to_linear(GDScriptGlobals.audio_source.volume_db)
		
		bars[i].size.y = Helper.smooth(bars[i].size.y, minf(min_height + height, max_height), 40)
		bars[i].position.y = -bars[i].size.y / 2
