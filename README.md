# FourBarLinkage
Animation of a four-bar linkage

![MainScreen](http://www.hilab.it/FBL)

This package simulates the displacement of a four-bar linkage (https://en.wikipedia.org/wiki/Four-bar_linkage) creating 
a Web Server in C# that 
* receives the linkage parameters (side length and more)
* creates an octave script (https://en.wikipedia.org/wiki/GNU_Octave) and launches the octave interpreter to execute it
* receives the computed results (time dependent state) 
* creates an html page on the fly that shows the animation executed by javascript embedded code.
* sends the page back to the client computer.

# Installation
The provided solution can be compiled and run under linux/MonoDevelop and supposes the octave packages is installed.
It should be usable on Windows as well, directly compilable under VisualStudio but needing minor modifications to the **PlaneFourBarLinkage.OctaveConfigurationComputer** to select the octave executable.

# User manual
The server runs as a console app and connects to the TCP/IP port 1234 responding to the HTTP protocol.

Once che command line is executing, open a browser to http://localhost:1234/ to show a web page similar to the following.

![fig1](https://github.com/fjovine/FourBarLinkage/blob/master/Doc/FBL_01.png)

The top bar contains

* I - Four text fields to select the sizes of the four bars, namely 

|   | Description                    |
|---|--------------------------------|
| a | size of the left rotating bar  |
| b | size if the floating bar       |
| c | size of the right rotating bar |
| d | size of the fixed ground bar   |

* II - The coordinate of a point integral to the floating bar (with respect to a system of coordinates moving with it
* III - + and - buttons to zoom in and out the pigrures
* IV - Hyperlinks to show four predefined configurations following the Grashof classification (see wiki) The "Pic"
hyperlink simply shows the simulation without control fields for documentin porposes.

![fig2](https://github.com/fjovine/FourBarLinkage/blob/master/Doc/FBL_02.png)

This figure shows additional geometrical elements, as they are referenced by the theretical description.

|    | Description
|----|------------
| V  | Trajectory of the left rotating hinge, it can be the whole circumference or arcs of it 
|    | symmetrical with respect to the ground bar
| IV | Trajectory of the right rotating hinge, it can be the whole circumference or arcs of it 
|    | symmetrical with respect to the ground bar
| P  | The coordinate system integral to the floating bar connecting the two rotating hinges
|    | determines the point P (blue in the figure) that moves as the four-bar linkage moves on the trajectory shown.

# Known bugs
In some configuration, notably when the floating bar is aligned with one of the rotating bars, the numerical algorithm is instable due to numerical errors and this that turns into oscillations of the blue trajectory.
