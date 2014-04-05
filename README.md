LDEngine
========

LDEngine is a 2D pixelart engine/framework for XNA and Monogame. I created it to use in Ludum Dare jams, thus the name. The idea is to provide basic support for all the following stuff, while remaining flexible enough to use for virtually any type of game:

* Nearest-neighbour display scaling
* [Tiled](http://mapeditor.org) map support with per-pixel collision detection
* Camera with rotation and zoom
* Timers and Tweening
* Sprite animation
* [Spine](http://esotericsoftware.com/) animation
* Gamestate management using a lightly-modified version of the good old [Game State Management sample](http://xbox.create.msdn.com/en-US/education/catalog/sample/game_state_management)
* Particles

There's a mish-mash of code from various sources and I haven't cleaned any of it so don't go expecting consistent code quality here! 

* Nick Gravelyn wrote the Tiled content pipeline lib. Can't find a link to the original code, but I've been using it for years!
* The Spine library comes from the [official spine-runtimes repo](https://github.com/EsotericSoftware/spine-runtimes)
* Gamestate Management from the above link

Building
--------

Requires:
* VS2012 or 2013 (only tested on 2012 but 2013 should be fine)
* XNA Game Studio - can be installed using the [unofficial installers](http://msxna.codeplex.com/releases)
* Required font is included in the dependencies dir.

The engine can be compiled with either the standard XNA libraries or Monogame, simply by swapping out the references. The main game project currently references the supplied Monogame .dll which is a recent build of the WindowsGL port. 

Full support for Mac and Linux will be along shortly. I'm 99% sure it'll run on Linux right now by swapping the dll out for a compiled Monogame Linux dll. I haven't tried building it on Mac yet.

