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
* Entities and pooling (Work in progress)
* Particles
* Rudimentary sound effects and music playback
* Windows, Linux and MacOS support (Work in progress)

![Animgif](http://i.imgur.com/R4jI3h0.gif)

There's a mish-mash of code from various sources and I haven't cleaned any of it so don't go expecting consistent code quality here! 

* Nick Gravelyn wrote the Tiled content pipeline lib. Can't find a link to the original code, but I've been using it for years!
* The Spine library comes from the [official spine-runtimes repo](https://github.com/EsotericSoftware/spine-runtimes)
* Gamestate Management from the above link

Usage and Contributing
----------------------
You can use my code for anything you like without credit. You may need to take a look at licences for the Gamestate Management sample and the Spine runtimes if you use those.

I'll take a look at any PRs submitted - just remember that the idea is to keep the engine flexible!

Send me a tweet [@garethiw](http://twitter.com/garethiw), and throw out a tweet linking this repo if you like it/use it!

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

This engine won't make your game for you - I'm assuming you're here because you have experience with XNA and/or Monogame and making 2D games.

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

You still need to write actual collision code for your game entities, but you'll be able to check whether a pixel is collidable or not by using map.CheckCollision(V2 position) and reacting accordingly. See an example in ParticleController.Update().

Camera
------

The camera is a pretty basic "virtual" camera class. It can be intialised by passing a Tiled Map to the contructor, or by setting a width and height of the boundary. Think of the camera as a rectangle sitting inside another rectangle. The camera has a width, height and position (position is always the center of the camera) and is clamped to the outer boundary.

To move the camera, set the Target and the position will lerp smoothly toward it. If you want to instantly move the camera to a new location, set the Position and the Tartget to the same value.

The camera updates a CameraMatrix, which can be passed into a SpriteBatch to neatly handle scrolling for you. Just make sure you draw all your in-world entities using a spritebatch with CameraMatrix.

You can set Rotation (radians) and Zoom (float scale) for fun! Also, try calling Shake(milliseconds, float amount) to Juice It Up.

Timers
------

Using timers is pretty straightfoward. If you've ever used setInterval or setTimeout in JS, then you're golden.

```C#
TimerController.Instance.Create(name, callback, milliseconds, looping);

// example:
TimerController.Instance.Create("shake", () => camera.Shake(500, 2f), 3000, true);
```

If you do not create a looped timer (looping=true) then the timer will fire once and then be destroyed.

You can use timers as simple fire-and-forgets, or you can assign them to a local variable and then use Pause(), Resume(), Reset() and Kill() to manipulate them.

Tweening
--------

A simple yet powerful tweening engine. Works a little like timers, but the callback function is called every time the tween updates. A tween is simply a way of getting from 0f to 1f (and back again if needed), in a stylish fashion.

```C#
TweenController.Instance.Create(name, tween function, callback, milliseconds, pingpong, looping);

// example:
TweenController.Instance.Create("spintext", TweenFuncs.Linear, (tween) =>
            {
                textRot = MathHelper.TwoPi * tween.Value;
            }, 3000, false, true);
```

A tween that has "pingpong" set will go from 0f to 1f and then back to 0f again. It will do that in the supplied millisecond time (rather than taking 2*milliseconds).

A non-pingponging looped tween will go from 0f to 1f and then reset directly to 0f.

Here are the built-in tweening functions:

* Linear
* QuadraticEaseIn
* QuadraticEaseOut
* QuadraticEaseInOut
* CubicEaseIn
* CubicEaseOut
* CubicEaseInOut
* QuarticEaseIn
* QuarticEaseOut
* QuarticEaseInOut
* QuinticEaseIn
* QuinticEaseOut
* QuinticEaseInOut
* SineEaseIn
* SineEaseOut
* SineEaseInOut
* Bounce
* Elastic

Sprite Animation
----------------

![](http://i.imgur.com/qFsWt4i.png)

This is a really bog-standard animation sheet support with no frills! Each instance of SpriteAnim represents ONE row in a spritesheet. There is currently no support for multi-row animations or sheets with cell padding/margins.

There are a few different overloads on the SpriteAnim constructor, here's the most verbose:

```C#
SpriteAnim(Texture2D sheet, 
           int row, int numframes, 
           int width, int height, 
           double frametime, 
           Vector2 offset, 
           bool loop, bool pingpong, 
           int startframe)

// example:
_runAnim = new SpriteAnim(spritesheet, 1, 7, 16, 16, 60, new Vector2(8f,16f), true, false, 0);
```

Mostly straightforward. Row is the row number of the animation in the spritesheet texture. Width and Height are the size of each cell of animation.

Offset works in the same way as the SpriteBatch.Draw origin. So an offset of Vector2.Zero would the top-left of the frame at the location supplied to SpriteAnim.Draw().

pingpong=true plays the anim in reverse once it reaches the last frame.

SpriteAnim has a default state of Paused. You can use SpriteAnim.Play(), Pause() and Reset() to control playback.

See Hero.cs for a simple example.

Particles
---------

The particle engine is built to be as flexible as possible. One particle texture "sheet" is used for all particles, and each particle can define its source rectangle from within the particle texture.

Particles have Attack, Life and Decay times and Values. For example, a particle with the following values:

AttackTime = 1000;
LifeTime = 5000;
DecayTime = 1000;

Will spend one second coming into existence, five seconds alive, and then one second disappearing.

During each stage of the particle's life, you can measure how far through it is with the Value vars:

AttackValue
LifeValue
DecayValue

Each stage will take the corresponding amount of time to go from 0f to 1f. The point of this is so that you can use a callback function to control exactly how the particle behaves during each stage of its life:

```C#
public static void FadeInOut(Particle p)
{
    switch (p.State)
    {
        case ParticleState.Attack:
            p.Alpha = p.AttackValue;
            break;
        case ParticleState.Alive:
            p.Alpha = 1f;
            break;
        case ParticleState.Decay:
            p.Alpha = 1f - p.DecayValue;
            break;
    }
}
```

You can create your own Particle Callbacks in the static ParticleFunctions class (ParticleController.cs). Because they're static functions, we're not allocating memory for each particle callback.

To add a particle:

```C#
ParticleController.Instance.Add(Vector2 spawnPos, Vector2 velocity, 
                                double attackTime, double lifeTime, double decayTime, 
                                bool affectedbygravity, bool canCollide, 
                                Rectangle sourcerect, Color col, 
                                Action<Particle> particleFunc, 
                                float startScale, float startRot, 
                                int depth, ParticleBlend blend)

// example:
ParticleController.Instance.Add(new Vector2(17, 40), new Vector2(Helper.RandomFloat(2f), -1.5f),
                                100, 3000, 1000,
                                true, true,
                                new Rectangle(0, 0, 2, 2),
                                new Color(new Vector3(1f, 0f, 0f) * (0.25f + Helper.RandomFloat(0.5f))),
                                ParticleFunctions.FadeInOut,
                                1f, 0f,
                                1, ParticleBlend.Alpha);
```

Phew, that's a lot of parameters! They're mostly straightforward, so I'll explain the ones that need it.

When canCollide is set, it'll make the particle collide with the Tiled map per-pixel collision detection. If you're not using a Tiled map, you could add your own collision routines in ParticleController.Update()

SourceRect is the source rectangle for SpriteDatch.Draw. Add your own particles to particles.png and set the rectangle appropriately.

particleFunc sould be set to one of the static function in ParticleFunctions (or your own).

depth is simply a way of grouping particles so that you can draw them in the right order in your scene. Supply depth when you call ParticleController.Draw() and only the particles with the same value for depth will be drawn.

The ParticleBlend enum sets one of three BlendStates for use when drawing the particle:

 * Alpha - uses BlendState.Alpha. For drawing standard particles.
 * Additive - uses BlanedState.Additive. Useful for drawing smoke/cloud particles
 * Multiplicative - uses a custom 2x multiplicative blend state. See [Shawn Hargreave's blog](http://blogs.msdn.com/b/shawnhar/archive/2011/08/01/1401282.aspx) for an example. You can use this mode to do crude lighting effects with particles - see the ExampleGameplayScreen for a couple of uses.
 
Entities and Pooling
--------------------

I've included a very basic pooling system for your in-game objects. You'd use pooling for any game entity that you'd expect to have more than one of - enemies and pickups for example. Of course, you don't need to use these classes -  they're there because I'm going to make use of them!

There's a very basic Entity class (Entities\Entity.cs) to inherit into your own game classes. See Hero.cs for an example of an inherited class. The base Entity contains overloads for drawing, updating and handling input. You can use an object derived from Entity without adding it to a pool - your main player class might be a standalone entity for instance.

The base Entity holds a position vector2, a speed vector2, a hitbox rectangle (to be used for collision detection), and a hitbox offset vector. Be default the hitbox will be centered at the entity's position - you can change this by supplying an offset.

The base entity also has an Active flag (defaults to false) which is how the Pool knows which items are currently in use. Any entities in the pool which are Active==false are available for re-use.

Entity pools are initialised with a fixed capacity that you would set as needed. The entity pool also keeps hold of the sprite sheet (Texture) that will be used by all of its children to draw themselves.

To create a new pool:

```C#
heroPool = new EntityPool(100, 
                          sheet => new Hero(sheet, new Rectangle(0, 0, 10, 10), new Vector2(0, -5)),
                          content.Load<Texture2D>("testhero"));
```

So a little explanation here. The first parameter is the maximum capacity of the pool (this pool will allocate 100 Heroes).

The second parameter is a delegate function that should return a new instance of the entity we wish to pool. In this case, we're creating a new Hero. The sprite texture held by the Pool is provided so that we can pass it through into our Hero entiy's constructor (which in turn passes it into the base Entity constructor). We also set up the size and offset of the entity's Hitbox rectangle. This function will be called the number of times you passed in the first param, each time adding the result to the pool's Entities list.

The last parameter is the spritesheet itself, in this case loaded via our contentmanager, but you may already have a texture ready.

To spawn an entity, use the EntityPool.Spawn method:

```C#
heroPool.Spawn(entity =>
               {
                   entity.Position = new Vector2(Helper.Random.Next(ScreenManager.Game.RenderWidth-64)+32, 32);
                   ((Hero)entity).FaceDir = Helper.Random.Next(2) == 0 ? -1 : 1;
               });
```
 
Use the delegate here to initialise the "new" entity. In this case, we're setting a spawn position and a facing direction. Note that you'll need to cast the entity to the pool's entity type to access the overloaded methods or additional fields.
 
The Entity class has a Reset()method you should override to set default values when an entity in the pool is re-used. For example:

```C#
public override void Reset()
{
    Life = MAX_LIFE;
    Speed = Vector2.Zero;

    base.Reset();
}
```

The reset method is called before the delegate function supplied to the Spawn method.

See ExampleGameplayScreen.cs and Hero.cs for an exampl;e use of the entity and pooling system.

TODO: Entity-entity collision detection (soon!)
 
Audio
-----

The AudioController is a barely-worth-mentioning static class for playing sound effects from wherever you are in the code. It also handles playback of "music", which are just looped SoundEffectInstances.

The AudioController is initialised (LoadContent) right in the main Game.cs LoadContent method. There are two string-keyed dictionaries (_effects and _songs). _effects is the sound effect dictionary and holds SoundEffects, while _songs holds SoundEffectInstances which are defaulted to IsLooped.

Add your effects and songs in AudioController.LoadContent() - comments provided to help you.

There are several overloads for AudioController.PlaySFX. The one I use the most allows you to pass the Camera and a Position and will calculate a pan relative the the center of the camera. The effect won't play if the position is outside the bounds of the camera, so you can use this call to emit sounds from any of your in-game objects without worrying about where they are.

More
----

The Spine runtimes should all work okay, but you'll need to head over to the Spine site (link at the top) to get more info on their usage. I have used the included code several times in my own projects just fine.
