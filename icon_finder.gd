extends HTTPRequest

@export var discord:DiscordUpdater

@export var song:Label
@export var artist:Label

var state:int = 0 # 0 : Get link, 1 : Get cover

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	discord.update_details.connect(check)
	request_completed.connect(on_request_completed)

func check():
	var song_name = song.text.strip_edges().replace(' ', '%20')
	var artist_name = artist.text.strip_edges().replace(' ', '%20')
	
	#var link = "https://open.spotify.com/track/6xV7Be6XEvkSnighmh2Tzj?si=87047ecdb3bc4ef3"
	
	#var headers = 
	#request(link, ["Content-Type: application/json"])

func on_request_completed(result:int, _response_code:int, _headers:PackedStringArray, body:PackedByteArray):
	if result != RESULT_SUCCESS:
		printerr(str("Https request failed. Something went wrong server side: ", result))
		return
	
	var meta:String = "Top Result"#"<meta property=\"og:image\" content="
	
	var text:String = body.get_string_from_utf8()
	#text = text.substr(text.find(meta))
	#text = text.substr(0, text.find('"/>'))
	
	#DiscordRPC.large_image = text
