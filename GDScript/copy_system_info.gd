extends Button

func _ready() -> void:
	button_up.connect(copy_info)

func copy_info():
	var operating_system:String = get_operating_system()
	var software_version:String = get_software_version()
	var gpu:String = get_gpu()
	var cpu:String = get_cpu()
	var memory:String = get_memory()
	
	var to_copy:String = " - ".join([operating_system, software_version, gpu, cpu, memory])
	
	DisplayServer.clipboard_set(to_copy)

func get_operating_system() -> String:
	var str:String = OS.get_version_alias()
	return str(OS.get_name(), ' ', str.substr(0, str.find(' ')))

func get_software_version() -> String:
	return str("simplesound ", ProjectSettings.get_setting("application/config/version"))

func get_gpu() -> String:
	return str(RenderingServer.get_video_adapter_name(), ' (', RenderingServer.get_video_adapter_vendor(), '; ', OS.get_video_adapter_driver_info()[1], ')')

func get_cpu() -> String:
	return str(OS.get_processor_name(), " (", OS.get_processor_count(), " threads)")

func get_memory() -> String:
	return str("%.2f" % (OS.get_memory_info()["physical"] / 1073741824.0), "GiB memory")
