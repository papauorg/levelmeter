---
name: 📏 New Scale Request (Cylindric)
about: You want to create a new scale with a default style for a vessel with new size or different style?
title: 'New Scale: '
labels: scale-request
assignees: 'papauorg'
body:
  - type: markdown
    attributes: 
      value: |
        [] I had a look at the available scales and the one I request does not exist yet.

  - type: dropdown
    id: containerForm
    attributes:
      label: Container form
      description: Choose the form of the container the scale should be calculated for.
      multiple: false
      options:
        - cylinder
      default: cylinder
    validations:
      required: true

  - type: dropdown
     id: lengthUnit
     attributes:
       label: Length unit
       description: In what unit do you supply your measurements?
       multiple: false
       options:
         - mm
         - in
       default: mm
    validations:
      required: true

  - type: dropdown
    id: volumeUnit
    attributes:
      label: Volume unit
      description: What volume unit should your scale use?
      multiple: false
      options:
        - l
        - gal (U.S.)
      default: l
    validations:
      required: true

  - type: input
    id: diameter
    attributes:
      label: Inner diameter
      description: The inner diameter of your container to calculate the volme.
    validations:
      required: true

  - type: input
    id: height
    attributes:
      label: Container height
      description: Height of the container to calculate the volumes of the scale.
    validations:
      required: true

  - type: input
    id: minVolume
    attributes:
      label: Min. Volume
      description: The minimum volume that should appear on the scale.
      default: 1
    validations:
      required: true

  - type: input
    id: maxVolume
    attributes:
      label: Max. Volume
      description: The maximum volume that should appear on the scale. Use 0 for no maximum.
      default: 0
    validations:
      required: true

  - type: input
    id: description
    attributes:
      label: Description
      description: Describe what this scale is for and how it will look.
    validations:
      required: false

  - type: textarea
    id: graduationMarks
    attributes:
      label: Graduation mark settings
      description: Anything you want the scale to be different than the default. Keep in mind that this may take longer to get created as it will be manual work to do it.
    validations:
      required: false
---


