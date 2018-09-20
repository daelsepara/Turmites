# Turmites

## About

![About](/Screenshots/Turmites.png)

Turmites is a program for programming, simulating, and visualizing 2D Turing machines, called Turmites. This software is a spin-off from the GtkArtificialLife project (https://github.com/daelsepara/GtkArtificialLife) focusing exclusively on Turing Machines. Unlike the artificial life simulator where entire colonies are manipulated, this software allows you to program individual Turmites. Also, individual Turmites can interact with other Turmites (or the trails they leave behind).

## Programming Turmites

A Turmite program consists of Tuples

SRWNT

Where

* S - Current state of the Turmite (numeric value) 
* R - Color read by Turmite at its current location (character A-Z)
* W - new color to write at its current location (chacters A-Z)
* N - new state (numeric value)
* T - Turn direction

Possible values for color (Read and Write) are:

O, X, A to Z

* A color of 'O' means that the location or cell is empty
* colors X, and A to Z are colors for non-empty cells

Possible values for Turn (T)

* L - left
* R - right
* S - stay in current direction (move forward)
* B - move backward

You can specify the neighborhood configuration of a Turmite. Turn directions automatically adopt to the configuration you have specified

## Sample Program

Lanton's ant has a simple program consisting of 2 Tuples:

* 1OX1R
* 1XO1L

Each iteration, a Turmite moves forward in the current direction, reads the color at current location of its head, then consults its program.

Then performs an action specified by the Tuple that applies to its present configuration (Location, Current Direction, State):

* 1OX1R

If the Turmite is in state **1** and reads and empty cell **O**, it will mark the cell with the **X** color, remain in state **1** and turn right **R** 

* 1XO1L

If the Turmite is in state **1** and reads a non-empty cell **X**, it will erase the cell with the **O** color, remain in state **1** and turn left **L**

More complex behaviors are observed depending on the complexity of the program

## Turmite Library

Turmites software has a library of sample programs which you can copy into your Turmite.

## Generating Turmites

To generate a Turmite, you can either toggle the pencil or edit icon then select a location on the world map to the left, or you can specify its coordinates then click on the 'Add Turmite' button next to the color button.

## Simulation

At any time you can start / stop the simulation by clicking on the buttons at the top. You can also save an image of an individual Turmite's trails or the entire 'world'. You can also reset the world, i.e. remove all Turmites and reset the current age (Epoch)

# Gallery

## Maze

![Maze](/Screenshots/Maze.png)

## Worm Trails

![Worm Trails](/Screenshots/WormTrails.png)

## Striped Spiral

![Striped Spiral](/Screenshots/StripedSpiral.png)
