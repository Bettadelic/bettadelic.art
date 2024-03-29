---
layout: page
title: About
description: "A little about this site."
comments: false
tags: [about, bettadelic, history, algorithmic, art]
---

Welcome to Bettadelic!

Bettadelic's purpose is to create colorful Betta fish once per day every day for everyone to enjoy!  At at random time throughout the day, a new colorful rending of a Betta fish will be posted.

The Betta fish that are posted are photos of our actual Betta fish, but transformed in a colorful way.

We hope that this website can bring some color to your life!

## How it Works

Our Betta fish artwork could be considered a form of [algorithmic art](https://en.wikipedia.org/wiki/Algorithmic_art).  This is where a computer algorithm is used to assist in finishing the artwork.  No, this is not [AI-generated art](https://en.wikipedia.org/wiki/Artificial_intelligence_art), which involves neural nets and training data.  Algorithmic art is deterministic, and repeatable.  In fact, if we got rid of our random number generator, the same art would always be generated!

The Betta fish you see start from actual photographs of the various Betta fish we've had over the years.  We then use [GIMP](https://en.wikipedia.org/wiki/GIMP)'s built in high-pass filter tools to make a black-and-white base image of the Betta fish.  This part is mostly done by hand, to get the outlines just right.

Once we're happy with the base image, that's when the base image gets fed into an algorithm.  The algorithm separates the black pixels from the white pixels.  The black lines become the outline, while groups white pixels separated by the outline become "areas to color".  Each area to color then gets its own unique pattern.

### Types of Patterns

#### Fill

The fill pattern colors a color area a single, randomly chosen, color.  Think of it like the paint can option in MS Paint.

* [Click to see all Bettas with a fill pattern.](/tag/fill-pattern/index.html)

#### Edge

The edge pattern tries to do a spiral inwards for a color area by hugging the edge of the color area as much as possible, while slowly going inwards.  From a starting spot, the algorithm goes around the edge of the color area, coloring any pixels it hits along the way that has not been colored yet.  Once it reaches the starting spot, or it hits a dead end, a new starting spot and color is chosen and the cycle repeats.

* [Click to see all Bettas with an edge pattern.](/tag/edge-pattern/index.html)

#### Snake

The snake pattern is an early (and incorrect) implementation of the edge pattern; but it looked cool so we kept it.  Its a little tough to describe, but it starts out by hugging the edge of an area to color, but once it reaches the other side, it turns around and heads the other direction until it reaches the original side, and goes back the other way.  The result is a snaking pattern.  Once the pattern reaches the starting point or a dead end, a new starting spot and color is chosen and the cycle repeats.

* [Click to see all Bettas with a snake pattern.](/tag/snake-pattern/index.html)

#### Stripe

The stripe pattern makes each color area stripped, where each row or column or diagonal is a randomly chosen color.  The direction of the stripes (horizontal, vertical, or diagonal) is also randomly chosen.

* [Click to see all Bettas with a stripe pattern.](/tag/stripe-pattern/index.html)

#### Noise

The noise pattern is simple, in that each and every pixel that should be colored in is randomized.  The effect makes it look like static, or a bunch of noise.

* [Click to see all Bettas with a noise pattern.](/tag/noise-pattern/index.html)

#### Mixed

The mixed pattern chooses a random pattern for each separate area to color.  The end result could have a Betta with stripes, static, and solid colors all in one!

* [Click to see all Bettas with a mixed pattern.](/tag/mixed-pattern/index.html)

### Types of Background Patterns

In addition to a pattern going on the betta fish itself, the background also has a pattern.

#### Solid

The Solid background is a background of a single, randomly chosen, color.  Can't get any easier than that!

* [Click to see all Bettas with a solid background pattern.](/tag/solid-background-pattern/index.html)

#### Halo

This background starts from a randomly chosen color and shifts to a different color before shifting back again.  The end-result resembles a halo around the betta fish.

* [Click to see all Bettas with a halo background pattern.](/tag/halo-background-pattern/index.html)

#### Multisquare

This background is similar to the Halo background, but two parameters are reversed during the calculation.  This was found by accident when trying to create the halo pattern.  The end-result is a fish that sit on top of a tower of multiple squares.

* [Click to see all Bettas with a multisquare background pattern.](/tag/multisquare-background-pattern/index.html)

#### Plus

By tweaking the Halo patterns algorithm a bit, we get a pattern that looks like a plus.  Like the Multisquare, this pattern was found by accident when trying to create the halo pattern.  The end-result is a background that looks like a plus sign.

* [Click to see all Bettas with a plus background pattern.](/tag/plus-background-pattern/index.html)

#### Gradient

The background pattern starts with a base color, and then it randomly increases or decreases each pixel's color (Red, Green, Blue) in one direction for each row or column in the background until it reaches 0 or 255.  Then goes the other direction until it reaches 0 or 255 and repeats.  However, there is a small chance each time a color is being incremented or decremented where the color will change direction, thus ensuring the patterns is not repeated over and over again.

* [Click to see all Bettas with a gradient background pattern.](/tag/gradient-background-pattern/index.html)
