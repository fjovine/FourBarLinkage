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



