# SSL Syntax

The syntax is very similar to CSS and JSON. The only difference is that it has function like syntax, and it does not work with semi colons at the end of each line.  

At the start of each SSL file, you want to define parameters about the playlist such as name, artist, and type.  
You can define these parameters in the Config{} area.

Here is an example of that

```ssl
Config
{
	Name : Songs of Innocence
	Artist : U2
	Cover : D:\\Songs\\Songs of Innocence.png
}
```

In the backend of SSL it is just setting the parameters of the Playlist class that simplesound has, so you can set anything really, but it is not suggested since the parsing does not have the ability to parse arrays.

SSL has code comments too

```
Cover : Songs of Innocence.png // Wtf even is this cover?
```

SSL when it comes to defining lists (it did just say it can't understand them, but it can in this way) like the songs in a playlist  
It is much different. You just list them line by line, nothing else.

```
Songs
{
	D:\\Songs\\Songs of Innocence\\The Miracle.mp3
	D:\\Songs\\Songs of Innocence\\Every Breaking Wave.mp3
	D:\\Songs\\Songs of Innocence\\California.mp3
	D:\\Songs\\Songs of Innocence\\Song for Someone.mp3
	D:\\Songs\\Songs of Innocence\\Iris.mp3
	D:\\Songs\\Songs of Innocence\\Volcano.mp3
}
```

Each song can have their own individual info, but it is saved in their metadata, and has separate syntax from SSL to make it easy to edit outside of simplesound.  

Songs can also just include full folders too.

```
Songs
{
	D:\\Songs\\Songs of Innocence
}
```
