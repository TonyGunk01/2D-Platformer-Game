# 2D-Platformer-Game

This is a simple 2D platformer game made in Unity2D and code in C#. I have used the Animator to load in sprites for all the characters (Ellen, Chomper, Gunner) and then spaced out time keys to ensure smooth animation rendering. After that, using Animator states, I controlled the flow of animations based on the status of the object. Eg: when dead is true, all the existing animations stop and death animation plays once and signals 'Game Over'.

The environments for the game was designed using tile maps and layers. Tile maps helped in speeding up the process to place platform blocks, Spikes and background plants and vines in the scene. It made it easier to set rigidbody properties for collisions. With the help of layers, I could make the background plants seem to be in a somewhat further distance from the camera POV than the player character hence it gives an illusion of 2.5D.

There are three types of ways in which the player can die: contact with a chasing Chomper, shot by a Gunner or jumping onto Spikes. There are also coins to be collected which gives a challenge to the player even though they aren't necessary for game completion.

I then created a UI Lobby system that allowed you to select levels only when you've unlocked them (eg: Level 3 is unlocked only if you finish Level 2). This can be accessed for replayability to collect coins.

[Click here for video demo:](https://youtu.be/2KJWq5rU834)
