# SSL Syntax

The syntax is a mix of both CSS and JSON. The only difference is that it has function like syntax like CSS, but it doesn't require semi colons at the end of any line like JSON.  

SSL files can hold multiple different data types, such as classes, structs, dictionaries, arrays, and lists.  
Files can optionally be parsed for one object or an array of objects.  

## SSL Tags

At the start of each SSL file, you can define different tags that the interpreter can gather, it is similar to arguments you pass to an application.  
To create a tag, you just type `#` then the text you want to put into that tag. There is no other syntax to them, so they are just outputted as they are to the program, and the developer has to do their own parsing on it, this is for more freedom.  

Tags are very useful, for example, you can make a tag to store the version of a program so you can see any conflictions between the ssl file and the current program version. it would look like this  
`# version 1.0`  

## SSL Variables

To set data, you put the name of the variable you want to set, then a colon `:` and the value you want to set it to.  

NOTE: Strings do need quotation marks to be defined in this case, but not in classes / structs where their type is known.  

Setting variables should look like this  

```ssl
playlist_index : 8
song_index : 136
time : 139.12267
volume : -12.84
shuffled : true
```

## SSL Array / List Definition

SSL when it comes to defining arrays / lists, a new line is a separator, and a semicolon is one too, along with the traditional comma.
Also arrays and lists are the defined the same way, in or outside of classes / structs.

Defining an array / list should look similar to this  

```ssl
Songs : [
	D:\Songs\Songs of Innocence\The Miracle.mp3
	D:\Songs\Songs of Innocence\Every Breaking Wave.mp3
	D:\Songs\Songs of Innocence\California.mp3
	D:\Songs\Songs of Innocence\Song for Someone.mp3
	D:\Songs\Songs of Innocence\Iris.mp3
	D:\Songs\Songs of Innocence\Volcano.mp3
]
```

## SSL Dictionary Definition

Dictionaries use the same syntax as arrays, but instead of using brackets `[]`, it uses curly brackets `{}`.  
Also you define keys with values, just like variables, with a colon inbetween.  

NOTE: If you are using a string as the key value, a semicolon will automatically turn everything after into the value, so to include a semicolon, you need to add a backslash first `\\` or put quote marks around the string  

Here is an example of a dictionary:  

```ssl
item_prices : Dictionary {
	"sword" : 10
	"healing potion" : 12.5
	"fireball" : 25
}
```

## SSL Class / Struct Definition

To define a class or struct in SSL, you must put a name for accessing the class, then you must put the name of the class / struct then curly brackets.  

Like this  

```ssl
cool_playlist : Playlist
{
	// Stuff
}
```

To set data inside the class, you put the name of the variable you want to change, then a colon `:` and the value you want to set it to.  

NOTE: Strings do not need quotation marks to be defined, if the type of the variable is a string, it will just set the string to the variable.  

Setting variables should look like this  

```ssl
songs_of_innocence : Playlist
{
	Name : Songs of Innocence
	Artist : U2
	Cover : D:\Songs\Songs of Innocence.png
}
```

As you saw in the first example,  
SSL has code comments too  

```ssl
Cover : D:\Songs\Songs of Innocence.png // Wtf even is this cover?
```

## SSL Constructors

You can define classes and structs using function like syntax given that it has a constructor on the code end.  
it is just like how many normal programming languages do it, so it doesn't require much detail.  

```ssl
Overlay-Color : rgb(255, 0, 0) // Red
Volume-Reactive : {Overlay-Color : bool3(true, false, false)}
```
