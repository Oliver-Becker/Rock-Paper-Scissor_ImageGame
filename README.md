# Rock-Paper-Scissor_ImageGame

Final project for SCC0251 - Image Processing @ ICMC/USP.
* 10284890 - Óliver Savastano Becker
* 10295412 - Rafael Farias Roque

# Abstract

The objective of this project is to create an app of Rock Paper Scissor game, played using images of human hands. The game will take a photo of the player's hand and identify the symbol made, then use this as input to get the game result.
There will be 2 playing modes: Vs CPU and Vs player.

To obtain the symbol made by the player, we are going to apply filters to enhance the image and then use it in a classifier.

# Partial Report

## Objective
The objective of this project is to create an application that allows the user to play rock-paper-scissors game using pictures of it's own hands. The player will be able to take a picture of its hands using a phone and that picture will be processed and classified as one of the three possible choices: rock, when the hand is closed; paper, when the hand is wide open; and scissor, when only two fingers are raised. 

Initially we are going to implement only single player mode, which the player will play against the computer, however we are aiming to implement a player versus player mode in the future.

## Description of input images
We are going to use hand pictures with the three desired symbols in different angles and some random hand gestures to test the classificator. To get these images will be photographing our own hand and asking for familiars/friends to send some photos of theirs too. This way we can get some variety of hand types. There are three restrictions to the pictures: first the photo must contain the whole hand inside it, second the background must be clear (without much objects) and third the environment must be well illuminated.

## Steps
After obtaining the image, the first step is to rescale the picture so we are able to perform the same operation in less time. Then, we segmentate the image, so  that we know what part is the hand and what is background. To accomplish this, we are going to test two image segmentation algorithms: Edge detection and Region-based segmentation. Finally, we use these segments as input to a neural network, most likely a multilayer perceptron (MLP), to classify the image as rock, paper, scissor or invallid/uncertain. If the image is uncertain we will ask for another one to the player.

The game itself will be implemented using Unity, a tool that we are familiarized with and can build the game to run on a lot of different platforms. 
