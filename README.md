# Cybots!

I made Cybots as part of my Com S 437 project. The game is built around the enemy cybots which are behavior based AI each with a uniqe set of behaviors that govern how they move around the map and how they pursue the player.


<image src="docs/images/Maze.png" height=300 />

*An image of the maze shortly after the game starts*

## Gameplay

### Objective 
Move through the maze while being hunted by the Cybots and try to reach the end. If you are caught and have too many Cybots near you; you are sent back to the start. 

### Cybots (and behaviors):
*In Spawn order (ex. If only two cybots are set to spawn it will be the first two)* 

**QuarterbackCy** \
This cybot runs between the four corners of the map. If it sees the Player it will start to chase them until it gets close enough or it loses the player and then resumes moving between the corners. You can tell it is nearby by the sound of his workout music

**SentryCy** \
This cybot likes to play the waiting game. It will choose a random spot on the map, go to it, and wait for the player. If it sees the it will start to chase them. It will chase them until it loses the player or captures them. If it loses the player it will find a new spot to guard. You can know it is around if you hear the mechanical sounds of it's heavy body.

**SlackerCy** \
This cybot likes to piggyback off the others work. It will stay idle until another Cybot starts chasing the player; at which point it will start running towards the player and try to chase them. However if the chase stops it will return to its waiting. You can tell it's around by the beeps of the video game console it forgot to turn off.

**WandererCy** \
This cybot is a little bit aimless. It will repeatedly move to random spaces in the maze; stopping to chase the player if it sees them and returning to its wandering afterwards. You can tell if it's wandering nearby you by listening for its footsteps.


**HunterCy** \
This is the oldest and most experienced cybot. It has honed its senses to the point where it always knows where the player is. However its gotten a little bit slow in it old age and can only walk. This cybot walks towards the players current position and starts to chase the player at a walking speed when it sees them. You will know it's gaining on you when you hear it's mechanical laughter. 
<br/><br/>
| The HunterCy Cybot | Two Cybots chasing the player |
| :----------------: | :---------------------------: |
| ![A blue cybot chasing the player](docs/images/HunterCy.png) | ![Two cybots chasing the player](docs/images/MultipleCybots.png) | 

### Powerups
The player can encounter different powerups to help them against the Cybots while exploring the maze

**Light Up** (Yellow Capsule) \
Walking into a yellow capsule will turn on the directional light and allow the player to see better for 10 seconds.

**Invincibility** (Green Capsule) \
Walking into a green capsule will turn make the player immune to the Cybot's freeze rays for 30 seconds.

<image src="docs/images/PowerUp.png" height=300 />

*One of the game's powerups*

## Acknowledgments
- Cybot Models/ Animations - James Lathrope 
- Cybot Animations - Mixamo 
- Ice Texture - Alexander Pogorelsky on unsplash.com 
- Ground Texture - Wesley Tingey on unsplash.com  
- Wall Texture - Simon Berger on unsplash.com 
- HunterCy Sound Effect - copyc4t on freesound.org 
	- TF_Robotic-Laughter-2.flac by copyc4t -- https://freesound.org/s/235739/ -- License: Attribution 4.0 
- QuarterBackCy Sound Effect - Migfus20 on Freesound.org \
*Edited to remove lead in and fade out* 
	- Rock Background Music by Migfus20 -- https://freesound.org/s/560443/ -- License: Attribution 4.0 
- SentryCy Sound Effect - qubodup on Freesound.org 
	- Tank or Tower Turret Moving and Turning by qubodup -- https://freesound.org/s/171657/ -- License: Creative Commons 0 
- HunterCy Sound Effect - morganpurkis on Freesound.org 
	- Concrete Footstep 1.wav by morganpurkis -- https://freesound.org/s/384635/ -- License: Attribution 4.0
