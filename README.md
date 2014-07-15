VisualME7Logger-plus
===============

Continuing development on the already open source VisualME7Logger. This Application pulls data from the Motronic (ME7.1) ECU currently used in select Audi/VW vehicles. The purpose of this application is to pull real-time data to visually monitor specific variables within the engine management system.

I did not write any of the base ME7Logger application but am utilizing it in this project to enhance the user experiance. My ultimate goal is for people to be able to use this as a 'carputer' so that they can visually monitor critical components of the engine.

The original ME7Logger was basically a console application that was not visually pleasing and difficult for the new user to set up and get running. Some time later, someone built off of this console app by building an interface around it to solve these issues, called VisualME7Logger.

The main goal of VisualME7Logger-plus is to add additional visual measurements along side the current line graph. It will be using a wpf controls suite called Dashboarding, originally written by David Black. It contains a good selection of controls that are visually pleasing and simple to use. With all these things combined, the user will have a better experience with logging and monitoring the inner workings of the engine management system created by the wonderful folks at Bosch and VW/Audi.
