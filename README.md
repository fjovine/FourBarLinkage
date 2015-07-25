# FourBarLinkage
Animation of a four-bar linkage

![MainScreen](http://www.hilab.it/FBL)

This package simulates the displacement of a four-bar linkage (https://en.wikipedia.org/wiki/Four-bar_linkage) creating 
a Web Server in C# that 
* receives the linkage parameters (side length and more) from the client
* creates an octave script (https://en.wikipedia.org/wiki/GNU_Octave) and launches the octave interpreter to execute it
* receives the computed results (time dependent state) 
* creates an html page on the fly containing javascript embedded code.
* sends the page back to the client computer where the script shows the animation.

# Installation
The provided solution can be compiled and run under linux/MonoDevelop. Prerequisite is that the octave package is installed.
It should be directly usable on Windows as MonoDevelop solutions can be compiled under VisualStudio. Probably it needs minor modifications to the **PlaneFourBarLinkage.OctaveConfigurationComputer** class to launch the Windows version of octave.

# User manual
The server runs as a console app and connects to the TCP/IP port 1234 responding to the HTTP protocol.

Once launched, open a browser to http://localhost:1234/ and a web page will be loaded similar to the following.

![fig1](https://github.com/fjovine/FourBarLinkage/blob/master/Doc/FBL_01.png)

The top bar contains

* I - Four text fields to set the size of the four bars, namely:

|   | Description                    |
|---|--------------------------------|
| a | size of the left rotating bar  |
| b | size of the floating bar       |
| c | size of the right rotating bar |
| d | size of the fixed ground bar   |

* II - The coordinate of point P integral to the floating bar (with respect to a system of coordinates xy that can be seen in the following picture)
* III - Buttons (+/-) to zoom the animation in and out
* IV - Hyperlinks to show some predefined configurations following the Grashof classification (see wiki) The "Pic"
hyperlink simply shows the simulation without control fields. (Useful for documentation).

![fig2](https://github.com/fjovine/FourBarLinkage/blob/master/Doc/FBL_02.png)

This figure shows additional geometrical elements.

|    | Description
|----|------------
| V  | Trajectory of the left rotating hinge. It can be the whole circumference or symmetrical arcs with respect to the ground bar
| IV | Trajectory of the right rotating hinge. It can be the whole circumference or symmetrical arcs with respect to the ground bar
| P  | The coordinate system integral to the floating bar (connecting the two rotating hinges) determines the point P (blue in the figure) that moves on the trajectory shown following the linkage displacement.

# Known bugs
In some configuration, notably when the floating bar is aligned with one of the rotating bars, the numerical algorithm shows instability and this turns into oscillations of the blue trajectory.
