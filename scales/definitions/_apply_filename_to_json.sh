#!/bin/bash

set -e

for file in $(find . -iname '*.json' -type f -printf "%f\n"); do

    form=$(echo "$file" | cut -d _ -f1)
    lengthUnit=$(echo "$file" | cut -d _ -f2)
    diameter=$(echo "$file" | cut -d _ -f3)
    diameter=${diameter:1}
    height=$(echo "$file" | cut -d _ -f4)
    height=${height:1}
    range=$(echo "$file" | cut -d _ -f5)
    volumeUnit=$(echo "$range" | sed -E 's/[0-9]+-[0-9]+//g')
    rangeWithoutVolume=$(echo "$range" | sed -E "s/([0-9]+-[0-9]+)$volumeUnit/\1/g")
    rangeBegin=${rangeWithoutVolume%%'-'*}
    rangeEnd=${rangeWithoutVolume##*-}
    interval=$(echo "$file" | cut -d _ -f6)
    interval=${interval:1}
    interval=${interval::-5}

    # fix volume unit for galons
    if [ $volumeUnit = "gal-us" ]; then
        volumeUnit="gal (U.S)"
    fi

    lowestInterval=$(cat $file | jq '."scale-config".graduationMarkSettings | min_by(.interval) | .interval')
    lowestTextInterval=$(cat $file | jq '."scale-config".graduationMarkSettings | map(select((.textTempate == null) and (.textTemplate != ""))) | min_by(.interval) | .interval')

    formAdjective=$form
    if [ $form == "cylinder" ]; then
        formAdjective="cylindric" 
    fi

    description="Scale for $diameter$lengthUnit diameter $formAdjective container. $rangeBegin-$rangeEnd $volumeUnit in $lowestInterval $volumeUnit intervals with text every $lowestTextInterval $volumeUnit."

    echo "Going to edit $file"
    cat $file | \
        jq ".\"scale-config\".containerForm = \"$form\"" | \
        jq ".\"scale-config\".lengthUnit = \"$lengthUnit\"" | \
        jq ".\"scale-config\".diameter = $diameter" | \
        jq ".\"scale-config\".height = $height" | \
        jq ".\"scale-config\".volumeUnit = \"$volumeUnit\"" | \
        jq ".\"scale-config\".minVolume = $rangeBegin" | \
        jq ".\"scale-config\".maxVolume = $rangeEnd" | \
        jq ".\"scale-config\".description = \"$description\"" > \
        "$file.edited"

    mv "$file.edited" "$file" -f
done