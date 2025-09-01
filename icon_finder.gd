extends HTTPRequest

@export var discord:DiscordUpdater
@export var main:Control

var link:String

# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	discord.update_details.connect(check)
	request_completed.connect(on_request_completed)

func check():
	var input_link = main.current_share_link # https://www.youtube.com/watch?v=kky1L2jzc8c
	
	if input_link == '':
		DiscordRPC.large_image = "logotrans"
		return
	
	link = parse_url(input_link)
	
	if link != input_link:
		DiscordRPC.large_image = link
		return
	
	cancel_request()
	
	request(link, ["Content-Type: application/json"])

static func parse_url(url:String) -> String: # Process string without web request
	var converted_url:String = url
	if url.contains("www.youtube") || url.contains("https://youtu.be/"):
		converted_url = "https://i.ytimg.com/vi_webp/"
		converted_url += url.right(-"https://".length()).get_slice("/", 1).lstrip("watch?v=").get_slice("?", 0)
		converted_url += "/maxresdefault.webp"
	return converted_url

func on_request_completed(result:int, _response_code:int, _headers:PackedStringArray, body:PackedByteArray):
	if result != RESULT_SUCCESS:
		printerr(str("Https request failed. Something went wrong server side: ", result))
		return
	
	var text:String = body.get_string_from_utf8()
	
	if link.contains("open.spotify.com") || link.contains("soundcloud.com"):
		var meta:String = "<meta property=\"og:image\" content="
		
		text = text.substr(text.find(meta) + meta.length() + 1)
		text = text.substr(0, text.find('"'))
	elif link.contains("music.apple.com"):
		var meta:String = "<source sizes=\" (max-width:1319px) 296px,(min-width:1320px) and (max-width:1679px) 316px,316px\" srcset=\""
		
		text = text.substr(text.find(meta) + meta.length())
		text = text.substr(0, text.find(' 296w,https://'))
	
	DiscordRPC.large_image = text if text != '' else "logotrans"
