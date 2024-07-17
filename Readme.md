# Levelmeter
[![GitHub license](https://img.shields.io/github/license/papauorg/levelmeter)](https://github.com/papauorg/levelmeter/blob/master/LICENSE)
![GitHub issues](https://img.shields.io/github/issues-raw/papauorg/levelmeter)
![Happy Brewing](https://img.shields.io/badge/Levelmeter-Happy%20Brewing-%23FBB117)

<p align="center">
  <img src="https://github.com/papauorg/levelmeter/blob/main/src/dotnet-levelmeter/icon.png?raw=true" alt="Levelmeter Logo"/>
</p>

This repository contains a collection of svg graphics that can be used to etch scales into containers like fermeters or pots to be able
to easily identify how much liquid they contain. This is very useful for e.g. homebrewing. Also there is a small helper tool to create
scales for containers of different sizes.

## 📏 Scales Database
The already generated or created scales can be found under the [scales/svgs](./scales/svgs) subfolder. Or for easier access use the website mentioned below:

## 🌍 Website
This repo also contains a jekyll website where you can find a list of available scales that is easier to look through than the svg files. 
Also you'll find instructions on how to create new scales and how they can typically be applied.

The website is hosted via github pages and can be reached here: [https://papauorg.github.io/levelmeter](http://papauorg.github.io/repository)

## 🤖 Dotnet tool levelmeter
Scales can be created by writing definition files and then running the small tool. It will create an svg file with the scale. See the [documentation](https://papauorg.github.io/levelmeter/create) for more detailed instructions.

## 🧑‍🤝‍🧑 Contribute
You want to help develop levelmeter or create new custom scales? To get you quickly stated, this repository comes with a preconfigured
development environment. To be able to use this environment you need 2 things installed on your computer:

- docker
- visual studio code (vscode)

To start developing clone this repository, open the folder in vscode and use the _development container_ feature. The command is called _Reopen in container_.

## 🏅 Credits

### Fonts
This repo contains and uses the following fonts:
- BigShoulderStencilText (OFL) - https://github.com/xotypeco/big_shoulders

### Others
- Icon - https://www.freepic.com
