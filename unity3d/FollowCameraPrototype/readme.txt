A simple prototype to experiment with a camera that follows the terrain and is able to dolly in/out.

Setup:
1. Create a camera.
2. Attach the (standard) "Smooth Follow" script to the camera.
2.1 Set the "target" to the object you want to follow.
2.2 Set "Height damping" to some high value so height adjustment is pretty much immediate, if not it makes zooming in/out rotate the camera.
3. Attach the "Camera Manager" script.
3.1 Set "Smoother" to the "Smooth Follow" you added in #2.
3.2 Tweak all other "Camera Manager" props to your liking.