# TButt
A lightweight multiplatform interface for making VR games in Unity.

Visit our site at http://www.tbutt.net for documentation and more.

## BETA Branch Warning
This branch includes work-in-progress features that may be undocumented and untested, and is only recommended if it has functionality you need for your project. We don't recommend using the beta branch in a shipping product. Things can (and probably) will change between BETA and release.

### BETA Features
* Optional Steam VR Plugin v2.2.0 Support (including Valve Index controllers). After importing Steam VR Plugin v2.2.0, make sure you allow TButt's actions to be merged into or replace your existing action set. TButt's actions are required for TButt to work with the new input system.

### Known Issues
* If Index controllers aren't turned on when TButt initializes, they may be recognized as Vive controllers instead
* Full finger presence for Index controllers is not yet implemented. Finger tracking is still limited to index finger, thumb, and grip values.
* Windows Mixed Reality controllers are not currently functional with TButt when using Steam VR Plugin v2.2.0.
* Gamepads are not currently functional with TButt when using Steam VR Plugin v2.2.0.
* Certain settings around the Index headset may not be detected correctly yet.