---
layout: page
title: Create a new scale
permalink: /create/
---

You did not find a fitting scale for your container? You can request or create a new scale by following a few steps described below.

## Cylindrical containers only
Currently only cylindrical containers are supported for automatic creation!

## Necessary measurements
First you need to measure your container. You need the following measurements:

- _Inner_ diameter
- Height
- (optional) Maximum volume that should appear on scale. E.g. if the container height in theory allows 33l but you want the scale to stop at 30l.

## Clone the repository
The easiest way to get started is to clone the git repository, and use the devcontainer feature. The necessary tools installed on your computer for this are:

- git
- docker
- visual studio code

```bash
git clone {{ site.data.githubinfo.repositoryUrl }}.git
```

The repository comes with a preconfigured development environment that contains all relevant dependencies, so you don't have to install them on your computer. To use it
open your newly cloned folder with visual studio code. Then use the `Reopen in container` function of vscode.

## Create scale definition
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
have a look at already existing scale definitions and their resulting svgs for inspiration. For a little visual on how the settings affect the outcome see this image:

![JSON Definition to SVG output](/assets/img/scale_definition_json.png)

If you are done editing the definition file you can use the VSCode Task `Generate scale`. Select your newly created file and the tool should now create a new svg scale for you. You can edit the definition and repeat the process until you are satisfied.

When the scale is ready, commit the definition and the svg file and create a pull request for your changes to share it with others. If you do so make sure you use a font with a licence that permits this. Also stencil fonts are preferrable because applying the scales do not require transfer tape this way.
