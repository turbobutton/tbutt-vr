# TButt
A lightweight multiplatform interface for making VR games in Unity.

Visit our site at http://www.tbutt.net for documentation and more.

## BETA Branch Warning
This branch includes work-in-progress features that may be undocumented and untested, and is only recommended if it has functionality you need for your project. We don't recommend using the beta branch in a shipping product. Things can (and probably) will change between BETA and release.

### BETA Features
* Optional Steam VR Plugin v2.2.0 Support (including Valve Index controllers). After importing Steam VR Plugin v2.2.0, make sure you allow TButt's actions to be merged into or replace your existing action set. TButt's actions are required for TButt to work with the new input system.

* Input Debugger. While in play mode in any scene, open the new input debugger window to see what values TButt is reading from your active controller.

* TBTracking Overhaul. TBTracking is being updated to add more features for room scale VR, like detecting play space size and determining whether or not objects are inside the play space.

### Known Issues
* If Index controllers aren't turned on when TButt initializes, they may be recognized as Vive controllers instead
* Windows Mixed Reality controllers are not currently functional with TButt when using Steam VR Plugin v2.2.0.
* Gamepads are not currently functional with TButt when using Steam VR Plugin v2.2.0.
* Certain settings around the Index headset may not be detected correctly yet.
* Play area detection is not currently functional with Steam VR.
