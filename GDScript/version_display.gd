extends LinkButton

func _ready() -> void:
	text = str("simplesound ", ProjectSettings.get_setting("application/config/version"))
