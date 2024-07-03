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

## 📏 Scales
The available scales can be found under the ./scales/svg subfolder.

## 🧑‍🤝‍🧑 Contribute
You want to help develop levelmeter or create new custom scales? To get you quickly stated, this repository comes with a preconfigured
development environment. To be able to use this environment you need 2 things installed on your computer:

- docker
- visual studio code (vscode)

To start developing clone this repository, open the folder in vscode and use the _development container_ feature. The command is called _Reopen in container_.

### Create new scales
To create a new scale go to the `scales/definitions` folder and copy the template to a new file. The file names should follow a convention that looks like this:
```
<container_type>_<length_unit>_d<container_diameter>_h<container_height>_<scale_range><volume_unit>_i<graduation_mark_interval>_<optional_variation_information>.json
```
seems complicated? See the following file name for example: `cylinder_mm_d360_h335_1-30l_i1.json` likely contains the configuration for a scale that:
* is calculated for a cylindric container (`cylinder`)
* has a diameter of 360mm (`mm` and `d360`)
* has a height of 335mm (`mm` and 'h335`)
* has graduation marks for volumes from 1 to 30 liters (`1-30l`)
* with graduation marks in intervalls of 1 liter (`i1`)

The template already has a schema assigned that contains descriptions of the available config values and helps you create the json file correctly (if you use e.g. VS code). Also 
have a look at already existing scale definitions and their resulting svgs for inspiration.

If you are done editing the definition file you can use the VSCode Task `Generate scale`. Select your newly created file and the tool should now create a new svg scale for you. You can edit the definition and repeat the process until you are satisfied.

When the scale is ready, commit the definition and the svg file and create a pull request for your changes to share it with others. If you do so make sure you use a font with a licence that permits this. Also stencil fonts are preferrable because applying the scales do not require transfer tape this way.

## 🏅 Credits

### Fonts
This repo contains and uses the following fonts:
- BigShoulderStencilText (OFL) - https://github.com/xotypeco/big_shoulders
