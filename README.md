LDEngine
========

LDEngine is a 2D pixelart engine/framework for XNA and [Monogame](https://github.com/mono/monogame). I created it to use in Ludum Dare jams, thus the name. The idea is to provide basic support for all the following stuff, while remaining flexible enough to use for virtually any type of game:

* Nearest-neighbour display scaling
* [Tiled](http://mapeditor.org) map support with per-pixel collision detection
* Camera with rotation and zoom
* Timers and Tweening
* Sprite animation
* [Spine](http://esotericsoftware.com/) animation
* Gamestate management using a lightly-modified version of the good old [Game State Management sample](http://xbox.create.msdn.com/en-US/education/catalog/sample/game_state_management)
* Entities and pooling
* Particles
* Sound effects and music playback
* Windows, Linux and MacOS support

![Animgif](http://i.imgur.com/R4jI3h0.gif)

There's a mish-mash of code from various sources and I haven't cleaned any of it so don't go expecting consistent code quality here! 

* Nick Gravelyn wrote the Tiled content pipeline lib. Can't find a link to the original code, but I've been using it for years!
* The Spine library comes from the [official spine-runtimes repo](https://github.com/EsotericSoftware/spine-runtimes)
* Gamestate Management from the above link

Building
--------

(Windows)
Requires:
* VS2012 or 2013 (only tested on 2012 but 2013 should be fine)
* XNA Game Studio - can be installed using the [unofficial installers](http://msxna.codeplex.com/releases)
* Font is included in the dependencies dir.

The engine can be compiled with either the standard XNA libraries or Monogame, simply by swapping out the references. The main game project currently references the supplied Monogame .dll which is a recent build of the WindowsGL port. 

Full support for Mac and Linux will be along shortly. I'm 99% sure it'll run on Linux right now by swapping the dll out for a compiled Monogame Linux dll. I haven't tried building it on Mac yet.

Usage
-----

This engine won't help you make a game - I'm assuming you're here because you have experience with XNA and/or Monogame and making 2D games.

I have provided an ExampleGameplayScreen, which the main Game.cs class loads by default. It shows how to load a map, spawn a "Hero", add particles and use the tweening and timer functions.

In Game.cs you can set which screen to load at startup (I've inlcuded some comments to help with that)

You'll probbaly want to start adding your code to GameplayScreen.cs

Tiled
-----

Tiled maps should load out of the box when compiled using the Tiled pipeline extension. You must ensure your layer data (Map->Map Options->Layer format) is Base64 GZip compressed.

To set up the per-pixel collision detection:
* On each of the tilesheets you want to generate collision data for:
  - Select the first (top-left) tile
  - Right-click -> Tile Properties
  - Add a new property: CollisionSet = True

![](http://i.imgur.com/tNn5eJ5.png) 
  
* Then, for each layer you would like to enable collision data on:
  - Right-click -> Layer Properties
  - Add a new property: Collision = On

![](http://i.imgur.com/gdrUCaO.png) 

Camera
------

The camera is a pretty basic "virtual" camera class. It can be intialised by passing a Tiled Map to the contructor, or by setting a width and height of the boundary. Think of the camera as a rectangle sitting inside another rectangle. The camera has a width, height and position (position is always the center of the camera) and is clamped to the outer boundary.

To move the camera, set the Target and the position will lerp smoothly toward it. If you want to instantly move the camera to a new location, set the Position and the Tartget to the same value.

The camera updates a CameraMatrix, which can be passed into a SpriteBatch to neatly handle scrolling for you. Just make sure you draw all your in-world entities using a spritebatch with CameraMatrix.

You can set Rotation (radians) and Zoom (float scale) for fun! Also, try calling Shake(milliseconds, float amount) to Juice It Up.

